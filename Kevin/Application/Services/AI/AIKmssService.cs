using kevin.Domain.Entities.AI;
using kevin.Domain.Interfaces.IRepositories;
using kevin.Domain.Interfaces.IServices.AI;
using kevin.Domain.Share.Dtos.AI;
using kevin.Domain.Share.Enums;
using kevin.FileStorage;
using Kevin.RAG;
using Kevin.RAG.Dto;
using Kevin.RAG.Ollama;
using Kevin.RAG.Qdrant;
using Kevin.RAG.Tools;
using NetCore.Util;

namespace kevin.Application.Services.AI
{
    /// <summary>
    /// AIKmss服务接口
    /// </summary>
    public class AIKmssService : BaseService, IAIKmssService
    {
        public IAIKmssRp AIKmssRp { get; set; }
        public IAIKmsDetailsRp AIKmsDetailsRp { get; set; }

        public IFileRp FileRp { get; set; }

        public IAIModelsRp AIModelsRp { get; set; }

        public IFileStorage FileStorage { get; set; }

        private IQdrantClientService QdrantClientService { get; set; }

        private IFileService FileService { get; set; }

        private IOllamaApiService OllamaApiService { get; set; }


        public AIKmssService(IHttpContextAccessor _httpContextAccessor, IAIKmssRp _AIKmssRp, IAIKmsDetailsRp _AIKmsDetailsRp, IFileRp _FileRp, IFileStorage _FileStorage, IQdrantClientService _qdrantClientService, IFileService fileService, IOllamaApiService ollamaApiService, IAIModelsRp aIModelsRp) : base(_httpContextAccessor)
        {
            this.AIKmssRp = _AIKmssRp;
            this.AIKmsDetailsRp = _AIKmsDetailsRp;
            this.FileRp = _FileRp;
            this.FileStorage = _FileStorage;
            this.QdrantClientService = _qdrantClientService;
            FileService = fileService;
            OllamaApiService = ollamaApiService;
            AIModelsRp = aIModelsRp;
        }
        public async Task<dtoPageList<AIKmssDto>> GetList(dtoPagePar<string> dtoPagePar)
        {
            var data = AIKmssRp.Query(isDataPer: true).Where(t => t.IsDelete == false).OrderByDescending(x => x.CreateTime).ToList().MapToList<TAIKmss, AIKmssDto>();
            return new dtoPageList<AIKmssDto>() { List = data, Total = data.Count };
        }
        public async Task<dtoPageData<AIKmssDto>> GetPageData(dtoPagePar<string> dtoPagePar)
        {
            int skip = dtoPagePar.GetSkip();
            var result = new dtoPageData<AIKmssDto>();
            var data = AIKmssRp.Query(isDataPer: true).Where(t => t.IsDelete == false);
            result.total = await data.CountAsync();
            result.data = (await data.Skip(skip).Take(dtoPagePar.pageSize).OrderByDescending(x => x.CreateTime).ToListAsync()).MapToList<TAIKmss, AIKmssDto>();
            var aikmsData = await AIKmsDetailsRp.Query().Where(t => t.IsDelete == false && result.data.Select(t => t.Id).ToList().Contains(t.KmsId)).ToListAsync();
            var flieData = FileRp.Query().Where(t => t.IsDelete == false && aikmsData.Select(a => a.FileId.ToTryInt64()).ToList().Contains(t.Id)).ToList().MapToList<TFile, FileDto>().ToList();
            foreach (var item in result.data)
            {
                item.AIKmssDetailsList = aikmsData.Where(t => t.KmsId == item.Id).MapToList<TAIKmsDetails, AIKmsDetailsDto>();
                foreach (var itemDetails in item.AIKmssDetailsList)
                {
                    if (itemDetails.FileId != default)
                    {
                        var file = flieData.Where(t => t.Id == itemDetails.FileId.ToTryInt64()).FirstOrDefault();
                        if (file != default)
                        {
                            itemDetails.Url = file.Url;
                            itemDetails.ContentName = file.Name;
                        }
                    }

                }
            }
            result.pageSize = dtoPagePar.pageSize;
            result.pageNum = dtoPagePar.pageNum;
            return result;
        }
        public async Task<AIKmssDto> GetDetails(long id)
        {
            var data = (await AIKmssRp.Query(isDataPer: true).FirstOrDefaultAsync(t => t.IsDelete == false && t.TenantId == CurrentUser.TenantId && t.Id == id)).MapTo<AIKmssDto>();
            if (data == default)
            {
                throw new UserFriendlyException("不存在或已删除");
            }
            data.AIKmssDetailsList = (await AIKmsDetailsRp.Query().Where(t => t.IsDelete == false && t.KmsId == id).ToListAsync()).MapToList<TAIKmsDetails, AIKmsDetailsDto>();
            return data;
        }
        public async Task<bool> AddEdit(AIKmssDto data)
        {
            var isAdd = data.Id == default;
            long id = data.Id;
            if (!isAdd)
            {
                var msg = AIKmssRp.Query().Where(t => t.IsDelete == false && t.Id == data.Id).FirstOrDefault();
                if (msg == default)
                {
                    isAdd = true;
                }
            }
            if (isAdd)
            {
                var add = new TAIKmss();
                add.Id = data.Id == default ? SnowflakeIdService.GetNextId() : data.Id;
                id = add.Id;
                add.IsDelete = false;
                add.CreateTime = DateTime.Now;
                add.CreateUserId = CurrentUser.UserId;
                add.TenantId = CurrentUser.TenantId;
                add.Name = data.Name;
                add.MaxTokensPerParagraph = data.MaxTokensPerParagraph;
                add.MaxTokensPerLine = data.MaxTokensPerLine;
                add.OverlappingTokens = data.OverlappingTokens;
                add.aIModelsId = data.aIModelsId;
                AIKmssRp.Add(add);
                var addTAIKmsDetailss = new List<TAIKmsDetails>();
                foreach (var item in data.AIKmssDetailsList)
                {
                    var additem = new TAIKmsDetails();
                    additem.Id = additem.Id == default ? SnowflakeIdService.GetNextId() : additem.Id;
                    additem.IsDelete = false;
                    additem.CreateTime = DateTime.Now;
                    additem.CreateUserId = CurrentUser.UserId;
                    additem.Status = ImportKmsStatus.Loadding;
                    additem.TenantId = CurrentUser.TenantId;
                    additem.FileId = item.FileId;
                    additem.FileType = item.FileType;
                    additem.Content = item.Content;
                    additem.ContentName = item.ContentName;
                    additem.Url = item.Url;
                    additem.DataCount = item.DataCount;
                    additem.KmsId = add.Id;
                    addTAIKmsDetailss.Add(additem);
                }
                AIKmsDetailsRp.AddRange(addTAIKmsDetailss);
            }
            else
            {
                var upData = AIKmssRp.Query().Where(t => t.IsDelete == false && t.Id == data.Id).FirstOrDefault();
                if (upData != default)
                {
                    id = upData.Id;
                    upData.UpdateTime = DateTime.Now;
                    upData.UpdateUserId = CurrentUser.UserId;
                    upData.TenantId = CurrentUser.TenantId;
                    upData.Name = data.Name;
                    upData.MaxTokensPerParagraph = data.MaxTokensPerParagraph;
                    upData.MaxTokensPerLine = data.MaxTokensPerLine;
                    upData.aIModelsId = data.aIModelsId;
                    upData.OverlappingTokens = data.OverlappingTokens;
                    var UpAIKmsDetails = await AIKmsDetailsRp.Query().Where(t => t.IsDelete == false && t.KmsId == data.Id).ToListAsync();
                    var addTAIKmsDetailss = new List<TAIKmsDetails>();
                    UpAIKmsDetails.ForEach(t =>
                    {
                        if (!data.AIKmssDetailsList.Select(t => t.Id).ToList().Contains(t.Id))
                        {
                            t.IsDelete = true;
                            t.DeleteTime = DateTime.Now;
                            t.DeleteUserId = CurrentUser.UserId;
                        }
                        else
                        {
                            var updateItem = data.AIKmssDetailsList.Where(d => d.Id == t.Id).FirstOrDefault();
                            if (updateItem != default)
                            {
                                t.FileId = updateItem.FileId;
                                t.FileType = updateItem.FileType;
                                t.Content = updateItem.Content;
                                t.ContentName = updateItem.ContentName;
                                t.Url = updateItem.Url;
                                t.DataCount = updateItem.DataCount;
                                t.UpdateTime = DateTime.Now;
                                t.UpdateUserId = CurrentUser.UserId;
                            }
                        }
                    });
                    data.AIKmssDetailsList.ForEach(item =>
                    {
                        if (!UpAIKmsDetails.Select(t => t.Id).ToList().Contains(item.Id))
                        {
                            var additem = new TAIKmsDetails();
                            additem.Id = additem.Id == default ? SnowflakeIdService.GetNextId() : additem.Id;
                            additem.IsDelete = false;
                            additem.CreateTime = DateTime.Now;
                            additem.CreateUserId = CurrentUser.UserId;
                            additem.Status = ImportKmsStatus.Loadding;
                            additem.TenantId = CurrentUser.TenantId;
                            additem.FileId = item.FileId;
                            additem.FileType = item.FileType;
                            additem.Content = item.Content;
                            additem.ContentName = item.ContentName;
                            additem.Url = item.Url;
                            additem.DataCount = item.DataCount;
                            additem.KmsId = upData.Id;
                            addTAIKmsDetailss.Add(additem);
                        }
                    });
                    if (addTAIKmsDetailss.Count > 0)
                    {
                        AIKmsDetailsRp.AddRange(addTAIKmsDetailss);
                    }
                }
                else
                {
                    throw new UserFriendlyException("数据不存在或已删除");
                }
            }
            await AIKmssRp.SaveChangesAsync();
            await AIKmsDetailsRp.SaveChangesAsync();
            await ProcessKmssVectorData(id);
            return true;
        }
        /// <summary>
        /// 处理上传知识库矢量数据库
        /// </summary>
        /// <param name="KmsId"></param>
        /// <returns></returns>
        public async Task<bool> ProcessKmssVectorData(long KmsId)
        {
            var data = await AIKmsDetailsRp.Query().Where(t => t.IsDelete == false && t.Status == ImportKmsStatus.Loadding && t.KmsId == KmsId).ToListAsync();
            var kmssList = await AIKmssRp.Query().Where(t => t.IsDelete == false && data.Select(d => d.KmsId).Distinct().ToList().Contains(t.Id)).ToListAsync();
            if (data.Count > 0 && kmssList.Count > 0)
            {
                foreach (var item in data)
                {
                    try
                    {
                        var kmss = kmssList.Where(t => t.Id == item.KmsId).FirstOrDefault();
                        if (kmss != default)
                        {
                            //内容
                            string content = "";
                            string FileName = "";
                            //如果是文件
                            if (item.FileId != default || !string.IsNullOrEmpty(item.Url))
                            {
                                #region 文件处理 
                                var file = await FileRp.Query().Where(t => t.IsDelete == false && t.Id == item.FileId).FirstOrDefaultAsync();
                                Stream stream = default;
                                if (file != default)
                                {
                                    FileName = file.Name;
                                    var fileData = await FileService.GetFile(file.Id);
                                    if (fileData.Item1 != default)
                                    {
                                        stream = fileData.Item1;
                                    }
                                }
                                else
                                {
                                    stream = await FileHelper.GetRemoteFileStreamAsync(item.Url);
                                    FileName = await FileHelper.GetRealFileNameFromUrlAsync(item.Url);
                                }
                                if (stream != default)
                                {
                                    switch (item.FileType.Trim().ToLower())
                                    {
                                        case "text":
                                            content = TextStreamReader.ReadTextFromStream(stream);
                                            break;
                                        case "markdown":
                                            content = TextStreamReader.ReadMarkdownFromStream(stream).RawContent;
                                            break;
                                        case "pdf":
                                            content = PDFReader.ReadPdfToMarkdown(stream);
                                            break;
                                        case "word":
                                            content = WordReader.ReadParagraphs(stream);
                                            break;
                                        case "html":
                                            content = await HtmlReader.ExtractTextFromStreamAsync(stream);
                                            break;
                                        default:
                                            content = TextStreamReader.ReadTextFromStream(stream);
                                            break;
                                    }
                                }

                                #endregion

                            }
                            else
                            {
                                switch (item.FileType.Trim().ToLower())
                                {
                                    case "text":
                                        content = item.Content;
                                        break;
                                    case "markdown":
                                        content = TextStreamReader.ReadMarkdownFromText(item.Content).RawContent;
                                        break;  
                                    case "html":
                                        content = await HtmlReader.ExtractTextFromTextAsync(item.Content);
                                        break;
                                    default:
                                        content = item.Content;
                                        break;
                                } 
                                FileName = item.ContentName;
                            }
                            var allChunks = new List<DocumentChunkDto>();
                            // 2. 初始化服务
                            var documentProcessor = new DocumentProcessor(chunkSize: kmss.MaxTokensPerParagraph, chunkOverlap: kmss.OverlappingTokens);
                            // 清理文档
                            var cleanedContent = documentProcessor.CleanDocument(content);
                            // 分块
                            var chunks = documentProcessor.ChunkByParagraph(cleanedContent);
                            Console.WriteLine($"文档 '{FileName}' 分成了 {chunks.Count} 个块");
                            // 创建文档块对象
                            for (int i = 0; i < chunks.Count; i++)
                            {
                                allChunks.Add(new DocumentChunkDto
                                {
                                    Content = chunks[i],
                                    SourceFile = FileName,
                                    Id = SnowflakeIdService.GetNextId(),
                                    CreatedAt = DateTime.Now,
                                    Title = FileName.ToLower().Replace(".txt", "").Replace(".word", "").Replace(".pdf", "").Replace(".markdown", "").Replace(".html", ""),
                                    Category = kmss.Name,
                                    ChunkIndex = i
                                });
                            }
                            ulong embeddingValueSize = 2048;
                            if (kmss.aIModelsId != default)
                            {
                                var aimode = AIModelsRp.Query().Where(t => t.IsDelete == false && t.Id == kmss.aIModelsId).FirstOrDefault();
                                if (aimode?.AIModelType == AIModelType.Embedding)
                                {
                                    OllamaApiService = new OllamaApiService(aimode.EndPoint, aimode.ModelName, aimode.ModelKey);
                                    embeddingValueSize = (ulong)aimode.EmbeddingValueSize;
                                }
                            }
                            // 生成嵌入向量
                            foreach (var chunk in allChunks)
                            {
                                chunk.ContentVector = await OllamaApiService.GetEmbedding(chunk.Content);
                            }
                            Console.WriteLine($"\n正在上传 {allChunks.Count} 个文档块到向量数据库...");
                            // 上传到向量数据库
                            var isaddok = await QdrantClientService.AddData("AIKmss-" + kmss.Id, allChunks, embeddingValueSize);
                            if (isaddok)
                            {
                                item.Status = ImportKmsStatus.Success;
                                item.UpdateTime = DateTime.Now;
                                item.ContentName = FileName;
                            }
                        }
                        else
                        {
                            throw new UserFriendlyException("数据不存在或已删除");
                        }
                    }
                    catch (Exception ex)
                    {
                        item.Status = ImportKmsStatus.Fail;
                        item.UpdateTime = DateTime.Now;
                        item.ErrorMessage = ex.Message;
                    }
                }
                await AIKmsDetailsRp.SaveChangesAsync();
            }

            return true;
        }

        public async Task<bool> Delete(long id)
        {
            var data = await AIKmssRp.Query(isDataPer: true).Where(t => t.IsDelete == false && t.Id == id).FirstOrDefaultAsync();
            if (data != default)
            {

                data.IsDelete = true;
                data.DeleteTime = DateTime.Now;
                AIKmssRp.SaveChangesWithSaveLog();
            }
            else
            {
                throw new UserFriendlyException("数据不存在或已删除");
            }
            return true;
        }
    }
}

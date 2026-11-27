using kevin.Domain.Entities.AI;
using kevin.Domain.Interfaces.IServices.AI;
using kevin.Domain.Share.Dtos.AI;
using kevin.Domain.Share.Enums;
using kevin.FileStorage;
using Kevin.log4Net;
using Kevin.RAG.Dto;
using Kevin.RAG.Interfaces;
using Kevin.RAG.Ollama;
using Medallion.Threading;
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

        private IRAGStorageService QdrantClientService { get; set; }

        private IFileService FileService { get; set; }

        private IOllamaApiService OllamaApiService { get; set; }
        private IDistributedLockProvider distLock { get; set; }

        public AIKmssService(IHttpContextAccessor _httpContextAccessor, IAIKmssRp _AIKmssRp, IAIKmsDetailsRp _AIKmsDetailsRp,
            IFileRp _FileRp, IFileStorage _FileStorage, IRAGStorageService _qdrantClientService, IFileService fileService,
            IOllamaApiService ollamaApiService, IAIModelsRp aIModelsRp, IDistributedLockProvider distributed) : base(_httpContextAccessor)
        {
            this.AIKmssRp = _AIKmssRp;
            this.AIKmsDetailsRp = _AIKmsDetailsRp;
            this.FileRp = _FileRp;
            this.FileStorage = _FileStorage;
            this.QdrantClientService = _qdrantClientService;
            FileService = fileService;
            OllamaApiService = ollamaApiService;
            AIModelsRp = aIModelsRp;
            distLock = distributed;
        }
        public async Task<dtoPageList<AIKmssDto>> GetList(dtoPagePar<string> dtoPagePar)
        {
            var data = (await AIKmssRp.Query(isDataPer: true).Where(t => t.IsDelete == false).OrderByDescending(x => x.CreateTime).ToListAsync()).MapToList<TAIKmss, AIKmssDto>();
            return new dtoPageList<AIKmssDto>() { List = data, Total = data.Count };
        }
        public async Task<dtoPageData<AIKmssDto>> GetPageData(dtoPagePar<string> dtoPagePar)
        {
            int skip = dtoPagePar.GetSkip();
            var result = new dtoPageData<AIKmssDto>();
            var data = AIKmssRp.Query(isDataPer: true).Where(t => t.IsDelete == false);
            result.total = await data.CountAsync();
            var dbdata = await data.OrderByDescending(x => x.CreateTime).Skip(skip).Take(dtoPagePar.pageSize).Include(t => t.CreateUser).Include(t => t.UpdateUser).ToListAsync();
            result.data = dbdata.MapToList<TAIKmss, AIKmssDto>();
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
                            itemDetails.Url = file.Url ?? "";
                            itemDetails.ContentName = file.Name ?? "";
                        }
                    }

                }
                item.CreateUser = dbdata.FirstOrDefault(d => d.Id == item.Id)?.CreateUser?.Name;
                item.UpdateUser = dbdata.FirstOrDefault(d => d.Id == item.Id)?.UpdateUser?.Name;
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
            var flieData = FileRp.Query().Where(t => t.IsDelete == false && data.AIKmssDetailsList.Select(a => a.FileId.ToTryInt64()).ToList().Contains(t.Id)).ToList().MapToList<TFile, FileDto>().ToList();
            foreach (var itemDetails in data.AIKmssDetailsList)
            {
                if (itemDetails.FileId != default)
                {
                    var file = flieData.Where(t => t.Id == itemDetails.FileId.ToTryInt64()).FirstOrDefault();
                    if (file != default)
                    {
                        itemDetails.Url = file.Url ?? "";
                        itemDetails.ContentName = file.Name ?? "";
                    }
                }

            }
            return data;
        }
        public async Task<bool> AddEdit(AIKmssDto data)
        {
            var isAdd = data.Id == default;
            long id = data.Id;
            // 本次被移除的明细文件名：保存后按文件清掉它们已入库的向量块（否则数据库已看不到、检索仍能检到）
            var removedSourceFiles = new List<string>();
            if (!isAdd)
            {
                var msg = AIKmssRp.Query().Where(t => t.IsDelete == false && t.Id == data.Id).FirstOrDefault();
                if (msg == default)
                {
                    isAdd = true;
                }
            }
            if (data.aIModelsId == default)
            {
                throw new UserFriendlyException("矢量化模型必填");
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
                add.aIRerankModelsId = data.aIRerankModelsId;
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
                    upData.aIRerankModelsId = data.aIRerankModelsId;
                    var UpAIKmsDetails = await AIKmsDetailsRp.Query().Where(t => t.IsDelete == false && t.KmsId == data.Id).ToListAsync();
                    var addTAIKmsDetailss = new List<TAIKmsDetails>();
                    UpAIKmsDetails.ForEach(t =>
                    {
                        if (!data.AIKmssDetailsList.Select(t => t.Id).ToList().Contains(t.Id))
                        {
                            t.IsDelete = true;
                            t.DeleteTime = DateTime.Now;
                            t.DeleteUserId = CurrentUser.UserId;
                            removedSourceFiles.Add(t.ContentName ?? "");
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
                            // 同名文件重新导入：复用已有明细并重置为待处理，避免产生重复明细
                            // （重复明细会各自触发一次向量化，导致同一文件的 chunkIndex 出现两组点）
                            var sameFile = !string.IsNullOrWhiteSpace(item.ContentName)
                                ? UpAIKmsDetails.FirstOrDefault(t => !t.IsDelete && t.ContentName == item.ContentName)
                                : null;
                            if (sameFile != default)
                            {
                                sameFile.FileId = item.FileId;
                                sameFile.FileType = item.FileType;
                                sameFile.Content = item.Content;
                                sameFile.Url = item.Url;
                                sameFile.ContentName = item.ContentName;
                                sameFile.Status = ImportKmsStatus.Loadding;
                                sameFile.ErrorMessage = "";
                                sameFile.UpdateTime = DateTime.Now;
                                sameFile.UpdateUserId = CurrentUser.UserId;
                                return;
                            }
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

            foreach (var sourceFile in removedSourceFiles.Where(t => !string.IsNullOrWhiteSpace(t)).Distinct())
            {
                await RemoveVectorData(id, GetCollectionName(id), sourceFile);
            }
            return true;
        }
        /// <summary>
        /// 处理上传知识库矢量数据库
        /// </summary>
        /// <param name="KmsId"></param>
        /// <returns></returns>
        public async Task<bool> ProcessKmssVectorData(long KmsId = default)
        {
            var lock1 = distLock.TryAcquireLock("kevin.Application.Services.AI.ProcessKmssVectorData:" + KmsId);
            if (lock1 == null)
            {
                LogHelper.logger.Error("kevin.Application.Services.AI.ProcessKmssVectorData:处理上传知识库矢量数据库: 获取分布式锁失败");
                return false;
            }
            using (lock1)
            {
                try
                {
                    var query = AIKmsDetailsRp.Query().Where(t => t.IsDelete == false && t.Status == ImportKmsStatus.Loadding);
                    if (KmsId != default)
                    {
                        query = query.Where(t => t.KmsId == KmsId);
                    }
                    var data = await query.ToListAsync();
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
                                        Stream? stream = null;
                                        if (file != default)
                                        {
                                            FileName = file.Name ?? "";
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
                                            switch ((item.FileType ?? "").Trim().ToLower())
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
                                                case "image":
                                                    content = ImageReader.DescribeImage(stream);
                                                    break;
                                                case "excel":
                                                    content = ExcelReader.ReadExcelToMarkdown(stream, FileName);
                                                    break;
                                                case "pptx":
                                                    content = PptxReader.ReadSlidesToMarkdown(stream);
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
                                        switch ((item.FileType ?? "").Trim().ToLower())
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
                                    // 2. 初始化服务（行最大数参与超长行切断，与段落/重叠配置一起生效）
                                    var documentProcessor = new DocumentProcessor(chunkSize: kmss.MaxTokensPerParagraph, chunkOverlap: kmss.OverlappingTokens, maxLineSize: kmss.MaxTokensPerLine);
                                    // 清理文档
                                    var cleanedContent = documentProcessor.CleanDocument(content);
                                    // 分块
                                    var chunks = documentProcessor.ChunkByParagraph(cleanedContent).Where(t => !string.IsNullOrWhiteSpace(t)).ToList();
                                    Console.WriteLine($"文档 '{FileName}' 分成了 {chunks.Count} 个块，最大块长度 {(chunks.Count > 0 ? chunks.Max(t => t.Length) : 0)}（配置块上限 {kmss.MaxTokensPerParagraph}）");
                                    if (chunks.Count > 0)
                                    {    // 创建文档块对象
                                        for (int i = 0; i < chunks.Count; i++)
                                        {
                                            allChunks.Add(new DocumentChunkDto
                                            {
                                                Content = chunks[i],
                                                SourceFile = FileName ?? "",
                                                Id = SnowflakeIdService.GetNextId(),
                                                CreatedAt = DateTime.Now,
                                                Title = (FileName ?? "").ToLower().Replace(".txt", "").Replace(".word", "").Replace(".pdf", "").Replace(".markdown", "").Replace(".html", ""),

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
                                        // 写入前先清掉该文件的旧块：重复导入/重复明细时保证幂等，避免同 ChunkIndex 的点叠加。
                                        // 此处不能直接调用 RemoveVectorData（内部会再次获取同一把锁导致死锁），直接走 RAG 层。
                                        await QdrantClientService.DeleteData(GetCollectionName(kmss.Id), FileName);
                                        // 上传到向量数据库
                                        var isaddok = await QdrantClientService.AddData(GetCollectionName(kmss.Id), allChunks, embeddingValueSize);
                                        if (isaddok)
                                        {
                                            item.Status = ImportKmsStatus.Success;
                                            item.UpdateTime = DateTime.Now;
                                            item.ContentName = FileName ?? "";
                                            // 回写抽取文本：预览接口直接展示 Content，文件型导入时前端未传 Content
                                            item.Content = cleanedContent;
                                        }

                                    }
                                    else
                                    {
                                        item.Status = ImportKmsStatus.Success;
                                        item.UpdateTime = DateTime.Now;
                                        item.ContentName = FileName ?? "";
                                        item.Content = cleanedContent;
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
                }
                catch (Exception ex)
                {
                    LogHelper.logger.Error("处理上传知识库矢量数据库:" + ex);
                }
            }
            return true;
        }

        /// <summary>
        /// 知识库文件碎片分页预览：只读实时查询向量库，不与导入互斥（不获取分布式锁）。
        /// 归属校验失败抛 UserFriendlyException；Qdrant 异常/集合不存在/无碎片时归一为空结果。
        /// </summary>
        /// <param name="kmsId">知识库Id</param>
        /// <param name="sourceFile">来源文件名（导入成功后回写的 ContentName）</param>
        /// <param name="pageNum">页码，从 1 开始</param>
        /// <param name="pageSize">每页条数（钳制在 1..100）</param>
        /// <returns>碎片总数与当前页碎片列表（按 ChunkIndex 升序）</returns>
        public async Task<KmsChunksDto> GetKmsChunks(long kmsId, string sourceFile, int pageNum = 1, int pageSize = 10)
        {
            // 归属校验：与 GetDetails 一致，数据权限 + 租户隔离
            var kmss = AIKmssRp.Query(isDataPer: true)
                .Where(t => t.IsDelete == false && t.Id == kmsId && t.TenantId == CurrentUser.TenantId)
                .FirstOrDefault();
            if (kmss == default)
            {
                throw new UserFriendlyException("不存在或已删除");
            }
            pageNum = Math.Max(1, pageNum);
            pageSize = Math.Clamp(pageSize, 1, 100);
            var result = new KmsChunksDto();
            try
            {
                var (total, chunks) = await QdrantClientService.GetChunksBySourceFile(
                    GetCollectionName(kmsId), sourceFile,
                    (ulong)pageSize, (ulong)((pageNum - 1) * pageSize));
                result.Total = (long)total;
                result.List = chunks.OrderBy(c => c.ChunkIndex).Select(c => new KmsChunkItemDto
                {
                    Id = c.Id,
                    Content = c.Content,
                    ChunkIndex = c.ChunkIndex,
                    Category = c.Category,
                    CreatedAt = c.CreatedAt
                }).ToList();
            }
            catch (Exception ex)
            {
                // Qdrant 未配置/停服/网络故障时归一为空结果，不阻断预览
                LogHelper.logger.Warn("预览碎片查询失败:" + ex);
            }
            return result;
        }

        public async Task<bool> Delete(long id)
        {
            var data = await AIKmssRp.Query(isDataPer: true).Where(t => t.IsDelete == false && t.Id == id).FirstOrDefaultAsync();
            if (data != default)
            {

                data.IsDelete = true;
                data.DeleteTime = DateTime.Now;
                AIKmssRp.SaveChangesWithSaveLog();

                // 明细一并软删：否则主表已删而明细仍存留，后续导入流程会把它们重新扫出来写回向量库
                var details = await AIKmsDetailsRp.Query().Where(t => t.IsDelete == false && t.KmsId == id).ToListAsync();
                if (details.Count > 0)
                {
                    details.ForEach(t =>
                    {
                        t.IsDelete = true;
                        t.DeleteTime = DateTime.Now;
                        t.DeleteUserId = CurrentUser.UserId;
                    });
                    await AIKmsDetailsRp.SaveChangesAsync();
                }

                // 向量库不在数据库事务内：删除失败只记日志，不能把知识库删除卡住（否则数据库已删、接口报错，重试也删不掉）
                var collectionName = GetCollectionName(id);
                await RemoveVectorData(id, collectionName, sourceFile: null);
            }
            else
            {
                throw new UserFriendlyException("数据不存在或已删除");
            }
            return true;
        }

        /// <summary>知识库向量集合名：与 <see cref="ProcessKmssVectorData"/> 写入时保持一致</summary>
        private static string GetCollectionName(long kmsId)
        {
            return "AIKmss-" + kmsId;
        }

        /// <summary>
        /// 清理向量库数据：<paramref name="sourceFile"/> 为空时删除整个集合（删知识库），
        /// 否则只删该来源文件的块（移除单个文档）。
        /// <para>
        /// 与导入共用同一把分布式锁，避免删除与导入并发时点被重新写回去；
        /// 异常全部包住并落日志，向量库故障不影响数据库侧的删除结果。
        /// </para>
        /// </summary>
        private async Task RemoveVectorData(long kmsId, string collectionName, string? sourceFile)
        {
            var lock1 = distLock.TryAcquireLock("kevin.Application.Services.AI.ProcessKmssVectorData:" + kmsId);
            if (lock1 == null)
            {
                LogHelper.logger.Error($"清理知识库向量数据获取分布式锁失败，已跳过: {collectionName}");
                return;
            }
            using (lock1)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(sourceFile))
                    {
                        await QdrantClientService.DeleteCollection(collectionName);
                    }
                    else
                    {
                        await QdrantClientService.DeleteData(collectionName, sourceFile);
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.logger.Error($"清理知识库向量数据失败(数据库侧已处理): {collectionName} {sourceFile}", ex);
                }
            }
        }
    }
}

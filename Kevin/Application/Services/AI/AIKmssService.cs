using kevin.Domain.Entities.AI;
using kevin.Domain.Interfaces.IRepositories;
using kevin.Domain.Interfaces.IRepositories.AI;
using kevin.Domain.Interfaces.IServices.AI;
using kevin.Domain.Share.Dtos.AI;
using kevin.Domain.Share.Enums;
using kevin.FileStorage;
using Kevin.RAG.Dto;
using Kevin.RAG.Qdrant;
using Kevin.RAG.Tools;
using Kevin.SnowflakeId.Service;
using System;

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

        public IFileStorage FileStorage { get; set; }

        private IQdrantClientService QdrantClientService { get; set; }
        public AIKmssService(IHttpContextAccessor _httpContextAccessor, IAIKmssRp _AIKmssRp, IAIKmsDetailsRp _AIKmsDetailsRp, IFileRp _FileRp, IFileStorage _FileStorage, IQdrantClientService _qdrantClientService) : base(_httpContextAccessor)
        {
            this.AIKmssRp = _AIKmssRp;
            this.AIKmsDetailsRp = _AIKmsDetailsRp;
            this.FileRp = _FileRp;
            this.FileStorage = _FileStorage;
            this.QdrantClientService = _qdrantClientService;
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
            data.AIKmssDetailsList = (await AIKmsDetailsRp.Query(isDataPer: true).Where(t => t.IsDelete == false && t.KmsId == id).ToListAsync()).MapToList<TAIKmsDetails, AIKmsDetailsDto>();
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
                var add = data.MapTo<TAIKmss>();
                add.Id = data.Id == default ? SnowflakeIdService.GetNextId() : data.Id;
                id = add.Id;
                add.IsDelete = false;
                add.CreateTime = DateTime.Now;
                add.CreateUserId = CurrentUser.UserId;
                add.TenantId = CurrentUser.TenantId;
                AIKmssRp.Add(add);
                var addTAIKmsDetailss = new List<TAIKmsDetails>();
                foreach (var item in data.AIKmssDetailsList)
                {
                    var additem = item.MapTo<TAIKmsDetails>();
                    additem.Id = additem.Id == default ? SnowflakeIdService.GetNextId() : additem.Id;
                    additem.IsDelete = false;
                    additem.CreateTime = DateTime.Now;
                    additem.CreateUserId = CurrentUser.UserId;
                    additem.Status = ImportKmsStatus.Loadding;
                    additem.TenantId = CurrentUser.TenantId;
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
                    upData = data.MapTo(upData);
                    upData.UpdateTime = DateTime.Now;
                    upData.UpdateUserId = CurrentUser.UserId;
                    upData.TenantId = CurrentUser.TenantId;
                    var UpAIKmsDetails = await AIKmsDetailsRp.Query().Where(t => t.IsDelete == false && t.KmsId == data.Id).ToListAsync();
                    bool isUpAIKmsDetail = false;//是否需要更新
                    UpAIKmsDetails.ForEach(t =>
                    {
                        if (!data.AIKmssDetailsList.Select(t => t.Id).ToList().Contains(t.Id))
                        {
                            isUpAIKmsDetail = true;
                        }
                    });
                    if (UpAIKmsDetails.Count != data.AIKmssDetailsList.Count)
                    {
                        isUpAIKmsDetail = true;
                    }
                    if (isUpAIKmsDetail)
                    {
                        if (UpAIKmsDetails.Count > 0)
                        {
                            foreach (var item in UpAIKmsDetails)
                            {
                                item.IsDelete = true;
                                item.DeleteTime = DateTime.Now;
                                item.DeleteUserId = CurrentUser.UserId;
                            }
                            AIKmsDetailsRp.UpdateRange(UpAIKmsDetails.ToArray());
                        }
                        var addTAIKmsDetailss = new List<TAIKmsDetails>();
                        foreach (var item in data.AIKmssDetailsList)
                        {
                            var additem = item.MapTo<TAIKmsDetails>();
                            additem.Id = additem.Id == default ? SnowflakeIdService.GetNextId() : additem.Id;
                            additem.IsDelete = false;
                            additem.CreateTime = DateTime.Now;
                            additem.CreateUserId = CurrentUser.UserId;
                            additem.Status = ImportKmsStatus.Loadding;
                            additem.TenantId = CurrentUser.TenantId;
                            additem.KmsId = upData.Id;
                            addTAIKmsDetailss.Add(additem);
                        }
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
            Task.Run(() =>
            {
                ProcessKmssVectorData(id);
            }).Start();
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
                                #endregion

                            }
                            else
                            {
                                content = item.Content;
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
                            Console.WriteLine($"\n正在上传 {allChunks.Count} 个文档块到向量数据库...");
                            // 上传到向量数据库
                            var isaddok = await QdrantClientService.AddData("AIKmss-" + kmss.Id, allChunks);
                            if (isaddok)
                            {
                                item.Status = ImportKmsStatus.Success;
                                item.UpdateTime = DateTime.Now;
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

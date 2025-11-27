using kevin.Domain.Entities;
using kevin.Domain.Share.Dtos;
using kevin.Domain.Share.Dtos.Msg;
using kevin.Domain.Share.Enums;
using kevin.Share.Dtos;
using Web.Global.Exceptions;

namespace kevin.Application.Services
{
    public class MessageService : BaseService, IMessageService
    {
        public IMessageRp messageRp { get; set; }

        public IMessageReadRp messageReadRp { get; set; }

        public IFileService fileService { get; set; }
        public MessageService(IHttpContextAccessor _httpContextAccessor, IMessageRp _messageRp, IMessageReadRp messageReadRp, IFileService fileService) : base(_httpContextAccessor)
        {
            messageRp = _messageRp;
            this.messageReadRp = messageReadRp;
            this.fileService = fileService;
        }
        /// <summary>
        /// 获取我的未读消息数量
        /// </summary>
        /// <returns></returns>
        public async Task<int> GetMyNoReadCount(MessageType messageType = MessageType.All)
        {
            var data = messageRp.Query().Where(t => t.IsDelete == false && t.TenantId == CurrentUser.TenantId);
            data = data = data.Where(t => t.SendUserId == CurrentUser.UserId.ToString()
            || t.RecipientUserId == CurrentUser.UserId.ToString()
            || t.SysMsgType == MessageType.System
             || t.SysMsgType == MessageType.Announcement
            );
            if (messageType != MessageType.All)
            {
                data = data.Where(t => t.SysMsgType == messageType);
            }
            var toData = await data.AsNoTracking().ToListAsync();
            if (toData.Count > 0)
            {
                var readData = messageReadRp.Query().Where(t => t.IsDelete == false && t.TenantId == CurrentUser.TenantId
                                             && t.CreateUserId == CurrentUser.UserId).ToList();
                toData = toData.Where(t => !readData.Any(x => x.MessageId == t.Id)).ToList();
            }
            return toData.Count();
        }

        public async Task<dtoPageData<MessageDto>> GetPageData(dtoPagePar<MsgGetPageDataParDto> par)
        {
            var dataPage = new dtoPageData<MessageDto>();
            int skip = par.GetSkip();
            var data = messageRp.Query().Where(t => t.IsDelete == false && t.TenantId == CurrentUser.TenantId);
            if (!string.IsNullOrEmpty(par.searchKey))
            {
                data = data.Where(t => (t.Title ?? "").Contains(par.searchKey) || (t.MessageContent ?? "").Contains(par.searchKey));
            }
            if (par.Parameter != default)
            {
                if (!string.IsNullOrEmpty(par.Parameter.AssociatedId))
                {
                    data = data.Where(t => t.AssociatedId == par.Parameter.AssociatedId);
                }
                if (!string.IsNullOrEmpty(par.Parameter.UserId))
                {
                    data = data.Where(t => (t.SendUserId == par.Parameter.UserId || t.RecipientUserId == par.Parameter.UserId));
                }
                if (par.Parameter.SysMsgType != MessageType.All)
                {
                    data = data.Where(t => t.SysMsgType == par.Parameter.SysMsgType);
                }
            }

            dataPage.total = await data.CountAsync();
            var dbdata = await data.Skip(skip).Take(par.pageSize).OrderByDescending(x => x.CreateTime).Include(t => t.CreateUser).ToListAsync();
            dataPage.data = dbdata.MapToList<TMessage, MessageDto>();
            var ids = dataPage.data.Select(m => m.Id).ToList();
            if (dataPage.total > 0)
            {
                var readData = messageReadRp.Query().Where(t => t.IsDelete == false && t.TenantId == CurrentUser.TenantId
                                              && t.CreateUserId == CurrentUser.UserId && ids.Contains(t.MessageId)).ToList();

                var files = await fileService.GetFileDtos(ids.Select(t => t.ToString()).ToList(), "t_message");
                dataPage.data.ForEach(t =>
                {
                    t.IsRead = readData.Any(x => x.MessageId == t.Id);
                    t.Files = files.Where(f => f.TableId == t.Id.ToString()).ToList();
                    t.CreateUser = dbdata.FirstOrDefault(d => d.Id == t.Id)?.CreateUser?.Name;
                });
            }
            return dataPage;
        }

        public async Task<bool> Read(long id)
        {
            var addread = new TMessageRead();
            addread.MessageId = id;
            addread.CreateUserId = CurrentUser.UserId;
            addread.CreateTime = DateTime.Now;
            addread.IsDelete = false;
            addread.TenantId = CurrentUser.TenantId;
            messageReadRp.Add(addread);
            await messageReadRp.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AddEdit(MessageDto message)
        {
            var isAdd = message.Id == default;
            if (!isAdd)
            {
                var msg = messageRp.Query().Where(t => t.IsDelete == false && t.Id == message.Id).FirstOrDefault();
                if (msg == default)
                {
                    isAdd = true;
                }
            }
            if (isAdd)
            {
                var add = message.MapTo<TMessage>();
                add.Id = message.Id == default ? SnowflakeIdService.GetNextId() : message.Id;
                add.IsDelete = false;
                add.CreateTime = DateTime.Now;
                add.CreateUserId = CurrentUser.UserId;
                add.TenantId = CurrentUser.TenantId;
                messageRp.Add(add);
            }
            else
            {
                var msg = messageRp.Query().Where(t => t.IsDelete == false && t.Id == message.Id).FirstOrDefault();
                if (msg != default)
                {
                    msg.UpdateTime = DateTime.Now;
                    msg.UpdateUserId = CurrentUser.UserId;
                    msg.Title = message.Title;
                    msg.MessageContent = message.MessageContent;
                    msg.SendUserId = message.SendUserId;
                    msg.SendUserName = message.SendUserName;
                    msg.RecipientUserId = message.RecipientUserId;
                    msg.RecipientUserName = message.RecipientUserName;
                    msg.AssociatedId = message.AssociatedId;
                    msg.AssociatedTable = message.AssociatedTable;
                    msg.TenantId = CurrentUser.TenantId;
                }
                else
                {
                    throw new UserFriendlyException("数据不存在或已删除");
                }

            }
            await messageRp.SaveChangesAsync();

            return true;
        }

        public async Task<bool> Delete(long id)
        {
            var like = await messageRp.Query().Where(t => t.IsDelete == false && t.Id == id).FirstOrDefaultAsync();

            if (like != null)
            {
                like.IsDelete = true;
                like.DeleteTime = DateTime.Now;
                messageRp.SaveChangesWithSaveLog();
            }
            else
            {
                throw new UserFriendlyException("数据不存在或已删除");
            }
            return true;
        }
    }
}

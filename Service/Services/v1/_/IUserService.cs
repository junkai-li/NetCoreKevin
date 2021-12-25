using Models.Dtos;
using Models.Dtos.System;
using Service.Dtos.v1.User;
using System;
using System.Collections.Generic;

namespace Service.Services.v1._
{
    public interface IUserService
    {
        dtoUser GetUser(Guid userId);

        bool EditUserPhoneBySms(dtoKeyValue keyValue);

        string GetWeiXinMiniAppOpenId(Guid weixinkeyid, string code);

        string GetWeiXinMiniAppPhone(string iv, string encryptedData, string code, Guid weixinkeyid);

        bool EditUserPassWordBySms(dtoKeyValue keyValue);
        dtoPageData<dtoUser> GetUserList(dtoPageData<dtoUser> dtoPage); 

        dtoPageData<dtoUser> GetSysUserList(dtoPageData<dtoUser> dtoPage);

        dtoUser GetSysUserWhereId(Guid Id);
        bool EditUser(dtoUser user);

        bool DeleteUser(Guid Id);

        dtoPageData<dtoRole> GetUserRoleList(dtoPageData<dtoRole> dtoPage);

        dtoRole GetRoleWhereId(Guid Id);

        bool EditUserRole(dtoRole role);

        bool DeleteUserRole(Guid Id);

        List<dtoKeyValue> GetUserRoleKey();

        List<dtoKeyValue> GetUserSystemKey();
    }
}

import http from '../utils/http';

export const GetGuId = () => {
  return http.get(`/api/base/GetGuId`);
};

//获取登录用户角色权限
export const getUserPermissions = () => {
  return http.get(`/api/Permission/GetUserPermissions`);
};
import http from '../utils/http';

export const login = (acctount, password,tenantId) => 
{
  return http.post(`/api/Authorize/GetToken`,{ name:acctount, passWord: password,tenantId:tenantId });
};
export const getTokenUser = () => {
  return http.get(`/api/User/GetUser`);
};

export const createUser = (userData) => {
  return http.post('/api/User/EditUser', userData);
};

export const updateUser = (userData) => {
  return http.post(`/api/User/EditUser`, userData);
}; 

export const getUserList = (params) => {
  return http.post('/api/User/GetSysUserList', params);
};

export const getUserRoleList = () => {
  return http.get('/api/User/GetUserRoleKey');
};
 
export const DeleteUser = (id) => {
  return http.delete('/api/User/DeleteUser?id='+id);
};

export const ExportGetSysUserList = (params) => {
  return http.post('/api/User/ExportGetSysUserList', params,{ responseType: 'blob' });
};

// 修改密码接口
export const changePassword = (data) => {
  return http.post('/api/User/ChangePasswordTokenUser', data);
};

// 获取用户总数接口
export const getAllUserCount = () => {
  return http.get('/api/User/GetAllUserCount');
};

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
  return http.put(`/api/User/EditUser`, userData);
}; 

export const getUserList = (params) => {
  return http.post('/api/User/GetSysUserList', params);
};

export const getUserRoleList = () => {
  return http.get('/api/User/GetUserRoleKey');
};
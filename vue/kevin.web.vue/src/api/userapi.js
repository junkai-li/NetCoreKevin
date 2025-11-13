import http from '../utils/http';

export const login = (acctount, password,tenantId) => 
{
  return http.post(`/api/Authorize/GetToken`,{ name:acctount, passWord: password,tenantId:tenantId });
};
export const getUser = (userId) => {
  return http.get(`/users/${userId}`);
};

export const createUser = (userData) => {
  return http.post('/users', userData);
};
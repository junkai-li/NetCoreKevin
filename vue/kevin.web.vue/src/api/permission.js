import http from '../utils/http';

export const reload = () => {
  return http.get('/api/Permission/Reload');
};
export const GetPageData = (params) => {
  return http.post('/api/Permission/GetPageData', params);
};

export const addEidt = (params) => {
  return http.post('/api/Permission/Edit', params);
};

export const Delete = (roleId) => {
  return http.delete(`/api/Permission/Del?Id=${roleId}`);
};

// 获取详情
export const getDetails = (roleId) => {
  return http.get(`/api/Permission/GetDetails?Id=${roleId}`);
};  
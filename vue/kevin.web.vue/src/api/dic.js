import http from '../utils/http';
// 系统字典相关接口
export const GetPageData = (params) => {
  return http.post('/api/Dictionary/GetPageData', params);
};

export const GetTypeList = () => {
  return http.get('/api/Dictionary/GetTypeList');
};
export const addEditMessage = (data) => {
  return http.post('/api/Dictionary/AddEdit', data);
};
export const GetTypeWhereList = (type) => {
  return http.delete(`/api/Dictionary/GetTypeWhereList?type=${type}`);
};
export const deleteMessage = (id) => {
  return http.delete(`/api/Dictionary/Delete?Id=${id}`);
};
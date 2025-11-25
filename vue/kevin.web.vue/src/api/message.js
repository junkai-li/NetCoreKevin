import http from '../utils/http';

export const GetMyNoReadCount = () => {
  return http.get('/api/Message/GetMyNoReadCount');
};

// 系统消息相关接口
export const getSystemPageData = (params) => {
  return http.post('/api/Message/GetSystemPageData', params);
};

export const addEditMessage = (data) => {
  return http.post('/api/Message/AddEdit', data);
};

export const deleteMessage = (id) => {
  return http.delete(`/api/Message/Delete?Id=${id}`);
};

// 我的消息相关接口
export const getPrivateUserSystemPageData = (params) => {
  return http.post('/api/Message/GetPrivateUserSystemPageData', params);
};

export const getPrivateUserPageData = (params) => {
  return http.post('/api/Message/GetPrivateUserPageData', params);
};

export const getAnnouncementPageData = (params) => {
  return http.post('/api/Message/GetAnnouncementPageData', params);
};
import http from '../../utils/http';
export const getAIChatsMyPageData = (params) => {
  return http.post('/api/AIChats/GetMyPageData', params);
};

export const addAIChats = (data) => {
  return http.post('/api/AIChats/Add', data);
};
export const deleteAIChats = (id) => {
  return http.delete('/api/AIChats/Delete?Id='+id);
};
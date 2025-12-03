import http from '../../utils/http';
export const getAIChatHistorysPageData = (params) => {
  return http.post('/api/AIChatHistorys/GetPageData', params);
};

export const addAIChatHistorys = (data) => {
  return http.post('/api/AIChatHistorys/Add', data);
};
export const deleteAIChatHistorys = (id) => {
  return http.delete('/api/AIChatHistorys/Delete?Id='+id);
};
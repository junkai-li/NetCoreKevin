import http from '../../utils/http';
export const getAIAppsPageData = (params) => {
  return http.post('/api/AIApps/GetPageData', params);
};

export const addEditAIApp = (data) => {
  return http.post('/api/AIApps/AddEdit', data);
};
export const deleteAIApp = (id) => {
  return http.delete('/api/AIApps/Delete?Id='+id);
};
export const getAIAppsALLList = () => {
  return http.get('/api/AIApps/GetALLList');
};
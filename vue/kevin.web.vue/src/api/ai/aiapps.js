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
export const getMyAIAppsALLList = () => {
  return http.get('/api/AIApps/GetMyALLList');
};
export const getAIAppsDetails = (id) => {
  return http.get('/api/AIApps/GetDetails?Id=' + id);
};
export const NewInitialization = () => {
  return http.get('/api/AIApps/NewInitialization');
};
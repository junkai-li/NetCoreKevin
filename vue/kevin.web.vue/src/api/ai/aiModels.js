import http from '../../utils/http';
export const getAIModelsPageData = (params) => {
  return http.post('/api/AIModels/GetPageData', params);
};

export const addEditAIModel = (data) => {
  return http.post('/api/AIModels/AddEdit', data);
};
export const deleteAIModel = (id) => {
  return http.delete('/api/AIModels/Delete?Id='+id);
};
import http from '../../utils/http';
export const getAIPromptsPageData = (params) => {
  return http.post('/api/AIPrompts/GetPageData', params);
};

export const addEditAIPrompts = (data) => {
  return http.post('/api/AIPrompts/AddEdit', data);
};
export const deleteAIPrompts = (id) => {
  return http.delete('/api/AIPrompts/Delete?Id='+id);
};
export const getAIPromptsALLList = () => {
  return http.get('/api/AIPrompts/GetALLList');
};
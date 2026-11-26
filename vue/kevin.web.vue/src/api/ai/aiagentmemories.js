import http from '../../utils/http';
export const getMemoryPageData = (params) => {
  return http.post('/api/AIAgentMemories/GetPageData', params);
};

export const addEditMemory = (data) => {
  return http.post('/api/AIAgentMemories/AddEdit', data);
};
export const deleteMemory = (id) => {
  return http.delete('/api/AIAgentMemories/Delete?Id='+id);
};
export const getMemoryCountsByAIApps = () => {
  return http.get('/api/AIAgentMemories/GetCountsByAIApps');
};

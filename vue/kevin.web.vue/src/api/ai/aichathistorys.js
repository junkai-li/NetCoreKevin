import http from '../../utils/http';
export const getAIChatHistorysPageData = (params) => {
  return http.post('/api/AIChatHistorys/GetPageData', params);
};

export const addAIChatHistorys = (data, signal) => {
  return http.post('/api/AIChatHistorys/Add', data, { signal });
};
export const deleteAIChatHistorys = (id) => {
  return http.delete('/api/AIChatHistorys/Delete?Id='+id);
};

// 当前会话上下文使用量（未压缩历史 + 压缩摘要估算 / 模型可用提问预算）
export const getAIChatContextUsage = (aIChatsId) => {
  return http.get('/api/AIChatHistorys/GetContextUsage', { params: { AIChatsId: aIChatsId } });
};
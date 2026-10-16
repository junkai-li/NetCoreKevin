import http from '../../utils/http';

// 当前用户 OpenAI 兼容接口 Key 信息（不含明文）
export const getMyOpenAIKey = () => {
  return http.get('/api/OpenAICompatibleKey/GetMyKey');
};

// 生成或重新生成 Key：返回明文（仅此次返回）
export const generateOpenAIKey = (name) => {
  return http.post('/api/OpenAICompatibleKey/Generate', { name: name || null });
};

// 查看当前 Key 明文（用户主动点击"显示"时调用）
export const revealOpenAIKey = () => {
  return http.post('/api/OpenAICompatibleKey/Reveal');
};

// 更新备注名
export const updateOpenAIKeyName = (name) => {
  return http.post('/api/OpenAICompatibleKey/UpdateName', { name: name || null });
};

// 启用 / 禁用
export const setOpenAIKeyEnabled = (enabled) => {
  return http.post('/api/OpenAICompatibleKey/SetEnabled', { enabled });
};

// 删除（作废）当前 Key
export const deleteOpenAIKey = () => {
  return http.delete('/api/OpenAICompatibleKey/Delete');
};

// 获取对接文档：Base URL、可调用的智能体列表、cURL/Python/Node.js 示例
export const getOpenAIIntegrationDoc = () => {
  return http.get('/api/OpenAICompatibleKey/GetIntegrationDoc');
};

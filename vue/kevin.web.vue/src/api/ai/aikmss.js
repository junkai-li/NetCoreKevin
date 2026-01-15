import http from '../../utils/http';

// 获取知识库列表
export const getAIKmssList = (params) => {
  return http.post('/api/AIKmss/GetList', params);
};

// 获取知识库分页数据
export const getAIKmssPageData = (params) => {
  return http.post('/api/AIKmss/GetMyPageData', params);
};

// 获取知识库详情
export const getAIKmssDetails = (id) => {
  return http.get(`/api/AIKmss/GetDetails?id=${id}`);
};

// 添加或编辑知识库
export const addEditAIKmss = (data) => {
  return http.post('/api/AIKmss/AddEdit', data);
};

// 删除知识库
export const deleteAIKmss = (id) => {
  return http.delete(`/api/AIKmss/Delete?Id=${id}`);
};
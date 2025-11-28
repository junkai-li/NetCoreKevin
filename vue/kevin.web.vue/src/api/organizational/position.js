import http from '../../utils/http';

// 岗位信息相关接口
export const getPositionPageData = (params) => {
  return http.post('/api/Position/GetPageData', params);
};

export const getPositionTree = () => {
  return http.get('/api/Position/GetPositionTree');
};
export const getPositionALLList = () => {
  return http.get('/api/Position/GetALLList');
};

export const addEditPosition = (data) => {
  return http.post('/api/Position/AddEdit', data);
};

export const deletePosition = (id) => {
  return http.delete(`/api/Position/Delete?Id=${id}`);
};
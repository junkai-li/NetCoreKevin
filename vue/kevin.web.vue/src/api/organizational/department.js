import http from '../../utils/http';

// 部门信息相关接口
export const getDepartmentPageData = (params) => {
  return http.post('/api/Department/GetPageData', params);
};

export const addEditDepartment = (data) => {
  return http.post('/api/Department/AddEdit', data);
};

export const deleteDepartment = (id) => {
  return http.delete('/api/Department/Delete?Id='+id);
};
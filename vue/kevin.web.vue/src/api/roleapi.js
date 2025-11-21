import http from '../utils/http';

export const getRolePage = (params) => {
  return http.post('/api/Role/GetRolePage', params);
};

export const addEidtRole = (params) => {
  return http.post('/api/Role/AddEidt', params);
};

export const deleteRole = (roleId) => {
  return http.delete(`/api/Role/deleteRole?roleId=${roleId}`);
};

// 获取角色详情
export const getRoleById = (roleId) => {
  return http.get(`/api/Role/GetRoleById?roleId=${roleId}`);
};

// 获取所有角色
export const getAllRoles = () => {
  return http.get('/api/Role/GetAllRoles');
};
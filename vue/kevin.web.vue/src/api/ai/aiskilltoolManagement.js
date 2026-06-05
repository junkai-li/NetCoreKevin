import http from '../../utils/http';
//获取AI技能工具列表
export const getAISkillToolManagementPageData = (params) => {
  return http.post('/api/AISkillToolManagement/GetPageData', params);
};
//获取AI技能工具详情
export const getAISkillToolManagementById = (id) => {
  return http.get('/api/AISkillToolManagement/GetById?Id=' + id);
};
//添加/编辑AI技能工具
export const addEditAISkillToolManagement = (data) => {
  return http.post('/api/AISkillToolManagement/AddEdit', data);
};
//删除AI技能工具
export const deleteAISkillToolManagement = (id) => {
  return http.delete('/api/AISkillToolManagement/Delete?Id='+id);
};
//获取AI工具列表
export const GetAllTools = () => {
  return http.get('/api/AISkillToolManagement/GetAllTools');
};
//获取AISkill技能列表
export const GetAllSkills = () => {
  return http.get('/api/AISkillToolManagement/GetAllSkills');
};
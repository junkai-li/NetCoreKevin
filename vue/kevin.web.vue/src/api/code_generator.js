import http from '../utils/http';
// 自动生成代码相关接口
//获取区域列表 
export const GetAreaNames = () => {
  return http.get('/api/CodeGenerator/GetAreaNames');
};

//获取区域下的实体列表 
export const GetAreaNameEntityItems = (name) => {
  return http.get('/api/CodeGenerator/GetAreaNameEntityItems?name='+name);
};

//生成仓储和服务基础代码 
export const BulidCode = (data) => {
  return http.post('/api/CodeGenerator/BulidCode',data);
};
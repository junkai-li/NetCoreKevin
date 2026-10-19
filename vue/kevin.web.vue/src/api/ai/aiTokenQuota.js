import http from '../../utils/http';

// 获取当前登录用户当月 Token 额度
export const getMyTokenQuota = () => {
  return http.get('/api/AIUserTokenQuota/GetMyQuota');
};

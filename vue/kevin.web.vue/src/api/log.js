import http from '../utils/http';

export const GetHttpLogPageData = (params) => {
  return http.post('/api/Log/GetHttpLogPageData', params);
};

export const GetOSLogPageData = (params) => {
  return http.post('/api/Log/GetOSLogPageData', params);
};
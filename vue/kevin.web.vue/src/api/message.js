import http from '../utils/http';

export const GetMyNoReadCount = () => {
  return http.get('/api/Message/GetMyNoReadCount');
};
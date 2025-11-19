import http from '../utils/http';

export const GetGuId = () => {
  return http.get(`/api/base/GetGuId`);
};
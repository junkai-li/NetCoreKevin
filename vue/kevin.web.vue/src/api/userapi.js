import http from '../utils/http';

export const login = (acctount, password,tenantId) => 
{
  return http.post(`/api/Authorize/GetToken`,{ name:acctount, passWord: password,tenantId:tenantId });
};
export const getTokenUser = () => {
  return   http.get(`/api/User/GetUser`);
};

export const createUser = (userData) => {
  return http.post('/users', userData);
};

export const getUserProfile = () => {
  // 模拟用户数据，实际项目中应该调用真实API
  return new Promise((resolve) => {
    setTimeout(() => {
      resolve({
        "id": "eef5525d-5d64-46ad-8d64-79fb3ad9724f",
        "name": "admin",
        "nickName": "admin",
        "phone": "admin",
        "email": "admin",
        "roles": [
          {
            "id": "c23301b7-f9e0-464c-b76d-1f0a5a557548",
            "name": "admin",
            "remarks": "admin",
            "createTime": "2025/11/14 10:20:05"
          }
        ],
        "passWord": "",
        "createTime": "2025/11/14 10:20:05",
        "headImgs": []
      });
    }, 500);
  });
};
import axios from 'axios'
import { message } from 'ant-design-vue';
const apiBaseUrl = process.env.VUE_APP_API_BASE_URL
// 创建 Axios 实例
const myAxios = axios.create({
  baseURL: apiBaseUrl,
  timeout: 60000,
  withCredentials: true,
}) 
 
// 添加请求拦截器
myAxios.interceptors.request.use(config => {
  // 在发送请求之前做些什么，例如设置 token
  const token = localStorage.getItem('token');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
}, error => { 
      if(error.status==400){
        message.error(error.response.data.errMsg) 
        
      } 
      return error; 
});

import FileHandler from "../utils/fileHandler";
// 全局响应拦截器
myAxios.interceptors.response.use( 
  function (response) { 
    // 判断是否为文件下载响应
    const contentType = response.headers['content-type'];
    if (contentType && (contentType.indexOf('application/octet-stream') !== -1 || 
                        contentType.indexOf('application/force-download') !== -1 ||
                        contentType.indexOf('application/vnd.ms-excel') !== -1 ||
                        contentType.indexOf('application/vnd.openxmlformats-officedocument') !== -1 ||
                        contentType.indexOf('application/pdf') !== -1)) {
      // 文件下载响应，直接返回
      return FileHandler.responsedownload(response);
    }

    try { 
      const { data } = response
      // 未登录
      if (data.code === 401) {
        // 不是获取用户信息的请求，并且用户目前不是已经在用户登录页面，则跳转到登录页面
        if (
          !response.request.responseURL.includes('user/get/login') &&
          !window.location.pathname.includes('/login')
        ) {
          message.error('请先登录')
          window.location.href = `/login?redirect=${window.location.href}`
        }
      }
      if(data.code==400){ 
        console.log(data.errMsg)    
        message.error(data.errMsg) 
        return Promise.reject(new Error(data.errMsg)) 
      }
      return response
    } catch (error) { 
      if(error.status==400){
        message.error(error.response.data.err_msg) 
        return Promise.reject(new Error(error.response.data.err_msg)) 
      } 
      return error; 
    }
  },
  function (error) {
    // Any status codes that falls outside the range of 2xx cause this function to trigger
    // Do something with response error  
    console.log(error)
    if(error.status==400){
      message.error(error.response.data.err_msg)  
      return Promise.reject(new Error(error.response.data.err_msg)) 
    }  
    return Promise.reject(new Error(error)) 
  }
);

export default myAxios;
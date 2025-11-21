import axios from 'axios';
import { message } from 'ant-design-vue';

/**
 * 下载文件
 * @param {string} url - 文件下载地址
 * @param {string} fileName - 保存的文件名
 * @param {Object} params - 请求参数
 */
export async function downloadFile(url, fileName, params = {}) {
  try {
    const response = await axios({
      method: 'GET',
      url: url,
      params: params,
      responseType: 'blob'
    });

    // 创建下载链接
    const blob = new Blob([response.data]);
    const link = document.createElement('a');
    link.href = window.URL.createObjectURL(blob);
    link.download = fileName || getFileNameFromHeaders(response.headers) || 'downloaded_file';
    link.click();
    window.URL.revokeObjectURL(link.href);
    
    message.success('文件下载成功');
  } catch (error) {
    console.error('文件下载失败:', error);
    message.error('文件下载失败');
    throw error;
  }
}

/**
 * 从响应头中提取文件名
 * @param {Object} headers - 响应头
 */
function getFileNameFromHeaders(headers) {
  const contentDisposition = headers['content-disposition'];
  if (contentDisposition) {
    const match = contentDisposition.match(/filename="?([^"]+)"?/);
    if (match && match[1]) {
      return decodeURIComponent(match[1]);
    }
  }
  return null;
}

/**
 * 上传文件
 * @param {string} url - 上传地址
 * @param {File} file - 要上传的文件
 * @param {Object} extraData - 额外的表单数据
 */
export async function uploadFile(url, file, extraData = {}) {
  try {
    const formData = new FormData();
    formData.append('file', file);
    
    // 添加额外数据
    Object.keys(extraData).forEach(key => {
      formData.append(key, extraData[key]);
    });

    const response = await axios({
      method: 'POST',
      url: url,
      data: formData,
      headers: {
        'Content-Type': 'multipart/form-data'
      }
    });

    message.success('文件上传成功');
    return response.data;
  } catch (error) {
    console.error('文件上传失败:', error);
    message.error('文件上传失败');
    throw error;
  }
}

/**
 * 批量上传文件
 * @param {string} url - 上传地址
 * @param {Array<File>} files - 要上传的文件列表
 * @param {Object} extraData - 额外的表单数据
 */
export async function uploadFiles(url, files, extraData = {}) {
  try {
    const formData = new FormData();
    
    // 添加所有文件
    files.forEach((file, index) => {
      formData.append(`files[${index}]`, file);
    });
    
    // 添加额外数据
    Object.keys(extraData).forEach(key => {
      formData.append(key, extraData[key]);
    });

    const response = await axios({
      method: 'POST',
      url: url,
      data: formData,
      headers: {
        'Content-Type': 'multipart/form-data'
      }
    });

    message.success('文件上传成功');
    return response.data;
  } catch (error) {
    console.error('文件上传失败:', error);
    message.error('文件上传失败');
    throw error;
  }
}

/**
 * 在浏览器中预览文件
 * @param {Blob} blob - 文件blob
 * @param {string} mimeType - 文件MIME类型
 */
export function previewFile(blob, mimeType) {
  const url = window.URL.createObjectURL(new Blob([blob], { type: mimeType }));
  const win = window.open(url, '_blank');
  if (!win || win.closed || typeof win.closed == 'undefined') {
    message.warning('弹窗被阻止，请允许弹窗以预览文件');
  }
  return url;
}
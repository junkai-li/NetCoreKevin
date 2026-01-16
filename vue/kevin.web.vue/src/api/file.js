import http from '../utils/http';

/**
 * 文件相关的API接口
 */

// 单文件上传
export const uploadFile = (business, key, sign, file, options = {}) => {
  const formData = new FormData();
  formData.append('file', file);

  return http.post('/api/File/UploadFile', formData, {
    headers: {
      'Content-Type': 'multipart/form-data'
    },
    params: {
      business,
      key,
      sign
    },
    ...options
  });
};

// 批量文件上传
export const batchUploadFile = (business, key, sign, files, options = {}) => {
  const formData = new FormData();
  
  for (let i = 0; i < files.length; i++) {
    formData.append('files', files[i]);
  }

  return http.post('/api/File/BatchUploadFile', formData, {
    headers: {
      'Content-Type': 'multipart/form-data'
    },
    params: {
      business,
      key,
      sign
    },
    ...options
  });
};

// 远程文件上传
export const remoteUploadFile = (business, key, sign, fileInfo, options = {}) => {
  return http.post('/api/File/RemoteUploadFile', fileInfo, {
    params: {
      business,
      key,
      sign
    },
    ...options
  });
};

// 获取文件
export const getFileById = (fileId, options = {}) => {
  return http.get(`/api/File/GetFile?fileid=${fileId}`, {
    responseType: 'blob', // 返回文件流
    ...options
  }).then((response) => {
    // 检查响应是否为错误消息（JSON格式）
    if (response.type === "application/json") {
      // 如果是JSON响应，将其读取为文本并返回
      const reader = new FileReader();
      reader.onload = () => {
        console.error('获取文件失败:', reader.result);
      };
      reader.readAsText(response);
      return Promise.reject(new Error('获取文件失败'));
    }
     
    let filename = "downloaded_file"; 
    
    // 创建Blob对象
    const blob = new Blob([response]);
    // 创建下载链接
    const url = window.URL.createObjectURL(blob);
    const a = document.createElement("a");
    a.href = url;
    a.download = filename;
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
    window.URL.revokeObjectURL(url);
  }).catch((error) => {
    console.error('下载文件失败:', error);
    throw error;
  });
};

// 获取图片
export const getImageById = (fileId, width = 0, height = 0, options = {}) => {
  let url = `/api/File/GetImage?fileId=${fileId}`;
  if (width) url += `&width=${width}`;
  if (height) url += `&height=${height}`;

  return http.get(url, {
    responseType: 'blob', // 返回图片流
    ...options
  });
};

// 获取文件路径
export const getFilePathById = (fileId, options = {}) => {
  return http.get(`/api/File/GetFilePath?fileid=${fileId}`, options);
};

// 删除文件
export const deleteFileById = (id, options = {}) => {
  return http.delete(`/api/File/DeleteFile?id=${id}`, options);
};

/**
 * 通用文件上传方法
 * @param {string} business - 业务领域
 * @param {string} key - 记录值
 * @param {string} sign - 自定义标记
 * @param {File|File[]} file - 单个文件或文件数组
 * @param {Object} options - 额外选项
 * @returns {Promise}
 */
export const upload = (business, key, sign, file, options = {}) => {
  if (Array.isArray(file)) {
    return batchUploadFile(business, key, sign, file, options);
  } else {
    return uploadFile(business, key, sign, file, options);
  }
};

export default {
  uploadFile,
  batchUploadFile,
  remoteUploadFile,
  getFileById,
  getImageById,
  getFilePathById,
  deleteFileById,
  upload
};
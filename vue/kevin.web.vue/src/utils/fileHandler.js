import { message } from 'ant-design-vue';
import myAxios from './http';

class FileHandler {
  
    /**
   * 下载文件
   * @param {string} url - 下载地址
   * @param {string} filename - 文件名
   * @param {Object} params - 请求参数
   */
  static async responsedownload(response) {
    try { 
      this.saveFile(response.data,this.getFilenameFromResponse(response));
      message.success('文件下载成功');
      return response;
    } catch (error) {
      message.error('文件下载失败: ' + (error.message || '未知错误'));
      throw error;
    }
  }
  /**
   * 下载文件
   * @param {string} url - 下载地址
   * @param {string} filename - 文件名
   * @param {Object} params - 请求参数
   */
  static async download(url, filename, params = {}) {
    try {
      const response = await myAxios({
        method: 'GET',
        url,
        params,
        responseType: 'blob'
      });

      this.saveFile(response.data, filename || this.getFilenameFromResponse(response));
      message.success('文件下载成功');
      return response;
    } catch (error) {
      message.error('文件下载失败: ' + (error.message || '未知错误'));
      throw error;
    }
  }

  /**
   * 上传文件
   * @param {string} url - 上传地址
   * @param {File|ArrayBuffer|Blob} file - 文件对象
   * @param {Object} options - 上传选项
   */
  static async upload(url, file, options = {}) {
    try {
      const formData = new FormData();
      
      // 根据不同类型处理文件
      if (file instanceof File) {
        formData.append('file', file);
      } else if (file instanceof Blob) {
        formData.append('file', file, options.filename || 'file.blob');
      } else {
        throw new Error('不支持的文件类型');
      }

      // 添加额外参数
      if (options.extraData) {
        Object.keys(options.extraData).forEach(key => {
          formData.append(key, options.extraData[key]);
        });
      }

      const config = {
        method: 'POST',
        url,
        data: formData,
        headers: {
          'Content-Type': 'multipart/form-data'
        }
      };

      // 添加进度回调
      if (options.onProgress) {
        config.onUploadProgress = options.onProgress;
      }

      const response = await myAxios(config);
      message.success('文件上传成功');
      return response.data;
    } catch (error) {
      message.error('文件上传失败: ' + (error.message || '未知错误'));
      throw error;
    }
  }

  /**
   * 批量上传文件
   * @param {string} url - 上传地址
   * @param {File[]} files - 文件列表
   * @param {Object} options - 上传选项
   */
  static async uploadMultiple(url, files, options = {}) {
    try {
      const formData = new FormData();
      
      files.forEach((file) => {
        formData.append(`files`, file);
      });

      // 添加额外参数
      if (options.extraData) {
        Object.keys(options.extraData).forEach(key => {
          formData.append(key, options.extraData[key]);
        });
      }

      const config = {
        method: 'POST',
        url,
        data: formData,
        headers: {
          'Content-Type': 'multipart/form-data'
        }
      };

      // 添加进度回调
      if (options.onProgress) {
        config.onUploadProgress = options.onProgress;
      }

      const response = await myAxios(config);
      message.success(`${files.length} 个文件上传成功`);
      return response.data;
    } catch (error) {
      message.error('文件批量上传失败: ' + (error.message || '未知错误'));
      throw error;
    }
  }

  /**
   * 从响应中获取文件名
   * @param {Object} response - HTTP响应对象
   */
  static getFilenameFromResponse(response) {
    const disposition = response.headers['content-disposition'];
    if (disposition && disposition.indexOf('attachment') !== -1) {
      const filenameRegex = /filename[^;=\n]*=((['"]).*?\2|[^;\n]*)/;
      const matches = filenameRegex.exec(disposition);
      if (matches != null && matches[1]) {
        return decodeURIComponent(matches[1].replace(/['"]/g, ''));
      }
    }
    return 'downloaded_file';
  }

  /**
   * 保存文件到本地
   * @param {Blob} blob - 文件blob数据
   * @param {string} filename - 文件名
   */
  static saveFile(blob, filename) {
    // 创建下载链接
    const url = window.URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.style.display = 'none';
    link.href = url;
    link.setAttribute('download', filename);
    
    // 触发下载
    document.body.appendChild(link);
    link.click();
    
    // 清理
    document.body.removeChild(link);
    window.URL.revokeObjectURL(url);
  }

  /**
   * 预览文件
   * @param {Blob} blob - 文件blob数据
   * @param {string} mimeType - MIME类型
   */
  static preview(blob, mimeType) {
    const url = window.URL.createObjectURL(new Blob([blob], { type: mimeType }));
    const win = window.open(url, '_blank');
    
    if (!win || win.closed || typeof win.closed == 'undefined') {
      message.warning('弹窗被阻止，请允许弹窗以预览文件');
    }
    
    return url;
  }

  /**
   * 将Base64转换为Blob
   * @param {string} base64 - Base64字符串
   * @param {string} mimeType - MIME类型
   */
  static base64ToBlob(base64, mimeType) {
    const byteString = atob(base64.split(',')[1]);
    const ab = new ArrayBuffer(byteString.length);
    const ia = new Uint8Array(ab);
    
    for (let i = 0; i < byteString.length; i++) {
      ia[i] = byteString.charCodeAt(i);
    }
    
    return new Blob([ab], { type: mimeType });
  }

  /**
   * 将Blob转换为Base64
   * @param {Blob} blob - Blob对象
   */
  static blobToBase64(blob) {
    return new Promise((resolve, reject) => {
      const reader = new FileReader();
      reader.onloadend = () => resolve(reader.result);
      reader.onerror = reject;
      reader.readAsDataURL(blob);
    });
  }

  /**
   * 获取文件信息
   * @param {File} file - 文件对象
   */
  static getFileInfo(file) {
    return {
      name: file.name,
      size: file.size,
      type: file.type,
      lastModified: file.lastModified,
      readableSize: this.formatFileSize(file.size)
    };
  }

  /**
   * 格式化文件大小
   * @param {number} bytes - 字节数
   */
  static formatFileSize(bytes) {
    if (bytes === 0) return '0 Bytes';
    
    const k = 1024;
    const sizes = ['Bytes', 'KB', 'MB', 'GB'];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    
    return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i];
  }
}

export default FileHandler;
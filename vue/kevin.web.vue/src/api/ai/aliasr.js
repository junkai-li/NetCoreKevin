import http from '../../utils/http';

/**
 * 获取阿里云实时语音识别 Token
 * 后端需要实现 /api/AliAsr/GetToken 接口，使用阿里云 AK/SK 生成临时 Token
 * @returns {Promise<{code: number, data: {token: string, appKey: string}}>}
 */
export const getAliAsrToken = () => {
  return http.post('/api/AliAsr/GetToken');
};
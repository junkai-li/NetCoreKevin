/**
 * 阿里云 ASR 适配器 —— 兼容浏览器 SpeechRecognition 接口
 *
 * 提供与 window.SpeechRecognition 一致的 API（start / stop / abort / onresult / onerror / onend），
 * 底层使用阿里云 NLS 实时语音识别 WebSocket 服务。
 *
 * 替换方式：
 *   const rec = new AliAsrAdapter({ appKey, token });
 *   // 后续使用方式与原 SpeechRecognition 完全一致
 *   rec.onresult = (event) => { ... };
 *   rec.start();
 *   rec.stop();
 */

import { createAliAsr } from './aliyunAsr.js';

export class AliAsrAdapter {
  constructor(options) {
    this.appKey = options.appKey;
    this.token = options.token;
    this.lang = 'zh-CN';
    this.continuous = true;
    this.interimResults = true;
    this.maxAlternatives = 1;

    // 回调
    this.onresult = null;
    this.onerror = null;
    this.onend = null;

    this._asr = null;
    this._running = false;
    this._totalText = ''; // 累积的完整文本
    this._resultIndex = 0;
    this._accumulatedResults = []; // 模拟 SpeechRecognition 的累积 results
    this._lastText = ''; // 上次文本（避免相同内容重复触发 onresult）
    this._lastFireTime = 0; // 上次触发 onresult 的时间（节流）
  }

  /**
   * 开始识别
   */
  start() {
    if (this._running) return;
    this._running = true;
    this._totalText = '';
    this._resultIndex = 0;
    this._accumulatedResults = [];
    this._lastText = '';
    this._lastFireTime = 0;

    this._asr = createAliAsr({
      appKey: this.appKey,
      token: this.token,
      onResult: (text, isFinal) => this._handleResult(text, isFinal),
      onError: (err) => this._handleError(err),
      onStateChange: (state) => this._handleStateChange(state),
    });

    this._asr.start();
  }

  /**
   * 停止识别（等最终结果）
   */
  stop() {
    if (!this._running) return;
    this._running = false;
    this._asr?.stop();
    this._asr = null;
    this._fireEnd();
  }

  /**
   * 立即中止识别
   */
  abort() {
    if (!this._running && !this._asr) return;
    this._running = false;
    this._asr?.stop();
    this._asr = null;
    this._fireEnd();
  }

  /**
   * 重置累积文本缓冲区（不停止识别，用于字幕模式连续聆听）
   * 清空已累积的 results，新的识别结果会重新开始累积
   */
  resetBuffer() {
    this._accumulatedResults = [];
    this._resultIndex = 0;
    this._totalText = '';
    this._lastText = '';
    this._lastFireTime = 0;
  }

  // ========== 内部处理 ==========

  _handleResult(text, isFinal) {
    if (!this.onresult) return;

    // 节流：相同文本且非 final 时跳过，避免频繁触发 onresult 导致 Vue 响应式卡顿
    if (text === this._lastText && !isFinal) return;
    // 非 final 结果最多每 150ms 触发一次
    const now = Date.now();
    if (!isFinal && now - this._lastFireTime < 150) return;

    this._lastText = text;
    this._lastFireTime = now;
    this._totalText = text;

    const result = {
      isFinal,
      [0]: { transcript: text, confidence: 1.0 },
      length: 1,
    };

    // _accumulatedResults 累积逻辑：
    // - 非 final 结果替换上一条（如果上一条也是 interim），否则追加
    // - final 结果：先移除上一条 interim（避免文字重复），再追加
    // 这样 onresult 处理函数遍历全部 results 累加文字时能得到完整文本
    if (isFinal) {
      const last = this._accumulatedResults[this._accumulatedResults.length - 1];
      if (last && !last.isFinal) {
        this._accumulatedResults.pop(); // 移除上一条 interim，避免重复
      }
      this._accumulatedResults.push(result);
      this._resultIndex++;
    } else {
      const last = this._accumulatedResults[this._accumulatedResults.length - 1];
      if (last && !last.isFinal) {
        this._accumulatedResults[this._accumulatedResults.length - 1] = result;
      } else {
        this._accumulatedResults.push(result);
      }
    }

    // 构造类 SpeechRecognitionEvent
    const event = {
      resultIndex: this._resultIndex,
      results: this._accumulatedResults,
      type: 'result',
    };

    if (isFinal) {
      this._resultIndex++;
    }

    this.onresult(event);
  }

  _handleError(err) {
    if (this.onerror) {
      const msg = err?.message || err?.toString() || 'asr_error';
      this.onerror({ error: msg });
    }
  }

  _handleStateChange(state) {
    if (state === 'error' || state === 'stopped') {
      if (this._running) {
        this._running = false;
        this._fireEnd();
      }
    }
  }

  _fireEnd() {
    if (this.onend) {
      const cb = this.onend;
      this.onend = null;
      cb();
    }
  }
}
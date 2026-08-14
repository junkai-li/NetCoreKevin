/**
 * 阿里云实时语音识别 (ASR) 前端模块
 *
 * 使用阿里云 NLS 实时语音识别 WebSocket 服务，将麦克风音频流实时转换为文字。
 * 需要后端提供 /api/AliAsr/GetToken 接口获取临时 Token。
 */

// 阿里云实时语音识别 WebSocket 端点
const NLS_GATEWAY = 'wss://nls-gateway.cn-shanghai.aliyuncs.com/ws/v1';

// 音频参数
const SAMPLE_RATE = 16000; // 16kHz
const CHUNK_INTERVAL_MS = 200; // 每 200ms 发送一次音频数据

/**
 * 创建阿里云实时 ASR 实例
 */
export function createAliAsr(options) {
  const { onResult, onError, onStateChange, appKey, token } = options;

  let ws = null;
  let audioContext = null;
  let mediaStream = null;
  let mediaSource = null;
  let processor = null;
  let chunkTimer = null;
  let audioQueue = [];
  let taskId = null;
  let isListening = false;
  let isConnected = false;

  function genUuid() {
    // 阿里云 NLS 要求 UUID 不带横线（32位16进制字符）
    return 'xxxxxxxxxxxx4xxxyxxxxxxxxxxxxxxx'.replace(/[xy]/g, (c) => {
      const r = (Math.random() * 16) | 0;
      const v = c === 'x' ? r : (r & 0x3) | 0x8;
      return v.toString(16);
    });
  }

  function setState(state) {
    isListening = state === 'listening';
    onStateChange?.(state);
  }

  function connect() {
    const url = `${NLS_GATEWAY}?appkey=${appKey}&token=${token}`;
    ws = new WebSocket(url);
    ws.binaryType = 'arraybuffer';

    ws.onopen = () => {
      isConnected = true;
      const startCmd = {
        header: {
          message_id: genUuid(),
          task_id: taskId,
          namespace: 'SpeechTranscriber',
          name: 'StartTranscription',
          appkey: appKey,
        },
        payload: {
          format: 'pcm',
          sample_rate: SAMPLE_RATE,
          enable_intermediate_result: true,
          enable_punctuation_prediction: true,
          enable_inverse_text_normalization: true,
        },
      };
      ws.send(JSON.stringify(startCmd));
    };

    ws.onmessage = (event) => {
      try {
        const msg = JSON.parse(event.data);
        handleServerMessage(msg);
      } catch (e) {
        // 非 JSON 数据忽略
      }
    };

    ws.onerror = () => {
      onError?.(new Error('aliyun_asr_error'));
      setState('error');
      cleanup();
    };

    ws.onclose = () => {
      isConnected = false;
      setState('stopped');
    };
  }

  function handleServerMessage(msg) {
    const header = msg.header;
    if (!header) return;

    const name = header.name;
    // 阿里云 NLS 服务器使用 status 字段（不是 status_code）
    const statusCode = header.status || header.status_code;

    // 启动成功（服务器返回 TranscriptionStarted 或 StartTranscription）
    if ((name === 'TranscriptionStarted' || name === 'StartTranscription') && statusCode === 20000000) {
      setState('listening');
      startAudioCapture();
      return;
    }

    // 启动失败
    if ((name === 'TranscriptionStarted' || name === 'StartTranscription') && statusCode !== 20000000) {
      onError?.(new Error('aliyun_asr_error'));
      setState('error');
      cleanup();
      return;
    }

    // 中间结果
    if (name === 'TranscriptionResultChanged') {
      const payload = msg.payload;
      if (payload?.result) {
        onResult?.(payload.result, false);
      }
      return;
    }

    // 最终结果
    if (name === 'TranscriptionCompleted') {
      const payload = msg.payload;
      if (payload?.result) {
        onResult?.(payload.result, true);
      }
      return;
    }

    // 句子结束
    if (name === 'SentenceEnd') {
      const payload = msg.payload;
      if (payload?.result) {
        onResult?.(payload.result, true);
      }
      return;
    }

    // 任务失败
    if (name === 'TaskFailed') {
      const errMsg = header.status_text || 'task_failed';
      onError?.(new Error(errMsg));
      setState('error');
      cleanup();
      return;
    }
  }

  async function startAudioCapture() {
    try {
      mediaStream = await navigator.mediaDevices.getUserMedia({
        audio: {
          sampleRate: SAMPLE_RATE,
          channelCount: 1,
          echoCancellation: true,
          noiseSuppression: true,
        },
      });

      audioContext = new AudioContext({ sampleRate: SAMPLE_RATE });
      // AudioContext 在非用户手势中创建后处于 suspended 状态，
      // 必须 resume 后 onaudioprocess 才会触发采集音频数据
      if (audioContext.state === 'suspended') {
        await audioContext.resume();
      }
      mediaSource = audioContext.createMediaStreamSource(mediaStream);
      processor = audioContext.createScriptProcessor(4096, 1, 1);

      processor.onaudioprocess = (event) => {
        if (!isConnected) return;
        const inputData = event.inputBuffer.getChannelData(0);
        const pcmData = float32ToInt16(inputData);
        audioQueue.push(pcmData);
      };

      mediaSource.connect(processor);
      processor.connect(audioContext.destination);

      chunkTimer = setInterval(sendAudioChunk, CHUNK_INTERVAL_MS);
    } catch (err) {
      onError?.(new Error('aliyun_asr_error'));
      setState('error');
      cleanup();
    }
  }

  function sendAudioChunk() {
    if (!isConnected || audioQueue.length === 0) return;

    const totalLen = audioQueue.reduce((sum, buf) => sum + buf.byteLength, 0);
    const merged = new Uint8Array(totalLen);
    let offset = 0;
    for (const buf of audioQueue) {
      merged.set(new Uint8Array(buf), offset);
      offset += buf.byteLength;
    }
    audioQueue = [];

    ws.send(merged.buffer);
  }

  function float32ToInt16(float32Array) {
    const len = float32Array.length;
    const int16Array = new Int16Array(len);
    for (let i = 0; i < len; i++) {
      const s = Math.max(-1, Math.min(1, float32Array[i]));
      int16Array[i] = s < 0 ? s * 0x8000 : s * 0x7fff;
    }
    return int16Array.buffer;
  }

  function cleanup() {
    if (chunkTimer) {
      clearInterval(chunkTimer);
      chunkTimer = null;
    }
    audioQueue = [];

    if (processor) {
      processor.disconnect();
      processor = null;
    }
    if (mediaSource) {
      mediaSource.disconnect();
      mediaSource = null;
    }
    if (audioContext) {
      audioContext.close().catch(() => {});
      audioContext = null;
    }
    if (mediaStream) {
      mediaStream.getTracks().forEach((t) => t.stop());
      mediaStream = null;
    }
    if (ws) {
      ws.onopen = null;
      ws.onmessage = null;
      ws.onerror = null;
      ws.onclose = null;
      if (ws.readyState === WebSocket.OPEN || ws.readyState === WebSocket.CONNECTING) {
        ws.close();
      }
      ws = null;
    }
  }

  async function start() {
    if (isListening) return;
    taskId = genUuid();
    setState('connecting');
    connect();
  }

  function stop() {
    if (ws && isConnected) {
      const stopCmd = {
        header: {
          message_id: genUuid(),
          task_id: taskId,
          namespace: 'SpeechTranscriber',
          name: 'StopTranscription',
          appkey: appKey,
        },
      };
      try {
        ws.send(JSON.stringify(stopCmd));
      } catch (e) {
        // ignore
      }
    }
    cleanup();
    setState('stopped');
  }

  return { start, stop, get isListening() { return isListening; } };
}
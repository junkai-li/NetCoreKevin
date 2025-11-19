// resizeObserverErrorHandler.js
// 用于处理 ResizeObserver 相关错误的工具函数

/**
 * 忽略 ResizeObserver 错误和警告
 * 这可以解决 "ResizeObserver loop completed with undelivered notifications" 错误
 */
export function ignoreResizeObserverErrors() {
  // 保存原始的错误处理函数
  const originalErrorHandler = window.onerror;
  const originalWarnHandler = window.onwarn;

  // 重写 window.onerror
  window.onerror = function(message, source, lineno, colno, error) {
    // 如果是 ResizeObserver 相关错误，则忽略
    if (message.includes('ResizeObserver')) {
      return true;
    }
    // 其他错误调用原始处理函数
    if (originalErrorHandler) {
      return originalErrorHandler(message, source, lineno, colno, error);
    }
    return false;
  };

  // 重写 window.onwarn (如果存在)
  window.onwarn = function(message) {
    // 如果是 ResizeObserver 相关警告，则忽略
    if (message.includes('ResizeObserver')) {
      return true;
    }
    // 其他警告调用原始处理函数
    if (originalWarnHandler) {
      return originalWarnHandler(message);
    }
    return false;
  };

  // 返回恢复函数
  return function restore() {
    window.onerror = originalErrorHandler;
    window.onwarn = originalWarnHandler;
  };
}

/**
 * 更加精细的 ResizeObserver 错误处理方式
 * 使用 setTimeout 来避免循环限制错误
 */
export class SafeResizeObserver {
  constructor(callback, options) {
    this.callback = callback;
    this.options = options;
    this.ro = new ResizeObserver((entries) => {
      // 使用 setTimeout 来避免 ResizeObserver 循环限制错误
      setTimeout(() => {
        this.callback(entries);
      }, 0);
    });
  }

  observe(target, options) {
    this.ro.observe(target, options);
  }

  unobserve(target) {
    this.ro.unobserve(target);
  }

  disconnect() {
    this.ro.disconnect();
  }
}
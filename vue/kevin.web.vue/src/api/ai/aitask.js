import http from '../../utils/http';
//查询我的任务（旧接口，字符串拼接格式，保留兼容）
export const getTaskList = () => {
  return http.post('/api/AITasks/GetTaskList');
};
//分页查询我的任务（结构化：任务类型/暂停状态/下次与上次执行时间）
export const getMyTaskPage = (params) => {
  return http.post('/api/AITasks/GetMyTaskPage', params);
};
//删除任务 返回 true or false
export const removeCronTask = (name) => {
  return http.delete('/api/AITasks/RemoveCronTask?name='+name);
};
//执行任务 返回 true or false
export const triggerCronTask = (name) => {
  return http.get('/api/AITasks/TriggerCronTask?name='+name);
};
//暂停任务（暂停后不再执行，可恢复） 返回 true or false
export const pauseTask = (name) => {
  return http.post('/api/AITasks/PauseTask?name='+name);
};
//恢复任务（按落库配置重新注册调度） 返回 true or false
export const resumeTask = (name) => {
  return http.post('/api/AITasks/ResumeTask?name='+name);
};

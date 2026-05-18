import http from '../../utils/http';
//查询我的任务 返回数据list $"name:{r.Id} | Cron:{r.Cron} | Next:{next} | Last:{last} | TimeZone:{r.TimeZoneId}";
export const getTaskList = () => {
  return http.post('/api/AITasks/GetTaskList');
};
//删除任务 返回 true or false
export const removeCronTask = (name) => {
  return http.delete('/api/AITasks/RemoveCronTask?name='+name);
};
//执行任务 返回 true or false
export const triggerCronTask = (name) => {
  return http.get('/api/AITasks/TriggerCronTask?name='+name);
};
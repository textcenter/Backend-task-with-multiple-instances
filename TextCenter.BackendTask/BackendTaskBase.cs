using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
namespace TextCenter.BackendTask
{
    public class BackendTaskBase
    {
        public BackendTaskInfo TaskInfo { get; set; }
        public int EndTaskSleepMiliSecond { get; set; }
        public int InstanceIndex { get; set; }
      
        private Action<BackendTaskInfo> TaskExcecutor { get; set; }
        
        public BackendTaskBase(string taskName, string taskDescription)
        {
            TaskInfo = new BackendTaskInfo()
            {
                TaskName = taskName,
                TaskDescription = taskDescription,
            };


            TaskInfo.TaskStatus = BackendTaskInfo.RunningStatuses.Stopped;
            EndTaskSleepMiliSecond = 1000;
        }
        public void RegisterCallback(Action<BackendTaskInfo> taskExcecutor)
        {
            TaskExcecutor = taskExcecutor;
        }
        public virtual void Start(int instanceCount)
        {
            
           
            if (TaskExcecutor == null)
                throw new Exception("There is no taskExcecutor register");
            if (TaskInfo.TaskStatus == BackendTaskInfo.RunningStatuses.Running)
                return;
            TaskInfo.TaskStatus = BackendTaskInfo.RunningStatuses.Running;
            Task.Factory.StartNew(() =>
            {
                bool processing = false;
                while (TaskInfo.TaskStatus == BackendTaskInfo.RunningStatuses.Running)
                {
                    try
                    {

                        if (!processing)
                        {
                            ++TaskInfo.ProcessCount;
                            processing = true;
                            TaskInfo.CurrentInstance = this;
                            TaskExcecutor.Invoke(TaskInfo);

                        }
                        TaskInfo.LastProcess = DateTime.UtcNow;
                    }
                    catch (Exception ex)
                    {

                        ++TaskInfo.ProcessCount;
                        TaskInfo.LastError = ex.Message;

                    }
                    processing = false;
                    Thread.Sleep(EndTaskSleepMiliSecond);
                }
                TaskInfo.TaskStatus = BackendTaskInfo.RunningStatuses.Stopped;
            }, TaskCreationOptions.LongRunning);

        }
        public virtual void Stop()
        {
            TaskInfo.TaskStatus = BackendTaskInfo.RunningStatuses.Stopping;
            SpinWait.SpinUntil(() => { return TaskInfo.TaskStatus == BackendTaskInfo.RunningStatuses.Stopped; });
            TaskInfo.TaskStatus = BackendTaskInfo.RunningStatuses.Stopped;
            Console.WriteLine($"Instance {this.InstanceIndex} stopped");
        }
    }
}

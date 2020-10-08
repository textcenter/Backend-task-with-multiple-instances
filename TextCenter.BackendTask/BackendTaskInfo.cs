using System;
using System.Collections.Generic;
using System.Text;

namespace TextCenter.BackendTask
{
   public class BackendTaskInfo
    {
        public enum RunningStatuses { Stopped = 1, Running = 2, Stopping = 3 }
        public int ProcessCount { get; set; }
        public int ErrorCount { get; set; }
        public string LastError { get; set; }
        public DateTime LastProcess { get; set; }
        public string TaskName { get; set; }
        public string TaskDescription { get; set; }
        public RunningStatuses TaskStatus { get; set; }

        public BackendTaskBase CurrentInstance { get; set; }
    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace TextCenter.BackendTask.MyBackendTask
{
    public class CustomerInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int ProcessedByInstanceIndex { get; set; }
    }
    
    public class EmailCustomerTask
    {
        public static List<CustomerInfo> CustomerList { get; set; }
        public static BackendTaskInstanceManager InstanceMngr { get; set; }
       
        static EmailCustomerTask()
        {
          
            CustomerList = CreateSampleDataForInstancesProcess().ToList();
         

        }
        public static List<BackendTaskInfo> GetBackendTaskInfos()
        {
            return InstanceMngr.Instances.Select(x => x.TaskInfo).ToList();
        }
        public static void Start(int numberOfInstance)
        {
            InstanceMngr = new BackendTaskInstanceManager();
            for (var i = 0;i< numberOfInstance;++i)
            {
                BackendTaskBase processor = new BackendTaskBase($"Email Task instance number: {i}", "");
                processor.RegisterCallback((TaskInfo) => {

                    /*Sending email logic here*/
                  
                    var rs = CustomerList.Where(x =>
                       x.ProcessedByInstanceIndex == -1 &&  //select records that has not bee processed
                       x.Id % InstanceMngr.InstanceCount == TaskInfo.CurrentInstance.InstanceIndex //Use mod operator so that each instance working on their own record
                     ).ToList();
                    var total = rs.Count;
                    foreach (var r in rs)
                    {
                        //Need to stop the process quick, whenever the status is not running.
                        //The happen when ever the stop get called for any reason
                        //If this won't be handle correctly (keep going regardless stopping request ) might call the app get prozen
                        if (TaskInfo.TaskStatus != BackendTaskInfo.RunningStatuses.Running)
                            break;
                        // hold the thread to pretend slow emailing, also make each instance has diffrent speed
                        r.ProcessedByInstanceIndex = TaskInfo.CurrentInstance.InstanceIndex; //Mark record as processed
                    }

                });
                InstanceMngr.AddInstance(processor);

            }
            InstanceMngr.InstancesStart();
        }
        public static void Restart()
        {
            InstanceMngr.InstancesStart();
        }
        public static void Stop()
        {
            InstanceMngr.InstancesStop();
        }
        public static IEnumerable<CustomerInfo> CreateSampleDataForInstancesProcess()
        {
            for (int i = 1; i < 100; ++i)
                yield return new CustomerInfo()
                {
                    Id = i,
                    Name = $"Person {i}",
                    Email = $"email{i}@nodomain.whatever",
                    ProcessedByInstanceIndex = -1
                };

        }
    }
}

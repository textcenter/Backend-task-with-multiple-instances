using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using TextCenter.BackendTask;

namespace TextCenter.BackendConsoleExample
{

    public class CustomerInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int ProcessedByInstanceIndex { get; set; }
    }

    class Program
    {
        static List<CustomerInfo> CustomerList { get; set; }
        static BackendTaskInstanceManager InstanceMngr { get; set; }
        static void Main(string[] args)
        {

         

            //Create sample data
            CustomerList = CreateSampleDataForInstancesProcess().ToList();
            //Save data befre process
            SaveData("BeforeProcess.csv");

            //Create an instance manager
            InstanceMngr = new BackendTaskInstanceManager();
           
            //Create 5 instances to process.
            //Because of no custom logic, the base is used directly

            for (var counter = 0; counter < 5;++counter)
                {
                //new an instance
                BackendTaskBase instance = new BackendTaskBase($"Email Processor instance {counter}", "");
                //Register call back
                instance.RegisterCallback(

                    (TaskInfo) => {
                        //Email logic happen here
                        Console.WriteLine($"Instance: {TaskInfo.CurrentInstance.InstanceIndex} running");
                        var rs = CustomerList.Where(x => 
                           x.ProcessedByInstanceIndex == -1   &&  //select records that has not bee processed
                           x.Id % InstanceMngr.InstanceCount == TaskInfo.CurrentInstance.InstanceIndex //Use mod operator so that each instance working on their own record
                         ).ToList();
                        int total = rs.Count;
                        int processCount = 0;
                        foreach(var r in rs)
                        {
                            //Need to stop the process quick, whenever the status is not running.
                            //The happen when ever the stop get called for any reason
                            //If this won't be handle correctly (keep going regardless stopping request ) might call the app get prozen
                            if (TaskInfo.TaskStatus != BackendTaskInfo.RunningStatuses.Running)
                                break;
                            // hold the thread to pretend slow emailing, also make each instance has diffrent speed
                            Thread.Sleep(1000 * (TaskInfo.CurrentInstance.InstanceIndex +1)); 
                            Console.WriteLine($"Instance {TaskInfo.CurrentInstance.InstanceIndex} processing {++processCount}/{total}. Just sent email to {r.Email} with Id {r.Id}");
                            r.ProcessedByInstanceIndex = TaskInfo.CurrentInstance.InstanceIndex; //Mark record as processed
                        }

                    }
                 );
                InstanceMngr.AddInstance(instance);
            }
            Console.WriteLine("Stanby. Enter any key to start. When it is running. Enter any key to stop");
            Console.ReadKey();

            InstanceMngr.InstancesStart();
            
            Console.ReadKey();


            InstanceMngr.InstancesStop();
            Console.WriteLine("Tasks Stopped.");
            SaveData("AfterProcess.csv");
            Console.ReadKey();
        }
        static IEnumerable<CustomerInfo> CreateSampleDataForInstancesProcess()
        {
            for (int i = 1; i < 100; ++i)
                yield return new CustomerInfo() {
                    Id = i,
                    Name =$"Person {i}",
                    Email =$"email{i}@nodomain.whatever",
                    ProcessedByInstanceIndex = -1
                };

        }
        static void SaveData(string fileName)
        {
            StringBuilder strb = new StringBuilder();
            foreach(var c in CustomerList)
            {
                strb.Append(c.Id).Append(',');
                strb.Append(c.Name).Append(',');
                strb.Append(c.Email).Append(',');
                strb.AppendLine(c.ProcessedByInstanceIndex.ToString());
            }
            var dir = Environment.GetFolderPath((Environment.SpecialFolder.MyDocuments));
            var f = Path.Combine(dir, fileName);
            File.WriteAllText(f, strb.ToString());
        }
    }
}

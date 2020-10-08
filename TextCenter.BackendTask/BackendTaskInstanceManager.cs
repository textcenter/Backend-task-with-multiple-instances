using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TextCenter.BackendTask
{
    public class BackendTaskInstanceManager
    {
        public int InstanceCount
        {
            get
            {
                return
                  Instances.Count;
            }
        }
        public List<BackendTaskBase> Instances { get; set; }
        public BackendTaskInstanceManager()
        {
            Instances = new List<BackendTaskBase>();
        }
        public void AddInstance(BackendTaskBase instance)
        {
            instance.InstanceIndex = Instances.Count;
            Instances.Add(instance);
        }
        public void InstancesStart()
        {
            foreach (var instance in Instances)
                instance.Start(Instances.Count);
        }
        public void InstancesStop()
        {
            List<Task> tsks = new List<Task>();
            foreach (var instance in Instances)
            {
                var t = Task.Factory.StartNew(() =>
                 {
                     instance.Stop();
                 });
                tsks.Add(t);
            }
            Task.WaitAll(tsks.ToArray());
        }
    }
}

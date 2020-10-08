using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TextCenter.BackendTask.MyBackendTask;
namespace TextCenter.NetAPI.Controllers
{
    public class BackendTasksController: ApiController
    {
        [HttpGet]
        [Route("EmailBackendInformation")]
        public object EmailBackendInformation()
        {

            object ret = new
            {
                NumberOfInstance = EmailCustomerTask.InstanceMngr.InstanceCount,
                InstanceInfo = EmailCustomerTask.GetBackendTaskInfos().Select(x =>
                  new { 
                      x.TaskName,
                      x.ProcessCount,
                      Status = x.TaskStatus.ToString(),
                     
                  }
                ).ToArray(),
                Customers = EmailCustomerTask.CustomerList
            };
            return ret;
        }
        [HttpGet]
        [Route("ManualStop")]
        public string ManualStop()
        {
            try
            {
                EmailCustomerTask.Stop();
                return "+OK";
            }
            catch(Exception ex)
            {
                return $"-NO{ex.Message}";
            }

        }
        [HttpGet]
        [Route("ManualStart")]
        public string ManualStart()
        {
            try
            {

                EmailCustomerTask.Restart();
                return "+OK";
            }
            catch (Exception ex)
            {
                return $"-NO{ex.Message}";
            }

        }
    }
}
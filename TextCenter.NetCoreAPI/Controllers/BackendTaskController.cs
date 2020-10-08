using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TextCenter.BackendTask.MyBackendTask;

namespace TextCenter.NetCoreAPI.Controllers
{
    [ApiController]
    public class BackendTaskController : ControllerBase
    {
       

        private readonly ILogger<BackendTaskController> _logger;
        public BackendTaskController(ILogger<BackendTaskController> logger)
        {
            _logger = logger;
        }
        //EmailBackendInformation

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
            catch (Exception ex)
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

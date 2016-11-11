using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace CodeEndeavors.ServicePerformanceMonitor.Http
{
    public class RequestPerformanceHandler : DelegatingHandler
    {
        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var requestInstance = Services.RequestPerformance.RecordStart(request);

            // SETUP A CALLBACK FOR CATCHING THE RESPONSE - AFTER ROUTING HANDLER, AND AFTER CONTROLLER ACTIVITY
            var ret = base.SendAsync(request, cancellationToken);
            if (requestInstance != null)
            {
                await ret.ContinueWith(
                        task =>
                        {
                        // GET THE COPY OF THE TASK, AND PASS TO A CUSTOM ROUTINE
                        ResponseHandler(requestInstance, task);

                        // RETURN THE ORIGINAL RESULT
                        var response = task.Result;
                            return response;
                        }
                );
            }
            return await ret;

        }

        public void ResponseHandler(Models.RequestInstance requestInstance, Task<HttpResponseMessage> task)
        {
            if (task.Result != null)
                Services.RequestPerformance.RecordEnd(requestInstance, task.Result);
        }

    }
}

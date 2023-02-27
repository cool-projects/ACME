using Acme.Data;
using Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers
{
    public class LoggingHelper : ILoggingHelper
    {
        private readonly AcmeDBContext db;

        public LoggingHelper(AcmeDBContext dataContext)
        {
            db = dataContext;
        }

        public int RequestLogger(string request, string response, RequestTypes requestType, int requestId)
        {
            try
            {
                if (requestId == 0)
                {
                    RequestLog newRequest = new RequestLog
                    {
                        Request = request,
                        RequestType = requestType.ToString(),
                        RequestDate = DateTime.Now
                    };
                    db.RequestLogs.Add(newRequest);
                    db.SaveChanges();
                    return newRequest.Id;
                }
                else
                {
                    var myRequest = db.RequestLogs.Where(x => x.Id == requestId).SingleOrDefault();
                    if (myRequest != null)
                    {
                        RequestLog model = new RequestLog
                        {
                            Id = myRequest.Id,
                            Request = myRequest.Request,
                            RequestType = myRequest.RequestType,
                            RequestDate = myRequest.RequestDate,
                            Response = response,
                            ResponseCode = null,
                            ResponseDate = DateTime.Now
                        };
                        if (myRequest != null)
                        {
                            db.Entry(myRequest).CurrentValues.SetValues(model);
                            db.SaveChanges();
                        }
                    }
                    return myRequest.Id;
                }
            }
            catch (Exception ex)
            {
                ExceptionLogger(ex.Message, ex.StackTrace);
                return 0;
            }
        }

        public void ExceptionLogger(string message, string stackTrace)
        {
            try
            {
                ExceptionLog newException = new ExceptionLog
                {
                    Message = message,
                    StackTrace = stackTrace,
                    LogDate = DateTime.Now
                };
                db.ExceptionLogs.Add(newException);
                db.SaveChanges();
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}

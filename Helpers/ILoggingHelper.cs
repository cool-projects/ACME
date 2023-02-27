using Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers
{
    public interface ILoggingHelper
    {
        int RequestLogger(string request, string response, RequestTypes requestType, int requestId);
        void ExceptionLogger(string message, string? stackTrace);
    }
}

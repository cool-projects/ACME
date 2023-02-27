using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class BaseResponse
    {
        public bool Success { get; set; }
        public int ResponseCode { get; set; }
        public List<string>? Errors { get; set; }
    }
}

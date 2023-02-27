using System.Collections.Generic;

namespace Models.UI
{
    public class BaseResponse
    {
        public bool Success { get; set; }
        public int ResponseCode { get; set; }
        public List<string> Errors { get; set; }
    }
}

// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Acme.Data
{
    public partial class RequestLog
    {
        public int Id { get; set; }
        public string RequestType { get; set; }
        public string Request { get; set; }
        public DateTime RequestDate { get; set; }
        public string Response { get; set; }
        public DateTime? ResponseDate { get; set; }
        public bool? ResponseCode { get; set; }
    }
}
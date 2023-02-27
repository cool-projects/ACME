using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.UI
{
    public class DocumentRequest
    {
        public int productid { get; set; }
        public string filename { get; set; }
        public string filesize { get; set; }
        public string filetype { get; set; }
        public string filepath { get; set; }
        //public byte[] file { get; set; }
    }
}

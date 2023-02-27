using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class UploadDocumentRequest
    {
        public DocumentRequest Document { get; set; }
        public byte[] File { get; set; }
    }
}

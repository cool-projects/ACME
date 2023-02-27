using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.UI
{
    public class UploadDocumentResponse : BaseResponse
    {
        public int DocumentId { get; set; }
        public string DocumentPath { get; set; }
    }
}

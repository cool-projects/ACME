using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.UI
{
    public class DownloadDocumentResponse : BaseResponse
    {
        public int DocumentId { get; set; }
        public string DocumentPathServer { get; set; }
        public string DocumentName { get; set; }
        public byte[] ActualDocument { get; set; }
    }
}

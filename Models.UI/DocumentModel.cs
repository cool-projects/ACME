using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.UI
{
    public class DocumentModel
    {
        public int? DocumentId { get; set; }
        public string DocumentName { get; set; }
        public string FileSize { get; set; }
        public string FileType { get; set; }
        public string ContentType { get; set; }
        public string FilePath { get; set; }
        public byte[] ActualDocument { get; set; }
    }
}

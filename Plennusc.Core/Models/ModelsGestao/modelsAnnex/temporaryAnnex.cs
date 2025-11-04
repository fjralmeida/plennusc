using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.Models.ModelsGestao.modelsAnnex
{
    [Serializable]
    public class temporaryAnnex
    {
        public string FileName { get; set; }
        public long Size { get; set; }
        public string SizeFormatted { get; set; }
        public byte[] Content { get; set; }
        public string ContentType { get; set; }
        public int Index { get; set; }
    }
}

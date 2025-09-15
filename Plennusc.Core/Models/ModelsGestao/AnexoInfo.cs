using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.Models.ModelsGestao
{
    public class AnexoInfo
    {
        public int CodAnexo { get; set; }
        public string NomeArquivo { get; set; }
        public DateTime DataEnvio { get; set; }
        public long TamanhoBytes { get; set; }
    }
}

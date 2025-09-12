using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.Models.ModelsGestao
{
    public class DemandaCriticaInfo
    {
        public int CodDemanda { get; set; }
        public string Titulo { get; set; }
        public DateTime DataDemanda { get; set; }
        public string Situacao { get; set; }
    }
}

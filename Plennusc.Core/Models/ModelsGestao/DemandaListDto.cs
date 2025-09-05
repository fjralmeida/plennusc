using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.Models.ModelsGestao
{
    public class DemandaListDto
    {
        public int CodDemanda { get; set; }
        public string Titulo { get; set; }
        public string Categoria { get; set; }
        public string Subtipo { get; set; }
        public string Status { get; set; }
        public string Solicitante { get; set; }
        public DateTime DataSolicitacao { get; set; }
    }
}

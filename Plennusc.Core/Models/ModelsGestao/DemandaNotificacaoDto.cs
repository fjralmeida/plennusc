using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.Models.ModelsGestao
{
    public class DemandaNotificacaoDto
    {
        public int CodDemanda { get; set; }
        public string Titulo { get; set; }
        public DateTime DataDemanda { get; set; }
        public DateTime? DataPrazo { get; set; }
        public string Prioridade { get; set; }
        public int CodPrioridade { get; set; }
        public string Solicitante { get; set; }
    }
}

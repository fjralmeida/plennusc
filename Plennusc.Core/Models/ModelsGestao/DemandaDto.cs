using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.Models.ModelsGestao
{
    public class DemandaDto
    {
        public int CodDemanda { get; set; }
        public string Titulo { get; set; }
        public string TextoDemanda { get; set; }
        public string StatusNome { get; set; }
        public int? StatusCodigo { get; set; } 
        public string Solicitante { get; set; }
        public DateTime? DataSolicitacao { get; set; }
        public int CodPessoaSolicitacao { get; set; }
        public int? CodPessoaAprovacao { get; set; }

    }
}
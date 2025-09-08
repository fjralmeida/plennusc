using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.Models.ModelsGestao
{
    public class HistoricoDto
    {
        public int CodDemandaHistorico { get; set; }
        public int CodDemanda { get; set; }
        public int? CodEstr_SituacaoDemandaAnterior { get; set; }
        public int? CodEstr_SituacaoDemandaAtual { get; set; }
        public string SituacaoAnterior { get; set; }
        public string SituacaoAtual { get; set; }
        public int CodPessoaAlteracao { get; set; }
        public string Usuario { get; set; }
        public DateTime? DataAlteracao { get; set; } 
    }
}

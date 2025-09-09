using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.Models.ModelsGestao
{
    public class DemandaCreate
    {
        public int CodPessoaSolicitacao { get; set; }
        public int CodSetorOrigem { get; set; }
        public int CodSetorDestino { get; set; }
        public int CodEstr_TipoDemanda { get; set; }
        public int CodEstr_NivelPrioridade { get; set; }
        public string Titulo { get; set; }
        public string TextoDemanda { get; set; }
        public bool Conf_RequerAprovacao { get; set; }
        public int? CodPessoaAprovacao { get; set; }
        public DateTime? DataPrazoMaximo { get; set; }
    }
}

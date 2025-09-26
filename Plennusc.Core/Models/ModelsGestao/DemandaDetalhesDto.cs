using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.Models.ModelsGestao
{
    public class DemandaDetalhesDto
    {
        public int CodDemanda { get; set; }
        public string Titulo { get; set; }
        public string TextoDemanda { get; set; }

        // Status
        public string StatusNome { get; set; }
        public int? StatusCodigo { get; set; }

        // Solicitante / datas
        public string Solicitante { get; set; }
        public DateTime? DataSolicitacao { get; set; }
        public int CodPessoaSolicitacao { get; set; }

        // Executor / aceite
        public int? CodPessoaExecucao { get; set; }
        public DateTime? DataAceitacao { get; set; }
        public string NomePessoaExecucao { get; set; }

        // Aprovação
        public int? CodPessoaAprovacao { get; set; }

        // Novos campos para a tela de detalhes
        public int CodSetorDestino { get; set; }
        public string Categoria { get; set; }
        public string Prioridade { get; set; }
        public string Importancia { get; set; }
        public string DataPrazo { get; set; }
        public string Status { get; set; }
    }
}

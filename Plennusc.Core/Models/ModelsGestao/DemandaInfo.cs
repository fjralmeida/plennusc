using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.Models.ModelsGestao
{
    public class DemandaInfo
    {
        public int CodDemanda { get; set; }
        public string Titulo { get; set; }
        public string Categoria { get; set; }
        public string Subtipo { get; set; }

        public int CodPrioridade { get; set; }
        public string Prioridade { get; set; }

        public string Status { get; set; }
        public string Solicitante { get; set; }

        public DateTime DataSolicitacao { get; set; }

        public DateTime? DataPrazo { get; set; } 
        public string Importancia { get; set; }
        public int? CodImportancia { get; set; }

        public int? CodPessoaExecucao { get; set; }
        public DateTime? DataAceitacao { get; set; }
        public string NomePessoaExecucao { get; set; }
    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.Models.ModelsGestao
{
    public class DemandaFiltro
    {
        public int? CodPessoa { get; set; }
        public int? CodSetor { get; set; }
        public int? CodStatus { get; set; }
        public int? CodCategoria { get; set; }
        public int? CodSubtipo { get; set; }
        public int? CodPrioridade { get; set; }
        public string NomeSolicitante { get; set; }
        public string Visibilidade { get; set; }

        public int? CodPessoaExecucao { get; set; }
    }
}

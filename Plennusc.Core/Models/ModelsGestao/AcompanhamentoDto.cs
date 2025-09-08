using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.Models.ModelsGestao
{
    public class AcompanhamentoDto
    {
        public int CodDemandaAcompanhamento { get; set; }
        public int CodDemanda { get; set; }
        public string TextoAcompanhamento { get; set; }
        public DateTime? DataAcompanhamento { get; set; } 
        public int CodPessoaAcompanhamento { get; set; }
        public string Autor { get; set; }
    }
}

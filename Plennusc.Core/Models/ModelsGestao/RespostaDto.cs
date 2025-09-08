using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.Models.ModelsGestao
{
    public class RespostaDto
    {
        public int CodResposta { get; set; }
        public int CodDemanda { get; set; }
        public string Autor { get; set; }
        public string TextoResposta { get; set; }
        public DateTime DataResposta { get; set; }
    }
}
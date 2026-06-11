using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.Models.ModelsGestao
{
    public class AcessorioListDto
    {
        public int CodigoAcessorio { get; set; }
        public string NomeAcessorio { get; set; }
        public int ValorAcessorio { get; set; }
        public int QuantidadeProduto { get; set; }
        public string ACC_Sincor { get; set; }
        public bool Conf_Ativo { get; set; }
        public bool Conf_Exibir { get; set; }
    }
}

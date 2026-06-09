using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.Models.ModelsGestao
{
    public class EntidadeListDto
    {
        public int CodigoEntidade {  get; set; }
        public string RazaoSocial { get; set; }
        public string Numero_CNPJ { get; set; }
        public bool Conf_Ativo { get; set; }
    }
}

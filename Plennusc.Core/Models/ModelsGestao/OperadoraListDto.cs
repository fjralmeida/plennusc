using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.Models.ModelsGestao
{
    public class OperadoraListDto
    {
        public int CodigoOperadora { get; set; }
        public string RegistroAns { get; set; }
        public string Numero_CNPJ { get; set; }
        public string RazaoSocial { get; set; }
        public string NomeComercial { get; set; }
    }
}
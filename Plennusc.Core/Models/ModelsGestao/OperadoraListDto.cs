using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.Models.ModelsGestao
{
    public class OperadoraListDto
    {
        public int CodOperadora { get; set; }
        public string RegistroAns { get; set; }
        public string Cnpj { get; set; }
        public string RazaoSocial { get; set; }
        public string NomeComercial { get; set; }
    }
}
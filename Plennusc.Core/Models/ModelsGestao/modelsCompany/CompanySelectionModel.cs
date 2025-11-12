using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.Models.ModelsGestao.modelsCompany
{
    public class CompanySelectionModel
    {
        public int CodEmpresa { get; set; }
        public string NomeFantasia { get; set; }
        public string RazaoSocial { get; set; }
        public string CNPJ { get; set; }
        public bool Selecionada { get; set; }
        public bool JaVinculada { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.Models.ModelsGestao.modelsButYou
{
    public class BeneficiarioEmpresa
    {
        public Dictionary<string, string> DadosTitular { get; set; }
        public List<Dictionary<string, string>> Dependentes { get; set; }
    }
}

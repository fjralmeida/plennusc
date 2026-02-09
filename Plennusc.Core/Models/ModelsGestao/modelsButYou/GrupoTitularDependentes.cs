using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.Models.ModelsGestao.modelsButYou
{
    public class GrupoTitularDependentes
    {
        public DadosAssociadoCompleto Titular { get; set; }
        public List<DadosAssociadoCompleto> Dependentes { get; set; } = new List<DadosAssociadoCompleto>();
    }
}

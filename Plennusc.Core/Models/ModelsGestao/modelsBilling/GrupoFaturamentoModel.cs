using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.Models.ModelsGestao.modelsBilling
{
    public class GrupoFaturamentoModel
    {
        public int CodigoGrupoFaturamento { get; set; }
        public string DescricaoGrupoFaturamento { get; set; }
    }

    public class MesAnoReferenciaModel
    {
        public string MesAnoReferencia { get; set; }
    }
}

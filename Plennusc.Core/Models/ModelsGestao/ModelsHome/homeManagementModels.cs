using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.Models.ModelsGestao.ModelsHome
{
    public class homeManagementModels
    {
        public int NovasDemandasHoje { get; set; }
        public int DemandasFinalizadasHoje { get; set; }
        public int AprovacoesPendentes { get; set; }
        public int AtrasosCriticos { get; set; }
        public int DemandasAbertas { get; set; }
        public int DemandasAndamento { get; set; }
        public int DemandasAguardando { get; set; }
        public int DemandasAtrasadas { get; set; }

    }
}

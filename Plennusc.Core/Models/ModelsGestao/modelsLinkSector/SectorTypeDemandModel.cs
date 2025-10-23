using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.Models.ModelsGestao.modelsLinkSector
{
    public class SectorTypeDemandModel
    {
        public int CodSetorTipoDemanda { get; set; }
        public int CodSetor { get; set; }
        public int CodEstr_TipoDemanda { get; set; }
        public string NomeSetor { get; set; }
        public bool Conf_Status { get; set; }
    }
}

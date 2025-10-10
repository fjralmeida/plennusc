using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.Models.ModelsGestao.modelsStructure
{
    public class structureModel
    {
        public int CodEstrutura { get; set; }
        public int? CodEstruturaPai { get; set; }
        public int CodTipoEstrutura { get; set; }
        public string DescEstrutura { get; set; }
        public string MemoEstrutura { get; set; }
        public string InfoEstrutura { get; set; }
        public bool Conf_IsDefault { get; set; }
        public int ValorPadrao { get; set; }
        public string DescTipoEstrutura { get; set; } 
        public string NomeView { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.Models.ModelsGestao.modelsStructure
{
    public class structureTypeModel
    {
        public int CodTipoEstrutura { get; set; }
        public string DescTipoEstrutura { get; set; }
        public int? CodTipoEstruturaPai { get; set; }
        public string NomeView { get; set; }
        public bool Editavel { get; set; }
        public string Definicao { get; set; }
        public string Utilizacao { get; set; }
        public DateTime Informacoes_Log_I { get; set; }
        public DateTime? Informacoes_Log_A { get; set; }
    }
}

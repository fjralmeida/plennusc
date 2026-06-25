using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.Models.ModelsGestao.modelsMenu
{
    public class MenuModel
    {
        public int CodMenu { get; set; }
        public string NomeMenu { get; set; }
        public string NomeDisplay { get; set; }
        public int Conf_Nivel { get; set; }
        public int Conf_Ordem { get; set; }
        public int? CodMenuPai { get; set; } 
        public bool Vinculado { get; set; }
        public string NomeObjeto { get; set; } 
        public string HttpRouter { get; set; } 
    }
}

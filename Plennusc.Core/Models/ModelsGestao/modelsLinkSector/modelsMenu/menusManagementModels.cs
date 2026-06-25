using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.Models.ModelsGestao.modelsMenu
{
    public class menusManagementModels
    {
        public int CodMenu { get; set; }
        public string NomeMenu { get; set; }
        public string NomeDisplay { get; set; }
        public string NomeObjeto { get; set; }
        public string CaptionObjeto { get; set; }
        public string HttpRouter { get; set; }
        public int? CodMenuPai { get; set; }
        public int Conf_Ordem { get; set; }
        public int Conf_Nivel { get; set; }
        public bool Conf_Habilitado { get; set; }

        // Campo auxiliar para exibição
        public string NomeMenuPai { get; set; }
    }
}

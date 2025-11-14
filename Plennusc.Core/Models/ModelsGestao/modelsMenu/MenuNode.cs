using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.Models.ModelsGestao.modelsMenu
{
    public class MenuNode
    {
        public MenuModel Menu { get; set; }
        public List<MenuNode> Filhos { get; set; } = new List<MenuNode>();
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.Models.ModelsGestao.modelsBilling
{
    public class IItemRelatorioImportado
    {
        string Credencial { get; set; }
        string Descricao { get; set; }
        decimal ValorOperadora { get; set; }
        string Cpf { get; set; }
    }
}

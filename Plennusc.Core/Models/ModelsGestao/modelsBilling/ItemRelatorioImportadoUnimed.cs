using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.Models.ModelsGestao.modelsBilling
{
    public class ItemRelatorioImportadoUnimed : IItemRelatorioImportado
    {
        public string Credencial { get; set; }
        public string NomeBeneficiario { get; set; }
        public string NomeTitular { get; set; }
        public string Descricao { get; set; }
        public decimal ValorOperadora { get; set; }
        public string Cpf { get; set; }

        // Preenchidos depois, na etapa de conferência com a VW_RELATORIO_CONFERENCIA
        public decimal? ValorSistema { get; set; }
        public bool Conferido { get; set; }
        public bool Divergente { get; set; }
    }
}

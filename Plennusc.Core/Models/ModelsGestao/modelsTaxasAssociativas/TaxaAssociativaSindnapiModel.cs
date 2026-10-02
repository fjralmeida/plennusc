using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.Models.ModelsGestao.modelsTaxasAssociativas
{
    /// <summary>
    /// Linha do relatório de Taxas Associativas — SINDNAPI.
    /// Filtro: PS1029.CODIGO_EVENTO = 17 (evento SINDNAPI).
    /// </summary>
    public class TaxaAssociativaSindnapiModel
    {
        public string Cpf { get; set; }
        public string NomeBeneficiario { get; set; }
        public DateTime? DataNascimento { get; set; }
        public DateTime? DataPagamento { get; set; }
        public decimal ValorEvento { get; set; }
        public decimal ValorTotal { get; set; }
    }
}
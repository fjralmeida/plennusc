using System;

namespace Plennusc.Core.Models.ModelsGestao.modelsTaxasAssociativas
{
    /// <summary>
    /// Linha do relatório de Taxas Associativas — SINCOR (PF e PJ).
    /// Filtro: PS1029.CODIGO_EVENTO IN (13, 45).
    /// </summary>
    public class TaxaAssociativaSincorModel
    {
        public string CpfCnpj { get; set; }
        public string NomeBeneficiario { get; set; }

        // Propriedades de contato adicionadas
        public string Email { get; set; }
        public string Telefone { get; set; }

        public DateTime? DataNascimento { get; set; }
        public DateTime? DataPagamento { get; set; }
        public decimal ValorEvento { get; set; }
        public string TipoPessoa { get; set; } // "PF" ou "PJ"
    }

    /// <summary>
    /// Representa uma opção do dropdown do SINCOR (PF ou PJ).
    /// </summary>
    public class OpcaoSincorModel
    {
        public int CodigoEvento { get; set; }
        public string Descricao { get; set; }
    }
}
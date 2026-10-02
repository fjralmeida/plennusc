using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.Models.ModelsGestao.modelsTaxasAssociativas
{
    /// <summary>
    /// Linha do relatório de Taxas Associativas — Demais Entidades.
    /// Filtro: PS1024.NOME_EVENTO LIKE 'TX ASSOC%' AND CODIGO_EVENTO <> 17.
    /// </summary>
    public class TaxaAssociativaDemaisEntidadesModel
    {
        public int NumeroRegistroPs1020 { get; set; }
        public string NomeBeneficiario { get; set; }
        public string TipoContratacaoAns { get; set; }
        public DateTime? DataPagamento { get; set; }
        public decimal ValorEvento { get; set; }
        public decimal ValorTotal { get; set; }
    }

    /// <summary>
    /// Representa uma entidade (evento) disponível no dropdown da tela "Demais Entidades".
    /// Alimentado pela PS1024 filtrando NOME_EVENTO LIKE 'TX ASSOC%' AND CODIGO_EVENTO <> 17.
    /// </summary>
    public class EntidadeTaxaAssociativaModel
    {
        public int CodigoEvento { get; set; }
        public string NomeEvento { get; set; }
    }
}
using Plennusc.Core.Models.ModelsGestao.modelsTaxasAssociativas;
using Plennusc.Core.SqlQueries.SqlQueriesGestao.taxasAssociativas;
using System;
using System.Collections.Generic;

namespace Plennusc.Core.Service.ServiceGestao.serviceTaxasAssociativas
{
    /// <summary>
    /// Serviço de Taxas Associativas (Comissões / Premiações).
    /// Faz a ponte entre o code-behind das telas e o SqlTaxasAssociativas.
    /// </summary>
    public class ServiceTaxasAssociativas
    {
        private readonly SqlTaxasAssociativas _sql = new SqlTaxasAssociativas();

        // Dropdown "Demais Entidades"
        public List<EntidadeTaxaAssociativaModel> ObterEntidades()
            => _sql.BuscarEntidadesTaxaAssociativa();

        // Tela SINDNAPI
        public List<TaxaAssociativaSindnapiModel> ObterTaxasSindnapi(DateTime dataInicio, DateTime dataFim)
            => _sql.BuscarTaxasSindnapi(dataInicio, dataFim);

        // Tela Demais Entidades (agora recebe uma lista)
        public List<TaxaAssociativaDemaisEntidadesModel> ObterTaxasDemaisEntidades(
            List<int> codigosEvento, DateTime dataInicio, DateTime dataFim)
            => _sql.BuscarTaxasDemaisEntidades(codigosEvento, dataInicio, dataFim);
    }
}
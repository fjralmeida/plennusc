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

        // DEMAIS ENTIDADES

        // Dropdown "Demais Entidades"
        public List<EntidadeTaxaAssociativaModel> ObterEntidades()
            => _sql.BuscarEntidadesTaxaAssociativa();

        // Tela Demais Entidades (recebe uma lista de códigos)
        public List<TaxaAssociativaDemaisEntidadesModel> ObterTaxasDemaisEntidades(
            List<int> codigosEvento, DateTime dataInicio, DateTime dataFim)
            => _sql.BuscarTaxasDemaisEntidades(codigosEvento, dataInicio, dataFim);

        // SINDNAPI
        

        // Tela SINDNAPI
        public List<TaxaAssociativaSindnapiModel> ObterTaxasSindnapi(DateTime dataInicio, DateTime dataFim)
            => _sql.BuscarTaxasSindnapi(dataInicio, dataFim);

        // SINCOR (PF e PJ)

        // Opções do dropdown do SINCOR (PF / PJ)
        public List<OpcaoSincorModel> ObterOpcoesSincor()
            => _sql.BuscarOpcoesSincor();

        // Tela SINCOR (agora recebe uma lista)
        public List<TaxaAssociativaSincorModel> ObterTaxasSincor(
            List<int> codigosEvento, DateTime dataInicio, DateTime dataFim)
            => _sql.BuscarTaxasSincor(codigosEvento, dataInicio, dataFim);
    }
}
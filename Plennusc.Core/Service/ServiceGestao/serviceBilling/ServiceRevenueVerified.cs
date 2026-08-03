using Plennusc.Core.Models.ModelsGestao.modelsBilling;
using Plennusc.Core.SqlQueries.SqlQueriesGestao.billing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.Service.ServiceGestao.serviceBilling
{
    public class ServiceRevenueVerified
    {
        private readonly SqlRevenueVerified _sql = new SqlRevenueVerified();

        #region LÓGICA DE FATURAMENTOS CONFERIDOS

        // Busca os faturamentos que já foram conferidos (DATA_CONFERENCIA_FATUR não é NULL)
        public List<ItemFaturamentosConferidos> ObterFaturamentosConferidos(string mesAnoReferencia, int codigoGrupoContrato, List<int> codigosGrupoFaturamento)
        {
            return _sql.BuscarFaturamentosConferidos(mesAnoReferencia, codigoGrupoContrato, codigosGrupoFaturamento);
        }

        // Obtém a lista de operadoras disponíveis
        public List<OperadoraModel> ObterOperadoras()
        {
            return _sql.BuscarOperadoras();
        }

        // Obtém a lista de grupos de faturamento disponíveis
        public List<GrupoFaturamentoModel> ObterGruposFaturamento()
        {
            return _sql.BuscarGruposFaturamento();
        }

        #endregion
    }
}
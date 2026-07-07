using DocumentFormat.OpenXml.Wordprocessing;
using Plennusc.Core.Models.ModelsGestao.modelsBilling;
using Plennusc.Core.SqlQueries.SqlQueriesGestao.billing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.Service.ServiceGestao.serviceBilling
{
    public class ServiceBillingReconciliation
    {
        private readonly SqlBillingReconciliation _sql = new SqlBillingReconciliation();
        private readonly ServiceBillingReconciliationHapvida _hapvida = new ServiceBillingReconciliationHapvida();

        public List<OperadoraModel> ObterOperadoras()
        {
            return _sql.BuscarOperadoras();
        }

        public List<GrupoFaturamentoModel> ObterGruposFaturamento()
        {
            return _sql.BuscarGruposFaturamento();
        }

        // Recebe o nome da operadora que já veio selecionado em tela (ddlOperadora.SelectedItem.Text)
        public List<ItemRelatorioImportadoHapVida> ProcessarRelatorioImportado(string nomeOperadora, Stream arquivo, string extensao)
        {
            if (nomeOperadora.IndexOf("HAPVIDA", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return _hapvida.LerRelatorio(arquivo, extensao);
            }

            throw new NotSupportedException($"Ainda não existe leitura implementada para a operadora '{nomeOperadora}'.");
        }
    }
}


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
    public class ServiceBillingReconciliationUnimed
    {
        private const decimal TOLERANCIA_DIVERGENCIA = 0.10m;

        private readonly SqlBillingReconciliation _sql = new SqlBillingReconciliation();

        public List<ItemRelatorioImportadoHapVida> LerRelatorioUnimed(Stream arquivo, string extensao)
        {
            switch (extensao)
            {
                case ".csv":
                    //return LerRelatorioTXT(arquivo);

                case ".xlsx":
                case ".xls":
                    throw new NotSupportedException("Leitura de Excel para Hapvida ainda não implementada.");

                case ".docx":
                    throw new NotSupportedException("Leitura de Word para Hapvida ainda não implementada.");

                default:
                    throw new NotSupportedException($"Extensão '{extensao}' não suportada para Hapvida.");
            }
        }

    }
}

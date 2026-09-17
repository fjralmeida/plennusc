using Plennusc.Core.Models.ModelsGestao.modelsBilling;
using Plennusc.Core.Models.ModelsGestao.modelsCIDs;
using Plennusc.Core.SqlQueries.SqlQueriesGestao.billing;
using Plennusc.Core.SqlQueries.SqlQueriesGestao.dataCIDs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.Service.ServiceGestao.CIDsService
{
    public class activityReportService
    {
        private readonly dataActivityReport _sql = new dataActivityReport();

        public List<OperadoraModel> ObterOperadoras() => _sql.BuscarOperadoras();

        public List<ActivityReportModel> GerarRelatorio(int codigoGrupoContrato, DateTime vigencia)
            => _sql.BuscarRelatorio(codigoGrupoContrato, vigencia);
    }
}
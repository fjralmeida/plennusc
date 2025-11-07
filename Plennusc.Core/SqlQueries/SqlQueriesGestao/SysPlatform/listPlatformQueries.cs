using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.SqlQueries.SqlQueriesGestao.SysPlatform
{
    public class listPlatformQueries
    {
        // VERIFICA Sistemas
        public const string VerificaSistamaExistente = @"
            SELECT * FROM Sistema";
    }
}

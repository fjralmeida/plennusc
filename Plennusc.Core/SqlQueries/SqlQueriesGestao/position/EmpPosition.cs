using appWhatsapp.Data_Bd;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.SqlQueries.SqlQueriesGestao.position
{
    public class EmpPosition
    {
        public DataTable  GetPositions()
        {
            string sql = "SELECT * FROM Cargo";

            Banco_Dados_SQLServer bd = new Banco_Dados_SQLServer();
            return bd.LerPlennus(sql);
        }
        public DataTable GetTotalCargos()
        {
            string sql = "SELECT COUNT(*) AS Total FROM Cargo";

            Banco_Dados_SQLServer bd = new Banco_Dados_SQLServer();
            return bd.LerPlennus(sql);
        }
    }
}

using appWhatsapp.Data_Bd;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.SqlQueries.SqlQueriesGestao.department
{
    public class EmpDepartment
    {
        public DataTable GetDepartments()
        {
            string sql = "SELECT * FROM Departamento";

            Banco_Dados_SQLServer bd = new Banco_Dados_SQLServer();
            return bd.LerPlennus(sql);
        }
    }
}

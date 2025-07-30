using appWhatsapp.Data_Bd;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.SqlQueries.SqlQueriesMedic.department
{
    public class EmpDepartmentMedic
    {
        public DataTable GetDepartments()
        {
            string sql = "SELECT * FROM Departamento";

            Banco_Dados_SQLServer bd = new Banco_Dados_SQLServer();
            return bd.LerPlennus(sql);
        }
        public DataTable GetTotalDepartamentos()
        {
            string sql = "SELECT COUNT(*) AS Total FROM Departamento";

            Banco_Dados_SQLServer bd = new Banco_Dados_SQLServer();
            return bd.LerPlennus(sql);
        }
    }
}

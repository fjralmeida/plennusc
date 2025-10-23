using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.SqlQueries.SqlQueriesGestao.department
{
    public class departmentQueries
    {
        public const string BuscarTodosDepartamentos = @"
            SELECT CodDepartamento, Nome, NumRamal, EmailGeral, Telefone
            FROM Departamento 
            WHERE CodDepartamento IS NOT NULL
            ORDER BY Nome";

        public const string BuscarDepartamentoPorId = @"
            SELECT CodDepartamento, Nome, NumRamal, EmailGeral, Telefone
            FROM Departamento 
            WHERE CodDepartamento = @CodDepartamento";
    }
}

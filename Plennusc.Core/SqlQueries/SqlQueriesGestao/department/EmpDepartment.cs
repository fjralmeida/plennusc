using appWhatsapp.Data_Bd;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.SqlQueries.SqlQueriesGestao.department
{
    public class EmpDepartment
    {
        private readonly string connectionString;

        public EmpDepartment()
        {
            // Obter connection string do web.config
            connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["Plennus"].ConnectionString;
        }

        // Método existente para buscar departamentos
        public DataTable GetDepartments()
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT 
                        CodDepartamento,
                        Nome,
                        NumRamal,
                        EmailGeral,
                        Telefone,
                        Informacoes_Log_I
                    FROM Departamento
                    ORDER BY Nome";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }

            return dt;
        }

        // NOVO MÉTODO: Obter total de departamentos (usado em HomeGestao)
        public DataTable GetTotalDepartamentos()
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT 
                        COUNT(*) as Total
                    FROM Departamento
                    WHERE 1=1";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }

            return dt;
        }

        // NOVO MÉTODO: Criar departamento
        public int CreateDepartment(string nome, string numRamal, string emailGeral, string telefone)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                    INSERT INTO Departamento 
                    (Nome, NumRamal, EmailGeral, Telefone, Informacoes_Log_I)
                    VALUES 
                    (@Nome, @NumRamal, @EmailGeral, @Telefone, GETDATE());
                    
                    SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Nome", nome ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@NumRamal", !string.IsNullOrEmpty(numRamal) ? numRamal : (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@EmailGeral", !string.IsNullOrEmpty(emailGeral) ? emailGeral : (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Telefone", !string.IsNullOrEmpty(telefone) ? telefone : (object)DBNull.Value);

                    conn.Open();

                    // Executar e retornar o ID gerado
                    var resultado = cmd.ExecuteScalar();
                    return resultado != null ? Convert.ToInt32(resultado) : 0;
                }
            }
        }

        // Método adicional para validar se departamento já existe
        public bool DepartmentExists(string nome)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT COUNT(1) FROM Departamento WHERE Nome = @Nome";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Nome", nome);

                    conn.Open();
                    int count = (int)cmd.ExecuteScalar();
                    return count > 0;
                }
            }
        }

        // MÉTODO ADICIONAL: Obter departamento por ID
        public DataTable GetDepartmentById(int departmentId)
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT 
                        CodDepartamento,
                        Nome,
                        NumRamal,
                        EmailGeral,
                        Telefone,
                        Informacoes_Log_I
                    FROM Departamento
                    WHERE CodDepartamento = @DepartmentId";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@DepartmentId", departmentId);

                    conn.Open();
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }

            return dt;
        }

        // MÉTODO ADICIONAL: Atualizar departamento
        public bool UpdateDepartment(int departmentId, string nome, string numRamal, string emailGeral, string telefone)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                    UPDATE Departamento 
                    SET Nome = @Nome,
                        NumRamal = @NumRamal,
                        EmailGeral = @EmailGeral,
                        Telefone = @Telefone
                    WHERE CodDepartamento = @DepartmentId";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@DepartmentId", departmentId);
                    cmd.Parameters.AddWithValue("@Nome", nome ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@NumRamal", !string.IsNullOrEmpty(numRamal) ? numRamal : (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@EmailGeral", !string.IsNullOrEmpty(emailGeral) ? emailGeral : (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Telefone", !string.IsNullOrEmpty(telefone) ? telefone : (object)DBNull.Value);

                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }

        // MÉTODO ADICIONAL: Excluir departamento
        // Em EmpDepartment.cs - verifique se este método existe:
        public bool DeleteDepartment(int departmentId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "DELETE FROM Departamento WHERE CodDepartamento = @DepartmentId";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@DepartmentId", departmentId);

                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }

        // MÉTODO ADICIONAL: Pesquisar departamentos por nome
        public DataTable SearchDepartments(string searchTerm)
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT 
                        CodDepartamento,
                        Nome,
                        NumRamal,
                        EmailGeral,
                        Telefone,
                        Informacoes_Log_I
                    FROM Departamento
                    WHERE Nome LIKE @SearchTerm
                    ORDER BY Nome";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@SearchTerm", $"%{searchTerm}%");

                    conn.Open();
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }

            return dt;
        }
    }
}
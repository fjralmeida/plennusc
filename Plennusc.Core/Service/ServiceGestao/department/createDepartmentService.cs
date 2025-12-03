using Plennusc.Core.SqlQueries.SqlQueriesGestao.department;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.Service.ServiceGestao.department
{
    public class CreateDepartmentService
    {
        private readonly EmpDepartment _departmentQueries;

        public CreateDepartmentService()
        {
            _departmentQueries = new EmpDepartment();
        }

        /// <summary>
        /// Cria um novo departamento
        /// </summary>
        /// <param name="nome">Nome do departamento (obrigatório)</param>
        /// <param name="numRamal">Número do ramal</param>
        /// <param name="emailGeral">Email geral do departamento</param>
        /// <param name="telefone">Telefone do departamento</param>
        /// <returns>Objeto com resultado da operação</returns>
        public DepartmentResult CreateDepartment(string nome, string numRamal = null, string emailGeral = null, string telefone = null)
        {
            try
            {
                // Validação básica
                if (string.IsNullOrWhiteSpace(nome))
                {
                    return new DepartmentResult
                    {
                        Success = false,
                        Message = "O nome do departamento é obrigatório.",
                        DepartmentId = 0
                    };
                }

                // Verificar se departamento já existe (opcional)
                if (_departmentQueries.DepartmentExists(nome))
                {
                    return new DepartmentResult
                    {
                        Success = false,
                        Message = $"Já existe um departamento com o nome '{nome}'.",
                        DepartmentId = 0
                    };
                }

                // Validar email se fornecido
                if (!string.IsNullOrWhiteSpace(emailGeral))
                {
                    if (!IsValidEmail(emailGeral))
                    {
                        return new DepartmentResult
                        {
                            Success = false,
                            Message = "O e-mail fornecido não é válido.",
                            DepartmentId = 0
                        };
                    }
                }

                // Chamar a query para criar o departamento
                int newDepartmentId = _departmentQueries.CreateDepartment(
                    nome.Trim(),
                    !string.IsNullOrWhiteSpace(numRamal) ? numRamal.Trim() : null,
                    !string.IsNullOrWhiteSpace(emailGeral) ? emailGeral.Trim() : null,
                    !string.IsNullOrWhiteSpace(telefone) ? telefone.Trim() : null
                );

                if (newDepartmentId > 0)
                {
                    return new DepartmentResult
                    {
                        Success = true,
                        Message = "Departamento criado com sucesso!",
                        DepartmentId = newDepartmentId,
                        DepartmentData = GetDepartmentById(newDepartmentId)
                    };
                }
                else
                {
                    return new DepartmentResult
                    {
                        Success = false,
                        Message = "Não foi possível criar o departamento. Tente novamente.",
                        DepartmentId = 0
                    };
                }
            }
            catch (Exception ex)
            {
                // Logar o erro (implementar logging apropriado)
                // Logger.LogError(ex, "Erro ao criar departamento");

                return new DepartmentResult
                {
                    Success = false,
                    Message = $"Erro interno ao criar departamento: {ex.Message}",
                    DepartmentId = 0
                };
            }
        }

        /// <summary>
        /// Obtém todos os departamentos
        /// </summary>
        public DataTable GetAllDepartments()
        {
            try
            {
                return _departmentQueries.GetDepartments();
            }
            catch (Exception ex)
            {
                // Logar erro
                throw new ApplicationException("Erro ao obter departamentos", ex);
            }
        }

        /// <summary>
        /// Obtém um departamento pelo ID
        /// </summary>
        public DataRow GetDepartmentById(int departmentId)
        {
            try
            {
                DataTable allDepartments = _departmentQueries.GetDepartments();
                DataRow[] rows = allDepartments.Select($"CodDepartamento = {departmentId}");

                return rows.Length > 0 ? rows[0] : null;
            }
            catch (Exception ex)
            {
                // Logar erro
                throw new ApplicationException($"Erro ao obter departamento ID {departmentId}", ex);
            }
        }

        /// <summary>
        /// Valida formato de email
        /// </summary>
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Valida formato de telefone (simplificado)
        /// </summary>
        public bool IsValidPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return true;

            // Remove caracteres não numéricos
            string cleanPhone = new string(phone.Where(char.IsDigit).ToArray());

            // Valida se tem entre 10 e 11 dígitos (com DDD)
            return cleanPhone.Length >= 10 && cleanPhone.Length <= 11;
        }
    }

    /// <summary>
    /// Classe para representar o resultado da operação de criação de departamento
    /// </summary>
    public class DepartmentResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public int DepartmentId { get; set; }
        public DataRow DepartmentData { get; set; }
    }

    /// <summary>
    /// DTO para criação de departamento
    /// </summary>
    public class CreateDepartmentDto
    {
        public string Nome { get; set; }
        public string NumRamal { get; set; }
        public string EmailGeral { get; set; }
        public string Telefone { get; set; }
    }
}
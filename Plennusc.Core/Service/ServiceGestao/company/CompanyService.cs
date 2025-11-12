using Plennusc.Core.Models.ModelsGestao.modelsCompany;
using Plennusc.Core.SqlQueries.SqlQueriesGestao.company;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.Service.ServiceGestao.company
{
    public class CompanyService
    {
        private CompanyDAO _companyDAO;

        public CompanyService()
        {
            _companyDAO = new CompanyDAO();
        }

        public int SalvarEmpresa(CompanyModel company)
        {
            // Validar dados obrigatórios
            if (string.IsNullOrWhiteSpace(company.RazaoSocial))
                throw new Exception("Razão Social é obrigatória");

            if (string.IsNullOrWhiteSpace(company.NomeFantasia))
                throw new Exception("Nome Fantasia é obrigatório");

            // Validar e formatar CNPJ
            if (string.IsNullOrWhiteSpace(company.CNPJ))
                throw new Exception("CNPJ é obrigatório");

            company.CNPJ = FormatarCNPJ(company.CNPJ);

            if (!ValidarCNPJ(company.CNPJ))
                throw new Exception("CNPJ inválido");

            // Validar se CNPJ já existe
            if (_companyDAO.CNPJExiste(company.CNPJ))
                throw new Exception("Já existe uma empresa cadastrada com este CNPJ");

            // Inserir empresa
            return _companyDAO.InserirEmpresa(company);
        }

        public bool ValidarCNPJ(string cnpj)
        {
            cnpj = FormatarCNPJ(cnpj);

            if (cnpj.Length != 14)
                return false;

            // Verificar se todos os dígitos são iguais
            if (cnpj.All(c => c == cnpj[0]))
                return false;

            // Validar dígitos verificadores
            int[] multiplicador1 = { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicador2 = { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

            string tempCnpj = cnpj.Substring(0, 12);
            int soma = 0;

            for (int i = 0; i < 12; i++)
                soma += int.Parse(tempCnpj[i].ToString()) * multiplicador1[i];

            int resto = (soma % 11);
            resto = resto < 2 ? 0 : 11 - resto;

            string digito = resto.ToString();
            tempCnpj += digito;

            soma = 0;
            for (int i = 0; i < 13; i++)
                soma += int.Parse(tempCnpj[i].ToString()) * multiplicador2[i];

            resto = (soma % 11);
            resto = resto < 2 ? 0 : 11 - resto;

            digito += resto.ToString();

            return cnpj.EndsWith(digito);
        }

        private string FormatarCNPJ(string cnpj)
        {
            return new string(cnpj.Where(char.IsDigit).ToArray());
        }
    }
}
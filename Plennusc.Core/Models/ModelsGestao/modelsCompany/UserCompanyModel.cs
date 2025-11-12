using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.Models.ModelsGestao.modelsCompany
{
    public class UserCompanyModel
    {
        public int CodPessoa { get; set; }
        public string Nome { get; set; }
        public string Sobrenome { get; set; }
        public string CPF { get; set; }
        public string Email { get; set; }
        public string Login { get; set; }
        public int CodAutenticacaoAcesso { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.Models.ModelsGestao.modelsButYou
{
    public class ProposalModel
    {
        // Cabeçalho
        public string NumeroProposta { get; set; }
        public bool IsPlanoNovo { get; set; }
        public bool IsInclusaoDependente { get; set; }

        public string Entidade { get; set; }
        public string InicioVigencia { get; set; }
        public string Vigencia { get; set; }
        public string Vencimento { get; set; }

        public string NomeTitular { get; set; }
        public string CpfTitular { get; set; }
        public string Filiacao { get; set; }
        public string DataNascimento { get; set; }
        public string Rg { get; set; }
        public string OrgaoExpedidor { get; set; }
        public string Endereco { get; set; }
        public string Cep { get; set; }
        public string Email { get; set; }
        public string TelefoneCelular { get; set; }
        public string TelefoneFixo { get; set; }

        public List<DependentModel> Dependentes { get; set; } = new List<DependentModel>();
    }
}

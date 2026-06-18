using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.Models.ModelsGestao.modelsOperator
{
    public class OperadoraAlteracaoDto
    {
        public int CodigoOperadora { get; set; } // PK no Plennus
        public string Numero_CNPJ { get; set; } // chave de comparação

        // Valores atuais no Plennus
        public string RegistroANS_Atual { get; set; }
        public string RazaoSocial_Atual { get; set; }
        public string NomeComercial_Atual { get; set; }

        // Valores novos vindos da Aliança
        public string RegistroANS_Novo { get; set; }
        public string RazaoSocial_Novo { get; set; }
        public string NomeComercial_Novo { get; set; }

        /// <summary>True se o novo RegistroANS é válido (numérico) e pode ser atualizado.</summary>
        public bool AnsNovoValido =>
            !string.IsNullOrWhiteSpace(RegistroANS_Novo) && int.TryParse(RegistroANS_Novo.Trim(), out _);

        /// <summary>True se há divergência relevante em RegistroANS (e o novo valor é válido).</summary>
        public bool DivergeAns =>
            AnsNovoValido && !string.Equals(RegistroANS_Atual?.Trim(), RegistroANS_Novo?.Trim(), StringComparison.OrdinalIgnoreCase);

        public bool DivergeRazaoSocial =>
            !string.Equals(RazaoSocial_Atual?.Trim(), RazaoSocial_Novo?.Trim(), StringComparison.OrdinalIgnoreCase);

        public bool DivergeNomeComercial =>
            !string.Equals(NomeComercial_Atual?.Trim(), NomeComercial_Novo?.Trim(), StringComparison.OrdinalIgnoreCase);

        /// <summary>True se existe ao menos um campo atualizável divergente.</summary>
        public bool TemDivergenciaValida => DivergeAns || DivergeRazaoSocial || DivergeNomeComercial;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.Models.ModelsGestao.modelsOperator
{
    public class OperadoraAlteracoesDto
    {
        public int CodigoOperadora { get; set; }
        public string Numero_CNPJ { get; set; }

        public string RegistroANS_Atual { get; set; }
        public string RazaoSocial_Atual { get; set; }
        public string NomeComercial_Atual { get; set; }

        public string RegistroANS_Novo { get; set; }
        public string RazaoSocial_Novo { get; set; }
        public string NomeComercial_Novo { get; set; }

        public bool AnsNovoValido =>
            !string.IsNullOrWhiteSpace(RegistroANS_Novo) && int.TryParse(RegistroANS_Novo.Trim(), out _);

        public bool DivergeAns =>
            AnsNovoValido && !string.Equals(RegistroANS_Atual?.Trim(), RegistroANS_Novo?.Trim(), StringComparison.OrdinalIgnoreCase);

        public bool DivergeRazaoSocial =>
            !string.Equals(RazaoSocial_Atual?.Trim(), RazaoSocial_Novo?.Trim(), StringComparison.OrdinalIgnoreCase);

        public bool DivergeNomeComercial =>
            !string.Equals(NomeComercial_Atual?.Trim(), NomeComercial_Novo?.Trim(), StringComparison.OrdinalIgnoreCase);

        public bool TemDivergenciaValida => DivergeAns || DivergeRazaoSocial || DivergeNomeComercial;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.Models.ModelsGestao.modelsOperator
{
    public class OperadoraSyncDto
    {
        public int CodigoGrupo { get; set; }
        public string Numero_CNPJ { get; set; }
        public string RegistroANS { get; set; }
        public string RazaoSocial { get; set; }
        public string NomeComercial { get; set; }

        /// <summary>
        /// True se RegistroANS é um número válido (pode ser convertido para int).
        /// Se false, a operadora não pode ser sincronizada — falta corrigir na origem.
        /// </summary>
        public bool AnsValido =>
            !string.IsNullOrWhiteSpace(RegistroANS) && int.TryParse(RegistroANS.Trim(), out _);
    }
}

using System;

namespace Plennusc.Core.Models.ModelsGestao.modelsSendMessage
{
    public class GrupoFamiliarEnvioModel
    {
        public string CodigoAssociado { get; set; }
        public string NomeTitular { get; set; }
        public string CpfCnpj { get; set; }
        public string GrupoContrato { get; set; }
        public string GrupoFaturamento { get; set; }
        public string NomeGrupoPessoas { get; set; }
        public int NumeroRegistroBoleto { get; set; }
        public DateTime DataVencimento { get; set; }
        public string Email { get; set; }
        public string Telefone { get; set; }
        public decimal ValorBoleto { get; set; }
        public string MesReferencia { get; set; }
    }

    public class FiltroEnvioModel
    {
        public string TipoPlano { get; set; }
        public string Operadora { get; set; }
        public string GrupoPessoas { get; set; } // Recebe o ID (string)
        public string GrupoFaturamento { get; set; }
        public DateTime? VencimentoInicio { get; set; }
        public DateTime? VencimentoFim { get; set; }
        public DateTime? EnvioInicio { get; set; }
        public DateTime? EnvioFim { get; set; }
        public string Mensagem { get; set; }
    }
}
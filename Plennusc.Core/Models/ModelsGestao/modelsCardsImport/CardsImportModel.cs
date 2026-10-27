using System;

namespace Plennusc.Core.Models.ModelsGestao.modelsCardsImport
{
    public class CardsImportCsvRowModel
    {
        public int LinhaCsv { get; set; }
        public string NumeroCpf { get; set; }
        public string DataAdmissao { get; set; }
        public string NumeroCarteirinha { get; set; }
        public string DataEmissaoCarteirinha { get; set; }
        public string ObservacaoCadastro { get; set; }
    }

    public class CardsImportResultModel
    {
        public int LinhaCsv { get; set; }
        public string Cpf { get; set; }
        public string NomeAssociado { get; set; }
        public string CodigoAssociado { get; set; }
        public string NumeroCarteirinha { get; set; }
        public string DataAdmissaoPlanilha { get; set; }
        public string DataAdmissaoBanco { get; set; }
        public bool Sucesso { get; set; }
        public string Motivo { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.Models.ModelsGestao.modelsCIDs
{ 
  /// <summary>
  /// Representa uma linha lida do arquivo CSV de importação de CID.
  /// </summary>
    public class CIDsCsvRowModel
    {
        public int LinhaCsv { get; set; }
        public string Operadora { get; set; }
        public DateTime? Vigencia { get; set; }
        public string Titular { get; set; }
        public string Beneficiario { get; set; }
        public string Cpf { get; set; }
        public string Proposta { get; set; }
        public DateTime? Data { get; set; }
        public string Horario { get; set; }
        public string DoencaOuLesaoPreexistente { get; set; }
        public string Cid { get; set; }
        public string Video { get; set; }
        public string Enfermeiro { get; set; }
        public string ParecerTecnico { get; set; }
        public string Observacao { get; set; }
        public string Pendencias { get; set; }
    }

    /// <summary>
    /// Retorno da PS1000 (dados do associado) usado para validar a importação.
    /// </summary>
    public class CIDsAssociadoModel
    {
        public string CodigoAssociado { get; set; }
        public DateTime? DataAdmissao { get; set; }
    }

    /// <summary>
    /// Dados necessários para inserir um registro na PS1009.
    /// </summary>
    public class CIDsRegistroInsertModel
    {
        public string CodigoAssociado { get; set; }
        public string CodigoCid { get; set; }
        public DateTime? DataTermino { get; set; }
        public string ReferenciaImportacao { get; set; }
        public string InformacoesLogI { get; set; }
        public string InformacoesLogA { get; set; }
        public string IdInstanciaProcesso { get; set; }
    }

    /// <summary>
    /// Resultado do processamento de cada linha do CSV, usado para popular os grids da tela.
    /// </summary>
    public class CIDsImportResultModel
    {
        public int LinhaCsv { get; set; }
        public string Cpf { get; set; }
        public string Titular { get; set; }
        public string Beneficiario { get; set; }
        public string Cid { get; set; }
        public string CodigoAssociado { get; set; }
        public bool Sucesso { get; set; }
        public string Motivo { get; set; }
    }
}
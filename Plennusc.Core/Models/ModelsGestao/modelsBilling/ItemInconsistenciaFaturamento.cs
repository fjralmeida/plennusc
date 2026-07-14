using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.Models.ModelsGestao.modelsBilling
{
    public class ItemInconsistenciaFaturamento
    {
        // Campos da PS1020
        public int NumeroRegistro { get; set; }
        public int CodigoEmpresa { get; set; }
        public string CodigoAssociado { get; set; }
        public DateTime? DataVencimento { get; set; }
        public decimal ValorConvenio { get; set; }
        public decimal ValorAdicional { get; set; }
        public decimal ValorFatura { get; set; }
        public string MesAnoReferencia { get; set; }

        // Campos vindos do JOIN com VW_RELATORIO_CONFERENCIA
        public string NomeDoAssociado { get; set; }
        public string NumeroCpf { get; set; }
        public string NomeTabelaPreco { get; set; }
        public string NomeGrupoPessoas { get; set; }
        public string DescricaoGrupoFaturamento { get; set; }
        public DateTime? DataAdmissao { get; set; }
        public DateTime? DataExclusao { get; set; }
        public string NomeMotivoExclusao { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.Models.ModelsGestao.modelsBilling
{
    public class ResultadoViewConferencia
    {
        public decimal? ValorOperadora { get; set; }
        public DateTime? DataAdmissao { get; set; }
        public DateTime? DataExclusao { get; set; }
        public string NomeMotivoExclusao { get; set; }
        public string NomeTabelaPreco { get; set; }
        public string NomeGrupoPessoas { get; set; }
        public string DescricaoGrupoFaturamento { get; set; }
    }
}
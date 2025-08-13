using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.Models.ModelsGestao
{
    public class DataInsertionPriceTableMessage
    {
        public int NUMERO_REGISTRO { get; set; }
        public int CODIGO_PLANO { get; set; }
        public int CODIGO_TABELA_PRECO { get; set; }
        public int IDADE_MINIMA { get; set; }
        public int IDADE_MAXIMA { get; set; }
        public decimal VALOR_PLANO { get; set; }
        public string TIPO_RELACAO_DEPENDENCIA { get; set; }
        public int CODIGO_GRUPO_CONTRATO { get; set; }
        public string NOME_TABELA { get; set; }
        public decimal VALOR_NET { get; set; }
        public string TIPO_CONTRATO_ESTIPULADO { get; set; }
    }
}

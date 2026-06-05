using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.Models.ModelsGestao
{
    public class PrecoListDto
    {

            public int CodigoTabelaPreco { get; set; }
            public int CodigoPlano { get; set; }
            public int CodigoProduto { get; set; }
            public int FaixaBeneficiarios { get; set; }
            public DateTime DataInicioVenda { get; set; }
            public DateTime? DataFimVenda { get; set; }  
            public decimal? Faixa0 { get; set; }         
            public decimal? Faixa1 { get; set; }        
            public decimal? Faixa2 { get; set; }         
            public decimal? Faixa3 { get; set; }         
            public decimal? Faixa4 { get; set; }        
            public decimal? Faixa5 { get; set; }         
            public decimal? Faixa6 { get; set; }         
            public decimal? Faixa7 { get; set; }         
            public decimal? Faixa8 { get; set; }         
            public decimal? Faixa9 { get; set; }
            public bool Conf_ExibirVenda { get; set; }

    }
}


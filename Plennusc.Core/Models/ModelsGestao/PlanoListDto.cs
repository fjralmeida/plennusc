using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.Models.ModelsGestao
{
    public class PlanoListDto
    {
        public int CodigoPlano { get; set; }
        public int CodigoProduto { get; set; }
        public string RegistroANS { get; set; } 
        public string Num_CNPJ_Operadora { get; set; }
        public string TipoContratacao { get; set; }
        public string NomePlanoComercial { get; set; }
        public string Segmentacao { get; set; }
        public string Abrangencia { get; set; }
        public string Coparticipacao { get; set; }
        public string Acomodacao { get; set; }
        public string DecSau { get; set; }
        public string Promocional { get; set; }
        public bool Conf_Ativo { get; set; }  // BIT no banco
    }
}
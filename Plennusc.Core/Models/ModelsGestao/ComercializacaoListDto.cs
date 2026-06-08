using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.Models.ModelsGestao
{
    public class ComercializacaoListDto
    {
        public int CodigoComercializacaoMunicipio { get; set; }  
        public int CodigoIBGE { get; set; }                      
        public string NomeCidade { get; set; }                   
        public string SiglaEstado { get; set; }                 
        public string NomePlanoComercial { get; set; }           
        public bool Conf_Ativo { get; set; }                     
        public bool Conf_Exibir { get; set; }
    }
}

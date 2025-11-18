using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.Models.ModelsGestao.modelsUser
{
    public class UsuarioSistemaEmpresaMenu
    {
        public int CodAutenticacaoAcesso { get; set; }
        public string NomeCompleto { get; set; }
        public string UsrNomeLogin { get; set; }
        public int CodSistemaEmpresa { get; set; }
        public string SistemaEmpresaDisplay { get; set; }
        public bool JaVinculado { get; set; }
        public int CodSistemaEmpresaMenu { get; set; }
        public int CodMenu { get; set; }
        public string NomeDisplay { get; set; }
        public int Conf_Nivel { get; set; }
        public int Conf_Ordem { get; set; }
        public int? CodMenuPai { get; set; }
        public bool MenuJaVinculado { get; set; }
    }
}

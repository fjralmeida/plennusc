using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.Models.ModelsGestao.modelsUser
{
    public class VinculoUsuarioRequest
    {
        public int CodSistemaEmpresa { get; set; }
        public int CodAutenticacaoAcesso { get; set; }
        public int CodSistemaEmpresaMenu { get; set; }
    }

}

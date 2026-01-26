using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.Models.ModelsGestao.modelsSendMessage
{
    public class DetailedSubmissionResult
    {
        public string CodigoAssociado { get; set; }
        public string Nome { get; set; }
        public string Telefone { get; set; }
        public string TemplateEscolhido { get; set; }
        public string TemplateAplicado { get; set; }
        public bool BoletoDisponivel { get; set; }
        public bool NotaFiscalDisponivel { get; set; }
        public bool BoletoEnviado { get; set; }
        public bool NotaFiscalEnviada { get; set; }
        public string Status { get; set; }
        public string Motivo { get; set; }
    }
}

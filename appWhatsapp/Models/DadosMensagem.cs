using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace appWhatsapp.Models
{
    public class DadosMensagem
    {
        public string Telefone { get; set; }
        public string Field1 { get; set; } // Nome (destinatário)
        public string Field2 { get; set; } // Plano
        public string Field3 { get; set; } // Operadora
        public string Field4 { get; set; } // Vencimento (resumo)
        public string Field5 { get; set; } // Nome (titular)
        public string Field6 { get; set; } // Vencimento (detalhado)
        public string Field7 { get; set; } // Valor
        public string Field8 { get; set; } // URL
    }
}
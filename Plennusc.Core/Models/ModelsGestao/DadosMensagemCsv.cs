using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.Models.ModelsGestao
{
    public class DadosMensagemCsv
    {
        public string Telefone { get; set; }
        public string Field1 { get; set; } // Saudação ("Prezado"/"Prezada")
        public string Field2 { get; set; } // Papel ("Beneficiário"/"Beneficiária")
        public string Field3 { get; set; } // Nome
        public string Field4 { get; set; } // Data
        public string Field5 { get; set; } // CPF
        public string ModeloTipo { get; set; } // "Completo", "NovoPlano", "Simples"

    }
}

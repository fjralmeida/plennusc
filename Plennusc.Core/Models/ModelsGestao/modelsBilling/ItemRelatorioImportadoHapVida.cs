using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.Models.ModelsGestao.modelsBilling
{
    public class ItemRelatorioImportadoHapVida
    {
        public string Empresa { get; set; }
        public string Unidade { get; set; }
        public string NomeUnidade { get; set; }
        public string Credencial { get; set; }
        public string Matricula { get; set; }
        public string Cpf { get; set; }
        public string Beneficiario { get; set; }
        public string NomeMae { get; set; }
        public DateTime? Nascimento { get; set; }
        public DateTime? Inicio { get; set; }
        public int? Idade { get; set; }
        public string Parentesco { get; set; }
        public string Plano { get; set; }
        public string Ac { get; set; }
        public decimal Mensalidade { get; set; }
        public decimal Adicional { get; set; }
        public decimal TaxaAdesao { get; set; }
        public decimal Desconto { get; set; }
        public decimal Cobrado { get; set; }
    }
}

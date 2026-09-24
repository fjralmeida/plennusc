using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.Models.ModelsGestao.modelsCIDs
{
    public class ActivityReportModel
    {
        public string Modalidade { get; set; }
        public string Plano { get; set; }
        public string NomeTabelaPreco { get; set; } // NOVO - nome da tabela de preço (vinda da PS1032)
        public string Entidade { get; set; }
        public string Titular { get; set; }
        public string Nome { get; set; }
        public string Tipo { get; set; }
        public string Parentesco { get; set; }
        public string EstadoCivil { get; set; }
        public string Sexo { get; set; }
        public DateTime? DataNascimento { get; set; }
        public string Cpf { get; set; }
        public string Cns { get; set; }
        public string Telefone { get; set; }
        public string Endereco { get; set; }
        public string Complemento { get; set; }
        public string Bairro { get; set; }
        public string Cep { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set; }
        public DateTime? DtVigencia { get; set; }
        public string Email { get; set; }
        public string Filiacao1 { get; set; }
        public string Filiacao2 { get; set; }
        public string NomenclaturaCarencia { get; set; }
        public string Cid { get; set; }
    }
}
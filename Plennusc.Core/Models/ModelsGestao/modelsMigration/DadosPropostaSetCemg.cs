using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.Models.ModelsGestao.modelsMigration
{
    public class DadosPropostaSetCemg
    {
        // Campos brutos (podem vir com múltiplos valores separados por ";")
        public string VigenciaRaw { get; set; }
        public string VencimentoRaw { get; set; }
        public string ProdutoRaw { get; set; }
        public string MensalidadeRaw { get; set; }

        // Listas já separadas (mesma ordem/posição entre Produto e Mensalidade)
        public List<string> Vigencias { get; set; } = new List<string>();
        public List<string> Vencimentos { get; set; } = new List<string>();
        public List<string> Produtos { get; set; } = new List<string>();
        public List<string> Mensalidades { get; set; } = new List<string>();

        public string Proposta { get; set; }
        public string RazaoSocial { get; set; }
        public string NomeFantasia { get; set; }
        public string Cnpj { get; set; }
        public string InscricaoEstadual { get; set; }
        public string InscricaoMunicipal { get; set; }
        public string Logradouro { get; set; }
        public string Numero { get; set; }
        public string Complemento { get; set; }
        public string Bairro { get; set; }
        public string MunicipioUf { get; set; }
        public string Cep { get; set; }
        public string Email { get; set; }
        public string Telefone { get; set; }
        public string NomeResponsavel { get; set; }
        public string TelefoneResponsavel { get; set; }
        public string Cargo { get; set; }
        public string EmailResponsavel { get; set; }
        public string Modalidade { get; set; } // UNITÁRIO / FAMILIAR
        public string Aeromedico { get; set; } // SIM / NÃO
        public string Odontologia { get; set; } // SIM / NÃO
    }
}
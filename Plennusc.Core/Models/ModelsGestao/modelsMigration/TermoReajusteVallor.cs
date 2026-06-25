using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.Models.ModelsGestao.modelsMigration
{
    public class TermoReajusteVallor
    {
        // ── Titular ──────────────────────────────────────────────────────
        public string Nome { get; set; }
        public string Cpf { get; set; }
        public string Email { get; set; }
        public string NomeProduto { get; set; }
        public string Valor { get; set; }   // ex: "R$ 224,04" ou "224.04"
        public string Data { get; set; }

        // ── Dependentes (podem ser 0..N) ──────────────────────────────────
        public List<TermoReajusteDependente> Dependentes { get; set; } = new List<TermoReajusteDependente>();
    }

    public class TermoReajusteDependente
    {
        public string Nome { get; set; }
        public string Cpf { get; set; }
        public string Email { get; set; }
        public string NomeProduto { get; set; }
        public string Valor { get; set; }
        public string Data { get; set; }
    }
}

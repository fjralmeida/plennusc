using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.Models.ModelsGestao.modelsOperator
{
    public class OperadoraListDto
    {
        public int CodigoOperadora { get; set; }
        public string RegistroAns { get; set; }
        public string Numero_CNPJ { get; set; }
        public string RazaoSocial { get; set; }
        public string NomeComercial { get; set; }
        public int? CodPessoaCadastro { get; set; }
        public int? CodPessoaAlteracao { get; set; }
        public DateTime? Informacoes_log_i { get; set; }
        public DateTime? Informacoes_log_a { get; set; }

        /// <summary>
        /// Calculado: true se foi inserido nas últimas 24 horas.
        /// Usa Informacoes_log_i — sem coluna nova no banco.
        /// </summary>
        public bool IsRegistroNovo =>
            Informacoes_log_i.HasValue &&
            (DateTime.Now - Informacoes_log_i.Value).TotalHours <= 24;
    }
}

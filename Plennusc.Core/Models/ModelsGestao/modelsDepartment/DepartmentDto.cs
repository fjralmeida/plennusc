using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.Models.ModelsGestao.modelsDepartment
{
    public class DepartmentDto
    {
        public int CodDepartamento { get; set; }

        [Required(ErrorMessage = "O nome do departamento é obrigatório")]
        [StringLength(100, ErrorMessage = "O nome não pode ter mais de 100 caracteres")]
        public string Nome { get; set; }

        [StringLength(10, ErrorMessage = "O ramal não pode ter mais de 10 caracteres")]
        public string NumRamal { get; set; }

        [EmailAddress(ErrorMessage = "Informe um e-mail válido")]
        [StringLength(100, ErrorMessage = "O e-mail não pode ter mais de 100 caracteres")]
        public string EmailGeral { get; set; }

        [StringLength(20, ErrorMessage = "O telefone não pode ter mais de 20 caracteres")]
        public string Telefone { get; set; }

        public DateTime Informacoes_Log_I { get; set; }
    }

}

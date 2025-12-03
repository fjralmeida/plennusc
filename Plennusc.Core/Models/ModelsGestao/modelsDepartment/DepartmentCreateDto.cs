using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.Models.ModelsGestao.modelsDepartment
{
    public class DepartmentCreateDto
    {
        [Required]
        public string Nome { get; set; }
        public string NumRamal { get; set; }
        [EmailAddress]
        public string EmailGeral { get; set; }
        public string Telefone { get; set; }
    }
}

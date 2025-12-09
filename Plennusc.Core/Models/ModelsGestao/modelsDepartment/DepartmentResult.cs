using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.Models.ModelsGestao.modelsDepartment
{
    public class DepartmentResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public int DepartmentId { get; set; }
        public DataRow DepartmentData { get; set; }

    }
}

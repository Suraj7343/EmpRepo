using EmployeeAttendance.DAL.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeAttendance.BAL.Models
{
   public class DepartmentVM
    {
        public System.Guid DepartmentId { get; set; }

        [Required]
        public string DepartmentName { get; set; }
        public bool? IsDeleted { get; set; }
        public DateTime? CreatedOn { get; set; }
        public virtual ICollection<EmployeeDetail> EmployeeDetails { get; set; }
    }
}

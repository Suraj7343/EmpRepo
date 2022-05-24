using EmployeeAttendance.DAL.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace EmployeeAttendance.BAL.Models
{
   public class ProjectVM
    {
        public Guid ProjectId { get; set; }

        [Display(Name = "Project")]
        public string ProjectName { get; set; } 

        [Display(Name ="Department")]
        public Guid? DepartmentId { get; set; }

        [Display(Name = "Department Name")]
        public string DepName { get; set; }

        public bool? IsDeleted { get; set; }
        public DateTime? CreatedOn { get; set; }
        public virtual Department Department { get; set; }
    }
}

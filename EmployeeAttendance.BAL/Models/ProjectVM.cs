using EmployeeAttendance.DAL.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace EmployeeAttendance.BAL.Models
{
   public class ProjectVM
    {
        public Guid ProjectId { get; set; }
        public Guid PId { get; set; }
        public string ProjectName { get; set; } 
        public string PName { get; set; }
        public List<Project> GetProjectList { get; set; }
        public List<SelectListItem> Projects { get; set; }
        public Guid[] ProjectIds { get; set; }
    }
}

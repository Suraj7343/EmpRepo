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
    public class EmployeeVM
    {
        //EmployeeDetail
        public Guid EmployeeId { get; set; }
        public bool? CanLogIn { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Display(Name = "Contact")]
        [Required]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid phone number")]
        //[StringLength(10, ErrorMessage = "Must be at least 10 digit long.")]
        public string ContactNumber { get; set; }

        [Required]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? DateOfBirth { get; set; }

        [Display(Name = "Address")]
        [Required]
        public string EmployeeAddress { get; set; }

        [Display(Name = "Salary")]
        [Required]      
        public int? EmployeeSalary { get; set; }

        [Display(Name = "Image")]
        public string EmployeeImage { get; set; }
        public bool? IsDeleted { get; set; }
        public DateTime? CreatedOn { get; set; }

        [Display(Name = "Department")]
        public Guid? DepId { get; set; }

        [Display(Name = "Project")]
        public Guid? ProjId { get; set; }
        public Guid DeparmentId { get; set; }
        public string DepartmentName { get; set; }
        public string ProjectName { get; set; }
       
        public string UserName { get; set; }
        public string Message { get; set; }
        public TimeSpan? TimeIn { get; set; }
        public TimeSpan? TimeOut { get; set; }
        public TimeSpan? TotalTime { get; set; }
        public bool IsChecked { get; set; }
        public Guid? PrjId { get; set; }
        public DateTime? JoiningDate { get; set; }
        public virtual EmployeeDetail EmployeeDetail { get; set; }
        public virtual ICollection<Leave> Leaves { get; set; }
        public virtual ICollection<EmployeeDetail> EmployeeDetails { get; set; }
        public virtual UserLoginDetail UserLoginDetail { get; set; }
        public virtual ICollection<UserLoginDetail> UserLoginDetails { get; set; }
        #region MyRegion

        //public string Password { get; set; }

        //public System.Guid UserLoginDetailsId { get; set; }


        //public System.Guid LoginTimeId { get; set; } 

        //public System.Guid DepartmentId { get; set; }

        //public string LeaveStatus { get; set; }

        //public List<SelectListItem> Projects { get; set; }

        //public int[] ProjectIds { get; set; }

        //public DateTime? RelievingDate { get; set; }

        //public Guid? EmpId { get; set; }

        //public Guid? DeptId { get; set; }

        //public string PName { get; set; }

        //public List<Project> GetProjectList { get; set; }

        //public virtual Department Department { get; set; }

        //public virtual Project Project { get; set; }

        //public System.Guid Id { get; set; }

        //[Required]
        //[DataType(DataType.Date)]
        //public DateTime? StartDate { get; set; }

        //[Required]
        //[DataType(DataType.Date)]
        //public DateTime? EndDate { get; set; }

        //public bool Approve { get; set; }

        //public bool Reject { get; set; }

        //public Guid? UserLeaveId { get; set; }

        //public string Reason { get; set; }

        //Project

        //public bool? IsAdmin { get; set; }

        //public Guid ProjectId { get; set; }

        //public Guid PId { get; set; }
        #endregion
    }
}

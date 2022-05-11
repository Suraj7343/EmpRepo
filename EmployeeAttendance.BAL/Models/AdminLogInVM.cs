using EmployeeAttendance.DAL.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeAttendance.BAL.Models
{
   public class AdminLogInVM
    {
        [Required]
        public string Password { get; set; }
        public bool? IsAdmin { get; set; }
        public string UserName { get; set; }
        public string UserPassword { get; set; }
        public Guid UserLoginDetailsId { get; set; } //foreign key with employee table column employeeId
        public System.Guid Id { get; set; }   //Primary key column of Login Times Column
        public virtual UserLoginDetail UserLoginDetail { get; set; }
        #region MyRegion

        ////LogInTime
        //public Guid LoginTimeId { get; set; }  //foreign key with userLoginDetail Id column
        //public TimeSpan? TimeIn { get; set; }
        //public TimeSpan? TimeOut { get; set; }
        //public TimeSpan? TotalTime { get; set; }
        //public bool? IsDeleted { get; set; }
        //public DateTime? CreatedOn { get; set; }
        //public Guid DepId { get; set; }
        //public Guid ProjId { get; set; }
        //public virtual Department Department { get; set; }
        //public virtual Project Project { get; set; }
        //public Guid LoginId { get; set; }
        //[Required]
        //public string ContactNumber { get; set; }
        //[Required]
        //public DateTime? DateOfBirth { get; set; }
        #endregion
   }
}

using EmployeeAttendance.DAL.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeAttendance.BAL.Models
{
    public class LogInVM
    {
        public DateTime? CreatedOn { get; set; }
        public string Date { get; set; }
        public Guid LoginTimeId { get; set; }  //foreign key with userLoginDetail Id column
        public TimeSpan? TimeIn { get; set; }
        public TimeSpan? TimeOut { get; set; }
        public TimeSpan? TotalTime { get; set; }
        public System.Guid Id { get; set; }   //Primary key column of Login Times Column
        public Guid? ProjectID { get; set; }
        public string ProjectName { get; set; }
        public string Message { get; set; }
        public bool? LeaveStatus { get; set; }

        //[Required]
        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        //public DateTime? FromDate { get; set; }

        //[Required]
        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        //public DateTime? ToDate { get; set; }

        #region MyRegion
        //public virtual Project Project { get; set; }
        //public virtual UserLoginDetail UserLoginDetail { get; set; }
        //public List<DateTime> AllProjectTime { get; }
        //public TimeSpan? SumOfTotalTime { get; set; }
        //public Guid UserLoginDetailsId { get; set; } //foreign key with employee table column employeeId
        //public string Password { get; set; }
        //public bool? IsAdmin { get; set; }
        //public List<string> WeekDayName { get; set; }
        //public Guid LoginId { get; set; }
        //public string UserName { get; set; }
        //public string UserPassword { get; set; }
        //public bool? IsDeleted { get; set; }
        #endregion
    }
}

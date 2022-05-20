using EmployeeAttendance.DAL.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeAttendance.BAL.Models
{
    public class leaveVM
    {
        public System.Guid Id { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime? StartDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }
        public bool Approve { get; set; }
        public bool Reject { get; set; }
        public Guid? UserLeaveId { get; set; }
        public string Reason { get; set; }
        public System.Guid EmployeeId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string ContactNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string EmployeeAddress { get; set; }
        public int? EmployeeSalary { get; set; }
        public string EmployeeImage { get; set; }
        public System.Guid UserLoginDetailsId { get; set; }
        public string UserName { get; set; }
        public string ProjectName { get; set; }
        public string DepartmentName { get; set; }
        public string MsgForLeave { get; set; }
        //public TimeSpan? TimeIn { get; set; }
        public DateTime? TimeIn { get; set; }
        //public TimeSpan? TimeOut { get; set; }
        public string TimeInString { get; set; }
        public DateTime? TimeOut { get; set; }
        public string TimeOutString { get; set; }
        public TimeSpan? TotalTime { get; set; }
        public Guid? ProjectID { get; set; }
    }
}

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
        public string TimeInString { get; set; }
        public string TimeOutString { get; set; }
        public Guid LoginTimeId { get; set; }  //foreign key with userLoginDetail Id column
        public DateTime? TimeIn { get; set; }
        public DateTime? TimeOut { get; set; }

        public TimeSpan? TotalTime { get; set; }

        public System.Guid Id { get; set; }   //Primary key column of Login Times Column
        public Guid? ProjectID { get; set; }
        public string ProjectName { get; set; }
        public string Message { get; set; }
        public bool? LeaveStatus { get; set; }

    }
}

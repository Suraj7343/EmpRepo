using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeAttendance.BAL.Models
{
  public class EmailModelVM
    {
        public System.Guid EmailId { get; set; }

        [Required]
        [EmailAddress]
        public string EmailTo { get; set; }

        [Required]
        public string Subject { get; set; }

        [Required]
        public string Body { get; set; }

        [Required]
        [EmailAddress]
        public string SenderEmail { get; set; }

        [Required]
        public string Password { get; set; }

        //public bool? IsDeleted { get; set; }

        //public DateTime? CreatedOn { get; set; }
    }
}

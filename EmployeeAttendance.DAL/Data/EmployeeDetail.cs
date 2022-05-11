//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EmployeeAttendance.DAL.Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class EmployeeDetail
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public EmployeeDetail()
        {
            this.UserLoginDetails = new HashSet<UserLoginDetail>();
            this.WorkingDiagnoses = new HashSet<WorkingDiagnose>();
        }
    
        public System.Guid EmployeeId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string ContactNumber { get; set; }
        public Nullable<System.DateTime> DateOfBirth { get; set; }
        public string EmployeeAddress { get; set; }
        public Nullable<int> EmployeeSalary { get; set; }
        public string EmployeeImage { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<System.Guid> DepId { get; set; }
        public Nullable<System.Guid> ProjId { get; set; }
        public Nullable<bool> IsAdmin { get; set; }
        public Nullable<System.DateTime> JoiningDate { get; set; }
        public Nullable<System.DateTime> RelievingDate { get; set; }
        public Nullable<bool> CanLogIn { get; set; }
    
        public virtual Department Department { get; set; }
        public virtual Project Project { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserLoginDetail> UserLoginDetails { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<WorkingDiagnose> WorkingDiagnoses { get; set; }
    }
}

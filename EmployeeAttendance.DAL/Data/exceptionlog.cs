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
    
    public partial class exceptionlog
    {
        public System.Guid id { get; set; }
        public string errormessage { get; set; }
        public string source { get; set; }
        public string stacktrace { get; set; }
        public string target { get; set; }
        public string innerexceptionmessage { get; set; }
        public Nullable<System.Guid> userid { get; set; }
        public string message { get; set; }
        public byte[] createdon { get; set; }
    }
}

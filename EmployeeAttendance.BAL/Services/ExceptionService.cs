using EmployeeAttendance.DAL.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeAttendance.BAL.Services
{
    public static class ExceptionService
    {
        public static void SaveException(Exception ex, Guid? userId = null)
        {
            using (EmployeeDetailsDBEntities1 context = new EmployeeDetailsDBEntities1())
            {
                try
                {
                    exceptionlog entity = new exceptionlog();
                    entity.id = Guid.NewGuid();
                    entity.errormessage = ex.Message;
                    entity.source = ex.Source;
                    entity.stacktrace = ex.StackTrace;
                    entity.target = ex.TargetSite?.ToString();
                    entity.innerexceptionmessage = ex.InnerException?.ToString();
                    //entity.createdon = DateTime.Now;
                    entity.userid = userId;

                    context.exceptionlogs.Add(entity);
                    context.SaveChanges();
                }
                catch (Exception ex1)
                {
                    exceptionlog entity = new exceptionlog();

                    entity.errormessage = ex1.Message;
                  //entity.createdon = DateTime.Now;
                    context.exceptionlogs.Add(entity);
                    context.SaveChanges();
                }
            }
        }
    }
}

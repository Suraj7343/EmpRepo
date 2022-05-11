using EmployeeAttendance.BAL.Models;
using EmployeeAttendance.DAL.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace EmployeeAttendance.BAL.Services
{
   public class DepartmentService
    {
        EmployeeDetailsDBEntities1 _context;

        public DepartmentService()
        {
            _context = new EmployeeDetailsDBEntities1();
        }

         public bool AddDepartment(DepartmentVM departmentVM)
        {
            bool result = false;

            Department dep = new Department();

            Department data= _context.Departments.FirstOrDefault(x => x.DepartmentName == departmentVM.DepartmentName);

            if(data != null)
            {
                HttpContext.Current.Session["message"] = "Duplicate Department";
            }
            else
            {
                if (departmentVM != null)
                {
                    dep.DepartmentId = Guid.NewGuid();
                    dep.DepartmentName = departmentVM.DepartmentName;
                    dep.CreatedOn = DateTime.Now;
                    dep.IsDeleted = false;

                    _context.Departments.Add(dep);
                    _context.SaveChanges();

                    result = true;
                }
            }          
            return result;
         }

        public List<DepartmentVM> DepartmentList()
        {
            List<DepartmentVM> depVM = new List<DepartmentVM>();

           var data= _context.Departments.Where(x => x.IsDeleted == false).ToList();

            foreach (var item in data)
            {
                DepartmentVM dep = new DepartmentVM();

                dep.DepartmentId = item.DepartmentId;
                dep.DepartmentName = item.DepartmentName;

                depVM.Add(dep);
            }
            return depVM;
        }

        public DepartmentVM EditDepartment(Guid? id)
        {
            DepartmentVM departmentVM = new DepartmentVM();

            if(id !=null)
            {
                var depdetail = _context.Departments.FirstOrDefault(x => x.DepartmentId == id);

                departmentVM.DepartmentId = depdetail.DepartmentId;
                departmentVM.DepartmentName = depdetail.DepartmentName;
                departmentVM.IsDeleted = false;
                departmentVM.CreatedOn = DateTime.Now;
            }
            return departmentVM;
        }

        public bool UpdateDepartment(DepartmentVM departmentVM)
        {
            bool result = true;

            Department dep=  _context.Departments.FirstOrDefault(x => x.DepartmentId == departmentVM.DepartmentId);

            if(dep != null)
            {             
                dep.DepartmentName = departmentVM.DepartmentName;
                dep.IsDeleted = false;
                dep.CreatedOn = DateTime.Now;

                _context.Entry(dep).State = System.Data.Entity.EntityState.Modified;
                _context.SaveChanges();
            }
            return result;
        }

        public bool DeleteDepartment(Guid id)
        {
            bool result = true;

           Department dep= _context.Departments.FirstOrDefault(x => x.DepartmentId == id);

            if(dep != null)
            {
                dep.IsDeleted = true;
                _context.SaveChanges();
            }
            return result;
        }
   }
}

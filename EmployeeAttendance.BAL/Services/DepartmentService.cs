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

        public List<ProjectVM> ProjectList()
        {
            List<ProjectVM> projectVMs = new List<ProjectVM>();

            var data = _context.Projects.Where(x => x.IsDeleted == false && x.DepartmentId !=null);

            foreach(var item in data)
            {
                ProjectVM model = new ProjectVM();

                model.ProjectId = item.ProjectId;
                model.ProjectName = item.ProjectName;
                model.DepartmentId = item.DepartmentId;
                model.DepName =item.Department.DepartmentName;
                projectVMs.Add(model);
            }
            return projectVMs;
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

            Department dep = _context.Departments.FirstOrDefault(x => x.DepartmentId == departmentVM.DepartmentId);

            if (dep != null)
            {
                dep.DepartmentName = departmentVM.DepartmentName;
                dep.IsDeleted = false;
                dep.CreatedOn = DateTime.Now;

                _context.Entry(dep).State = System.Data.Entity.EntityState.Modified;
                _context.SaveChanges();
            }
            return result;
        }

        public ProjectVM EditProject(Guid? id)
        {  
            ProjectVM projectVM = new ProjectVM();

            if(id !=null)
            {
                var detail = _context.Projects.FirstOrDefault(x => x.ProjectId == id);

                projectVM.ProjectId = detail.ProjectId;
                projectVM.ProjectName = detail.ProjectName;
                projectVM.DepartmentId = detail.DepartmentId;
                //projectVM.DepName = detail.Department.DepartmentName;
                projectVM.IsDeleted = false;
                projectVM.CreatedOn = DateTime.Now;
            }
            return projectVM;
        }
        
        public bool UpdateProject(ProjectVM projectVM)
        {
            bool result = true;

            Project project=  _context.Projects.FirstOrDefault(x => x.ProjectId == projectVM.ProjectId);

            if(project != null)
            {
                project.ProjectId = projectVM.ProjectId;
                project.ProjectName = projectVM.ProjectName;
                project.DepartmentId = projectVM.DepartmentId;
                //project.Department.DepartmentName = projectVM.DepName;
                project.IsDeleted = false;
                project.CreatedOn = DateTime.Now;

                _context.Entry(project).State = System.Data.Entity.EntityState.Modified;
                _context.SaveChanges();
            }
            return result;
        }

        public bool DeleteDepartment(Guid id)
        {
            bool result = true;

           Department dep= _context.Departments.FirstOrDefault(x => x.DepartmentId == id);
            Guid departmentId = dep.DepartmentId;

            if(dep != null)
            {
                dep.IsDeleted = true;
                Project model= _context.Projects.FirstOrDefault(x=>x.DepartmentId==departmentId);
                if(model != null)
                {
                   model.IsDeleted = true;
                }
                _context.SaveChanges();
            }
            return result;
        }

         public bool DeleteProject(Guid id)
        {
            bool result = true;

           Project project= _context.Projects.FirstOrDefault(x => x.ProjectId == id);

            if(project != null)
            {
                project.IsDeleted = true;
                _context.SaveChanges();
            }
            return result;
        }

        public bool CreateProjectOnDepartmentBasis(ProjectVM model)
        {
            bool result = false;

            try
            {
                Project project = new Project();

                if(model != null)
                {
                    project.ProjectId = Guid.NewGuid();
                    project.ProjectName = model.ProjectName;
                    project.DepartmentId = model.DepartmentId;
                    project.IsDeleted = false;
                    project.CreatedOn = DateTime.Now;

                    _context.Projects.Add(project);
                    _context.SaveChanges();
                    result = true;
                }
            }
            catch(Exception ex)
            {
                ExceptionService.SaveException(ex);
            }
            return result;
        }
   }
}

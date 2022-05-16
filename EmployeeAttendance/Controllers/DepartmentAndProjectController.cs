using EmployeeAttendance.BAL.Models;
using EmployeeAttendance.BAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EmployeeAttendance.Controllers
{
    public class DepartmentAndProjectController : Controller
    {
        private readonly DepartmentService _service;

        private readonly RegistrationService _regservice;
        public DepartmentAndProjectController()
        {
            _service = new DepartmentService();
            _regservice = new RegistrationService();
        }

        // GET: DepartmentAndProject
        public ActionResult Index()
        {
            List<DepartmentVM> departments = new List<DepartmentVM>();

            departments = _service.DepartmentList();
            return View(departments);
        }

        public ActionResult Projects()
        {
            List<ProjectVM> model = new List<ProjectVM>();
            ViewBag.DepartmentList = _regservice.GetDepartmentList();
            model = _service.ProjectList();
            return View(model);

        }

        public ActionResult CreateDepartment()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateDepartment(DepartmentVM departmentVM)
        {
            bool data = _service.AddDepartment(departmentVM);

            if (data == true)
            {
                return RedirectToAction("Index", "DepartmentAndProject");
            }
            else
            {
                TempData["message"] = Session["message"];
                TempData.Keep();

                return RedirectToAction(nameof(CreateDepartment));
            }

        }

        public ActionResult Edit(Guid? id)
        {
            if (id == null)
                return View();

            DepartmentVM data = _service.EditDepartment(id);
            return View(data);
        }

        [HttpPost]
        public ActionResult Edit(DepartmentVM departmentVM)
        {
            _service.UpdateDepartment(departmentVM);
            return RedirectToAction(nameof(Index));
        }

        public ActionResult EditProject(Guid? id)
        {
            if (id == null)
                return View();
            ViewBag.DepartmentList = _regservice.GetDepartmentList();
            ProjectVM data = _service.EditProject(id);
            return View(data);
        }

        [HttpPost]
        public ActionResult EditProject(ProjectVM projectVM)
        {
         
            _service.UpdateProject(projectVM);
            return RedirectToAction(nameof(Projects));
        }

        public ActionResult DeleteProject(Guid id)
        {
            _service.DeleteProject(id);
            return RedirectToAction(nameof(Projects));
        }

        public ActionResult Delete(Guid id)
        {
            _service.DeleteDepartment(id);
            return RedirectToAction(nameof(Index));
        }

        public ActionResult ProjectManagement()
        {
            ViewBag.DepartmentList = _regservice.GetDepartmentList();
            return View();
        }

        [HttpPost]
        public ActionResult ProjectManagement(ProjectVM model)
        {
            ViewBag.DepartmentList = _regservice.GetDepartmentList();
            _service.CreateProjectOnDepartmentBasis(model);
            return RedirectToAction(nameof(Projects));
        }
    }
}
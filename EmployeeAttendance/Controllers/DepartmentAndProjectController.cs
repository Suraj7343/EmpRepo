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
        public DepartmentAndProjectController()
        {
            _service = new DepartmentService();
        }

        // GET: DepartmentAndProject
        public ActionResult Index()
        {
            List<DepartmentVM> departments = new List<DepartmentVM>();

            departments = _service.DepartmentList();
            return View(departments);
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

        public ActionResult Delete(Guid id)
        {
            _service.DeleteDepartment(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
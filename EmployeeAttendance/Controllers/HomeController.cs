using EmployeeAttendance.BAL.Models;
using EmployeeAttendance.BAL.Services;
using EmployeeAttendance.WebHelper;
using System;
using System.Collections.Generic;
//using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EmployeeAttendance.Controllers
{
    public class HomeController : Controller
    {
        private readonly RegistrationService _service;

        public HomeController()
        {
            _service = new RegistrationService();
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login()
        {
            //var sessionTimeOut = Session.Timeout;
            //if (sessionTimeOut <= 20)
            //{
            //    Session.Timeout = 500;
            //}
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(AdminLogInVM adminLogInVM)
        {
            //    Guid logInVM = Guid.Empty;
            //bool logInVM = false;
            LogInVM model = new LogInVM();
            AdminLogInVM result = new AdminLogInVM();
            EmployeeVM userDetails = new EmployeeVM();

            if (adminLogInVM != null)
            {
                result = _service.Login(adminLogInVM);
                Session[SessionKey.isAdmin] = result.IsAdmin;
                Session[SessionKey.userId] = result.Id; //userlogindetail table id

                if (result != null)
                {
                    userDetails = _service.GetAllUserDetails(result.UserLoginDetailsId);
                    if (Session[SessionKey.userId] != null)
                    {
                        model = _service.LoginTime(result.Id);
                    }
                    Session[SessionKey.logInTimeTableId] = model.Id; //logintime table id

                    Session[SessionKey.userName] = userDetails.UserName;
                    Session[SessionKey.empId] = userDetails.EmployeeId;
                    if (result.IsAdmin == true)
                    {
                        return RedirectToAction("Index", "Employee");
                    }
                    else
                    {
                        var empDetails = _service.DisplayDetail(adminLogInVM);
                        Session[SessionKey.Data] = empDetails;
                        return RedirectToAction("Index", "DashBoard");
                    }
                }
                else
                {
                    ViewBag.Message = "wrong UserName Or Password..";
                    return View();
                }
            }
            return View();
        }
    }
}
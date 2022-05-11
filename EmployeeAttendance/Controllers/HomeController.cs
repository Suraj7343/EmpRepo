using EmployeeAttendance.BAL.Models;
using EmployeeAttendance.BAL.Services;
using EmployeeAttendance.WebHelper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
            Guid logInVM = Guid.Empty;
            AdminLogInVM result = new AdminLogInVM();
            EmployeeVM userDetails = new EmployeeVM();

            if (adminLogInVM != null)
            {
                result = _service.Login(adminLogInVM);
                Session[SessionKey.isAdmin] = result.IsAdmin;
                Session[SessionKey.userId] = result.Id;

                if (result != null)
                {
                    userDetails = _service.GetAllUserDetails(result.UserLoginDetailsId);
                    if (Session[SessionKey.userId] != null)
                    {
                        logInVM = _service.LoginTime(result.Id);
                    }
                    Session[SessionKey.logInTimeTableId] = logInVM;

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
        #region LogInAndLogOutTestCode
        //public ActionResult Login(AdminLogInVM adminLogInVM)
        //{
        //    #region MyRegion
        //    /* bool UserDetails = _service.AdminLogin(adminLogInVM);
        //   if (ModelState.IsValid)
        //   {
        //       if (UserDetails == false)
        //       {
        //           ModelState.AddModelError("Failure", "Wrong Username and password combination !");

        //           return View();

        //       }

        //       else
        //       {
        //           string abc = "EmployeeDetailsDBEntities";
        //           SqlConnection con = new SqlConnection(abc);
        //           con.Open();
        //           SqlCommand comm = new SqlCommand("insert into LoginTimes values(@UserId,@TimeIn)", con);
        //           comm.Parameters.AddWithValue("@UserId", adminLogInVM.LoginTimeId);
        //           comm.Parameters.AddWithValue("@TimeIn", DateTime.Now);
        //           comm.ExecuteNonQuery();
        //           //Close the Connection
        //           con.Close();
        //           return RedirectToAction("Index", "Employee");
        //       }
        //   }
        //   else
        //   {
        //       //If model state is not valid, the model with error message is returned to the View.
        //       return View(UserDetails);
        //   }*/
        //    #endregion

        //    bool UserDetails = _service.AdminLogin(adminLogInVM);
        //    //Session["userId"] = adminLogInVM.Id;
        //    if (UserDetails)
        //    {
        //        return RedirectToAction("Index", "Employee");
        //    }
        //    else
        //    {
        //        _service.DisplayDetail(adminLogInVM);
        //        return RedirectToAction("Index", "DashBoard");

        //    }
        //}

        //[HttpGet]
        //public ActionResult LogOff()
        //{
        //  bool abc=  _service.UpdateLoginLastUpdateStatus();
        //    if (abc)
        //    {
        //        Session.Abandon();
        //        return RedirectToAction("Index", "Home");
        //    }
        //    return View();

        //}
        #endregion
    }
}
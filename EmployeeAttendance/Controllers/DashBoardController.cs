using EmployeeAttendance.BAL.Models;
using EmployeeAttendance.BAL.Services;
using EmployeeAttendance.DAL.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EmployeeAttendance.WebHelper;


namespace EmployeeAttendance.Controllers
{
    public class DashBoardController : Controller
    {
         private readonly RegistrationService _service;

        // GET: DashBoard
        public DashBoardController()
        {
            _service = new RegistrationService();
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View();  
        }

        public ActionResult SearchByDates(DateTime? FromDate, DateTime? ToDate)
        {
            List<LogInVM> data = new List<LogInVM>();

            var userId = new Guid(Session[SessionKey.userId].ToString());

            data = _service.EmployeeDashBoardList(userId, FromDate, ToDate);
            return PartialView("_EmployeeDashBoard", data);
        }

        [HttpGet]
        public JsonResult TimeRelatedToProjec(DateTime? Date)
        {
            return Json(Date);
        }

        [HttpGet]
        public ActionResult PopUp(string Date/*, Guid LoginTimeId*/)
        {
            List<LogInVM> model = new List<LogInVM>();

            var LoginTimeId = new Guid(Session[SessionKey.userId].ToString());
            model = _service.TimeRelatedToProject(LoginTimeId, Date); //List of Time picked on the basis of created on
            return PartialView("PopUp", model);
        }

        public ActionResult SearchByDate()
        {
            return View();
        }

        //[HttpPost]
        //public ActionResult SearchByDate(DateTime? FromDate, DateTime? ToDate)
        //{
        //    List<LogInVM> model = new List<LogInVM>();
        //    model=  _service.SearchByDate(FromDate, ToDate);
        //    return View(model);
        //}

        public ActionResult Detail()
        {
            if (Session["EmpID"] != null)
            {
                ViewBag.ProjectList = _service.GetProjectList();
            }
            else
            {
                return View();
            }
            return View(Session["Data"]);
        }

        /// <summary>
        /// Time count ends 
        /// controller-DashBoardRelatedToProject
        /// BackToIndex
        /// </summary>
        /// <returns></returns>
       
        public ActionResult TimeCount()
        {
            _service.TotalTimeCount();
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Multiple checkboxes(Employee- switch to project by this)
        /// </summary>
        /// <param name="projectId">Id related to project i.e id of that project to which employee clicked</param>
        /// <returns>Next Page</returns>
      
        public JsonResult UpdateCheckBox(Guid projectId)
        {
            //var userId = new Guid(Session["LogOut"].ToString());
            var userId = new Guid(Session[SessionKey.userId].ToString());
            _service.StartTimeRelatedToProject(projectId, userId);
            return Json("Successful");
        }

        public ActionResult DashBoardRelatedToProject()
        {
            var projectName = Session["ProjectName"];

            TempData["projectName"] = projectName;
            TempData.Keep("projectName");

            return View();
        }

        #region LeaveNotification
        //list of leaves which is approved or rejected by the admin shown to employee
        #endregion

        public ActionResult LeaveNotification()
        {
            List<leaveVM> leave = new List<leaveVM>();

            leave = _service.ApproveOrReject();
            return View(leave);
        }

        #region ListOfLeave
        //all leaves requests
        #endregion

        public ActionResult ListOfLeave()
        {
            List<leaveVM> leave = new List<leaveVM>();

            leave = _service.ListOfLeaves();
            return View(leave);
        }

        public ActionResult Leave()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Leave(leaveVM leave)
        {
            _service.Leave(leave);
            return RedirectToAction(nameof(ListOfLeave));
        }

        public ActionResult Delete(Guid id)
        {
            _service.DeleteLeave(id);
            return RedirectToAction(nameof(ListOfLeave));
        }
    }
}

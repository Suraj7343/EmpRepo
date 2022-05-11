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
        EmployeeDetailsDBEntities1 _context;

        // GET: DashBoard
        private readonly RegistrationService _service;

        public DashBoardController()
        {
            _service = new RegistrationService();
            _context = new EmployeeDetailsDBEntities1();
        }

        public ActionResult Index()
        {
            if(Session["EmpID"] != null)
            {
               ViewBag.ProjectList = _service.GetProjectList();
            }
            else
            {
                return View();
            }
            return View(Session["Data"]);

        }

        public ActionResult EmployeeDashBoardList()
        {
            List<LogInVM> data = new List<LogInVM>();
            //var userId = (Guid)Session[SessionKey.userId];
            var userId = new Guid(Session[SessionKey.userId].ToString());

            data = _service.EmployeeDashBoardList(userId);

            return PartialView("_EmployeeDashBoard", data);
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
        public JsonResult UpdateCheckBox( Guid projectId)
        {
            //var userId = new Guid(Session["LogOut"].ToString());
            var userId = new Guid(Session[SessionKey.userId].ToString()); 
            _service.StartTimeRelatedToProject( projectId, userId);
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

            leave= _service.ApproveOrReject();
            return View(leave);
        }

        #region ListOfLeave
        //all leaves requests
        #endregion

        public ActionResult ListOfLeave()
        {
            List<leaveVM> leave = new List<leaveVM>();

          leave=  _service.ListOfLeaves();
            return View(leave);
        }

        public ActionResult GetData() //LogInVM logInVM
        {
            var xyz = _service.DashBoard();

            List<string> days = new List<string>();
            foreach (DayOfWeek day in Enum.GetValues(typeof(DayOfWeek))
                              .OfType<DayOfWeek>()
                              .ToList()
                              .Skip(1))
            {
                days.Add(day.ToString());
            }

            days.Add(DateTime.Now.DayOfWeek.ToString());

            ViewBag.days = DateTime.Now.DayOfWeek;

            LoginTime loginTime = new LoginTime();

            loginTime.TotalTime = (TimeSpan?)Session["Count"];

            var abc = loginTime.TotalTime.ToString();

            var query = _context.LoginTimes.Where(x=>x.TotalTime==xyz.TotalTime && x.CreatedOn == xyz.CreatedOn)/*.Where(x => x.LoginTimeId == xyz.LoginTimeId)*/
                   .GroupBy(p => p.UserLoginDetail.UserName )
                   .Select(g => new { name = g.Key, count = /*days*/loginTime.TotalTime.ToString()  /*count = loginTime.TotalTime*/ })
                   .ToList();  //count = loginTime.TotalTime

            return Json(query, JsonRequestBehavior.AllowGet);
     
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
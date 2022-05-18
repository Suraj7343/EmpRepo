using EmployeeAttendance.BAL.Models;
using EmployeeAttendance.BAL.Services;
using EmployeeAttendance.DAL.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Mvc;
using EmployeeAttendance.Common;
using EmployeeAttendance.WebHelper;
using System.Data.SqlClient;

namespace EmployeeAttendance.Controllers
{
    public class EmployeeController : Controller
    {

        // GET: Employee
        private readonly RegistrationService _service;

        public EmployeeController()
        {
            _service = new RegistrationService();
        }

        public ActionResult Index(string sortOrder, string CurrentSort, int? page)
        {
            var result = _service.Pagination(sortOrder, CurrentSort, page);
            return View(result);
        }

        [HttpPost]
        public JsonResult MultiSelectEmail(List<string> email)   //employeeId
        {
            string JoinDataString = string.Join(",", email.ToArray());
            return Json(JoinDataString);
        }

        //for Searching
        public ActionResult Display()
        {
            //var data = _service.GetEmployee();
            return View();
        }

        [HttpPost]
        public ActionResult Display(string Search)
        {
            var data = _service.FindData(Search);
            return View(data);
        }

        public ActionResult PopUp()
        {
            List<LogInVM> model = new List<LogInVM>();

            Guid? userId = (Guid?)null;
            if (Session["LogOut"] != null)
            {
                userId = new Guid(Session["LogOut"].ToString());
                model = _service.ProjectTotalTimeCount();
            }
            else
            {
                if (Session[SessionKey.logInTimeTableId] != null)
                {
                    return RedirectToAction(nameof(LogOff));
                }
                else
                {
                    return RedirectToAction("Index", "DashBoard");
                }
            }
            return View(model);
        }

        public ActionResult LogOff()
        {
            string status = Common.Constants.LogOff;
            Guid userId = (Guid)Session[SessionKey.userId];
            //Guid logInVM = _service.LoginTime(userId, status);
            if (Session[SessionKey.logInTimeTableId] != null)
            {
                var logInTimeTableId = (Guid)Session[SessionKey.logInTimeTableId];
                _service.DirectLogOutTime(logInTimeTableId);
            }
            Session.Abandon();
            return RedirectToAction("Login", "Home");
        }

        public ActionResult Create()
        {
            ViewBag.DepartmentList = _service.GetDepartmentList();
            return View();
        }

        [HttpPost]
        public ActionResult Create(HttpPostedFileBase file, EmployeeVM emp, ProjectVM project, string CanLogIn)
        {
            try
            {
                if (emp != null)
                {
                    string filename = Path.GetFileName(file.FileName);
                    string _filename = DateTime.Now.ToString("yymmssfff") + filename;
                    string extenion = Path.GetExtension(file.FileName);
                    string path = Path.Combine(Server.MapPath("~/images/"), _filename);
                    emp.EmployeeImage = "~/images/" + _filename;

                    if (extenion.ToLower() == Constants.JPG || extenion.ToLower() == Constants.JPEG || extenion.ToLower() == Constants.PNG)
                    {
                        if (file.ContentLength <= 1000000)
                        {
                            ViewBag.DepartmentList = _service.GetDepartmentList();

                            bool modal = _service.CreateEmployeeData(emp, CanLogIn);

                            file.SaveAs(path);
                            ModelState.Clear();

                            return RedirectToAction(nameof(Index));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionService.SaveException(ex);
            }
            return View();
        }

        //private static List<SelectListItem> PopulateProjects()
        //{
        //    List<SelectListItem> items = new List<SelectListItem>();

        //    string abc = ConfigurationManager.ConnectionStrings["EmployeeDetailsDBEntities1"].ConnectionString;

        //    if (abc.ToLower().StartsWith("metadata="))
        //    {
        //        System.Data.Entity.Core.EntityClient.EntityConnectionStringBuilder efBuilder = new System.Data.Entity.Core.EntityClient.EntityConnectionStringBuilder(abc);

        //        abc = efBuilder.ProviderConnectionString;
        //    }
        //    using (SqlConnection con = new SqlConnection(abc))
        //    {
        //        string query = " SELECT ProjectName, ProjectId FROM Project";

        //        using (SqlCommand cmd = new SqlCommand(query))
        //        {
        //            cmd.Connection = con;
        //            con.Open();
        //            using (SqlDataReader sdr = cmd.ExecuteReader())
        //            {
        //                while (sdr.Read())
        //                {
        //                    items.Add(new SelectListItem
        //                    {
        //                        Text = sdr["ProjectName"].ToString(),
        //                        Value = sdr["ProjectId"].ToString()
        //                    });
        //                }
        //            }
        //            con.Close();
        //        }
        //    }

        //    return items;
        //}

        public ActionResult Edit(Guid? id)
        {
            if (id == null)
                return View();

            ViewBag.DepartmentList = _service.GetDepartmentList();
            //ViewBag.ProjectList = _service.GetProjectList();
            EmployeeVM data = _service.EditEmployeeData(id);

            Session[SessionKey.ImagesPath] = data.EmployeeImage;

            return View(data);
        }

        [HttpPost]
        public ActionResult Edit(HttpPostedFileBase file, EmployeeVM emp)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (file != null)
                    {
                        string filename = Path.GetFileName(file.FileName);
                        string _filename = DateTime.Now.ToString("yymmssfff") + filename;
                        string extenion = Path.GetExtension(file.FileName);
                        string path = Path.Combine(Server.MapPath("~/images/"), _filename);
                        emp.EmployeeImage = "~/images/" + _filename;

                        if (extenion.ToLower() == Constants.JPG || extenion.ToLower() == Constants.JPEG || extenion.ToLower() == Constants.PNG)
                        {
                            if (file.ContentLength <= 1000000)
                            {
                                string oldImagePath = Request.MapPath(Session[SessionKey.ImagesPath].ToString());

                                bool modal = _service.UpdateEmployeeList(emp, oldImagePath);

                                file.SaveAs(path);
                                #region MyRegion
                                //if (System.IO.File.Exists(oldImagePath))
                                //{
                                //    System.IO.File.Delete(oldImagePath);
                                //}
                                //_context.Entry(employee).State = EntityState.Modified;
                                #endregion
                                return RedirectToAction(nameof(Details), new { id = emp.EmployeeId });
                            }
                        }
                    }
                }
                else
                {
                    ViewBag.DepartmentList = _service.GetDepartmentList();
                    emp.EmployeeImage = Session[SessionKey.ImagesPath].ToString();
                    string oldImagePath = null;
                    bool modal = _service.UpdateEmployeeList(emp, oldImagePath);
                    return RedirectToAction(nameof(Details), new { id = emp.EmployeeId });
                }
            }
            catch (Exception ex)
            {
                ExceptionService.SaveException(ex);
            }
            return View();
        }

        public ActionResult Delete(Guid id)
        {
            bool data = _service.DeleteData(id);
            return RedirectToAction(nameof(Display));
        }

        #region ProjectAssignedToEmployee
        // in details detail of the employee comes. also admin can assign number of projects to the employee
        #endregion

        public ActionResult Details(Guid id)
        {
            ViewBag.DepartmentList = _service.GetDepartmentList();
            ViewBag.ProjectList = _service.GetProjectOfEmployee();  //MultiSelect
            ViewBag.Projects = _service.ProjectAssigned(id);

            EmployeeVM employeeVM = _service.Detail(id);

            Session["deparmentId"] = employeeVM.DeparmentId;
            Session["empId"] = employeeVM.EmployeeId;

            return View(employeeVM);
        }

        /// <summary>
        /// add multiple project in WorkingDiagnose table(these id's are selected using json)
        /// </summary>
        /// <param name="projectId">List of all id's comes here i.e projects that asigned to employee</param>
        /// <returns>add all data in the database and gave succes msg</returns>

        [HttpPost]
        public JsonResult DetailsProject(List<Guid> projectId)
        {
            var depId = (Guid)Session["deparmentId"];
            var employeeId = (Guid)Session["empId"];

            bool result = _service.AddProject(projectId, depId, employeeId);

            return Json("Data Added Successfully");
        }

        public ActionResult SendM(Guid? id)
        {
            try
            {
                if (id != null)
                using (EmployeeDetailsDBEntities1 _context = new EmployeeDetailsDBEntities1())
                {
                    var result = _context.EmployeeDetails.FirstOrDefault(x => x.EmployeeId == id);

                    Session["result"] = result.Email;
                    return RedirectToAction("SendMail", new { result });
                }
            }
            catch (Exception ex)
            {
                ExceptionService.SaveException(ex);
            }
            return View();
        }

        [HttpGet]
        public ActionResult SendMail(List<string> email)
        {
            EmailModelVM model = new EmailModelVM();
            if (email != null)
            {
                model.EmailTo = string.Join(",", email.ToArray());
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult SendMail(EmailModelVM model)
        {
            bool result = _service.SendMailSmtp(model);
            return RedirectToAction(nameof(Index));
        }

        #region Notification
        //notification button to see the leaves requests(applied by the employess)
        #endregion

        public ActionResult Notification()
        {
            List<leaveVM> leave = new List<leaveVM>();

            leave = _service.ListOfLeavesShownToAdmin();
            return View(leave);
        }

        #region LeaveDetails
        //detail of the employees which applied for the leave(full detail like Name, address, salary, Dep, Proj etc)
        #endregion

        public ActionResult LeaveDetails(Guid id)
        {
            ViewBag.DepartmentList = _service.GetDepartmentList();

            leaveVM leaveVM = _service.LeaveDetails(id);

            Session["EmployeeId"] = leaveVM.EmployeeId;
            Guid Id = new Guid(Session["EmployeeId"].ToString());

            ViewBag.Projects = _service.ProjectAssigned(Id);
            return View(leaveVM);
        }

        #region ApproveRejectRequest
        //ApprovalOrRejection of the leave(done by the admin)
        #endregion

        [HttpGet]
        public ActionResult ApproveRejectRequest(Guid id)
        {
            leaveVM data = _service.ApproveOrRejectLeaveRequest(id);
            return View(data);
        }

        [HttpPost]
        public ActionResult ApproveRejectRequest(leaveVM leave)
        {
            _service.UpdateLeaveRequest(leave);

            TempData["message"] = Session["message"];
            TempData.Keep();

            return RedirectToAction(nameof(Notification));
        }

        public ActionResult TimeDetails(Guid id)
        {
            List<leaveVM> leave = new List<leaveVM>();

            leave = _service.TimeDetail(id);
            return View(leave);
        }

        public ActionResult TotalTimeOfEmployees(string Search)
        {
            var leave = _service.TotalTimeOfEmployees(Search);
            return View(leave);
        }
    }
}
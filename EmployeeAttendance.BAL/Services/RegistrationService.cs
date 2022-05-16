using EmployeeAttendance.BAL.Models;
using EmployeeAttendance.DAL.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using EmployeeAttendance.Common;
using PagedList;

namespace EmployeeAttendance.BAL.Services
{
    public class RegistrationService
    {
        private readonly EmployeeDetailsDBEntities1 _context;

        public RegistrationService()
        {
            _context = new EmployeeDetailsDBEntities1();
        }

        public AdminLogInVM Login(AdminLogInVM adminLogInVM)
        {
            AdminLogInVM result = new AdminLogInVM();
            try
            {
                var login = _context.UserLoginDetails.Where(x => x.IsDeleted == false && (x.Email == adminLogInVM.UserName || x.UserName == adminLogInVM.UserName) && x.Password == adminLogInVM.Password)
                    .FirstOrDefault();

                result.Id = login.Id;
                result.UserLoginDetailsId = login.UserLoginDetailsId;
                result.UserName = login.UserName;
                result.IsAdmin = login.IsAdmin;
            }
            catch(Exception ex)
            {
                ExceptionService.SaveException(ex);
            }
            return result;
        }

        public EmployeeVM GetAllUserDetails(Guid userId) // guid id
        {
            EmployeeVM result = new EmployeeVM();
            try
            {
                if (userId != Guid.Empty)
                {
                    var userDetails = _context.EmployeeDetails.FirstOrDefault(x => x.IsDeleted == false && x.EmployeeId == userId);

                    result.EmployeeId = userDetails.EmployeeId;
                    result.UserName = userDetails.FirstName + " " + userDetails.LastName;
                }
            }
            catch (Exception ex)
            {
                ExceptionService.SaveException(ex);
            }
            return result;
        }

        //public void LoginTime(Guid userId)
        //{
        //    if (userId != Guid.Empty)
        //    {
        //        LoginTime loginTime = new LoginTime();

        //        loginTime.Id = Guid.NewGuid();

        //        HttpContext.Current.Session["logInTimesTableID"] = loginTime.Id;

        //        loginTime.LoginTimeId = userId;
        //        TimeSpan CurrentTime = DateTime.Now.TimeOfDay;
        //        loginTime.TimeIn = CurrentTime;
        //        //loginTime.ProjectID = ProjectId;
        //        loginTime.IsDeleted = false;
        //        loginTime.CreatedOn = DateTime.Now.Date;
        //        loginTime.LeaveStatus = false;

        //        _context.LoginTimes.Add(loginTime);
        //        _context.SaveChanges();
        //    }
        //}

        public Guid LoginTime(Guid userId)
        {
            LogInVM logInVM = new LogInVM();
            Guid result = Guid.Empty;
            try
            {
                if (userId != Guid.Empty)
                {
                    LoginTime loginTime = new LoginTime();

                    loginTime.Id = Guid.NewGuid();
                    logInVM.Id = loginTime.Id;  // this id is used to logout the projects
                    result = loginTime.Id;

                    loginTime.LoginTimeId = userId;
                    TimeSpan CurrentTime = DateTime.Now.TimeOfDay;
                    loginTime.TimeIn = CurrentTime;
                    //loginTime.ProjectID = ProjectId;
                    loginTime.IsDeleted = false;
                    loginTime.CreatedOn = DateTime.Now.Date;
                    loginTime.LeaveStatus = false;

                    _context.LoginTimes.Add(loginTime);
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ExceptionService.SaveException(ex);
            }
            return result;
        }

        /// <summary>
        /// time countin start when employee switch to particular project 
        /// </summary>
        /// <param name="projectId">id of the project in which employee is working</param>
        /// <returns>true or false</returns>

        public bool StartTimeRelatedToProject(Guid projectId, Guid userId)
        {
            bool result = true;
            try
            {
                if (projectId != null)
                {
                    var project = _context.Projects.FirstOrDefault(x => x.ProjectId == projectId);

                    HttpContext.Current.Session["ProjID"] = project.ProjectId;
                    HttpContext.Current.Session["ProjectName"] = project.ProjectName;

                    LoginTime loginTime = new LoginTime();

                    loginTime.Id = Guid.NewGuid();

                    //loginTime.LoginTimeId = new Guid(HttpContext.Current.Session["userID"].ToString());
                    loginTime.LoginTimeId = userId;

                    TimeSpan CurrentTime = DateTime.Now.TimeOfDay;
                    loginTime.TimeIn = CurrentTime;
                    loginTime.ProjectID = project.ProjectId;
                    loginTime.IsDeleted = false;
                    loginTime.CreatedOn = DateTime.Now;
                    loginTime.LeaveStatus = false;

                    _context.LoginTimes.Add(loginTime);
                    _context.SaveChanges();

                    HttpContext.Current.Session["LogOut"] = loginTime.Id;
                    HttpContext.Current.Session["Time"] = loginTime.TimeIn;
                }
                else
                {

                }
            }
            catch (Exception ex)
            {
                ExceptionService.SaveException(ex);
            }
            return result;
        }

        /// <summary>
        /// list of totaltime of last five days show to employee when employee login
        /// </summary>
        /// <returns>List</returns>

        public List<LogInVM> EmployeeDashBoardList(Guid userId)  
        {
            List<LogInVM> logInVMs = new List<LogInVM>();
            //var logInTimeId = new Guid(HttpContext.Current.Session["userID"].ToString());
            try
            {
              var lastFourDays = DateTime.Now.AddDays(-5);
              var result = _context.LoginTimes.Where(x => x.LoginTimeId == userId && x.IsDeleted == false && x.ProjectID == null && x.CreatedOn >= lastFourDays && x.TotalTime != null)
                          .ToList();
             
              if (result.Count > 0)
              {
                  foreach (var item in result)
                  {
                      LogInVM logInVM = new LogInVM();
             
                      logInVM.LoginTimeId = item.LoginTimeId;
                      if (item.ProjectID != null)
                      {
                          logInVM.ProjectID = item.ProjectID;
                          logInVM.ProjectName = item.Project.ProjectName;
                      }
             
                      string shortString = item.CreatedOn.Value.ToShortDateString();  //remove time part from datetime datatype (CreatedOn)
                      logInVM.Date = shortString;   //Date in loginvm with datatype string
             
             
                      TimeSpan ts = TimeSpan.Parse(item.TimeIn.ToString());  //remove millisecond part from TimeSpan
                      ts = new TimeSpan(ts.Ticks / TimeSpan.TicksPerSecond * TimeSpan.TicksPerSecond);
                      logInVM.TimeIn = ts;
             
                      TimeSpan ts1 = TimeSpan.Parse(item.TimeOut.ToString());
                      ts1 = new TimeSpan(ts1.Ticks / TimeSpan.TicksPerSecond * TimeSpan.TicksPerSecond);
                      logInVM.TimeOut = ts1;
             
                      TimeSpan ts2 = TimeSpan.Parse(item.TotalTime.ToString());
                      ts2 = new TimeSpan(ts2.Ticks / TimeSpan.TicksPerSecond * TimeSpan.TicksPerSecond);
                      logInVM.TotalTime = ts2;
             
                      logInVM.LeaveStatus = item.LeaveStatus;
             
                      if (item.LeaveStatus == true)
                      {
                          logInVM.Message = "Leave";
                      }
                      else
                      {
                          logInVM.Message = "Present";
                      }
                      TimeSpan abc = TimeSpan.Parse(("08:00:00").ToString());
             
                      if (logInVM.TotalTime >= abc)
                      {
                          logInVM.Message = "8 Hours";
                      }
                      else
                      {
                          logInVM.Message = "Less Than 8 Hours";
                      }
                      logInVMs.Add(logInVM);
                  }
              }
            }
            catch (Exception ex)
            {
                ExceptionService.SaveException(ex);
            }
            return logInVMs;
        }

        public bool TotalTimeRelatedToProject()
        {
            bool result = true;

            return result;
        }

        public bool TotalTimeCount()
        {
            bool result = false;
            try
            {
                var userId = new Guid(HttpContext.Current.Session["LogOut"].ToString());
                var projectId = new Guid(HttpContext.Current.Session["ProjID"].ToString());

                if (projectId != null)
                {
                    LoginTime loginTime = _context.LoginTimes.FirstOrDefault(x => x.Id == userId && x.IsDeleted == false && x.ProjectID == projectId);
                    if (loginTime != null)
                    {
                        loginTime.TimeOut = DateTime.Now.TimeOfDay;
                        TimeSpan Interval = (TimeSpan)loginTime.TimeOut - (TimeSpan)loginTime.TimeIn;
                        loginTime.TotalTime = Interval;
                        HttpContext.Current.Session["Count"] = Interval;
                        _context.Entry(loginTime).State = System.Data.Entity.EntityState.Modified;
                        _context.SaveChanges();

                        #region MyRegion

                        //string abc = (loginTime.TimeOut).ToString();
                        ////TimeSpan timeSpan = (TimeSpan)(loginTime.TimeOut).Subtract(loginTime.TimeIn);
                        ////double totalHours = timeSpan.TotalHours;
                        //string[] fields = abc.Split(':');
                        ////if (fields.Length < 2) { throw new ArgumentException("No valid time of day pattern found in input text"); }
                        //int hour = Convert.ToInt32(fields[0]);
                        //int minutes = hour * 60;

                        //string hour1 = abc;
                        //string hour2 = (loginTime.TimeIn).ToString();

                        ////find total number of minutes for each hour above  
                        ////int minutes1 = (int.Parse(hour1.Split(':')[0]) * 60);
                        //int minutes1 = int.Parse(hour1.Split('.')[0]) * 60 + int.Parse(hour1.Split('.')[1] + int.Parse(hour1.Split('.')[2]) / 60);
                        //int minutes2 = int.Parse(hour2.Split('.')[0]) * 60 + int.Parse(hour2.Split('.')[1] + int.Parse(hour1.Split('.')[2]) / 60 + int.Parse(hour1.Split('.')[3]) / 60000);

                        ////calculate back to hours and minutes and reassemble as a string
                        //string result1 = (minutes1 + minutes2) / 60 + "." + (minutes1 + minutes2) % 60;

                        //loginTime.TotalTime =TimeSpan.Parse(result1);
                        //TimeSpan Time = TimeSpan.Parse((Interval.Hours).ToString()); 

                        //loginTime.TotalTime = Time;
                        #endregion
                    }
                }

                #region MyRegion
                // if (loginTime.LoginTimeId == (Guid)HttpContext.Current.Session["userID"])

                // {
                // loginTime.Id = (Guid)HttpContext.Current.Session["LogOut"];
                // loginTime.Id = (Guid)HttpContext.Current.Session["LogOut"];
                //loginTime.LoginTimeId = (Guid)HttpContext.Current.Session["userID"];
                //        loginTime.TimeIn= (TimeSpan)HttpContext.Current.Session["Time"];

                //loginTime.TimeOut = CurrentTime;
                //_context.LoginTimes.Add(loginTime);
                //_context.SaveChanges();
                //        bool result = true;
                //        var employeeDetail = _context.LoginTimes.FirstOrDefault(x => x.Id == employeeDetail);
                //        if (employeeDetail != null)
                //        {
                //            var logintime = employeeDetail.login



                //            employeeDetail.time = DateTime.Now;
                //            employeeDetail.DepId = employeeVM.DepId;
                //            employeeDetail.ProjId = employeeVM.ProjId; ;
                //            _context.Entry(employeeDetail).State = System.Data.Entity.EntityState.Modified;
                //            _context.SaveChanges();

                //            //}
                //        }
                //        return result;

                //}
                //public bool LogInTime(Guid id)
                //{

                //    return 
                //}
                /*  public void LoginStatus(LogInVM logInVM)
                  {
                      LoginTime login = new LoginTime();
                      UserLoginDetail userLoginDetails = new UserLoginDetail();

                      login.LoginTimeId = Guid.NewGuid();
                      login.IsDeleted = false;
                      login.CreatedOn = DateTime.Now;

                      login.UserId = adminSignUp.AdminId;
                      login.TimeIn = DateTime.Now;
                      _context.LoginTimes.Add(login);
                      _context.SaveChanges();
                  }*/
                #endregion

                #region MyRegion
                //    TimeSpan duration = (TimeSpan)loginTime.TotalTime;
                //TimeSpan ts = new TimeSpan();
                //TimeSpan duration=Interval.Duration;
                //double totalMinutes = Interval.TotalMinutes;
                //double time = totalMinutes;
                //double totalMinutes = span.TotalMinutes;
                //double totalMinutes = span.TotalMinutes;
                //double.Parse(string.Format("-HH.mm"));
                /*loginTime.TotalTime = TimeSpan.FromMinutes(totalMinutes);*/
                //loginTime.TotalTime = (TimeSpan)loginTime.TimeOut - (TimeSpan)loginTime.TimeIn;

                #endregion

                result = true;
            }
            catch (Exception ex)
            {
                ExceptionService.SaveException(ex);
            }
            return result;
        }

        public bool DirectLogOutTime(Guid logInTimeTableId)
        {
            bool result = false;
            try
            {
               //if (HttpContext.Current.Session["logInTimesTableID"] == null)
               if (logInTimeTableId == null)
               {
                   //AdminLogInVM adminLogInVM = AdminLogin();
               }
               else
               {
                   //var userID = new Guid(HttpContext.Current.Session["logInTimesTableID"].ToString());
                   LoginTime loginTime = _context.LoginTimes.FirstOrDefault(x => x.Id == logInTimeTableId && x.IsDeleted == false && x.ProjectID == null);
                   loginTime.TimeOut = DateTime.Now.TimeOfDay;
                   loginTime.TotalTime = loginTime.TimeOut - loginTime.TimeIn;
              
                   _context.Entry(loginTime).State = System.Data.Entity.EntityState.Modified;
                   _context.SaveChanges();
               }
            }
            catch (Exception ex)
            {
                ExceptionService.SaveException(ex);
            }
            return result;
        }

        public TimeSpan? LogOutAndTotalTimeCount()
        { 
            var userId = new Guid(HttpContext.Current.Session["LogOut"].ToString());

            var TotalLoginTime = _context.LoginTimes.FirstOrDefault(x => x.Id == userId && x.IsDeleted == false && x.CreatedOn == DateTime.Today);

            TimeSpan? totalTime = TotalLoginTime.TotalTime;
            return totalTime;
        }

        public List<LogInVM> ProjectTotalTimeCount()
        {
            List<LogInVM> logInVMs = new List<LogInVM>();
            try
            {
                var userId = new Guid(HttpContext.Current.Session["LogOut"].ToString());
               
                var LastDay = DateTime.Today;
              
                var data = _context.LoginTimes.FirstOrDefault(x => x.Id == userId && x.IsDeleted == false);
              
                HttpContext.Current.Session["LoginTimeId"] = data.LoginTimeId;
              
                var logInTimeId = new Guid(HttpContext.Current.Session["LoginTimeId"].ToString());
              
                var result = _context.LoginTimes.Where(x => x.LoginTimeId == logInTimeId && x.CreatedOn >= LastDay && x.TotalTime != null && x.ProjectID != null)
                  .ToList();
              
                foreach (var item in result)
                {
                    LogInVM logInVM = new LogInVM();
                    logInVM.LoginTimeId = item.LoginTimeId;
                    logInVM.ProjectID = item.ProjectID;
                    logInVM.ProjectName = item.Project.ProjectName;
                    logInVM.TimeIn = item.TimeIn;
                    logInVM.TimeOut = item.TimeOut;
                    logInVM.TotalTime = item.TotalTime;
              
                    logInVMs.Add(logInVM);
                }
            }
            catch (Exception ex)
            {
                ExceptionService.SaveException(ex);
            }
            return logInVMs;
        }

        public LogInVM DashBoard()
        {
            LogInVM model = new LogInVM();
            try
            {

               var userId = new Guid(HttpContext.Current.Session["userID"].ToString());
               
               LoginTime loginTime = new LoginTime();
               
               loginTime = _context.LoginTimes.Where(x => x.LoginTimeId == userId && x.IsDeleted == false && x.TotalTime != null)
                   .OrderByDescending(x => x.CreatedOn)
                   .Take(7).FirstOrDefault();
               
               HttpContext.Current.Session["Count"] = loginTime.TotalTime;   /*.Value.Minutes;*/
               
               model.CreatedOn = loginTime.CreatedOn;
               model.LoginTimeId = loginTime.LoginTimeId;
               model.TotalTime = loginTime.TotalTime;
            }
            catch (Exception ex)
            {
                ExceptionService.SaveException(ex);
            }
            return model;
        }

        public List<KeyValueModel<Guid, string>> GetDepartmentList()
        {
            List<KeyValueModel<Guid, string>> result = new List<KeyValueModel<Guid, string>>();

            result = _context.Departments.Where(x => x.IsDeleted == false)
                .Select(x => new KeyValueModel<Guid, string>
                { Key = x.DepartmentId, Value = x.DepartmentName })
                .ToList();

            return result;
        }

        public List<KeyValueModel<Guid, string>> GetProjectOfEmployee()
        {
            List<KeyValueModel<Guid, string>> result = new List<KeyValueModel<Guid, string>>();

            result = _context.Projects.Where(x => x.IsDeleted == false)
                .Select(x => new KeyValueModel<Guid, string>
                { Key = x.ProjectId, Value = x.ProjectName, IsChecked = false })
                .ToList();

            return result;
        }

        public bool CreateEmployeeData(EmployeeVM employeeVM, string CanLogIn)
        {
            bool result = false;
            try
            {
                EmployeeDetail employeeDetail = new EmployeeDetail();

                if (employeeVM != null)
                {
                    employeeDetail.EmployeeId = Guid.NewGuid();

                    employeeDetail.FirstName = employeeVM.FirstName;
                    employeeDetail.LastName = employeeVM.LastName;
                    employeeDetail.Email = employeeVM.Email;
                    employeeDetail.ContactNumber = employeeVM.ContactNumber;
                    employeeDetail.DateOfBirth = employeeVM.DateOfBirth;
                    employeeDetail.EmployeeAddress = employeeVM.EmployeeAddress;
                    employeeDetail.EmployeeSalary = employeeVM.EmployeeSalary;
                    employeeDetail.EmployeeImage = employeeVM.EmployeeImage;
                    employeeDetail.IsDeleted = false;
                    employeeDetail.IsAdmin = false;
                    employeeDetail.CreatedOn = DateTime.Now.Date;
                    employeeDetail.DepId = employeeVM.DepId;
                    employeeDetail.ProjId = employeeVM.ProjId;
                    employeeDetail.JoiningDate = employeeVM.JoiningDate;

                    if (CanLogIn == "true")
                    {
                        employeeDetail.CanLogIn = true;
                        UserLoginDetail userDetails = new UserLoginDetail();

                        userDetails.Id = Guid.NewGuid();

                        userDetails.UserLoginDetailsId = employeeDetail.EmployeeId;
                        userDetails.UserName = employeeDetail.FirstName + employeeDetail.ContactNumber.Substring(employeeDetail.ContactNumber.Length - 4);
                        userDetails.Password = employeeDetail.LastName + "@" + employeeDetail.DateOfBirth.Value.ToString("dd");
                        userDetails.CreatedOn = DateTime.Now.Date;
                        userDetails.IsDeleted = false;
                        userDetails.IsAdmin = false;
                        _context.UserLoginDetails.Add(userDetails);
                        _context.SaveChanges();
                    }
                    else
                    {
                        employeeDetail.CanLogIn = false;
                    }

                    _context.EmployeeDetails.Add(employeeDetail);
                    _context.SaveChanges();

                    result = true;
                }
            }
            catch (Exception ex)
            {
                ExceptionService.SaveException(ex);
            }
            return result;
        }

        public IPagedList<EmployeeDetail> Pagination(string sortOrder, string CurrentSort, int? page)
        {

            int pageSize = 9;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
           
               sortOrder = String.IsNullOrEmpty(sortOrder) ? "FirstName" : sortOrder;
                CurrentSort = sortOrder;
            
            IPagedList<EmployeeDetail> result = null;
            switch (sortOrder)
            {
                case "FirstName":
                    
                        if (sortOrder.Equals(CurrentSort))

                            result = _context.EmployeeDetails
                                     .Where(m => m.IsDeleted == false && m.IsAdmin==false)
                                     .OrderByDescending(x => x.CreatedOn)
                                     .ToPagedList(page ?? pageIndex, pageSize);
                    
                    break;
            }

            return result;
        }

        /// <summary>
        /// this code find all the records when click on search button 
        /// (Display all the employees)
        /// </summary>
        /// <param name="Search">Name of the employee</param>
        /// <returns>
        /// List of all employee records
        /// </returns>

        public List<EmployeeVM> FindData(string Search)
        {
            List<EmployeeVM> employee = new List<EmployeeVM>();
            try
            {
                if (Search != null)
                {
                    var findData = _context.EmployeeDetails.Where(x => x.FirstName.Contains(Search) && x.IsDeleted == false && x.IsAdmin == false).ToList();

                    foreach (var list in findData)
                    {
                        EmployeeVM employeeVM = new EmployeeVM();

                        employeeVM.EmployeeId = list.EmployeeId;

                        HttpContext.Current.Session["EmpId"] = employeeVM.EmployeeId;

                        employeeVM.FirstName = list.FirstName;
                        employeeVM.LastName = list.LastName;
                        employeeVM.Email = list.Email;
                        employeeVM.ContactNumber = list.ContactNumber;
                        employeeVM.DateOfBirth = list.DateOfBirth;
                        employeeVM.EmployeeAddress = list.EmployeeAddress;
                        employeeVM.EmployeeSalary = list.EmployeeSalary;
                        employeeVM.EmployeeImage = list.EmployeeImage;
                        employeeVM.IsDeleted = false;
                        employeeVM.CreatedOn = DateTime.Now;
                        employeeVM.DepId = list.DepId;
                        employeeVM.ProjId = list.ProjId;
                        employeeVM.DepartmentName = list.Department.DepartmentName;

                        employee.Add(employeeVM);
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionService.SaveException(ex);
            }
            return employee;
        }

        public List<EmployeeVM> GetEmployee()
        {
            List<EmployeeVM> employeeVMs = new List<EmployeeVM>();
            try
            {
                var lastFourDays = DateTime.Now.AddDays(-4);

                var employeeDetails = _context.EmployeeDetails.Where(x => x.IsDeleted == false && x.IsAdmin == false).Take(5).ToList();

                foreach (var list in employeeDetails)
                {
                    EmployeeVM employeeVM = new EmployeeVM();

                    employeeVM.EmployeeId = list.EmployeeId;

                    employeeVM.FirstName = list.FirstName;
                    employeeVM.LastName = list.LastName;
                    employeeVM.Email = list.Email;
                    employeeVM.ContactNumber = list.ContactNumber;
                    employeeVM.DateOfBirth = list.DateOfBirth;
                    employeeVM.EmployeeAddress = list.EmployeeAddress;
                    employeeVM.EmployeeSalary = list.EmployeeSalary;
                    employeeVM.EmployeeImage = list.EmployeeImage;
                    employeeVM.DepId = list.DepId;
                    employeeVM.ProjId = list.ProjId;
                    employeeVM.DepartmentName = list.Department.DepartmentName;

                    employeeVMs.Add(employeeVM);
                }
            }
            catch (Exception ex)
            {
                ExceptionService.SaveException(ex);
            }
            return employeeVMs;
        }

        public EmployeeVM EditEmployeeData(Guid? ID)
        {
            EmployeeVM employee = new EmployeeVM();
            try
            {
                if (ID != null)
                {
                    var employeeDetail = _context.EmployeeDetails.FirstOrDefault(x => x.EmployeeId == ID && x.IsDeleted == false);

                    employee.EmployeeId = employeeDetail.EmployeeId;

                    employee.FirstName = employeeDetail.FirstName;
                    employee.LastName = employeeDetail.LastName;
                    employee.Email = employeeDetail.Email;
                    employee.ContactNumber = employeeDetail.ContactNumber;
                    employee.DateOfBirth = employeeDetail.DateOfBirth;
                    employee.EmployeeAddress = employeeDetail.EmployeeAddress;
                    employee.EmployeeSalary = employeeDetail.EmployeeSalary;
                    employee.EmployeeImage = employeeDetail.EmployeeImage;
                    employee.IsDeleted = false;
                    employee.CreatedOn = DateTime.Now;
                    employee.DepId = employeeDetail.DepId;
                    employee.JoiningDate = employeeDetail.JoiningDate;
                }
            }
            catch (Exception ex)
            {
                ExceptionService.SaveException(ex);
            }
            return employee;
        }

        public bool UpdateEmployeeList(EmployeeVM employeeVM, string oldImagePath)
        {
            bool result = true;
            try
            {
                EmployeeDetail employeeDetail = _context.EmployeeDetails.FirstOrDefault(x => x.EmployeeId == employeeVM.EmployeeId);

                if (employeeDetail != null)
                {
                    employeeDetail.FirstName = employeeVM.FirstName;
                    employeeDetail.LastName = employeeVM.LastName;
                    employeeDetail.Email = employeeVM.Email;
                    employeeDetail.ContactNumber = employeeVM.ContactNumber;
                    employeeDetail.DateOfBirth = employeeVM.DateOfBirth;
                    employeeDetail.EmployeeAddress = employeeVM.EmployeeAddress;
                    employeeDetail.EmployeeSalary = employeeVM.EmployeeSalary;
                    if (oldImagePath != null)
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                    employeeDetail.EmployeeImage = employeeVM.EmployeeImage;
                    employeeDetail.IsDeleted = false;
                    employeeDetail.CreatedOn = DateTime.Now;
                    employeeDetail.DepId = employeeVM.DepId;
                    employeeDetail.ProjId = employeeVM.ProjId;

                    _context.Entry(employeeDetail).State = System.Data.Entity.EntityState.Modified;
                    _context.SaveChanges();

                }
            }
            catch (Exception ex)
            {
                ExceptionService.SaveException(ex);
            }
            return result;
        }

        public bool DeleteData(Guid id)
        {
            bool result = true;
            try
            {
                EmployeeDetail employeeDetail = _context.EmployeeDetails.FirstOrDefault(x => x.EmployeeId == id);

                if (employeeDetail != null)
                {
                    employeeDetail.IsDeleted = true;
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ExceptionService.SaveException(ex);
            }
            return result;
        }

        public bool DeleteLeave(Guid id) //delete leave request by the employee
        {
            bool result = true;
            try
            {
                Leave lev = _context.Leaves.FirstOrDefault(x => x.Id == id);

                if (lev != null)
                {
                    lev.IsDeleted = true;
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ExceptionService.SaveException(ex);
            }
            return result;
        }

        /// <summary>
        /// show the detail of particular record on click of detail button
        /// </summary>
        /// <param name="id">id=EmployeeId</param>
        /// <returns>
        /// return single record
        /// </returns>

        public EmployeeVM Detail(Guid id)
        {
            EmployeeVM employeeVM = new EmployeeVM();
            try
            {
                EmployeeDetail data = _context.EmployeeDetails.FirstOrDefault(x => x.EmployeeId == id && x.IsDeleted == false);

                if (id != null)
                {
                    employeeVM.EmployeeId = data.EmployeeId;

                    HttpContext.Current.Session["EmpID"] = employeeVM.EmployeeId;

                    employeeVM.FirstName = data.FirstName;
                    employeeVM.LastName = data.LastName;

                    employeeVM.Name = employeeVM.FirstName + " " + employeeVM.LastName;

                    employeeVM.Email = data.Email;
                    employeeVM.ContactNumber = data.ContactNumber;
                    employeeVM.DateOfBirth = data.DateOfBirth;
                    employeeVM.EmployeeAddress = data.EmployeeAddress;
                    employeeVM.EmployeeSalary = data.EmployeeSalary;
                    employeeVM.EmployeeImage = data.EmployeeImage;
                    employeeVM.IsDeleted = false;
                    employeeVM.CreatedOn = DateTime.Now;
                    employeeVM.DepId = data.DepId;
                    employeeVM.ProjId = data.ProjId;
                    employeeVM.DepartmentName = data.Department.DepartmentName;
                    employeeVM.DeparmentId = data.Department.DepartmentId;

                }
            }
            catch (Exception ex)
            {
                ExceptionService.SaveException(ex);
            }
            return employeeVM;
        }

        public List<EmployeeVM> ProjectAssigned(Guid id)
        {
            List<EmployeeVM> employeeVMs = new List<EmployeeVM>();

            //var empId = new Guid(HttpContext.Current.Session["EmpID"].ToString());
            try
            {
                var Data = _context.WorkingDiagnoses.Where(x => x.EmpId == id).ToList();
                if (Data.Count > 0)
                {
                    foreach (var item in Data)
                    {
                        EmployeeVM employeeVM = new EmployeeVM();
                        WorkingDiagnose work = new WorkingDiagnose();

                        employeeVM.PrjId = item.PrjId;
                        employeeVM.ProjectName = item.Project.ProjectName;
                        employeeVM.IsChecked = false;

                        employeeVMs.Add(employeeVM);
                    }
                }
                else
                {
                    EmployeeVM employeeVM = new EmployeeVM();
                    employeeVM.Message = "No Project Assigned";

                    employeeVMs.Add(employeeVM);
                }
            }
            catch (Exception ex)
            {
                ExceptionService.SaveException(ex);
            }
            return employeeVMs;
        }

        /// <summary>
        /// List of Projects show to employee when employee login.
        /// List of project according to id of the employee
        /// </summary>
        /// <returns> List of Project</returns>
        public List<EmployeeVM> GetProjectList()
        {
            List<EmployeeVM> employeeVMs = new List<EmployeeVM>();
            try
            {
                var empId = new Guid(HttpContext.Current.Session["EmpID"].ToString());

                var Data = _context.WorkingDiagnoses.Where(x => x.EmpId == empId).ToList();

                foreach (var item in Data)
                {
                    EmployeeVM employeeVM = new EmployeeVM();
                    WorkingDiagnose work = new WorkingDiagnose();

                    employeeVM.PrjId = item.PrjId;
                    employeeVM.ProjectName = item.Project.ProjectName;
                    employeeVM.IsChecked = false;

                    employeeVMs.Add(employeeVM);
                }
            }
            catch (Exception ex)
            {
                ExceptionService.SaveException(ex);
            }
            return employeeVMs;
        }

        /// <summary>
        ///  detail of the employee applied for leave request. opens on the click of detail button which is applied on Employee/Notification
        /// </summary>
        /// <param name="id">
        /// 
        /// </param>
        /// <returns>
        /// 
        /// </returns>

        public leaveVM LeaveDetails(Guid id)
        {
            leaveVM leaveVM = new leaveVM();
            try
            {
                Leave data = _context.Leaves.Where(x => x.Id == id && x.IsDeleted == false).FirstOrDefault();

                HttpContext.Current.Session["userLeaveID"] = data.UserLeaveId;

                leaveVM.UserLeaveId = data.UserLeaveId;

                var userLeaveId = new Guid(HttpContext.Current.Session["userLeaveID"].ToString());

                UserLoginDetail userLogin = _context.UserLoginDetails.FirstOrDefault(x => x.Id == userLeaveId);

                HttpContext.Current.Session["userLogInDetailID"] = userLogin.UserLoginDetailsId;

                leaveVM.UserName = userLogin.UserName;
                leaveVM.UserLoginDetailsId = userLogin.UserLoginDetailsId;

                var userLogInID = new Guid(HttpContext.Current.Session["userLogInDetailID"].ToString());

                EmployeeDetail emp = _context.EmployeeDetails.FirstOrDefault(x => x.EmployeeId == userLogInID);
                {
                    leaveVM.EmployeeId = emp.EmployeeId;

                    HttpContext.Current.Session["employeeID"] = leaveVM.EmployeeId;

                    leaveVM.FirstName = emp.FirstName;
                    leaveVM.LastName = emp.LastName;
                    leaveVM.Email = emp.Email;
                    leaveVM.ContactNumber = emp.ContactNumber;
                    leaveVM.DateOfBirth = emp.DateOfBirth;
                    leaveVM.EmployeeAddress = emp.EmployeeAddress;
                    leaveVM.EmployeeSalary = emp.EmployeeSalary;
                    leaveVM.EmployeeImage = emp.EmployeeImage;
                    leaveVM.DepartmentName = emp.Department.DepartmentName;
                }
            }
            catch (Exception ex)
            {
                ExceptionService.SaveException(ex);
            }
            return leaveVM;
        }

        #region DisplayDetail
        // display detail of the employee when employee login(shown in the fpirst page of the employee i.e dashboard page of the employee )
        #endregion

        /// <summary>
        /// call at the time of employee login
        /// </summary>
        /// <param name="adminLogInVM">email, username, password</param>
        /// <returns>whole detail of the which login</returns>

        public EmployeeVM DisplayDetail(AdminLogInVM adminLogInVM)
        {
            var login = _context.UserLoginDetails.FirstOrDefault(x => x.IsDeleted == false && (x.Email == adminLogInVM.UserName || x.UserName == adminLogInVM.UserName) && x.Password == adminLogInVM.Password);

            var id = login.UserLoginDetailsId;
            var idForLoginTimes = login.Id;

            EmployeeVM employeeVM = Detail(id);
            return employeeVM;
        }

        public bool Leave(leaveVM leaveVM) /*//employee fill leave application */
        {
            bool result = true;
            try
            {
                Leave leave = new Leave();

                if (leaveVM != null)
                {
                    leave.Id = Guid.NewGuid();

                    HttpContext.Current.Session["leaveID"] = leave.Id;

                    leave.StartDate = leaveVM.StartDate;
                    leave.EndDate = leaveVM.EndDate;
                    leave.Reason = leaveVM.Reason;
                    leave.IsDeleted = false;
                    leave.CreatedOn = DateTime.Now;
                    leave.Approve = leaveVM.Approve;
                    leave.Reject = leaveVM.Reject;

                    leave.UserLeaveId = new Guid(HttpContext.Current.Session["userID"].ToString());

                    _context.Leaves.Add(leave);
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ExceptionService.SaveException(ex);
            }
            return result;
        }

        #region ListOfLeaves
        //list of the leaves applied by the employee ApprovedOrRejected. in this list all the requests which applied for approval shown in employee applied request
        #endregion

        public List<leaveVM> ListOfLeaves()
        {
            List<leaveVM> leaveVMs = new List<leaveVM>();
            try
            {
                var userId = new Guid(HttpContext.Current.Session["userID"].ToString());
                var data = _context.Leaves.Where(x => x.UserLeaveId == userId && x.IsDeleted == false && x.Approve == false && x.Reject == false).ToList();

                foreach (var item in data)
                {
                    leaveVM lev = new leaveVM();

                    lev.Id = item.Id;
                    lev.StartDate = item.StartDate;
                    lev.EndDate = item.EndDate;
                    lev.Reason = item.Reason;
                    lev.Approve = (bool)item.Approve;
                    lev.Reject = (bool)item.Reject;

                    leaveVMs.Add(lev);
                }
            }
            catch (Exception ex)
            {
                ExceptionService.SaveException(ex);
            }
            return leaveVMs;
        }

        #region ApproveOrReject
        //notification of approval or rejection of leave. this shown to the employee after approval or rejection from the admin
        #endregion

        public List<leaveVM> ApproveOrReject()
        {
            List<leaveVM> leaveVMs = new List<leaveVM>();
            try
            {
                var userId = new Guid(HttpContext.Current.Session["userID"].ToString());
                var data = _context.Leaves.Where(x => x.UserLeaveId == userId && x.IsDeleted == false && x.Approve == true || x.Reject == true).ToList();

                foreach (var item in data)
                {
                    leaveVM lev = new leaveVM();

                    lev.Id = item.Id;

                    lev.StartDate = item.StartDate;
                    lev.EndDate = item.EndDate;
                    lev.Reason = item.Reason;
                    lev.Approve = (bool)item.Approve;
                    lev.Reject = (bool)item.Reject;

                    if ((bool)item.Approve)
                    {
                        lev.MsgForLeave = "Approve";
                    }
                    if ((bool)item.Reject)
                    {
                        lev.MsgForLeave = "Reject";
                    }
                    leaveVMs.Add(lev);
                }
            }
            catch (Exception ex)
            {
                ExceptionService.SaveException(ex);
            }
            return leaveVMs;
        }

        #region ListOfLeavesShownToAdmin
        /* all the leave which were applied by the employees shown to admin in list form. (on notification click)  */
        #endregion

        /// <summary>
        ///list of all the leaves which were applied by the employees shown to admin(on notification click)
        ///ActionMethod- Notification(Admin)
        /// </summary>
        /// <returns>List of Leaves</returns>

        public List<leaveVM> ListOfLeavesShownToAdmin()
        {
            List<leaveVM> leaveVMs = new List<leaveVM>();
            try
            {
                var userId = new Guid(HttpContext.Current.Session["userID"].ToString());
                var data = _context.Leaves.Where(x => x.IsDeleted == false && x.Approve == false && x.Reject == false).ToList();

                if (data.Count != 0)
                {
                    foreach (var item in data)
                    {
                        leaveVM lev = new leaveVM();

                        lev.Id = item.Id;

                        lev.StartDate = item.StartDate;
                        lev.EndDate = item.EndDate;
                        lev.Reason = item.Reason;
                        lev.Approve = (bool)item.Approve;
                        lev.Reject = (bool)item.Reject;

                        leaveVMs.Add(lev);
                    }
                }
                else
                {
                    HttpContext.Current.Session["message"] = "No Notification";
                }
            }
            catch (Exception ex)
            {
                ExceptionService.SaveException(ex);
            }
            return leaveVMs;
        }

        #region TotalTimeOfEmployees
        //public List<leaveVM> TotalTimeOfEmployees()
        //{
        //    List<leaveVM> leaveVMs = new List<leaveVM>();
        //    var LastFourDays = DateTime.Now.AddDays(-4);
        //    var data = _context.LoginTimes.Where(x => x.IsDeleted == false && x.CreatedOn >= LastFourDays && x.TotalTime != null).ToList();
        //    if (data.Count != 0)
        //    {
        //        foreach (var item in data)
        //        {
        //            leaveVM lev = new leaveVM();

        //            lev.Id = item.Id;
        //            lev.LoginTimeId = item.LoginTimeId;
        //            HttpContext.Current.Session["LoginTimeId"] = lev.LoginTimeId;
        //            lev.TimeIn = item.TimeIn;
        //            lev.TimeOut = item.TimeOut;
        //            lev.TotalTime = item.TotalTime;
        //            var LoginTimeId = new Guid(HttpContext.Current.Session["LoginTimeId"].ToString());

        //            UserLoginDetail userLoginDetail = _context.UserLoginDetails.Where(x => x.Id == LoginTimeId).FirstOrDefault();
        //            lev.UserLoginDetailsId = userLoginDetail.UserLoginDetailsId;
        //            HttpContext.Current.Session["UserLoginDetailsId"] = lev.UserLoginDetailsId;
        //            var UserLoginDetailsId = new Guid(HttpContext.Current.Session["UserLoginDetailsId"].ToString());
        //            EmployeeDetail emp = _context.EmployeeDetails.Where(x => x.EmployeeId == UserLoginDetailsId).FirstOrDefault();
        //            lev.FirstName = emp.FirstName;
        //            lev.LastName = emp.LastName;
        //            lev.DepartmentName = emp.Department.DepartmentName;
        //            lev.ProjectName = emp.Project.ProjectName;
        //            leaveVMs.Add(lev);
        //        }
        //    }
        //    return leaveVMs;
        //}
        #endregion

        /// <summary>
        /// list of employee records 
        /// ActionName=TotalTimeOfEmployees
        /// </summary>
        /// <param name="Search"></param>
        /// <returns>List of records</returns>

        public List<EmployeeVM> TotalTimeOfEmployees(string Search)
        {
            List<EmployeeVM> employee = new List<EmployeeVM>();
            try
            {
                if (Search != null)
                {
                    var findData = _context.EmployeeDetails.Where(x => x.FirstName.Contains(Search) && x.IsDeleted == false && x.IsAdmin == false).ToList();

                    foreach (var list in findData)
                    {
                        EmployeeVM employeeVM = new EmployeeVM();

                        employeeVM.EmployeeId = list.EmployeeId;

                        HttpContext.Current.Session["EmpId"] = employeeVM.EmployeeId;

                        employeeVM.FirstName = list.FirstName;
                        employeeVM.LastName = list.LastName;
                        employeeVM.Email = list.Email;
                        employeeVM.ContactNumber = list.ContactNumber;
                        employeeVM.DateOfBirth = list.DateOfBirth;
                        employeeVM.EmployeeAddress = list.EmployeeAddress;
                        employeeVM.EmployeeSalary = list.EmployeeSalary;
                        employeeVM.EmployeeImage = list.EmployeeImage;
                        employeeVM.IsDeleted = false;
                        employeeVM.CreatedOn = DateTime.Now;
                        employeeVM.DepId = list.DepId;
                        employeeVM.ProjId = list.ProjId;
                        employeeVM.DepartmentName = list.Department.DepartmentName;

                        var empID = new Guid(HttpContext.Current.Session["EmpId"].ToString());

                        UserLoginDetail userLogin = _context.UserLoginDetails.Where(x => x.UserLoginDetailsId == empID && x.IsDeleted == false).FirstOrDefault();

                        HttpContext.Current.Session["EmpId"] = userLogin.Id;

                        var userLogInId = new Guid(HttpContext.Current.Session["EmpId"].ToString());

                        var LastFourDays = DateTime.Now.AddDays(-1);

                        var loginTime = _context.LoginTimes.Where(x => x.LoginTimeId == userLogInId && x.IsDeleted == false && x.CreatedOn >= LastFourDays && x.TotalTime != null)
                            .ToList();

                        if (loginTime != null)
                        {
                            foreach (var lis in loginTime)
                            {
                                LoginTime loginTime1 = new LoginTime();

                                employeeVM.TimeIn = lis.TimeIn;
                                employeeVM.TimeOut = lis.TimeOut;
                                employeeVM.TotalTime = lis.TotalTime;
                            }
                        }
                        employee.Add(employeeVM);
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionService.SaveException(ex);
            }
            return employee;
        }

        /// <summary>
        /// total time of particular record 
        /// </summary>
        /// <param name="id">userloginid</param>
        /// <returns>single record data</returns>

        public List<leaveVM> TimeDetail(Guid id)
        {
            List<leaveVM> leaveVMs = new List<leaveVM>();
            try
            {
                EmployeeDetail emp = _context.EmployeeDetails.FirstOrDefault(x => x.EmployeeId == id && x.IsDeleted == false);

                HttpContext.Current.Session["EmpID"] = emp.EmployeeId;

                var empId = new Guid(HttpContext.Current.Session["EmpID"].ToString());

                UserLoginDetail userLogin = _context.UserLoginDetails.FirstOrDefault(x => x.UserLoginDetailsId == empId && x.IsDeleted == false);

                HttpContext.Current.Session["EmpId"] = userLogin.Id;

                var userLogInId = new Guid(HttpContext.Current.Session["EmpId"].ToString());

                var LastFourDays = DateTime.Now.AddDays(-4);

                var loginTime = _context.LoginTimes.Where(x => x.LoginTimeId == userLogInId && x.IsDeleted == false && x.CreatedOn >= LastFourDays && x.TotalTime != null )
                    .ToList();

                if (loginTime != null)
                {
                    foreach (var lis in loginTime)
                    {
                        leaveVM lev = new leaveVM();

                        if(lev.ProjectID == null)
                        {
                            lev.MsgForLeave = "TotalTime";
                        }
                        else
                        {
                          lev.ProjectID = lis.ProjectID;
                          lev.ProjectName = lis.Project.ProjectName;
                        }
                        lev.TimeIn = lis.TimeIn;
                        lev.TimeOut = lis.TimeOut;
                        lev.TotalTime = lis.TotalTime;

                        leaveVMs.Add(lev);
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionService.SaveException(ex);
            }
            return leaveVMs;
        }

        /// <summary>
        /// find code for updating the approval or rejection request by the admin
        ///  (on click of updatestatus this code find the particular record which applied for leave)
        /// </summary>
        /// <param name="id">id= id of the employee in leave table which apply for the leave</param>
        /// <returns>
        /// return single record (id)
        /// </returns>

        public leaveVM ApproveOrRejectLeaveRequest(Guid id)
        {
            leaveVM leaveVM = new leaveVM();
            try
            {
                if (id != null)
                {
                    Leave leave = _context.Leaves.FirstOrDefault(x => x.Id == id && x.Approve == false && x.Reject == false);

                    leaveVM.Id = leave.Id;

                    leaveVM.StartDate = leave.StartDate;
                    leaveVM.EndDate = leave.EndDate;
                    leaveVM.Reason = leave.Reason;
                    leaveVM.Approve = (bool)leave.Approve;
                    leaveVM.Reject = (bool)leave.Reject;
                }
            }
            catch (Exception ex)
            {
                ExceptionService.SaveException(ex);
            }
            return leaveVM;
        }

        /// <summary>
        /// leave is approved or rejected
        /// post method of ApproveOrRejectNotification
        /// </summary>
        /// <param name="leaveVM">status</param>
        /// <returns>ture or false</returns>

        public bool UpdateLeaveRequest(leaveVM leaveVM)
        {
            bool result = true;
            try
            {
                Leave lev = _context.Leaves.FirstOrDefault(x => x.Id == leaveVM.Id);

                HttpContext.Current.Session["userLeaveID"] = lev.UserLeaveId;

                if (lev != null)
                {
                    lev.Approve = leaveVM.Approve;
                    lev.Reject = leaveVM.Reject;

                    if (leaveVM.Approve == true && leaveVM.Reject == true)
                    {
                        HttpContext.Current.Session["message"] = "Select Only One";
                    }
                    else
                    {
                        _context.Entry(lev).State = System.Data.Entity.EntityState.Modified;
                        _context.SaveChanges();

                        if (lev.Approve == true)
                        {
                            LoginTime loginTime = new LoginTime();

                            loginTime.Id = Guid.NewGuid();

                            loginTime.LoginTimeId = new Guid(HttpContext.Current.Session["userLeaveID"].ToString());
                            TimeSpan workingHours = TimeSpan.Parse(("00:00:00").ToString());
                            loginTime.TimeIn = workingHours;
                            loginTime.TimeOut = workingHours;
                            loginTime.TotalTime = workingHours;
                            loginTime.CreatedOn = DateTime.Now.Date;
                            loginTime.IsDeleted = false;
                            loginTime.LeaveStatus = true;

                            _context.LoginTimes.Add(loginTime);
                            _context.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionService.SaveException(ex);
            }
            return result;
        }

        public bool AddProject(List<Guid> projectId, Guid departmentId, Guid empId)
        {
            bool result = false;
            try
            {
                foreach (var item in projectId)
                {
                    WorkingDiagnose model = new WorkingDiagnose();

                    model.Id = Guid.NewGuid();

                    model.EmpId = empId;
                    model.DeptId = departmentId;
                    model.PrjId = item;
                    model.CreatedOn = DateTime.Now;
                    model.IsDeleted = false;

                    _context.WorkingDiagnoses.Add(model);
                    _context.SaveChanges();
                     result = true;
                }
            }
            catch (Exception ex)
            {
                ExceptionService.SaveException(ex);
            }
            return result;
        }

        public List<LogInVM> SearchByDate(DateTime? FromDate, DateTime? ToDate)
        {
            List<LogInVM> logInVMs = new List<LogInVM>();

            var fromDate = FromDate;
            var toDate = ToDate;

            var data= _context.LoginTimes.Where(x => x.CreatedOn >= fromDate && x.CreatedOn <= toDate && x.IsDeleted==false && x.TotalTime != null && x.ProjectID == null).ToList();

            foreach (var item in data)
            {
                LogInVM logInVM1 = new LogInVM();

                logInVM1.CreatedOn = item.CreatedOn;
                if(item.TimeIn != null)
                {
                    TimeSpan ts = TimeSpan.Parse(item.TimeIn.ToString());  //remove millisecond part from TimeSpan
                    ts = new TimeSpan(ts.Ticks / TimeSpan.TicksPerSecond * TimeSpan.TicksPerSecond);
                    logInVM1.TimeIn = ts;
                     //logInVM1.TimeIn = item.TimeIn;
                }
                if(item.TimeOut != null)
                {
                    TimeSpan ts = TimeSpan.Parse(item.TimeOut.ToString());  //remove millisecond part from TimeSpan
                    ts = new TimeSpan(ts.Ticks / TimeSpan.TicksPerSecond * TimeSpan.TicksPerSecond);
                    logInVM1.TimeOut = ts;
                    //logInVM1.TimeOut = item.TimeOut;
                }
                if(item.TotalTime != null)
                {
                    TimeSpan ts = TimeSpan.Parse(item.TotalTime.ToString());  //remove millisecond part from TimeSpan
                    ts = new TimeSpan(ts.Ticks / TimeSpan.TicksPerSecond * TimeSpan.TicksPerSecond);
                    logInVM1.TotalTime = ts;
                    //logInVM1.TotalTime = item.TotalTime;
                }
                logInVMs.Add(logInVM1);
            }
            return logInVMs;
        }

         public bool MultiSelectEmail(List<string> email)
        {
            bool result = true;

            return result;
        }
    }
}




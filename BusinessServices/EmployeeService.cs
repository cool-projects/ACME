using Acme.Data;
using Helpers;
using IBusinessServices;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessServices
{
    public class EmployeeService : IEmployeeService
    {
        private readonly AcmeDBContext db;
        private readonly ILoggingHelper _logging;
        public EmployeeService(AcmeDBContext dataContext, ILoggingHelper loggingHelper)
        {
            db = dataContext;
            _logging = loggingHelper;
        }

        public SaveEmployeeResponse SaveEmployee(EmployeeModel model)
        {
            var response = new SaveEmployeeResponse();
            var employee = new Employee()
            {
                Age = model.Age,
                ReportingLine = model.ReportingLine,
                Name = model.Name,
                Email = model.Email,
                GenderId = model.GenderId
            };
            db.Employees.Add(employee);
            db.SaveChanges();
            response.EmployeeId = employee.EmployeeId;

            return response;
        }

        public BaseResponse UpdateEmployee(EmployeeModel model)
        {
            var response = new BaseResponse();
            var employee = db.Employees.Where(x => x.EmployeeId == model.EmployeeId).FirstOrDefault();
            if(employee != null)
            {
                employee.Age = model.Age;
                employee.ReportingLine = model.ReportingLine;
                employee.Name = model.Name;
                employee.Email = model.Email;
                employee.GenderId = model.GenderId;
                db.SaveChanges();

                var role = db.UserRoles.Where(x => x.EmployeeId == model.EmployeeId).FirstOrDefault();
                if (role != null && model.RoleId > 0)
                {
                    role.RoleId = model.RoleId.GetValueOrDefault();
                    db.SaveChanges();
                }

                response.Success = true;
            }
            else
            {
                response.Success = false;
            }

            return response;
        }

        public EmployeeModel GetEmployee(int employeeId)
        {
            var response = new EmployeeModel();
            var employee = db.Employees.Where(x => x.EmployeeId == employeeId).FirstOrDefault();
            if(employee != null)
            {
                response.Age = employee.Age;
                response.EmployeeId = employeeId;
                response.ReportingLine = employee.ReportingLine;
                response.Name = employee.Name;
                response.Email = employee.Email;
                response.GenderId = employee.GenderId;
                response.Gender = db.Genders.Where(x => x.GenderId == employee.GenderId).FirstOrDefault().GenderName;
                response.Projects = GetEmployeeProjects(employee.EmployeeId);
                var roleId = db.UserRoles.Where(x => x.EmployeeId == employee.EmployeeId).FirstOrDefault().RoleId;
                response.Role = db.Roles.Where(x => x.RoleId == roleId).FirstOrDefault().RoleDescription;
                response.RoleId = roleId;
            }

            return response;
        }

        public List<EmployeeModel> GetEmployees()
        {
            var response = new List<EmployeeModel>();
            var employees = db.Employees.ToList();
            foreach (var employee in employees)
            {
                var roleId = db.UserRoles.Where(x => x.EmployeeId == employee.EmployeeId).FirstOrDefault().RoleId;
                var model = new EmployeeModel()
                {
                    Age = employee.Age,
                    Email = employee.Email,
                    Name = employee.Name,
                    EmployeeId = employee.EmployeeId,
                    Gender = db.Genders.Where(x => x.GenderId == employee.GenderId).FirstOrDefault().GenderName,
                    ReportingLine = employee.ReportingLine,
                    Projects = GetEmployeeProjects(employee.EmployeeId),
                    Role = db.Roles.Where(x => x.RoleId == roleId).FirstOrDefault().RoleDescription
                };
               response.Add(model);
            }

            return response;
        }

        public List<ProjectModel> GetEmployeeProjects(int employeeId)
        {
            var response = new List<ProjectModel>();
            var employeeProjects = db.EmployeeProjects.Where(x => x.EmployeeId == employeeId).ToList();
            foreach (var employeeProject in employeeProjects)
            {
                var project = db.Projects.Where(x => x.ProjectId == employeeProject.ProjectId).FirstOrDefault();
                var model = new ProjectModel()
                {
                   ProjectName = project.ProjectName,
                   ProjectId = project.ProjectId,
                   Status = project.Status,
                };
                response.Add(model);
            }

            return response;
        }

        public BaseResponse DeleteEmployee(int employeeId)
        {
            var response = new BaseResponse();
            var employee = db.Employees.Where(x => x.EmployeeId == employeeId).FirstOrDefault();
            if (employee != null)
            {
                var employeeProjects = db.EmployeeProjects.Where(x => x.EmployeeId == employee.EmployeeId).FirstOrDefault();
                if(employeeProjects != null)
                {
                    db.Entry(employeeProjects).State = Microsoft.EntityFrameworkCore.EntityState.Deleted;
                    db.SaveChanges();
                }
                

                db.Entry(employee).State = Microsoft.EntityFrameworkCore.EntityState.Deleted;
                db.SaveChanges();

                var userRole = db.UserRoles.Where(x => x.EmployeeId == employee.EmployeeId).FirstOrDefault();
                if(userRole != null)
                {
                    db.Entry(userRole).State = Microsoft.EntityFrameworkCore.EntityState.Deleted;
                    db.SaveChanges();
                }
               

                var userLogin = db.UserLogins.Where(x => x.UserName == employee.Email).FirstOrDefault();
                if(userLogin != null)
                {
                    db.Entry(userLogin).State = Microsoft.EntityFrameworkCore.EntityState.Deleted;
                    db.SaveChanges();
                }
               
                response.Success = true;
            }
            else
            {
                response.Success = false;
            }
            return response;
        }

        public List<GenderModel> GetGender()
        {
            var response = new List<GenderModel>();

            var roles = db.Genders.ToList();
            foreach (var role in roles)
            {
                var model = new GenderModel()
                {
                    GenderId = role.GenderId,
                    GenderName = role.GenderName
                };
                response.Add(model);
            }
            return response;
        }
    }
}

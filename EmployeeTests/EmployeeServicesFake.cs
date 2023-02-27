using IBusinessServices;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeTests
{
    public  class EmployeeServicesFake : IEmployeeService
    {
        private readonly List<EmployeeModel> _employeesList;
        public EmployeeServicesFake()
        {
            _employeesList = new List<EmployeeModel>()
            {
                new EmployeeModel() { EmployeeId = 1,
                    Name = "James Bond", Age=40, Email = "james@mail.com", GenderId = 1, RoleId = 1 },
                 new EmployeeModel() { EmployeeId = 1,
                    Name = "Cari Bond", Age=32, Email = "cari@mail.com", GenderId = 2, RoleId = 2 },
                   new EmployeeModel() { EmployeeId = 1,
                    Name = "Paul Bond", Age=26, Email = "paul@mail.com", GenderId = 1, RoleId = 2 },
            };
        }
        public List<EmployeeModel> GetEmployees()
        {
            return _employeesList;
        }
        public SaveEmployeeResponse SaveEmployee(EmployeeModel newItem)
        {
            newItem.EmployeeId = DateTime.Now.Minute;
            _employeesList.Add(newItem);
            return new SaveEmployeeResponse() { EmployeeId = newItem.EmployeeId, Success = true};
        }
        public BaseResponse UpdateEmployee(EmployeeModel newItem)
        {
            _employeesList.Remove(newItem);
            _employeesList.Add(newItem);
            return new BaseResponse() { Success = true };
        }

        public EmployeeModel GetEmployee(int id)
        {
            return _employeesList.Where(a => a.EmployeeId == id)
                .FirstOrDefault();
        }
        public BaseResponse DeleteEmployee(int id)
        {
            var existing = _employeesList.First(a => a.EmployeeId == id);
            _employeesList.Remove(existing);
            return new BaseResponse() { Success = true };
        }

        public List<GenderModel> GetGender()
        {
            return new List<GenderModel>()
            {
                new GenderModel() { GenderId = 1, GenderName = "Male" },
                new GenderModel() { GenderId = 2, GenderName = "Feale" }
            };
        }
    }
}

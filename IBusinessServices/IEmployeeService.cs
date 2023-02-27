using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBusinessServices
{
    public interface IEmployeeService
    {
        SaveEmployeeResponse SaveEmployee(EmployeeModel employee);
        BaseResponse UpdateEmployee(EmployeeModel employee);
        EmployeeModel GetEmployee(int employeeId);
        List<EmployeeModel> GetEmployees();
        BaseResponse DeleteEmployee(int employeeId);
        List<GenderModel> GetGender();

    }
}

using Helpers;
using IBusinessServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace AcmeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("getemployee/{id}")]
        public ActionResult<EmployeeModel> GetEmployee(int id)
        {
            var result = _employeeService.GetEmployee(id);

            return Ok(result);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("getemployees")]
        public ActionResult<List<EmployeeModel>> GetEmployees()
        {
            var result = _employeeService.GetEmployees();

            return Ok(result);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPut("updateemployee")]
        public ActionResult<BaseResponse> UpdateEmployee(EmployeeModel model)
        {
            var result = _employeeService.UpdateEmployee(model);

            return Ok(result);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpDelete("deleteemployee/{id}")]
        public ActionResult<BaseResponse> DeleteEmployee(int id)
        {
            var result = _employeeService.DeleteEmployee(id);

            return Ok(result);
        }

        [HttpGet("getgender")]
        public ActionResult<List<GenderModel>> GetGender()
        {
            var result = _employeeService.GetGender();

            return Ok(result);
        }
    }
}

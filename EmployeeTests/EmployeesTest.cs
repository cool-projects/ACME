using AcmeAPI.Controllers;
using IBusinessServices;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace EmployeeTests
{
    public class EmployeesTest
    {
        private readonly EmployeeController _controller;
        private readonly IEmployeeService _service;
        public EmployeesTest()
        {
            _service = new EmployeeServicesFake();
            _controller = new EmployeeController(_service);
        }
        [Fact]
        public void Get_WhenCalled_ReturnsOkResult()
        {
            // Act
            var okResult = _controller.GetEmployees();
            // Assert
            Assert.IsType<ActionResult<List<EmployeeModel>>>(okResult as ActionResult<List<EmployeeModel>>);
        }
        [Fact]
        public void Get_WhenCalled_ReturnsAllItems()
        {
            // Act
            var okResult = _controller.GetEmployees();
            // Assert
            var items = okResult;
            Assert.NotNull(items);
        }


    }
}
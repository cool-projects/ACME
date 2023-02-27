using Acme.Data;
using AcmeAPI.Controllers;
using IBusinessServices;
using Microsoft.AspNetCore.Mvc;
using Models;
using Newtonsoft.Json;

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
        public void GetAll_WhenCalled_ReturnsOkResult()
        {
            // Act
            var okResult = _controller.GetEmployees();
            // Assert
            Assert.IsType<ActionResult<List<EmployeeModel>>>(okResult as ActionResult<List<EmployeeModel>>);
        }
        [Fact]
        public void GetAll_WhenCalled_ReturnsAllItems()
        {
            // Act
            var okResult = _controller.GetEmployees();
            // Assert
            var items = okResult.Result;
            Assert.NotNull(items);
        }
        [Fact]
        public void GetSingle_WhenCalled_ReturnsOkResult()
        {
            //Arrange
            int employeeId = 1;
            // Act
            var okResult = _controller.GetEmployee(employeeId);
            // Assert
            Assert.IsType<ActionResult<EmployeeModel>>(okResult as ActionResult<EmployeeModel>);
        }
        [Fact]
        public void GetSingle_WhenCalled_ReturnsAllItems()
        {
            //Arrange
            int employeeId = 1;
            // Act
            var okResult = _controller.GetEmployee(employeeId);
            // Assert
            var items = okResult.Result;
            Assert.NotNull(items);
        }
    }
}
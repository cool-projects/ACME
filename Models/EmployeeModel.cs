using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class EmployeeModel
    {
        public int EmployeeId { get; set; } 
        public string Name { get; set; }
        public int GenderId { get; set; }
        public string? Gender { get; set; }
        public int Age { get; set; }
        public string Email { get; set; }
        public string ReportingLine { get; set; }
        public int? RoleId { get; set; }
        public string? Role { get; set; }
        public List<ProjectModel>? Projects { get; set; }

    }
}

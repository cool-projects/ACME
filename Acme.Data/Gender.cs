﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Acme.Data
{
    public partial class Gender
    {
        public Gender()
        {
            Employees = new HashSet<Employee>();
        }

        public int GenderId { get; set; }
        public string GenderName { get; set; }

        public virtual ICollection<Employee> Employees { get; set; }
    }
}
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Test.Models
{
    public class Employee
    {
        public Employee(string name, TimeSpan totalTimeWorked)
        {
            Name = name;
            TotalTimeWorked = totalTimeWorked; 
        }

        [DisplayName("Name")]
        public string Name { get; set; }

        [DisplayName("Total Time Worked")]
        public TimeSpan TotalTimeWorked { get; set; }
    }
}

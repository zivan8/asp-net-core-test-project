using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Test.Models;

namespace Test.Data
{
    public class InMemoryEmployeesRepo : IEmployeesRepo
    {
        private IEnumerable<Employee> _employees = null;

        public void SetEmployees(IEnumerable<Employee> value)
        {
            _employees = value; 
        }

        public IEnumerable<Employee>? GetEmployees()
        {
            return _employees;
        }
    }
}

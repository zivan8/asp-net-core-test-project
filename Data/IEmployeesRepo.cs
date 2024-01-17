using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Test.Models;

namespace Test.Data
{
    public interface IEmployeesRepo
    { 
        IEnumerable<Employee> GetEmployees();

        void SetEmployees(IEnumerable<Employee> value);
    }
}

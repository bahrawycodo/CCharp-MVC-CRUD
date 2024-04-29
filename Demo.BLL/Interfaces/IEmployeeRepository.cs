using Demo.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.BLL.Interfaces
{
    public interface IEmployeeRepository:IGenericRepository<Employee>
    {
        IEnumerable<Employee> GetEmployeesByDepartmentName(string departmentName);
        IEnumerable<Employee> Search(string name);

        string GetEmployeeDepartmentName(int departmentId);
    }
}

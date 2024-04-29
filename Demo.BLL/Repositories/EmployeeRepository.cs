using Demo.BLL.Interfaces;
using Demo.DAL.Context;
using Demo.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace Demo.BLL.Repositories
{
    public class EmployeeRepository :GenericRepository<Employee> ,IEmployeeRepository
    {
        private readonly AppDbContext _context;

        public EmployeeRepository(AppDbContext context):base(context) {
            _context = context;
        }

        public string GetEmployeeDepartmentName(int departmentId)
            => _context.Departments.FirstOrDefault(x => x.Id == departmentId).Name;
        public IEnumerable<Employee> GetEmployeesByDepartmentName(string departmentName)
        => _context.Employees.Include(x=>x.Department).Where(x=>x.Department.Name == departmentName).ToList();

        public IEnumerable<Employee> Search(string name)
        =>_context.Employees.Where(x=> x.Name.Trim().ToLower().Contains(name.Trim().ToLower()) 
                                        || x.Address.Trim().ToLower().Contains(name.Trim().ToLower())
                                        || x.Email.Trim().ToLower().Contains(name.Trim().ToLower())).ToList();

        
    }
}

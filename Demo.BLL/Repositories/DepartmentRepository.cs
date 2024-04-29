using Demo.BLL.Interfaces;
using Demo.DAL.Context;
using Demo.DAL.Entities;

namespace Demo.BLL.Repositories
{
    public class DepartmentRepository :GenericRepository<Department>, IDepartmentRepository
    {
        private readonly AppDbContext _context;

        public DepartmentRepository(AppDbContext context):base(context) 
        {
            _context = context;
        }
      
    }
}

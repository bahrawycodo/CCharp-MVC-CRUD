using AutoMapper;
using Demo.BLL.Interfaces;
using Demo.DAL.Entities;
using Demo.PL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Demo.PL.Controllers
{
    [Authorize]
    public class DepartmentController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DepartmentController(
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public IActionResult Index()
        {
            var departmentViewModel = _mapper.Map<IEnumerable<DepartmentViewModel>>(_unitOfWork.DepartmentRepository.GetAll());
            return View(departmentViewModel);
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(DepartmentViewModel departmentViewModel)
        {
            if (ModelState.IsValid)
            {
                var department = _mapper.Map<Department>(departmentViewModel);
                _unitOfWork.DepartmentRepository.Add(department);
                _unitOfWork.Complete();
                return RedirectToAction(nameof(Index));
            }
            return View(departmentViewModel);
        }
        public IActionResult Update(int? id)
        {
            if (id is null)
                return BadRequest();
            var departmentViewModel = _mapper.Map<DepartmentViewModel>(_unitOfWork.DepartmentRepository.GetById(id));

            if (departmentViewModel is null)
               return NotFound();
            return View(departmentViewModel);

        }
        [HttpPost]
        public IActionResult Update(int id,DepartmentViewModel departmentViewModel)
        {
            if(id != departmentViewModel.Id)
                return BadRequest();
            if(ModelState.IsValid)
            {
                var department = _mapper.Map<Department>(departmentViewModel);
                _unitOfWork.DepartmentRepository.Update(department);
                _unitOfWork.Complete();
                return RedirectToAction(nameof(Index));
            }
            return View(departmentViewModel);
        }
        public IActionResult Details(int? id)
        {

          if (id is null)
            return BadRequest();

           var departmentViewModel = _mapper.Map<DepartmentViewModel>(_unitOfWork.DepartmentRepository.GetById(id));

            if (departmentViewModel is null)
              return NotFound();
            return View(departmentViewModel);
                      
        }

        //public IActionResult Delete(int? id)
        //{

        //        if(id is null)
        //            return BadRequest();
        //        Department department = _departmentRepository.GetById(id);
        //        if(department is null)
        //            return NotFound();
        //        _departmentRepository.Delete(department);
        //        return RedirectToAction(nameof(Index));
        //   
        //}
        public JsonResult Delete(int? id)
        {
          if(id is null)
             return Json(new { Message = "You must send id", Status = 400 });
          Department department = _unitOfWork.DepartmentRepository.GetById(id);
          if (department is null)
              return Json(new { Message = "There isn't Department with this Id", Status = 400 });
            _unitOfWork.DepartmentRepository.Delete(department);
            _unitOfWork.Complete();
          return Json(new  { Message= "Success" ,Status=200});
           
        }
    }
}

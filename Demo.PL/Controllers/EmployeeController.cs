using AutoMapper;
using Demo.BLL.Interfaces;
using Demo.DAL.Entities;
using Demo.PL.Helper;
using Demo.PL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Runtime.Serialization.Formatters;

namespace Demo.PL.Controllers
{
    [Authorize]
    public class EmployeeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public EmployeeController(IUnitOfWork unitOfWork,IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public IActionResult Index(string SearchValue = "")
        {

            ViewBag.SearchValue = string.IsNullOrEmpty(SearchValue) ? "": SearchValue;
            IEnumerable<Employee> employees;

            if (string.IsNullOrEmpty(SearchValue))
                employees = _unitOfWork.EmployeeRepository.GetAll();
            else
                employees = _unitOfWork.EmployeeRepository.Search(SearchValue);
            var demployeeViewModel = _mapper.Map<IEnumerable<EmployeeViewModel>>(employees);
            return View("Index", demployeeViewModel);
        }

        public IActionResult Create()
        {
            ViewBag.Departments = _mapper.Map<IEnumerable<DepartmentViewModel>>(_unitOfWork.DepartmentRepository.GetAll());
            return View();
        }
        [HttpPost]
        public IActionResult Create(EmployeeViewModel employeeViewModel)
        {
            if (ModelState.IsValid)
            {
                var employee = _mapper.Map<Employee>(employeeViewModel);
                employee.ImageUrl = DocumentSettings.UploadFile(employeeViewModel.Image, "Images/Employees");

                _unitOfWork.EmployeeRepository.Add(employee);
                _unitOfWork.Complete();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Departments = _mapper.Map<IEnumerable<DepartmentViewModel>>(_unitOfWork.DepartmentRepository.GetAll());
            return View(employeeViewModel);
        }
        public IActionResult Details(int? id)
        {
            if (id is null)
                return BadRequest();

            var employeeViewModel = _mapper.Map<EmployeeViewModel>(_unitOfWork.EmployeeRepository.GetById(id));
            if (employeeViewModel == null)
                return NotFound();

            ViewBag.DepartmentName = _unitOfWork.EmployeeRepository.GetEmployeeDepartmentName(employeeViewModel.DepartmentId);

            return View(employeeViewModel);
        }
        public IActionResult Update(int? id)
        {
            if (id is null)
                return BadRequest();
            var employeeViewModel = _mapper.Map<EmployeeViewModel>(_unitOfWork.EmployeeRepository.GetById(id));
            if (employeeViewModel is null)
                return NotFound();
            ViewBag.Departments = _mapper.Map<IEnumerable<DepartmentViewModel>>(_unitOfWork.DepartmentRepository.GetAll());

            return View(employeeViewModel);
        }
        [HttpPost]
        public IActionResult Update(int id, EmployeeViewModel employeeViewModel)
        {
            if (id != employeeViewModel.Id)
                return BadRequest();
            if (ModelState.IsValid)
            {
                var employee = _mapper.Map<Employee>(employeeViewModel);
                
                if (employeeViewModel.Image is not null)
                {
                    DocumentSettings.RemoveFile(employee.ImageUrl, "Images/Employees");
                    employee.ImageUrl = DocumentSettings.UploadFile(employeeViewModel.Image, "Images/Employees");
                }
               

                _unitOfWork.EmployeeRepository.Update(employee);
                _unitOfWork.Complete();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Departments = _mapper.Map<IEnumerable<DepartmentViewModel>>(_unitOfWork.DepartmentRepository.GetAll());
            return View(employeeViewModel);
        }

        [HttpPost]
        public JsonResult Delete(int? id)
        {
            if (id is null)
                return Json(new { Message = "You must send id", Status = 400 });
            Employee employee = _unitOfWork.EmployeeRepository.GetById(id);

            if (employee is null)
                return Json(new { Message = "There isn't Employee with this Id", Status = 400 });
            _unitOfWork.EmployeeRepository.Delete(employee);
            DocumentSettings.RemoveFile(employee.ImageUrl, "Images/Employees");

            _unitOfWork.Complete();
            return Json(new { Message = "Success", Status = 200 });
        }

    }
}

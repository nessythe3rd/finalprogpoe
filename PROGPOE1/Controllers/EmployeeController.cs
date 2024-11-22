using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PROGPOE1.DAL;
using PROGPOE1.Models;
using PROGPOE1.Models.DBEntities;
using System.Linq;

namespace PROGPOE1.Controllers
{
    [Authorize]
    public class EmployeeController : Controller
    {
        private readonly EmployeeDbContext _context;

        public EmployeeController(EmployeeDbContext context) => _context = context;

        [HttpGet]
        public IActionResult Index()
        {
            if (User.IsInRole("Lecturer"))
                return RedirectToAction("Create");

            var employees = _context.Employees
                .Select(e => new EmployeeViewModel
                {
                    Id = e.Id,
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    DateOfBirth = e.DateOfBirth,
                    Email = e.Email,
                    HoursWorked = e.HoursWorked,
                    HourlyRate = e.HourlyRate,
                    Salary = e.Salary,
                    Status = e.Status
                })
                .ToList();

            return View(employees);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var employee = _context.Employees.Find(id);
            if (employee == null)
                return NotFoundRedirect(id);

            var model = MapEmployeeToViewModel(employee);
            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(EmployeeViewModel model)
        {
            if (!ModelState.IsValid)
                return ViewWithError(model, "Invalid data.");

            var employee = _context.Employees.Find(model.Id);
            if (employee == null)
                return RedirectToAction("Index");

            UpdateEmployeeFromViewModel(employee, model);
            _context.SaveChanges();
            TempData["successMessage"] = "Employee details updated successfully";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        public IActionResult Create(EmployeeViewModel model)
        {
            var employee = new Employee
            {
                FirstName = model.FirstName ?? "Unknown",
                LastName = model.LastName ?? "Unknown",
                DateOfBirth = model.DateOfBirth == default ? DateTime.Now : model.DateOfBirth,
                Email = string.IsNullOrWhiteSpace(model.Email) ? "unknown@example.com" : model.Email,
                HoursWorked = model.HoursWorked > 0 ? model.HoursWorked : 0,
                HourlyRate = model.HourlyRate > 0 ? model.HourlyRate : 0,
                Salary = model.HoursWorked * model.HourlyRate,
                Status = model.Status ?? "Pending"
            };

            _context.Employees.Add(employee);
            _context.SaveChanges();
            TempData["successMessage"] = "Employee created successfully!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Accept(int id) => ChangeEmployeeStatus(id, "Accepted");

        [HttpPost]
        public IActionResult Reject(int id) => ChangeEmployeeStatus(id, "Rejected");

        private IActionResult ChangeEmployeeStatus(int id, string status)
        {
            var employee = _context.Employees.Find(id);
            if (employee == null)
                return RedirectToAction("Index");

            employee.Status = status;
            _context.SaveChanges();
            TempData["successMessage"] = $"Employee {employee.FirstName} {employee.LastName} has been {status.ToLower()}.";
            return RedirectToAction("Index");
        }

        private IActionResult NotFoundRedirect(int id)
        {
            TempData["errorMessage"] = $"Employee with id {id} not found.";
            return RedirectToAction("Index");
        }

        private EmployeeViewModel MapEmployeeToViewModel(Employee employee)
        {
            return new EmployeeViewModel
            {
                Id = employee.Id,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                DateOfBirth = employee.DateOfBirth,
                Email = employee.Email,
                HourlyRate = employee.HourlyRate,
                HoursWorked = employee.HoursWorked,
                Salary = employee.Salary,
                Status = employee.Status
            };
        }

        private void UpdateEmployeeFromViewModel(Employee employee, EmployeeViewModel model)
        {
            employee.FirstName = model.FirstName;
            employee.LastName = model.LastName;
            employee.DateOfBirth = model.DateOfBirth;
            employee.Email = model.Email;
            employee.HourlyRate = model.HourlyRate;
            employee.HoursWorked = model.HoursWorked;
            employee.Salary = model.HoursWorked * model.HourlyRate;  // Recalculate salary
            employee.Status = model.Status;
        }

        private IActionResult ViewWithError(EmployeeViewModel model, string errorMessage)
        {
            TempData["errorMessage"] = errorMessage;
            return View(model);
        }
    }
}

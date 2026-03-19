using FluentValidation;
using Microsoft.AspNetCore.Mvc;


public class EmployeesController : BaseController
{
    private readonly IRepository<Employee> _repository;
    private readonly IValidator<CreateEmployeeRequest> _createValidator;

    public EmployeesController(IRepository<Employee> repository, IValidator<CreateEmployeeRequest> createValidator)
    {
        _repository = repository;
        _createValidator = createValidator;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(_repository.GetAll().Select(employee => new GetEmployeeResponse{
        FirstName = employee.FirstName,
        LastName = employee.LastName,
        Address1 = employee.Address1,
        Address2 = employee.Address2,
        City = employee.City,
        State = employee.State,
        ZipCode = employee.ZipCode,
        PhoneNumber = employee.PhoneNumber,
        Email = employee.Email
    }));
    }

    [HttpGet("{id}")]

    public IActionResult GetById(int id)
    {
        var employee = _repository.GetById(id);
        if (employee == null)
        {
            return NotFound();
        }

        return Ok(new GetEmployeeResponse
        {
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            Address1 = employee.Address1,
            Address2 = employee.Address2,
            City = employee.City,
            State = employee.State,
            ZipCode = employee.ZipCode,
            PhoneNumber = employee.PhoneNumber,
            Email = employee.Email
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateEmployeeRequest employeeRequest)
    {
        var validationResults = await _createValidator.ValidateAsync(employeeRequest);
        if (!validationResults.IsValid)
        {
            return BadRequest(validationResults.ToDictionary());
        }

        var newEmployee = new Employee {
            FirstName = employeeRequest.FirstName!,
            LastName = employeeRequest.LastName!,
            SocialSecurityNumber = employeeRequest.SocialSecurityNumber,
            Address1 = employeeRequest.Address1,
            Address2 = employeeRequest.Address2,
            City = employeeRequest.City,
            State = employeeRequest.State,
            ZipCode = employeeRequest.ZipCode,
            PhoneNumber = employeeRequest.PhoneNumber,
            Email = employeeRequest.Email
        };
        _repository.Create(newEmployee);
        return Created($"/employees/{newEmployee.Id}", employeeRequest);
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, UpdateEmployeeRequest updatedEmployee)
    {
            var existingEmployee = _repository.GetById(id);
            if(existingEmployee == null)
            {
                return NotFound();
            }

            existingEmployee.Email = updatedEmployee.Email;
            existingEmployee.PhoneNumber = updatedEmployee.PhoneNumber;
            existingEmployee.Address1 = updatedEmployee.Address1;
            existingEmployee.Address2 = updatedEmployee.Address2;
            existingEmployee.City = updatedEmployee.City;
            existingEmployee.State = updatedEmployee.State;
            existingEmployee.ZipCode = updatedEmployee.ZipCode;

            _repository.Update(existingEmployee);

            return Ok(existingEmployee);
    }
}
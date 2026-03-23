using FluentValidation;
using Microsoft.AspNetCore.Mvc;


public class EmployeesController : BaseController
{
    private readonly IRepository<Employee> _repository;
    private readonly IValidator<CreateEmployeeRequest> _createValidator;
    private readonly ILogger<EmployeesController> _logger;

    public EmployeesController(IRepository<Employee> repository, IValidator<CreateEmployeeRequest> createValidator, ILogger<EmployeesController> logger)
    {
        _repository = repository;
        _createValidator = createValidator;
        _logger = logger;
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
            _logger.LogInformation("Updating employee with ID: {EmployeeId}", id);

            var existingEmployee = _repository.GetById(id);
            if(existingEmployee == null)
            {
                _logger.LogWarning("Employee with ID: {EmployeeId} not found", id);
                return NotFound();
            }

            _logger.LogDebug("Updating employee details for ID: {EmployeeId}", id);
            existingEmployee.Email = updatedEmployee.Email;
            existingEmployee.PhoneNumber = updatedEmployee.PhoneNumber;
            existingEmployee.Address1 = updatedEmployee.Address1;
            existingEmployee.Address2 = updatedEmployee.Address2;
            existingEmployee.City = updatedEmployee.City;
            existingEmployee.State = updatedEmployee.State;
            existingEmployee.ZipCode = updatedEmployee.ZipCode;

            try
            {
                _repository.Update(existingEmployee);
                _logger.LogInformation("Employee with ID: {EmployeeId} successfully updated", id);
                return Ok(existingEmployee);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating employee with ID: {EmployeeId}", id);
                return StatusCode(500, "An error occurred while updating the employee");
            }
    }
}
using Microsoft.AspNetCore.Mvc;


var builder = WebApplication.CreateBuilder(args);

var employees = new List<Employee>();

employees.Add(new Employee { Id= 1, FirstName= "John", LastName= "Does", SocialSecurityNumber = "123-45-6789"});
employees.Add(new Employee { Id= 2, FirstName= "Jane", LastName= "Does", SocialSecurityNumber = "987-65-4321"});


// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var employeeRoute = app.MapGroup("employees");

app.UseHttpsRedirection();

employeeRoute.MapGet(string.Empty, (EmployeeRepository repository) =>
{
    return Results.Ok(repository.GetAll().Select(employee => new GetEmployeeResponse{
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

});
employeeRoute.MapGet("/{id}", (int id, EmployeeRepository) =>
{
    var employee = repository.GetById(id);
    if (employee == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(new GetEmployeeResponse {
        FirstName = employee.FirstName,
        LastName = employee.LastName,
        Address1 = employee.Address1,
        Address2 = employee.Address2,
        City = employee.City,
        State = employee.State,
        ZipCode = employee.ZipCode,
        PhoneNumber = employee.PhoneNumber,
        Email = employee.Email,

    });
});

employeeRoute.MapPost(string.Empty,(CreateEmployeeRequest employeeRequest, EmployeeRepository repository) =>{
    var newEmployee = new Employee {
        Id = employees.Max(e=> e.Id) + 1,
        FirstName = employeeRequest.FirstName,
        LastName = employeeRequest.LastName,
        SocialSecurityNumber = employeeRequest.SocialSecurityNumber,
        Address1 = employeeRequest.Address1,
        Address2 = employeeRequest.Address2,
        City = employeeRequest.City,
        State = employeeRequest.State,
        ZipCode = employeeRequest.ZipCode,
        PhoneNumber = employeeRequest.PhoneNumber,
        Email = employeeRequest.Email
    };
    repository.Create(newEmployee);
    return Results.Created($"/employees/{newEmployee.Id}", employeeRequest);
});

employeeRoute.MapPut("/{id}", ([FromBody] UpdateEmployeeRequest updatedEmployee, int id, EmployeeRepository repository) =>
{
    var existingEmployee = repository.GetById(id);
    if(existingEmployee == null)
    {
        return Results.NotFound();
    }

    existingEmployee.FirstName = updatedEmployee.FirstName;
    existingEmployee.LastName = updatedEmployee.LastName;
    existingEmployee.Email = updatedEmployee.Email;
    existingEmployee.PhoneNumber = updatedEmployee.PhoneNumber;
    existingEmployee.Address1 = updatedEmployee.Address1;
    existingEmployee.Address2 = updatedEmployee.Address2;
    existingEmployee.City = updatedEmployee.City;
    existingEmployee.State = updatedEmployee.State;
    existingEmployee.ZipCode = updatedEmployee.ZipCode;
    existingEmployee.SocialSecurityNumber = updatedEmployee.SocialSecurityNumber;

    repository.Update(existingEmployee);

    return Results.Ok(existingEmployee);
});

app.Run();


public partial class Program {}


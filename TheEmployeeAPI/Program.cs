using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;


var builder = WebApplication.CreateBuilder(args);

var employees = new List<Employee>();

employees.Add(new Employee { Id= 1, FirstName= "John", LastName= "Does", SocialSecurityNumber = "123-45-6789"});
employees.Add(new Employee { Id= 2, FirstName= "Jane", LastName= "Does", SocialSecurityNumber = "987-65-4321"});


// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IRepository<Employee>, EmployeeRepository>();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddControllers();

var app = builder.Build();

// Seed the repository with initial data
using (var scope = app.Services.CreateScope())
{
    var repository = scope.ServiceProvider.GetRequiredService<IRepository<Employee>>();
    if (!repository.GetAll().Any())
    {
        repository.Create(new Employee { FirstName = "John", LastName = "Does", SocialSecurityNumber = "123-45-6789" });
        repository.Create(new Employee { FirstName = "Jane", LastName = "Does", SocialSecurityNumber = "987-65-4321" });
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var employeeRoute = app.MapGroup("employees");

app.UseHttpsRedirection();

employeeRoute.MapGet(string.Empty, ([FromServices] IRepository<Employee> repository) =>
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
employeeRoute.MapGet("/{id}", (int id, [FromServices] IRepository<Employee> repository) =>
{
    var employee = repository.GetById(id);
    if (employee == null)
    {
        return Results.NotFound();
    }

    return Results.Ok(new GetEmployeeResponse
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
});

employeeRoute.MapPost(string.Empty, async (CreateEmployeeRequest employeeRequest, IRepository<Employee> repository, IValidator<CreateEmployeeRequest> validator) => {
    var validationResults = await validator.ValidateAsync(employeeRequest);
    if (!validationResults.IsValid)
    {
        return Results.BadRequest(validationResults.ToDictionary());
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
    repository.Create(newEmployee);
    return Results.Created($"/employees/{newEmployee.Id}", employeeRequest);
});

employeeRoute.MapPut("/{id}", (UpdateEmployeeRequest updatedEmployee, int id, [FromServices] IRepository<Employee> repository) =>
{
    var existingEmployee = repository.GetById(id);
    if(existingEmployee == null)
    {
        return Results.NotFound();
    }

    existingEmployee.Email = updatedEmployee.Email;
    existingEmployee.PhoneNumber = updatedEmployee.PhoneNumber;
    existingEmployee.Address1 = updatedEmployee.Address1;
    existingEmployee.Address2 = updatedEmployee.Address2;
    existingEmployee.City = updatedEmployee.City;
    existingEmployee.State = updatedEmployee.State;
    existingEmployee.ZipCode = updatedEmployee.ZipCode;

    repository.Update(existingEmployee);

    return Results.Ok(existingEmployee);
});

app.MapControllers();
app.Run();


public partial class Program {}


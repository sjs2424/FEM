using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;


var builder = WebApplication.CreateBuilder(args);

var employees = new List<Employee>();


// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IRepository<Employee>, EmployeeRepository>();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers(options =>
    {
        options.Filters.Add<FluentValidationFilter>();
    });

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


app.UseHttpsRedirection();


app.MapControllers();
app.Run();


public partial class Program {}


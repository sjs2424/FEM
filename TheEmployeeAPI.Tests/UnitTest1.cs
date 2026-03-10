using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;

namespace TheEmployeeAPI.Tests;

public class BasicTests: IClassFixture<WebApplicationFactory<Program>>
{

    private readonly WebApplicationFactory<Program> _applicationFactory;

    public BasicTests()    {
        _applicationFactory = new WebApplicationFactory<Program>();
    }
    [Fact]
    public async Task GetAllEmployees_ReturnsOk()
    {
        var client = _applicationFactory.CreateClient();
        var response = await client.GetAsync("/employees");
        response.EnsureSuccessStatusCode();
    }

    [Fact]

    public async Task CreateEmployee_ReturnsCreated()
    {
        var client = _applicationFactory.CreateClient();
        var response = await client.PostAsJsonAsync("/employees", new Employee { FirstName = "Test", LastName = "User", SocialSecurityNumber = "111-22-3333" });
        response.EnsureSuccessStatusCode();
    }

    [Fact]

    public async Task CreateEmployee_ReturnsBadRequest_WhenInvalid()
    {
        var client = _applicationFactory.CreateClient();
        var response = await client.PostAsJsonAsync("/employees", new {});
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]

    public async Task UpdateEmployee_ReturnsOk()
    {
        var client = _applicationFactory.CreateClient();
        var response = await client.PutAsJsonAsync("/employees/1", new Employee { FirstName = "Updated", LastName = "User", SocialSecurityNumber = "111-22-3333" });
        response.EnsureSuccessStatusCode();
    }
}
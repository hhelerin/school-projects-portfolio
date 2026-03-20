using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace WebApp.Tests.Integration;

[Collection("Database tests")]
public class IntegrationTestHomeController : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory<Program> _factory;


    public IntegrationTestHomeController(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }


    [Fact]
    public async Task Get_Index_ReturnsOk_ForAnonymousUser()
    {
        // Act
        var response = await _client.GetAsync("/");
        
        // Assert - homepage is accessible to everyone, shows different content based on auth
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
    }

}
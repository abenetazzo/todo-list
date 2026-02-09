using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Todo.Domain.Todos;
using Todo.Api.Data;
using Todo.Api.Services;

namespace Todo.Api.Tests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
    }

    // Seed eseguito UNA VOLTA per fixture
    public Func<IServiceProvider, Task>? SeedData { get; set; } = null;
}
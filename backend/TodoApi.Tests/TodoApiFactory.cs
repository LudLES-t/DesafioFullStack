using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TodoApi.Data;
using TodoApi.Models;

namespace TodoApi.Tests;

public class TodoApiFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.RemoveAll(typeof(DbContextOptions<AppDbContext>));

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseInMemoryDatabase("TodoTestDb");
            });

            var serviceProvider = services.BuildServiceProvider();

            using var scope = serviceProvider.CreateScope();

            var context = scope.ServiceProvider
                .GetRequiredService<AppDbContext>();

            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            context.Todos.AddRange(
                new Todo { Id = 1, UserId = 1, Title = "Primeira tarefa", Completed = false },
                new Todo { Id = 2, UserId = 1, Title = "Segunda tarefa", Completed = false },
                new Todo { Id = 3, UserId = 1, Title = "Terceira tarefa", Completed = false },
                new Todo { Id = 4, UserId = 1, Title = "Quarta tarefa", Completed = false },
                new Todo { Id = 5, UserId = 1, Title = "Quinta tarefa", Completed = false },
                new Todo { Id = 6, UserId = 1, Title = "Tarefa completa", Completed = true },
                new Todo { Id = 7, UserId = 2, Title = "Expedita exemplo", Completed = true },
                new Todo { Id = 8, UserId = 2, Title = "Outra tarefa", Completed = false }
            );

            context.SaveChanges();
        });
    }
}
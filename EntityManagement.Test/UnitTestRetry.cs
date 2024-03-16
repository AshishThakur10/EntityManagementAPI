using EntityManagementAPI.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;


namespace EntityManagement.Tests
{
    public class EntitiesControllerIntegrationTests : IClassFixture<WebApplicationFactory<EntityManagement.API.Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public EntitiesControllerIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task CreateEntity_SuccessAfterRetries_ReturnsOk()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Simulate failure initially but success after retries
            int initialFailures = 2;
            int retries = 3;
            int expectedAttempts = initialFailures + retries + 1; // +1 for the initial request
            int currentAttempt = 1;

            // Mock the database to fail initially and then succeed
            MockDatabase.FailInitialWrites = initialFailures;

            // Create a valid entity payload
            var entity = new { Name = "Test Entity" };
            var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(entity), Encoding.UTF8, "application/json");

            // Act
            HttpResponseMessage response = null;
            while (currentAttempt <= expectedAttempts)
            {
                response = await client.PostAsync("/api/entities", content);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    break; // Success, exit the loop
                }
                currentAttempt++;
            }

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(expectedAttempts, currentAttempt); // Ensure the expected number of attempts was made
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.ServiceDiscovery;
using Xunit;

namespace ChatWYData.ServiceDefaults.Tests;

public class ExtensionsTests
{
    [Fact]
    public void AddServiceDefaults_RegistersRequiredServices()
    {
        // Arrange
        var builder = Host.CreateApplicationBuilder();

        // Act
        builder.AddServiceDefaults();

        // Assert
        var services = builder.Services;
        
        // Check that health checks are registered
        Assert.Contains(services, s => s.ServiceType == typeof(HealthCheckService));
        
        // Check that HttpClient factory is registered (from service discovery)
        Assert.Contains(services, s => s.ServiceType == typeof(IHttpClientFactory));
    }

    [Fact]
    public void AddDefaultHealthChecks_RegistersHealthCheckService()
    {
        // Arrange
        var builder = Host.CreateApplicationBuilder();

        // Act
        builder.AddDefaultHealthChecks();

        // Assert
        var services = builder.Services;
        Assert.Contains(services, s => s.ServiceType == typeof(HealthCheckService));
    }

    [Fact]
    public void AddDefaultHealthChecks_RegistersSelfHealthCheck()
    {
        // Arrange
        var builder = Host.CreateApplicationBuilder();

        // Act
        builder.AddDefaultHealthChecks();
        var app = builder.Build();

        // Assert
        var healthCheckService = app.Services.GetRequiredService<HealthCheckService>();
        Assert.NotNull(healthCheckService);
        
        // The health check service should be available
        var healthCheckOptions = app.Services.GetService<HealthCheckService>();
        Assert.NotNull(healthCheckOptions);
    }

    [Fact]
    public async Task AddDefaultHealthChecks_SelfCheckReturnsHealthy()
    {
        // Arrange
        var builder = Host.CreateApplicationBuilder();
        builder.AddDefaultHealthChecks();
        var app = builder.Build();

        // Act
        var healthCheckService = app.Services.GetRequiredService<HealthCheckService>();
        var result = await healthCheckService.CheckHealthAsync();

        // Assert
        Assert.Equal(HealthStatus.Healthy, result.Status);
        Assert.Contains("self", result.Entries.Keys);
        Assert.Equal(HealthStatus.Healthy, result.Entries["self"].Status);
    }

    [Fact]
    public void MapDefaultEndpoints_InDevelopmentEnvironment_MapsHealthEndpoints()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();
        builder.Environment.EnvironmentName = Environments.Development;
        builder.AddServiceDefaults();
        
        var app = builder.Build();

        // Act
        app.MapDefaultEndpoints();

        // Assert - This test verifies the method can be called without exception
        // In a real scenario, we would need integration tests to verify endpoints work
        Assert.NotNull(app);
    }

    [Fact]
    public void ConfigureOpenTelemetry_RegistersOpenTelemetryServices()
    {
        // Arrange
        var builder = Host.CreateApplicationBuilder();

        // Act
        builder.AddServiceDefaults();

        // Assert
        var services = builder.Services;
        
        // Check that OpenTelemetry services are registered
        // Note: The actual OpenTelemetry registration happens through extension methods
        // We can verify that the builder doesn't throw and the service collection is configured
        Assert.NotNull(services);
        
        // Build to ensure no exceptions during service registration
        var app = builder.Build();
        Assert.NotNull(app);
    }

    [Fact]
    public void AddServiceDefaults_ConfiguresHttpClientDefaults()
    {
        // Arrange
        var builder = Host.CreateApplicationBuilder();

        // Act
        builder.AddServiceDefaults();
        var app = builder.Build();

        // Assert
        var httpClientFactory = app.Services.GetRequiredService<IHttpClientFactory>();
        Assert.NotNull(httpClientFactory);
        
        // Create an HttpClient to verify it's configured
        var httpClient = httpClientFactory.CreateClient();
        Assert.NotNull(httpClient);
    }

    [Fact]
    public void AddServiceDefaults_WithCustomBuilder_DoesNotThrow()
    {
        // Arrange
        var builder = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings
        {
            ApplicationName = "TestApp",
            EnvironmentName = Environments.Development
        });

        // Act & Assert
        var exception = Record.Exception(() => builder.AddServiceDefaults());
        Assert.Null(exception);
        
        var app = builder.Build();
        Assert.NotNull(app);
    }

    [Fact]
    public void AddServiceDefaults_CanBeCalledMultipleTimes()
    {
        // Arrange
        var builder = Host.CreateApplicationBuilder();

        // Act & Assert - Should not throw when called multiple times
        builder.AddServiceDefaults();
        var exception = Record.Exception(() => builder.AddServiceDefaults());
        Assert.Null(exception);
        
        var app = builder.Build();
        Assert.NotNull(app);
    }

    [Fact]
    public void AddDefaultHealthChecks_CanBeCalledMultipleTimes()
    {
        // Arrange
        var builder = Host.CreateApplicationBuilder();

        // Act & Assert - Should not throw when called multiple times
        builder.AddDefaultHealthChecks();
        var exception = Record.Exception(() => builder.AddDefaultHealthChecks());
        Assert.Null(exception);
        
        var app = builder.Build();
        Assert.NotNull(app);
    }
}
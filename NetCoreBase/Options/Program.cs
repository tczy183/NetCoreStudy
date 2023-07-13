using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Runtime;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

var service = new ServiceCollection();
service.AddTransient<ITransientService, TransientService>();
service.AddTransient<ITransientService, TransientService2>();

service.AddScoped<IScopedService, ScopedService>();
service.AddSingleton<ISingletonService, SingletonService>();

#region Options

{
    Console.WriteLine("+++++++++++++++++++++++++++Options+++++++++++++++++++++++++++");
    IConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
    configurationBuilder.AddJsonFile("AppSettings.json", false, false);
    var configurationRoot = configurationBuilder.Build();
    service.AddOptions();
    service.Configure<TestOption>(configurationRoot.GetSection("Person").Bind);

    service.AddTransient<IConfigurationService, ConfigurationService>();

    using var buildServiceProvider = service.BuildServiceProvider();
    using var scope = buildServiceProvider.CreateScope();
    var configurationService = scope.ServiceProvider.GetRequiredService<IConfigurationService>();
    configurationService.ShowCode();
    Console.WriteLine("+++++++++++++++++++++++++++Options+++++++++++++++++++++++++++");
}

#endregion


#region 辅助测试类
public interface ITransientService
{
    void ShowCode();
}

public class TransientService : ITransientService
{
    public void ShowCode()
    {
        Console.WriteLine("Hello World!");
    }
}
public class TransientService2 : ITransientService
{
    public void ShowCode()
    {
        Console.WriteLine("Hello World2!");
    }
}

public interface IScopedService
{
    void ShowCode();
}

public class ScopedService : IScopedService
{
    public void ShowCode()
    {
        Console.WriteLine("Hello World!");
    }
}

public interface ISingletonService
{
    void ShowCode();
}

public class SingletonService : ISingletonService
{
    public void ShowCode()
    {
        Console.WriteLine("Hello World!");
    }
}

public class TestOption
{
    public string Name { get; set; }
    public int Age { get; set; }
}

public interface IConfigurationService
{
    void ShowCode();
}

public class ConfigurationService : IConfigurationService
{
    private readonly TestOption _testOptions;

    public ConfigurationService(IOptions<TestOption> testOptions)
    {
        _testOptions = testOptions.Value;
    }
    public void ShowCode()
    {
        Console.WriteLine(JsonConvert.SerializeObject(_testOptions));

    }
}

#endregion

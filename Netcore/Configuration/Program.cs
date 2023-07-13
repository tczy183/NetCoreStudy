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

#region 系统配置相关

{
    Console.WriteLine("+++++++++++++++++++++++++++系统配置相关+++++++++++++++++++++++++++");
    IConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
    configurationBuilder.AddJsonFile("AppSettings.json", false, false);
    var configurationRoot = configurationBuilder.Build();
    var s = configurationRoot["Person:Name"];
    Console.WriteLine(s);

    Console.WriteLine("+++++++++++++++++++++++++++系统配置相关+++++++++++++++++++++++++++");
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



#endregion

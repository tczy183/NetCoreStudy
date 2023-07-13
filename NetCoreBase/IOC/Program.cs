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

#region 测试注册多个和单个的区别
{
    using ServiceProvider sp = service.BuildServiceProvider();
    Console.WriteLine("+++++++++++++++++++++++++++测试注册多个和单个的区别+++++++++++++++++++++++++++");
    using var serviceScope = sp.CreateScope();
    var myServices = serviceScope.ServiceProvider.GetServices<ITransientService>();
    foreach (var myService in myServices)
    {
        myService.ShowCode();
    }

    Console.WriteLine("--------------------");
    var thsMyService = serviceScope.ServiceProvider.GetRequiredService<ITransientService>();
    thsMyService.ShowCode();
    Console.WriteLine("+++++++++++++++++++++++++++测试注册多个和单个的区别+++++++++++++++++++++++++++");
}
#endregion

#region 测试单例
{
    using ServiceProvider sp = service.BuildServiceProvider();

    Console.WriteLine("+++++++++++++++++++++++++++测试单例+++++++++++++++++++++++++++");
    using var serviceScope = sp.CreateScope();
    var myService = serviceScope.ServiceProvider.GetRequiredService<ISingletonService>();
    myService.ShowCode();

    Console.WriteLine("--------------------");
    var myService2 = serviceScope.ServiceProvider.GetRequiredService<ISingletonService>();
    myService2.ShowCode();

    Console.WriteLine("--------------------");
    using var serviceScope2 = sp.CreateScope();
    var myService3 = serviceScope2.ServiceProvider.GetRequiredService<ISingletonService>();
    myService3.ShowCode();
    Console.WriteLine("--------------------");

    Console.WriteLine(myService == myService2);
    Console.WriteLine(myService2 == myService3);
    Console.WriteLine(myService == myService3);
    Console.WriteLine("+++++++++++++++++++++++++++测试单例+++++++++++++++++++++++++++");
}
#endregion

#region 测试瞬时
{
    using ServiceProvider sp = service.BuildServiceProvider();

    Console.WriteLine("+++++++++++++++++++++++++++测试瞬时+++++++++++++++++++++++++++");
    using var serviceScope = sp.CreateScope();
    var myService = serviceScope.ServiceProvider.GetRequiredService<ITransientService>();
    myService.ShowCode();

    Console.WriteLine("--------------------");
    var myService2 = serviceScope.ServiceProvider.GetRequiredService<ITransientService>();
    myService2.ShowCode();

    Console.WriteLine("--------------------");
    using var serviceScope2 = sp.CreateScope();
    var myService3 = serviceScope2.ServiceProvider.GetRequiredService<ITransientService>();
    myService3.ShowCode();
    Console.WriteLine("--------------------");
    Console.WriteLine(myService == myService2);
    Console.WriteLine(myService2 == myService3);
    Console.WriteLine(myService == myService3);
    Console.WriteLine("+++++++++++++++++++++++++++测试瞬时+++++++++++++++++++++++++++");
}
#endregion

#region 测试作用域
{
    using ServiceProvider sp = service.BuildServiceProvider();

    Console.WriteLine("+++++++++++++++++++++++++++测试作用域+++++++++++++++++++++++++++");
    using var serviceScope = sp.CreateScope();
    var myService = serviceScope.ServiceProvider.GetRequiredService<IScopedService>();
    myService.ShowCode();

    Console.WriteLine("--------------------");
    var myService2 = serviceScope.ServiceProvider.GetRequiredService<IScopedService>();
    myService2.ShowCode();

    Console.WriteLine("--------------------");
    using var serviceScope2 = sp.CreateScope();
    var myService3 = serviceScope2.ServiceProvider.GetRequiredService<IScopedService>();
    myService3.ShowCode();
    Console.WriteLine("--------------------");
    Console.WriteLine(myService == myService2);
    Console.WriteLine(myService2 == myService3);
    Console.WriteLine(myService == myService3);
    Console.WriteLine("+++++++++++++++++++++++++++测试作用域+++++++++++++++++++++++++++");
}
#endregion

#region 替换服务
{

    Console.WriteLine("+++++++++++++++++++++++++++替换服务+++++++++++++++++++++++++++");
    service.Replace(ServiceDescriptor.Transient<ITransientService, TransientService>());
    //注意要在获取服务之前替换，否则替换无效
    using ServiceProvider sp = service.BuildServiceProvider();
    using var serviceScope = sp.CreateScope();
    var transientService = serviceScope.ServiceProvider.GetRequiredService<ITransientService>();
    transientService.ShowCode();
    Console.WriteLine("+++++++++++++++++++++++++++替换服务+++++++++++++++++++++++++++");
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

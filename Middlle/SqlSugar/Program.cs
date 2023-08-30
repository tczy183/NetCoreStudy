using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using SqlSugar;

ServiceCollection services = new ServiceCollection();

#region SqlSugarClient需要使用作用域
//注册SqlSugar用AddScoped
services.AddScoped<ISqlSugarClient>(s =>
{
    //Scoped用SqlSugarClient 
    SqlSugarClient sqlSugar = new SqlSugarClient(new ConnectionConfig()
    {
        DbType = SqlSugar.DbType.Sqlite,
        ConnectionString = "DataSource=sqlsugar-dev.db",
        IsAutoCloseConnection = true,
    },
    db =>
    {
        //单例参数配置，所有上下文生效
        db.Aop.OnLogExecuting = (sql, pars) =>
       {
           //获取IOC对象不要求在一个上下文
           //vra log=s.GetService<Log>()

           //获取IOC对象要求在一个上下文
           //var appServive = s.GetService<IHttpContextAccessor>();
           Console.WriteLine("****************************************");
           Console.WriteLine("sql:" + sql);
           Console.WriteLine("pars:" + JsonConvert.SerializeObject(pars));
           Console.WriteLine("****************************************");
       };
    });
    return sqlSugar;
});
#endregion

#region SqlSugarScope需要使用单例
//注册SqlSugar用AddSingleton
//services.AddSingleton<ISqlSugarClient>(s =>
//{
//    SqlSugarScope sqlSugar = new SqlSugarScope(new ConnectionConfig()
//    {
//        DbType = SqlSugar.DbType.Sqlite,
//        ConnectionString = "DataSource=sqlsugar-dev.db",
//        IsAutoCloseConnection = true,
//    },
//   db =>
//   {
//       //单例参数配置，所有上下文生效
//       db.Aop.OnLogExecuting = (sql, pars) =>
//       {
//           //获取IOC对象不要求在一个上下文
//           //vra log=s.GetService<Log>()

//           //获取IOC对象要求在一个上下文
//           //var appServive = s.GetService<IHttpContextAccessor>();
//       };
//   });
//    return sqlSugar;
//});
#endregion

using ServiceProvider serviceProvider = services.BuildServiceProvider();
using var scope = serviceProvider.CreateScope();
try
{
    ISqlSugarClient client = scope.ServiceProvider.GetRequiredService<ISqlSugarClient>();

    client.CodeFirst.InitTables<ULockEntity>();
    client.DbMaintenance.TruncateTable<ULockEntity>();
    //第一次插入ver=0
    var id = client.Insertable(new ULockEntity() { Name = "oldName" }).ExecuteReturnIdentity();
    var result = await client.Queryable<ULockEntity>().FirstAsync(x => x.Id == id);

    result.Name = "newname";
    var rows1 = client.Updateable(result).ExecuteCommandWithOptLock(true);

    result = await client.Queryable<ULockEntity>().FirstAsync(x => x.Id == id);
    result.Name = "newname2";
    var rows2 = client.Updateable(result).ExecuteCommandWithOptLock(true);
    //rows2=0  失败：数据库ver不等于0
}
catch (VersionExceptions ex)
{
    Console.WriteLine(ex.Message);
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}





public class ULockEntity
{
    [SqlSugar.SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public int Id { get; set; }
    public string Name { get; set; }
    [SqlSugar.SugarColumn(IsEnableUpdateVersionValidation = true)]//标识版本字段
    public long Ver { get; set; }

    //支持Guid long string DateTime (不推荐DateTime 时间有精度问题)
    //推荐用 string guid  （使用long要序列化成strting不然前端精度丢失）
}

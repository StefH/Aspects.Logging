# ![Logo](./Resources/logo_32x32.png) Aspects.Logging
Using [Aspect Injector](https://github.com/pamidur/aspect-injector), this projects provides logging aspects for .NET applications. 
It allows you to log method calls, exceptions, and performance metrics without modifying the original code.

Logging is done using the [Microsoft.Extensions.Logging](https://learn.microsoft.com/en-us/dotnet/core/extensions/logging) framework, which is widely used in .NET applications.

Using attributes on class-level or method-level, you can specify logging behavior such as logging before and after method execution, logging exceptions, and measuring performance.

## 📦 Aspects.Logging
[![NuGet Badge](https://img.shields.io/nuget/v/Aspects.Logging)](https://www.nuget.org/packages/Aspects.Logging)<br>

## ⭐ Usage

### Register

``` c#
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .AddJsonFile("appsettings.Development.json", optional: true)
    .Build();

services.RegisterAspectLogging(configuration.GetRequiredSection("AspectLoggingOptions"));

var serviceProvider = services.BuildServiceProvider();
```
 
### Example: logging on class-level

#### Add `LogAspect` attribute to a `class`
``` c#
[LogAspect(LogLevel.Information)]
class Test
{
    public void X()
    {
        Y();
    }

    private void Y()
    {
    }

    public void Throw()
    {
        var t = new TestEx();
        t.Do();
    }

    public Task ThrowAsync()
    {
        throw new AccessViolationException("test async");
    }
}
```

#### Logging Output
``` raw
[13:01:55 INF] PerformanceTest Test.X:f61c5318-14b8-459e-ae23-79fb9e75da19:OnBefore
[13:01:55 INF] PerformanceTest Test.Y:c4830b70-c5f2-4a57-8467-c802b8c662e7:OnBefore
[13:01:55 INF] PerformanceTest Test.Y:c4830b70-c5f2-4a57-8467-c802b8c662e7:OnAfter
[13:01:55 INF] PerformanceTest Test.Y:c4830b70-c5f2-4a57-8467-c802b8c662e7:OnFinally (00:00:00.0007321)
[13:01:55 INF] PerformanceTest Test.X:f61c5318-14b8-459e-ae23-79fb9e75da19:OnAfter
[13:01:55 INF] PerformanceTest Test.X:f61c5318-14b8-459e-ae23-79fb9e75da19:OnFinally (00:00:00.0020414)
[13:01:55 INF] PerformanceTest Test.Throw:7f8c161c-b621-4f26-86c5-ccfb644b7683:OnBefore
[13:01:55 INF] PerformanceTest Test.Throw:7f8c161c-b621-4f26-86c5-ccfb644b7683:OnFinally (00:00:00.0006461)
[13:01:55 FTL] Fatal error - Throw
System.AccessViolationException: TestEx
   at Aspects.Logging.ConsoleApp.TestEx.Do() in C:\dev\GitHub\Aspects.Logging\Aspects.Logging.ConsoleApp\TestLogic.cs:line 115
   at Aspects.Logging.ConsoleApp.Test.__a$_around_Throw_100663310_o() in C:\dev\GitHub\Aspects.Logging\Aspects.Logging.ConsoleApp\TestLogic.cs:line 102
   at Aspects.Logging.ConsoleApp.Test.__a$_around_Throw_100663310_u(Object[])
   at Aspects.Universal.Attributes.BaseUniversalWrapperAttribute.WrapSync[T](Func`2 target, Object[] args, AspectEventArgs eventArgs)
   at Aspects.Logging.Attributes.AbstractLogAspectAttribute.WrapSync[T](Func`2 target, Object[] args, AspectEventArgs eventArgs) in C:\dev\GitHub\Aspects.Logging\Aspects.Logging\Attributes\AbstractLogAspectAttribute.cs:line 77
   at Aspects.Universal.Aspects.BaseUniversalWrapperAspect.BaseHandle(Object instance, Type type, MethodBase method, Func`2 target, String name, Object[] args, Type returnType, Attribute[] triggers)
   at Aspects.Universal.Aspects.MethodWrapperAspect.Handle(Object instance, Type type, MethodBase method, Func`2 target, String name, Object[] args, Type returnType, Attribute[] triggers)
   at Aspects.Logging.ConsoleApp.Test.__a$_around_Throw_100663310_w_0(Object[])
   at Aspects.Logging.ConsoleApp.Test.Throw()
   at Aspects.Logging.ConsoleApp.TestLogic.RunAsync(CancellationToken cancellationToken) in C:\dev\GitHub\Aspects.Logging\Aspects.Logging.ConsoleApp\TestLogic.cs:line 33
```


#### Add `LogAspect` attribute to a `method`
``` c#
[LogAspect(logPoint: LogPoint.Finally)]
public void TestFinally()
{
    Thread.Sleep(100);
}
```

#### Logging Output
``` raw
[13:01:54 DBG] PerformanceTest TestLogic.TestFinally:a53bc7e5-b770-4429-bcda-92a108c6cb93:OnFinally (00:00:00.1087732)
```
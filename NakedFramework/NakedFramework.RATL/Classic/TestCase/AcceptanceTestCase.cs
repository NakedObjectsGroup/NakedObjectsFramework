using NakedFramework.RATL.Classic.Interface;
using NakedFramework.RATL.Classic.NonDocumenting;
using NakedFramework.RATL.TestCase;
using ROSI.Apis;

namespace NakedFramework.RATL.Classic.TestCase;

public abstract class AcceptanceTestCase : BaseRATLNUnitTestCase {
    public ITestObject NewTestObject<T>(string id = "1") {
        var type = typeof(T).FullName!;
        var innerObject = ROSIApi.GetObject(new Uri("http://localhost/"), type, id, TestInvokeOptions()).Result;
        return new TestObject(innerObject);
    }

    public ITestObject GetTestServiceByTypeName(string typeName) {
        try {
            var innerObject = ROSIApi.GetService(new Uri("http://localhost/"), typeName, TestInvokeOptions()).Result;
            return new TestObject(innerObject);
        }
        catch (Exception) {
            Assert.Fail($"No such service: {typeName}");
        }

        return null;
    }

    public ITestObject GetTestService(string title) {
        try {
            var home = ROSIApi.GetHome(new Uri("http://localhost/"), TestInvokeOptions()).Result;
            var services = home.GetServices(home.Options).Result;
            var service = services.GetServiceByTitle(title, home.Options).Result;
            return new TestObject(service);
        }
        catch (Exception) {
            Assert.Fail($"No such service: {title}");
        }

        return null;
    }

    public ITestObject GetTestService(Type t) => GetTestServiceByTypeName(t.FullName ?? "");

    public ITestObject GetTestService<T>() => GetTestServiceByTypeName(FullName<T>() ?? "");
}
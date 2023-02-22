using NakedFramework.RATL.Classic.Interface;
using NakedFramework.RATL.Classic.NonDocumenting;
using NakedFramework.RATL.TestCase;
using ROSI.Apis;
using ROSI.Records;

namespace NakedFramework.RATL.Classic.TestCase;

public abstract class AcceptanceTestCase : BaseRATLNUnitTestCase {

    public ITestObject NewTestObject<T>(string id = "1") {
        var type = typeof(T).FullName!;
        var innerObject = ROSIApi.GetObject(new Uri("http://localhost/"), type, id, TestInvokeOptions()).Result;
        return new TestObject(innerObject, this);
    }

    public ITestObject GetTestService(string id) {
        try {
            var innerObject = ROSIApi.GetService(new Uri("http://localhost/"), id, TestInvokeOptions()).Result;
            return new TestObject(innerObject, this);
        }
        catch (Exception) {
            Assert.Fail($"No such service: {id}");
        }

        return null;
    }

}
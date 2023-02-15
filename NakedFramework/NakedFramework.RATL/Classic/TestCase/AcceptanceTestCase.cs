using NakedFramework.RATL.Classic.Interface;
using NakedFramework.RATL.Classic.NonDocumenting;
using NakedFramework.RATL.TestCase;
using ROSI.Apis;
using ROSI.Records;

namespace NakedFramework.RATL.Classic.TestCase;

public abstract class AcceptanceTestCase : BaseRATLTestCase {

    public ITestObject NewTestObject<T>(string id = "1") {
        var type = typeof(T).FullName!;
        var innerObject = ROSIApi.GetObject(new Uri("http://localhost/"), type, id, TestInvokeOptions()).Result;
        return new TestObject(innerObject);
    }

}
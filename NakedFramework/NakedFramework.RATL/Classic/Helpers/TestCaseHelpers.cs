using NakedFramework.RATL.Classic.Interface;
using NakedFramework.RATL.Classic.NonDocumenting;
using ROSI.Apis;
using ROSI.Records;

namespace NakedFramework.RATL.Classic.Helpers;

public static class TestCaseHelpers {
    public static ITestObject NewTestObject<T>(string key = "1") {
        var name = typeof(T).FullName!;
        var innerObject = ROSIApi.GetObject(new Uri("localhost"), name, key, new InvokeOptions()).Result;
        return new TestObject(innerObject);
    }
}
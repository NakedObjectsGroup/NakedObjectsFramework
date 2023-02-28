using NakedFramework.RATL.Classic.Interface;
using NakedFramework.RATL.Classic.NonDocumenting;
using NakedFramework.RATL.Classic.TestCase;
using ROSI.Apis;
using ROSI.Records;

namespace NakedFramework.RATL.Classic.Helpers;

public static class TestCaseHelpers {
    public static ITestObject GetTestObject(Link l, AcceptanceTestCase tc) => new TestObject(ROSIApi.GetObject(l.GetHref(), tc.TestInvokeOptions()).Result, tc);
}
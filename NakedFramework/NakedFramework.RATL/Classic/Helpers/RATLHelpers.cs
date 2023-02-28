using NakedFramework.RATL.Classic.Interface;
using NakedFramework.RATL.Classic.NonDocumenting;
using NakedFramework.RATL.Classic.TestCase;
using ROSI.Apis;
using ROSI.Interfaces;
using ROSI.Records;

namespace NakedFramework.RATL.Classic.Helpers;

public static class RATLHelpers {
    public static ITestObject GetTestObject(Link l, AcceptanceTestCase tc) => new TestObject(ROSIApi.GetObject(l.GetHref(), tc.TestInvokeOptions()).Result, tc);

    public static Type GetType(string typeName) =>
        AppDomain.CurrentDomain.GetAssemblies().Select(assembly => assembly.GetType(typeName)).FirstOrDefault(type => type is not null);

    public static ITestNaked[] GetChoices(IHasChoices hasChoices, AcceptanceTestCase tc) {
        var valueChoices = hasChoices.GetChoices().ToArray();
        if (valueChoices?.Any() == true) {
            return valueChoices.Select(v => new TestValue(v)).Cast<ITestNaked>().ToArray();
        }

        return hasChoices.GetLinkChoices().Select(l => RATLHelpers.GetTestObject(l, tc)).Cast<ITestNaked>().ToArray();
    }
}
using NakedFramework.RATL.Classic.Interface;
using NakedFramework.RATL.Classic.NonDocumenting;
using ROSI.Apis;
using ROSI.Interfaces;
using ROSI.Records;

namespace NakedFramework.RATL.Classic.Helpers;

public static class RATLHelpers {
    public static ITestObject GetTestObject(Link l) => new TestObject(ROSIApi.GetObject(l.GetHref(), l.Options).Result);

    public static Type GetType(string typeName) =>
        AppDomain.CurrentDomain.GetAssemblies().Select(assembly => assembly.GetType(typeName)).FirstOrDefault(type => type is not null);

    private static ITestNaked GetChoiceValue(IHasExtensions hasExts, bool hasChoices, object dflt) {
        if (hasChoices) {
            var choices = hasExts.GetExtensions().GetExtension<Dictionary<string, object>>(ExtensionsApi.ExtensionKeys.x_ro_nof_choices);
            var title = choices.SingleOrDefault(kvp => kvp.Value.Equals(dflt)).Key;
            return title is null ? new TestValue(dflt) : new TestChoice(dflt, title);
        }

        return new TestValue(dflt);
    }

    public static ITestNaked GetChoiceValue(IHasChoices hasChoices, object dflt) => GetChoiceValue((IHasExtensions)hasChoices, hasChoices.GetChoices().Any(), dflt);

    public static ITestNaked GetChoiceValue(PropertyMember property, object dflt) => GetChoiceValue(property, property.GetChoices().Result.Any(), dflt);

    public static ITestNaked[] GetChoices(IHasChoices hasChoices) {
        var valueChoices = hasChoices.GetChoices().ToArray();
        if (valueChoices?.Any() == true) {
            return valueChoices.Select(v => GetChoiceValue(hasChoices, v)).ToArray();
        }

        return hasChoices.GetLinkChoices().Select(l => GetTestObject(l)).Cast<ITestNaked>().ToArray();
    }
}
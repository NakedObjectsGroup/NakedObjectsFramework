using ROSI.Apis;
using ROSI.Interfaces;
using ROSI.Records;

namespace NakedFramework.RATL.Extensions;

public static class Asserts {
    public static Menus AssertMenuOrder(this Menus? menus, params string[] menusNames) {
        Assert.IsNotNull(menus);
        var names = menus.GetValue().Select(l => l.GetTitle()).ToArray();

        Assert.AreEqual(menusNames.Length, names.Count(), "Menu count does not match names count");

        var zip = menusNames.Zip(names);

        foreach (var (first, second) in zip) {
            Assert.AreEqual(first, second);
        }

        return menus;
    }

    public static DomainObject AssertMemberOrder(this DomainObject? obj, params string[] memberNames) {
        Assert.IsNotNull(obj);
        var names = obj.GetActions().Select(a => a.GetId()).ToArray();

        Assert.AreEqual(memberNames.Length, names.Count(), "member count does not match names count");

        var zip = memberNames.Zip(names);

        foreach (var (first, second) in zip) {
            Assert.AreEqual(first, second);
        }

        return obj;
    }

    public static ActionMember AssertNumberOfParameters(this ActionMember? am, int numberOfParameters) {
        Assert.IsNotNull(am);
        Assert.AreEqual(numberOfParameters, am.GetParameters().Result.Parameters().Count(), "Unexpected number of parameters");
        return am;
    }

    public static ActionMember AssertValid(this ActionMember? am, params object[] pp) {
        Assert.IsNotNull(am);
        am.Validate(pp).Wait();
        return am;
    }

    public static ActionMember AssertReturnsList(this ActionMember? am) {
        Assert.IsNotNull(am);
        Assert.AreEqual("list", am.SafeGetExtension(ExtensionsApi.ExtensionKeys.returnType));
        return am;
    }

    public static T AssertName<T>(this T? hasExtensions, string name) where T : IHasExtensions {
        Assert.IsNotNull(hasExtensions);
        Assert.AreEqual(name, hasExtensions.SafeGetExtension(ExtensionsApi.ExtensionKeys.friendlyName)?.ToString());
        return hasExtensions;
    }

    public static T AssertOptional<T>(this T? hasExtensions) where T : IHasExtensions {
        Assert.IsNotNull(hasExtensions);
        Assert.AreEqual(true, hasExtensions.SafeGetExtension(ExtensionsApi.ExtensionKeys.optional));
        return hasExtensions;
    }

    public static T AssertChoice<T>(this T? hasChoices, int index, string value) where T : IHasChoices {
        Assert.IsNotNull(hasChoices);
        var choices = hasChoices.GetChoices().ToArray();
        Assert.AreEqual(value, choices[index]);
        return hasChoices;
    }

    public static Parameter AssertDefault(this Parameter parameter, string? value) {
        Assert.IsNotNull(parameter);
        Assert.AreEqual(value, parameter.GetDefault());
        return parameter;
    }

    public static T AssertType<T, TU>(this T? hasExtensions) where T : IHasExtensions {
        Assert.IsNotNull(hasExtensions);
        Assert.AreEqual(typeof(TU).FullName, hasExtensions.SafeGetExtension(ExtensionsApi.ExtensionKeys.returnType)?.ToString());
        return hasExtensions;
    }

    public static T AssertType<T>(this T? hasExtensions, string name) where T : IHasExtensions {
        Assert.IsNotNull(hasExtensions);
        Assert.AreEqual(name, hasExtensions.SafeGetExtension(ExtensionsApi.ExtensionKeys.returnType)?.ToString());
        return hasExtensions;
    }

    public static T AssertString<T>(this T? hasExtensions) where T : IHasExtensions {
        Assert.IsNotNull(hasExtensions);
        Assert.AreEqual("string", hasExtensions.SafeGetExtension(ExtensionsApi.ExtensionKeys.returnType)?.ToString());
        Assert.AreEqual("string", hasExtensions.SafeGetExtension(ExtensionsApi.ExtensionKeys.format)?.ToString());
        return hasExtensions;
    }
}
using System.Linq;
using System.Reflection;
using NakedLegacy;

namespace NakedLegacy.Reflector.Facet;

public static class AboutHelpers {
    public enum AboutType {
        Action,
        Field
    }

    public static object[] GetParameters(this MethodInfo method, object about) {
        var aboutParam = new[] { about };
        var placeholders = new object[method.GetParameters().Length - 1];
        return aboutParam.Union(placeholders).ToArray();
    }

    public static IAbout AboutFactory(this AboutType aboutType, AboutTypeCodes aboutTypeCode) =>
        aboutType is AboutType.Action
            ? new ActionAboutImpl(aboutTypeCode)
            : new FieldAboutImpl(aboutTypeCode);
}
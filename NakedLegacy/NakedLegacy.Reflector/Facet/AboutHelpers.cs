using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NakedLegacy;

namespace NakedLegacy.Reflector.Facet;

public static class AboutHelpers {
    public enum AboutType {
        Action,
        Field
    }

    public static object[] GetParameters(this MethodInfo method, object about, params object[] proposedValues) {
        var parameters = new List<object> { about };
        var placeholders = new object[method.GetParameters().Length - 1];
        if (proposedValues?.Any() == true) {
            placeholders = proposedValues;
        }

        parameters.AddRange(placeholders);

        return parameters.ToArray();
    }

    public static IAbout AboutFactory(this AboutType aboutType, AboutTypeCodes aboutTypeCode) =>
        aboutType is AboutType.Action
            ? new ActionAboutImpl(aboutTypeCode)
            : new FieldAboutImpl(aboutTypeCode);

}
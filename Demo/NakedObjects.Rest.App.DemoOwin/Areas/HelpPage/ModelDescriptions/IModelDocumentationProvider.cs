using System;
using System.Reflection;

namespace NakedObjects.Rest.App.DemoOwin.Areas.HelpPage.ModelDescriptions
{
    public interface IModelDocumentationProvider
    {
        string GetDocumentation(MemberInfo member);

        string GetDocumentation(Type type);
    }
}
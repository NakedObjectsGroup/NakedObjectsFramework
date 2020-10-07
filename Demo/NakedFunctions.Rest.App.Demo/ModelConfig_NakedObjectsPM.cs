using AdventureWorksModel;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.Entity;

namespace NakedObjects.Rest.App.Demo
{
    public class ModelConfig_NakedObjectsPM
    {
        public static Func<IConfiguration, DbContext> ContextInstaller => c => new AdventureWorksContext(c.GetConnectionString("AdventureWorksContext"));

        public static string[] ModelNamespaces() => new[]
        {
            "AdventureWorksModel"
        };

        public static List<Type> DomainTypes() => new List<Type>
        {
        };

        public static IList<(string name, Type rootType)> MainMenus() => new List<(string, Type)>
        {

        };

        public static List<Type> Services() => new List<Type> { };
    }
}

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace AW {
    public static class AWModelConfig {
        private static IEnumerable<Type> DomainClasses =>
            typeof(AWModelConfig).Assembly.GetTypes().Where(t => t.IsPublic && (t.IsClass || t.IsInterface || t.IsEnum));

        public static Func<IConfiguration, DbContext> DbContextCreator => 
            c => new AdventureWorksContext(c.GetConnectionString("AdventureWorksContext"));

        //IsAbstract && IsSealed defines a static class. Not really necessary here, just extra safety check.
        public static Type[] FunctionalTypes() =>
            DomainClasses.Where(t => t.Namespace == "AW.Types" && !(t.IsAbstract && t.IsSealed)).ToArray();

        public static Type[] Functions() =>
            DomainClasses.Where(t => t.Namespace == "AW.Functions" && t.IsAbstract && t.IsSealed).ToArray();

        public static Type[] MainMenuTypes() =>
            Functions().Where(t => t.FullName?.Contains("MenuFunctions") == true).ToArray();
    }
}
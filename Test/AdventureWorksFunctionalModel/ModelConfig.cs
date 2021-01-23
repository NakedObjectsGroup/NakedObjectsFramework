using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using NakedFramework;
using System.Data.Entity;

namespace AW
{
    public static class ModelConfig
    {
        //IsAbstract && IsSealed defines a static class. Not really necessary here, just extra safety check.
        public static Type[] FunctionalTypes() => 
          DomainClasses.Where(t => t.Namespace == "AW.Types" && !(t.IsAbstract && t.IsSealed)).ToArray();

        public static Type[] Functions() =>
          DomainClasses.Where(t => t.Namespace == "AW.Functions"   && t.IsAbstract && t.IsSealed).ToArray();

        private static IEnumerable<Type> DomainClasses =>
            typeof(ModelConfig).Assembly.GetTypes().Where(t => t.IsPublic && (t.IsClass || t.IsInterface || t.IsEnum));


        public static Func<IConfiguration, DbContext> DbContextInstaller => c => new AdventureWorksContext(c.GetConnectionString("AdventureWorksContext"));

        //TODO: Delete when new NF.PM Helpers is available
        public static IMenu[] MainMenus(IMenuFactory mf) =>
             MainMenuTypes().Select(t => mf.NewMenu(t, true)).ToArray();


        public static Type[] MainMenuTypes() =>
            Functions().Where(t => t.FullName.Contains("MenuFunctions")).ToArray();

    }
}

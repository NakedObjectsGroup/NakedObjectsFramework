using Microsoft.Extensions.Configuration;
using NakedObjects.Menu; //TODO: Replace with NakedFramework version when working.
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace AW
{
    public static class ModelConfig
    {
        //IsAbstract && IsSealed tests for a static class. Not really necessary here, just extra safety check.
        public static Type[] FunctionalTypes() => 
          Classes.Where(t => t.Namespace == "AW.Types" && !(t.IsAbstract && t.IsSealed)).ToArray();

        public static Type[] Functions() =>
          Classes.Where(t => t.Namespace == "AW.Functions" && t.IsAbstract && t.IsSealed).ToArray();

        private static IEnumerable<Type> Classes =>
            typeof(ModelConfig).Assembly.GetTypes().Where(t => t.IsClass);

        public static IMenu[] MainMenus(IMenuFactory mf) =>
            Functions().Where(t => t.FullName.Contains("MenuFunctions")).Select(t => mf.NewMenu(t)).ToArray();

        public static Func<IConfiguration, DbContext> DbContextInstaller => c => new AdventureWorksContext(c.GetConnectionString("AdventureWorksContext"));

    }
}

using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AW
{
    public static class AWModelConfig
    {
        //IsAbstract && IsSealed defines a static class. Not really necessary here, just extra safety check.
        public static Type[] FunctionalTypes() => 
          DomainClasses.Where(t => t.Namespace == "AW.Types" && !(t.IsAbstract && t.IsSealed)).ToArray();

        public static Type[] Functions() =>
          DomainClasses.Where(t => t.Namespace == "AW.Functions"   && t.IsAbstract && t.IsSealed).ToArray();

        private static IEnumerable<Type> DomainClasses =>
            typeof(AWModelConfig).Assembly.GetTypes().Where(t => t.IsPublic && (t.IsClass || t.IsInterface || t.IsEnum));


        public static Func<IConfiguration, System.Data.Entity.DbContext> DbContextCreator => c => new AdventureWorksContext(c.GetConnectionString("AdventureWorksContext"));

        public static Func<IConfiguration, Microsoft.EntityFrameworkCore.DbContext> EFCDbContextCreator => c => {
            
            var cc = new AdventureWorksEFCoreContext(c.GetConnectionString("AdventureWorksContext"));

            return cc;
        };

        public static Type[] MainMenuTypes() =>
            Functions().Where(t => t.FullName.Contains("MenuFunctions")).ToArray();

    }
}

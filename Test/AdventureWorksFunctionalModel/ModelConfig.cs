using NakedObjects.Menu; //TODO: Replace with NakedFramework version when working.
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace AW
{
    public static class ModelConfig
    {
        public static Type[] EntityTypes() => 
          Classes.Where(t => t.Namespace == "AW.Types").ToArray();

        public static Type[] FunctionTypes() =>
          Classes.Where(t => t.Namespace == "AW.Functions" && t.IsAbstract && t.IsSealed).ToArray();
        //IsAbstract && IsSealed tests for a static class. Not really necessary here, just extra safety check.

        private static IEnumerable<Type> Classes =>
            typeof(ModelConfig).Assembly.GetTypes().Where(t => t.IsClass);

        public static IMenu[] MainMenus(IMenuFactory mf) =>
            FunctionTypes().Where(t => t.FullName.Contains("MenuFunctions")).Select(t => mf.NewMenu(t)).ToArray();
    }
}

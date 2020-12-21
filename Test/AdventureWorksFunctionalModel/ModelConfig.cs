using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventureWorksModel
{
    public static class ModelConfig
    {
        public static Type[] EntityTypes() => 
          Classes.Where(t => !t.IsSealed).ToArray();


        public static Type[] FunctionTypes() =>
          Classes.Where(t => t.IsAbstract && t.IsSealed && t.Name.EndsWith("Functions")).ToArray();

        private static IEnumerable<Type> Classes =>
            typeof(Department).Assembly.GetTypes().Where(t => t.IsClass);


        //TODO: Main menus?
    }
}

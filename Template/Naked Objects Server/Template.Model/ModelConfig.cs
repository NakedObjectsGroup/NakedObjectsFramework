using Microsoft.Extensions.Configuration;
using System;

namespace Template.Model
{
    //A helper class to provide model configuration for use by Startup.cs in the Server project.
    //The implementation here relies on the conventions that:
    //- All domain classes are defined in namespace "Template.Model.Types"
    //- All domain functions are defined on static types in namespace "Template.Model.Functions"
    //- All main menu function are defined on static types that have 'MenuFunctions' in their name
    //This ModelConfig may be re-written to change the conventions, or to remove conventions altogether, and
    //specify the lists of types, functions, and menus explicitly.
    public static class ModelConfig
    {
        public static Type[] Types() => new[] { typeof(Student) };


        public static Type[] Services() => new[] { typeof(ExampleService) };

        public static Type[] MainMenus() => new[] { typeof(ExampleService) };


        public static Func<IConfiguration, Microsoft.EntityFrameworkCore.DbContext> EFCoreDbContextInstaller => 
            c => new ExampleDbContext(c.GetConnectionString("ExampleConnectionString"));
    }
}

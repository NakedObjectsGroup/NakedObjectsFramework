using AW;
using Microsoft.Extensions.Configuration;
using NakedObjects.Menu;
using System;
using System.Data.Entity;
using System.Linq;

namespace NakedFunctions.Rest.App.Demo
{
    public static class ModelConfig
    {
        public static Func<IConfiguration, DbContext> DbContextInstaller => c => new AdventureWorksContext(c.GetConnectionString("AdventureWorksContext"));

        public static Type[] DomainTypes() => AW.ModelConfig.EntityTypes();

        public static Type[] DomainFunctions() => AW.ModelConfig.FunctionTypes();

        public static IMenu[] MainMenus(IMenuFactory mf) => AW.ModelConfig.MainMenus(mf);
    }
}

using System;
using System.Data.Entity;
using Microsoft.Extensions.Configuration;
using NakedFramework.Menu;

namespace Template.RestTest.DomainModel
{
    public static class ModelConfig {
        public static Type[] Functions { get; } = {
            typeof(FooFunctions),
            typeof(BarMenu)
        };

        public static Type[] Records { get; } = {
            typeof(Foo)
        };

        public static Func<IConfiguration, DbContext>[] ContextInstallers =>
            new Func<IConfiguration, DbContext>[] {
                c => new ObjectDbContext(c.GetConnectionString("Spike"))
            };

        public static IMenu[] MainMenus(IMenuFactory factory) => new[] {factory.NewMenu(typeof(BarMenu), true, nameof(BarMenu))};
    }
}
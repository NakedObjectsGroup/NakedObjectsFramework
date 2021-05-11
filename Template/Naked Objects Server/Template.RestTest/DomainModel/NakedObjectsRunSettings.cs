using System;
using System.Data.Entity;
using Microsoft.Extensions.Configuration;
using NakedFramework.Menu;

namespace Template.Test.Data {
    public static class NakedObjectsRunSettings {
        public static Type[] Services { get; } = {
            typeof(BarService)
        };

        public static Type[] Types { get; } = {
            typeof(Foo)
        };

        public static Func<IConfiguration, DbContext>[] ContextInstallers =>
            new Func<IConfiguration, DbContext>[] {
                c => new ObjectDbContext(c.GetConnectionString("Spike"))
            };

        public static IMenu[] MainMenus(IMenuFactory factory) => new[] {factory.NewMenu<BarService>(true, "Bars")};
    }
}
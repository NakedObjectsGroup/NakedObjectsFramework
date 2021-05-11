using System;
using System.Data.Entity;
using Microsoft.Extensions.Configuration;

namespace Template.Test.Data {
    public static class DataSetup {
        public static Type[] Functions { get; } = {
            typeof(SimpleRecordFunctions)
        };

        public static Type[] Records { get; } = {
            typeof(SimpleRecord)
        };

        public static Func<IConfiguration, DbContext>[] ContextInstallers =>
            new Func<IConfiguration, DbContext>[] {
                c => new ObjectDbContext(c.GetConnectionString("Spike"))
            };
    }
}
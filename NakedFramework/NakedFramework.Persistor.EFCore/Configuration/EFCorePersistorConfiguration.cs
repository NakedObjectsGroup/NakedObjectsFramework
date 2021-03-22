using System;
using Microsoft.EntityFrameworkCore;

namespace NakedFramework.Persistor.EFCore.Configuration {
    public class EFCorePersistorConfiguration {
        public Func<DbContext> Context { get; set; }
        public int MaximumCommitCycles { get; set; }

        public Type[] PreCachedTypes() => Array.Empty<Type>();

        public Type[] NotPersistedTypes() => Array.Empty<Type>();
    }
}
using System;
using Microsoft.EntityFrameworkCore;

namespace NakedFramework.Persistor.EFCore.Configuration {
    public class EFCorePersistorConfiguration {
        public Func<DbContext>[] Contexts { get; set; }

        public int MaximumCommitCycles { get; init; }

        public Type[] PreCachedTypes() => Array.Empty<Type>();

        public Type[] NotPersistedTypes() => Array.Empty<Type>();
    }
}
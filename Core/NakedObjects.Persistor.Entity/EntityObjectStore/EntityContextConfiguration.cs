// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;

namespace NakedObjects.EntityObjectStore {
    public abstract class EntityContextConfiguration {
        private Func<Type[]> notPersistedTypes = () => new Type[] {};
        private Func<Type[]> preCachedTypes = () => new Type[] {};
        internal bool Validated { get; set; }
        public MergeOption DefaultMergeOption { get; set; }

        public Func<Type[]> PreCachedTypes {
            get { return preCachedTypes; }
            set { preCachedTypes = value; }
        }

        public Func<Type[]> NotPersistedTypes {
            get { return notPersistedTypes; }
            set { notPersistedTypes = value; }
        }
    }

    public class PocoEntityContextConfiguration : EntityContextConfiguration {
        public string ContextName { get; set; }
    }

    public class CodeFirstEntityContextConfiguration : EntityContextConfiguration {
        public Func<DbContext> DbContext { get; set; }
    }
}
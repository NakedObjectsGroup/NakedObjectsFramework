// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;

namespace NakedObjects.Persistor.Entity.Configuration {
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
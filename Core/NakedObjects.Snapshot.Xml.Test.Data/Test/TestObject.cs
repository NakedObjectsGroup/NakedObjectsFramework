// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using NakedObjects;

namespace Snapshot.Xml.Test {
    public class TestObjectContext : DbContext {
        public TestObjectContext(string name) : base(name) {}
        public DbSet<TestObject> TestObjects { get; set; }
    }

    public class TestObject {
        private IList<TestObject> testCollection = new List<TestObject>();

        [Key]
        [Hidden(WhenTo.Always)]
        public virtual int TestObjectId { get; set; }

        //Add properties with 'propv', collections with 'coll', actions with 'act' shortcuts

        public virtual int TestInt { get; set; }
        public virtual string TestString { get; set; }
        public virtual TestObject TestReference { get; set; }

        public virtual IList<TestObject> TestCollection {
            get { return testCollection; }
            set { testCollection = value; }
        }

        #region Injected Services

        // This region should contain properties to hold references to any services required by the
        // object.  Use the 'injs' shortcut to add a new service; 'injc' to add an injected Container

        #endregion

        #region Life Cycle Methods

        // This region should contain any of the 'life cycle' convention methods (such as
        // Created(), Persisted() etc) called by the framework at specified stages in the lifecycle.

        #endregion
    }
}
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
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local

namespace Snapshot.Xml.Test {
    public class TestObjectContext : DbContext {
        public TestObjectContext(string name) : base(name) { }
        public DbSet<TestObject> TestObjects { get; set; }
    }

    public class TestObject {
        [Key]
        [Hidden(WhenTo.Always)]
        public virtual int TestObjectId { get; set; }

        public virtual int TestInt { get; set; }
        public virtual string TestString { get; set; }
        public virtual TestObject TestReference { get; set; }

        public virtual IList<TestObject> TestCollection { get; set; } = new List<TestObject>();
    }
}
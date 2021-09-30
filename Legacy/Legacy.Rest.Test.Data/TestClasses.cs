// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Legacy.NakedObjects.Application.Collection;
using Legacy.NakedObjects.Application.ValueHolder;
using NakedObjects;

namespace Legacy.Rest.Test.Data {
    public class SimpleService {
        public ClassWithTextString GetClassWithTextString() => null;
    }

    public class ClassWithTextString {
        private TextString _name;
        private string name;

        [Key]
        public int Id { get; init; }

        public TextString Name => _name ??= new TextString(name) { BackingField = s => name = s };

        public ClassWithTextString ActionUpdateName(TextString newName) {
            Name.setValue(newName);
            return this;
        }
    }

    public class ClassWithInternalCollection {
        [NakedObjectsIgnore]
        public virtual ICollection<ClassWithTextString> _TestCollection { get; } = new List<ClassWithTextString>();

        private InternalCollection _testCollection;

        [Key]
        public int Id { get; init; }

        public InternalCollection TestCollection {
            get {
                if (_testCollection is null) {
                    _testCollection = new InternalCollection(typeof(ClassWithTextString).ToString());
                    _testCollection.init(_TestCollection.ToArray() as object[]);
                }

                return _testCollection;
            }
        }
    }
}
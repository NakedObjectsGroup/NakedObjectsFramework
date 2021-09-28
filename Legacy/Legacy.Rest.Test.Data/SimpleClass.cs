// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.ComponentModel.DataAnnotations;
using Legacy.NakedObjects.Application.ValueHolder;

namespace Legacy.Rest.Test.Data {
    public class SimpleService {
        public SimpleClass GetSimpleClass() => null;
    }

    public class SimpleClass {
        private TextString _name;

#pragma warning disable 649
        private string name;
#pragma warning restore 649

        [Key]
        public int Id { get; init; }

        public TextString Name => _name ??= new TextString(name) { BackingField = s => name = s };

        public SimpleClass ActionUpdateName(TextString newName) {
            Name.setValue(newName);
            return this;
        }
    }
}
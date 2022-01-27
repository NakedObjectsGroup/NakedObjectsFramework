// // Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// // Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// // You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// // Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// // WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// // See the License for the specific language governing permissions and limitations under the License.

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NakedObjects;

namespace NOF2.Rest.Test.Data;

public interface IRoleInterface { }

public class SimpleNOService {
    public ClassWithString GetClassWithString() => null;

    public void ContributedAction([ContributedAction] NOF2ClassWithInterface contributee) { }
}

public class ClassWithString {
    [Key]
    public int Id { get; init; }

    public string Name { get; set; }

    public virtual ClassWithTextString LinkToNOF2Class { get; set; }

    public virtual ICollection<ClassWithTextString> CollectionOfNOF2Class { get; set; } = new List<ClassWithTextString>();
}

public class ClassWithNOF2Interface : INOF2RoleInterface {
    [Key]
    public int Id { get; init; }
}
﻿// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.ComponentModel.DataAnnotations;
using NakedObjects;

namespace Template.RestTest.DomainModel
{
    [PresentationHint("Hint1")]
    public class Foo {
        [Key]
        public virtual int Id { get; set; }

        [PresentationHint("Hint2")]
        public virtual string Name { get; set; }

        public void ResetName() => Name = "New Name";

        public void UpdateName(string name) => Name = name;

        public void UpdateNameFrom(Foo from)
        {
            Name = from.Name;
        }

        public override string ToString() => Name;
    }
}
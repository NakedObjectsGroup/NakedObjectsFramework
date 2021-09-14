// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedFunctions;

namespace AW.Types {
    [Bounded]
    public record ProductSubcategory : IHasRowGuid, IHasModifiedDate {
        [Hidden]
        public virtual int ProductSubcategoryID { get; init; }

        public virtual string Name { get; init; } = "";

        [Hidden]
        public virtual int ProductCategoryID { get; init; }

#pragma warning disable 8618
        public virtual ProductCategory ProductCategory { get; init; }
#pragma warning restore 8618

        public virtual bool Equals(ProductSubcategory? other) => ReferenceEquals(this, other);

        [MemberOrder(99)]
        [Versioned]
        public virtual DateTime ModifiedDate { get; init; }

        [Hidden]
        public virtual Guid rowguid { get; init; }

        public override string ToString() => Name;

        public override int GetHashCode() => base.GetHashCode();
    }
}
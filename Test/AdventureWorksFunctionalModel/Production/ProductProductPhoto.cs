// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedFunctions;
using static AW.Utilities;

namespace AW.Types {
    public record ProductProductPhoto  {

        public virtual int ProductID { get; init; }
        public virtual int ProductPhotoID { get; init; }
        public virtual bool Primary { get; init; }
        public virtual Product Product { get; init; }
        public virtual ProductPhoto ProductPhoto { get; init; }

        [MemberOrder(99)]
        [Versioned]
		public virtual DateTime ModifiedDate { get; init; }

        public override string ToString() => $"ProductProductPhoto: {ProductID}-{ProductPhotoID}";

		public override int GetHashCode() => HashCode(this, ProductID, ProductPhotoID);

        public virtual bool Equals(ProductProductPhoto other) => ReferenceEquals(this, other);
    }
}
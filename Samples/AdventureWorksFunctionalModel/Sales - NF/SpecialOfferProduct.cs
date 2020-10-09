// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.ComponentModel.DataAnnotations;
using NakedFunctions;

namespace AdventureWorksModel
{
    public record SpecialOfferProduct
    {

        [NakedObjectsIgnore]
        public virtual int SpecialOfferID { get; init; }

        [NakedObjectsIgnore]
        public virtual int ProductID { get; init; }

        [MemberOrder(1)]
        public virtual SpecialOffer SpecialOffer { get; init; }

        [MemberOrder(2)]
        public virtual Product Product { get; init; }

        [NakedObjectsIgnore]
        public virtual Guid rowguid { get; init; }

        [MemberOrder(99), ConcurrencyCheck]
        public virtual DateTime ModifiedDate { get; init; }

        public override string ToString() => "Special offer - title TODO";
    }

    public static class SpecialOfferProductFunctions
    {
        public static Product Updating(Product p, [Injected] DateTime now) => p with { ModifiedDate = now };
        public static Product Persisting(Product p, [Injected] DateTime now, [Injected] Guid guid) => p with { rowguid = guid, ModifiedDate = now };

    }

}
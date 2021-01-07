// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.ComponentModel.DataAnnotations;
using NakedFunctions;

namespace AW.Types {
    public record TransactionHistory  {

        public virtual int TransactionID { get; init; }
        public virtual int ReferenceOrderID { get; init; }
        public virtual int ReferenceOrderLineID { get; init; }
        public virtual DateTime TransactionDate { get; init; }
        public virtual string TransactionType { get; init; }
        public virtual int Quantity { get; init; }
        public virtual decimal ActualCost { get; init; }

        [Hidden]
        public virtual int ProductID { get; init; }
        public Product Product { get; init; }

        [MemberOrder(99), ConcurrencyCheck]
        public virtual DateTime ModifiedDate { get; init; }
    }
}
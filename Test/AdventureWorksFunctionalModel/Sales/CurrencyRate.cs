// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedFunctions;

namespace AW.Types {
    public record CurrencyRate {
        [Hidden]
        public int CurrencyRateID { get; init; }

        public DateTime CurrencyRateDate { get; init; }

        public decimal AverageRate { get; init; }

        public decimal EndOfDayRate { get; init; }

        [Hidden]
        public string FromCurrencyCode { get; init; } = "";

#pragma warning disable 8618
        public virtual Currency Currency { get; init; }
#pragma warning restore 8618

        [Hidden]
        public string ToCurrencyCode { get; init; } = "";

#pragma warning disable 8618
        public virtual Currency Currency1 { get; init; }
#pragma warning restore 8618

        [MemberOrder(99)] [Versioned]
        public DateTime ModifiedDate { get; init; }

        public virtual bool Equals(CurrencyRate? other) => ReferenceEquals(this, other);

        public override string ToString() => $"{AverageRate}";

        public override int GetHashCode() => base.GetHashCode();
    }
}
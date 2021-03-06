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
        public virtual int CurrencyRateID { get; init; }

        public virtual DateTime CurrencyRateDate { get; init; }

        public virtual decimal AverageRate { get; init; }

        public virtual decimal EndOfDayRate { get; init; }

        [Hidden]
        public virtual string FromCurrencyCode { get; init; }
        public virtual Currency Currency { get; init; }

        [Hidden]
        public virtual string ToCurrencyCode { get; init; }

        public virtual Currency Currency1 { get; init; }

        [MemberOrder(99)][Versioned]
		public virtual DateTime ModifiedDate { get; init; }

        public override string ToString() => $"{AverageRate}";

		public override int GetHashCode() =>base.GetHashCode();

        public virtual bool Equals(CurrencyRate other) => ReferenceEquals(this, other);
    }
}
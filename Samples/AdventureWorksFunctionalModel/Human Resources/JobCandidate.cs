// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.ComponentModel.DataAnnotations;
using NakedFunctions;

namespace AdventureWorksModel {
    public record JobCandidate  {
        public virtual int JobCandidateID { get; init; }
        public virtual string Resume { get; init; }

        #region Employee
        [Hidden]
        public virtual int? EmployeeID { get; init; }
        public Employee Employee { get; init; }
        #endregion

        [MemberOrder(99)]
        
        [ConcurrencyCheck]
        public virtual DateTime ModifiedDate { get; init; }

        public override string ToString() => $"{Employee}";
    }

    public static class JobCandidateFunctions
    {
        #region Life Cycle Methods
        public static JobCandidate Updating(this JobCandidate x, [Injected] DateTime now) => x with { ModifiedDate = now };

        public static JobCandidate Persisting(this JobCandidate x, [Injected] DateTime now) => x with { ModifiedDate = now };
        #endregion
    }

}
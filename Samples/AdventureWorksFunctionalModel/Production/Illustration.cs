// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using NakedFunctions;

namespace AdventureWorksModel {
    public record Illustration  {

        public Illustration(
            int illustrationID,
            string diagram,
            ICollection<ProductModelIllustration> productModelIllustration,
            DateTime modifiedDate
            )
        {
            IllustrationID = illustrationID;
            Diagram = diagram;
            ProductModelIllustration = productModelIllustration;
            ModifiedDate = modifiedDate;
        }

        public Illustration() { }
 
        public virtual int IllustrationID { get; init; }
        public virtual string Diagram { get; init; }

        public ICollection<ProductModelIllustration> ProductModelIllustration { get; init; }

        [MemberOrder(99)]
        
        [ConcurrencyCheck]
        public virtual DateTime ModifiedDate { get; init; }
    }

    public static class IllustrationFunctions
    {
        public static Illustration Updating(Illustration ill, [Injected] DateTime now)
        {
            return ill with {ModifiedDate =  now};
        }
    }
}
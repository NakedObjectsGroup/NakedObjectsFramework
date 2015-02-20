// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MvcTestApp.Tests.Helpers {
    [ComplexType]
    public class InlineTestClass {
        public virtual string SubProp1 { get; set; }
        public virtual int SubProp2 { get; set; }

        public virtual IList<string> ChoicesSubProp1(int subProp2) {
            return new[] {"1", "2"};
        }

        public virtual string ValidateSubProp2(int val) {
            return null;
        }

        public virtual void AnAction(string aValue) {}
    }
}
// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NakedObjects;

namespace MvcTestApp.Tests.Helpers {
    [Bounded]
    public class AutoCompleteTestClass {
        [Hidden, Key]
        public int Id { get; set; }

        public IDomainObjectContainer Container { protected get; set; }

        [Title]
        public string Name { get; set; }

        public DescribedCustomHelperTestClass TestAutoCompleteProperty { get; set; }

        public string TestAutoCompleteStringProperty { get; set; }

        public IQueryable<DescribedCustomHelperTestClass> AutoCompleteTestAutoCompleteProperty(string name) {
            return Container.Instances<DescribedCustomHelperTestClass>();
        }

        [PageSize(10)]
        public IQueryable<string> AutoCompleteTestAutoCompleteStringProperty([MinLength(3)] string name) {
            return new List<string> {"test1", "test2"}.AsQueryable();
        }

        public void TestAutoCompleteAction([FindMenu] DescribedCustomHelperTestClass parm1, string parm2) {}

        public IQueryable<DescribedCustomHelperTestClass> AutoComplete0TestAutoCompleteAction(string name) {
            return Container.Instances<DescribedCustomHelperTestClass>();
        }

        [PageSize(5)]
        public IQueryable<string> AutoComplete1TestAutoCompleteAction([MinLength(2)] string name) {
            return new List<string> {"test1", "test2"}.AsQueryable();
        }
    }
}
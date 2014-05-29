// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NakedObjects;

namespace MvcTestApp.Tests.Helpers {
    [Bounded]
    public class AutoCompleteTestClass {
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

        public void TestAutoCompleteAction(DescribedCustomHelperTestClass parm1, string parm2) {}

        public IQueryable<DescribedCustomHelperTestClass> AutoComplete0TestAutoCompleteAction(string name) {
            return Container.Instances<DescribedCustomHelperTestClass>();
        }

        [PageSize(5)]
        public IQueryable<string> AutoComplete1TestAutoCompleteAction([MinLength(2)] string name) {
            return new List<string> {"test1", "test2"}.AsQueryable();
        }
    }
}
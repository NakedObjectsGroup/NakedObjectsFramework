// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NakedObjects;

namespace MvcTestApp.Tests.Controllers {
    public class AutoCompleteRepository {

        public IDomainObjectContainer Container { set; protected get; }

        public void AnAction(string parm1, string parm2, int parm3) {}

        public IQueryable<string> AutoComplete0AnAction(string name) {
            return new[] {"value1", "value2"}.AsQueryable();
        }

        public IQueryable<string> AutoComplete1AnAction([MinLength(3)]string name) {
            return new[] { "value3", "value4" }.AsQueryable();
        }

        public AutoCompleteObject GetAutoCompleteObject() {
            var co = Container.NewTransientInstance<AutoCompleteObject>(); 
            return co; 
        }
    }
}
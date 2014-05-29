// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Linq;

namespace MvcTestApp.Tests.Controllers {
    public class AutoCompleteObject {
        public virtual string Name { get; set; }

        public virtual string AProperty { get; set; }

        public IQueryable<string> AutoCompleteAProperty(string name) {
            return new[] {"value5", "value6"}.AsQueryable();
        }
    }
}
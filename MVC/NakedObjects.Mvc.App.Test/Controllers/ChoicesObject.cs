// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System.Collections.Generic;

namespace MvcTestApp.Tests.Controllers {
    public class ChoicesObject {


        public virtual string Name { get; set; }

        public virtual string AProperty { get; set; }

        public IList<string> ChoicesAProperty(string name) {
            if (string.IsNullOrEmpty(name)) {
                return new string[] {};
            }

            return new[] {name + "-A", name + "-B"};
        }
    }
}
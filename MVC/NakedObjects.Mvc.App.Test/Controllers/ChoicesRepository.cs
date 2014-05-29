// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System.Collections.Generic;
using System.Linq;
using NakedObjects;

namespace MvcTestApp.Tests.Controllers {
    public class ChoicesRepository {

        public IDomainObjectContainer Container { set; protected get; }


        public void AnAction(string parm1, string parm2, int parm3) {}

        public IList<string> Choices0AnAction() {
            return new[] {"value1", "value2"};
        }

        public IList<string> Choices1AnAction(string parm1, int parm3) {
            if (!string.IsNullOrEmpty(parm1)) {
                return new[] {parm1 + "postfix1", parm1 + "postfix2"};
            }

            if (parm3 > 0) {
                return new[] {"parm3A" + parm3, "parm3B" + parm3};
            }

            return new string[] {};
        }

        public void AnActionMultiple(IEnumerable<string> parm1, IEnumerable<string> parm2, IEnumerable<int> parm3) { }

        public IList<string> Choices0AnActionMultiple() {
            return new[] { "value1", "value2" };
        }

        public IList<string> Choices1AnActionMultiple(IEnumerable<string> parm1, IEnumerable<int> parm3) {
            if (parm1 != null && parm1.Any(s => !string.IsNullOrEmpty(s))) {

                if (parm1.Count() == 1) {
                    return new[] {parm1.First() + "postfix1", parm1.First() + "postfix2"};
                }
                if (parm1.Count() == 2) {
                    return new[] { parm1.First() + "postfix1", parm1.Last() + "postfix2" };
                }
            }

            if (parm3 != null && parm3.Any(i => i != 0)) {
                return new[] { "parm3A" + parm3.First(), "parm3B" + parm3.First() };
            }

            return new string[] { };
        }



        public ChoicesObject GetChoicesObject() {
            var co = Container.NewTransientInstance<ChoicesObject>(); 
            return co; 
        }
    }
}
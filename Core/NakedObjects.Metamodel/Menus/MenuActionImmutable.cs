using NakedObjects.Architecture.Menu;
using NakedObjects.Architecture.SpecImmutable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NakedObjects.Meta.Menus {
    public class MenuActionImmutable : IMenuAction {

        public MenuActionImmutable(IActionSpecImmutable actionSpec, string renamedTo = null) {
            this.Action = actionSpec;
            if (renamedTo == null) {
                Name = actionSpec.Identifier.MemberName;  
            } else {
                Name = renamedTo;
            }
        }

        public string Name { get; set; }

        public IActionSpecImmutable Action { get; set; }
    }
}

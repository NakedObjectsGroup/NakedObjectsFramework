using NakedObjects.Architecture.SpecImmutable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NakedObjects.Meta.Menus {
    public class MenuAction : IMenuItem {

        public MenuAction(IActionSpecImmutable actionSpec, string renamedTo = null) {
            this.ActionSpec = actionSpec;
            if (renamedTo == null) {
                Name = actionSpec.Identifier.MemberName;  
            } else {
                Name = renamedTo;
            }
        }

        public string Name { get; set; }

        public IActionSpecImmutable ActionSpec { get; set; }
    }
}

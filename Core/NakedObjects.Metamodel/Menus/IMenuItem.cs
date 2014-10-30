using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NakedObjects.Meta.Menus {
    //Abstraction of MenuAction and Menu, to allow sub-menus
    public interface IMenuItem {

        string Name { get; }
    }
}

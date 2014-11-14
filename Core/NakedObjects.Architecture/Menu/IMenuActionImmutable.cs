using NakedObjects.Architecture.SpecImmutable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NakedObjects.Architecture.Menu {
    /// <summary>
    /// Run-time metamodel of a menu action, which provides access to the
    /// action spec.
    /// </summary>
    public interface IMenuActionImmutable : IMenuItemImmutable {
        IActionSpecImmutable Action { get; }
    }
}

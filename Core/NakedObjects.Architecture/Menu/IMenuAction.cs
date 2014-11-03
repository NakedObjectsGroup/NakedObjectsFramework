using NakedObjects.Architecture.SpecImmutable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NakedObjects.Architecture.Menu {
    public interface IMenuAction : IMenuItem {
        IActionSpecImmutable Action { get; }
    }
}

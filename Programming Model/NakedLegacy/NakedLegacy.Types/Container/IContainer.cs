using System;
using System.Collections;

namespace NakedLegacy.Types.Container;

public interface IContainer {
    public IEnumerable AllInstances(Type ofType);
}
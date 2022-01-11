using System;
using System.Collections;
using System.Linq;

namespace NakedLegacy.Types.Container;

public interface IContainer {
    public IEnumerable AllInstances(Type ofType);

    public object Repository(Type ofType);

    public IQueryable<T> Instances<T>() where T : class;
}
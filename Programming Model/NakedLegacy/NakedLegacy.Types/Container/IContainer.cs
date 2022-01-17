using System;
using System.Collections;
using System.Linq;

namespace NakedLegacy;

public interface IContainer {
    public IEnumerable AllInstances(Type ofType);
    public object Repository(Type ofType);
    public IQueryable<T> Instances<T>() where T : class;
    public object CreateTransientInstance(Type ofType);
    public T CreateTransientInstance<T>() where T : new();
    public void MakePersistent(ref object transientObject);
}
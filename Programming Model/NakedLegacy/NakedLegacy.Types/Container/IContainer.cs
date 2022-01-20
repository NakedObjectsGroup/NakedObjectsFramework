using System;
using System.Collections;
using System.Linq;

namespace NakedLegacy;

public interface IContainer {
    public IEnumerable AllInstances(Type ofType);
    public IQueryable<T> AllInstances<T>() where T : class;
    public object Repository(Type ofType);
    public T Repository<T>();
    public object CreateTransientInstance(Type ofType);
    public T CreateTransientInstance<T>() where T : new();
    public void MakePersistent<T>(ref T transientObject);
}
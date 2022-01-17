using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NakedFramework.Architecture.Framework;
using NakedLegacy;

namespace NakedLegacy.Reflector.Component;

public class Container : IContainer {
    private readonly INakedFramework framework;

    public Container(INakedFramework framework) => this.framework = framework;

    public IEnumerable AllInstances(Type ofType) => framework.Persistor.Instances(ofType);

    private IEnumerable<object> Services => framework.ServicesManager.GetServices().Select(no => no.Object);

    public T Repository<T>() => Services.OfType<T>().SingleOrDefault();

    public object Repository(Type ofType) => Services.SingleOrDefault(o => o.GetType() == ofType);

    public IQueryable<T> Instances<T>() where T : class => framework.Persistor.Instances<T>();
    public object CreateTransientInstance(Type ofType) => throw new NotImplementedException();

    public T CreateTransientInstance<T>() => throw new NotImplementedException();

    public void MakePersistent(object obj) {
        throw new NotImplementedException();
    }
}
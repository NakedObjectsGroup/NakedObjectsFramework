using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities.Resources;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Framework;
using NakedFramework.Architecture.Spec;
using NakedFramework.Core.Error;
using NakedFramework.Core.Util;

namespace NakedLegacy.Reflector.Component;

public class Container : IContainer {
    private readonly INakedFramework framework;

    public Container(INakedFramework framework) => this.framework = framework;

    private IEnumerable<object> Services => framework.ServicesManager.GetServices().Select(no => no.Object);

    public IEnumerable AllInstances(Type ofType) => framework.Persistor.Instances(ofType);

    public object Repository(Type ofType) => Services.SingleOrDefault(o => o.GetType() == ofType);

    public IQueryable<T> AllInstances<T>() where T : class => framework.Persistor.Instances<T>();

    public object CreateTransientInstance(Type ofType) {
        var spec = (IObjectSpec)framework.MetamodelManager.GetSpecification(ofType);
        return framework.LifecycleManager.CreateInstance(spec).Object;
    }

    public T CreateTransientInstance<T>() where T : new() => (T)CreateTransientInstance(typeof(T));

    public T Repository<T>() => Services.OfType<T>().SingleOrDefault();
    private INakedObjectAdapter AdapterFor(object obj) => framework.NakedObjectManager.CreateAdapter(obj, null, null);

    private bool IsPersistent(object obj) => !AdapterFor(obj).Oid.IsTransient;

    public void MakePersistent<T>(ref T transientObject) {
        var adapter = framework.NakedObjectManager.GetAdapterFor(transientObject);
        if (IsPersistent(transientObject)) {
            throw new PersistFailedException($"Trying to persist an already persisted object: {adapter}");
        }

        framework.LifecycleManager.MakePersistent(adapter);
        transientObject = (T) adapter.GetDomainObject();
    }
}
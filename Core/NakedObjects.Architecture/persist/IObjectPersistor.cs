using System;
using System.Linq;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Spec;

namespace NakedObjects.Architecture.persist {
    public interface IObjectPersistor {
        IQueryable<T> Instances<T>() where T : class;

        IQueryable Instances(Type type);

        IQueryable Instances(INakedObjectSpecification specification);

        INakedObject LoadObject(IOid oid, INakedObjectSpecification spec);

        void AddPersistedObject(INakedObject nakedObject);

        void MadePersistent(INakedObject nakedObject);
    }
}

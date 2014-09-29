using System;
using System.Linq;
using System.Reflection;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;

namespace NakedObjects.Architecture.Persist {
    public interface IObjectPersistor {
        IQueryable<T> Instances<T>() where T : class;

        IQueryable Instances(Type type);

        IQueryable Instances(INakedObjectSpecification specification);

        INakedObject LoadObject(IOid oid, INakedObjectSpecification spec);

        void AddPersistedObject(INakedObject nakedObject);

        void Reload(INakedObject nakedObject);
        void ResolveField(INakedObject nakedObject, INakedObjectAssociation field);
        void LoadField(INakedObject nakedObject, string field);
        int CountField(INakedObject nakedObject, string field);
        PropertyInfo[] GetKeys(Type type);
        INakedObject FindByKeys(Type type, object[] keys);
        void Refresh(INakedObject nakedObject);
        void ResolveImmediately(INakedObject nakedObject);
        void ObjectChanged(INakedObject nakedObject);
        void DestroyObject(INakedObject nakedObject);
        object CreateObject(INakedObjectSpecification specification);
    }
}

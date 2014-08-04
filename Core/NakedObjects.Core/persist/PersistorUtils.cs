// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using Common.Logging;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Objects.Aggregated;
using NakedObjects.Architecture.Facets.Objects.ViewModel;
using NakedObjects.Architecture.Persist;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.Util;
using NakedObjects.Core.Adapter;
using NakedObjects.Core.Context;
using NakedObjects.Core.Util;

namespace NakedObjects.Core.Persist {
    public static class PersistorUtils {
        private static readonly ILog Log;

        static PersistorUtils() {
            Log = LogManager.GetLogger(typeof (PersistorUtils));
        }

        public static List<INakedObject> GetCollectionOfAdaptedObjects(IEnumerable domainObjects) {
            var adaptedObjects = new List<INakedObject>();
            foreach (object domainObject in domainObjects) {
                adaptedObjects.Add(CreateAdapter(domainObject));
            }
            return adaptedObjects;
        }

        public static INakedObject CreateAdapter(object domainObject) {
            return CreateAdapter(domainObject, null, null);
        }

        public static INakedObject CreateAdapter(IOid oid, object domainObject) {
            return CreateAdapter(domainObject, oid, null);
        }

        public static INakedObject RecreateInstance(IOid oid, INakedObjectSpecification specification) {
            return NakedObjectsContext.ObjectPersistor.RecreateInstance(oid, specification);
        }

        public static INakedObject CreateTransientInstance(INakedObjectSpecification specification) {
            return NakedObjectsContext.ObjectPersistor.CreateInstance(specification);
        }

        public static void ReplacePoco(INakedObject nakedObject, object newDomainObject) {
            NakedObjectsContext.ObjectPersistor.ReplacePoco(nakedObject, newDomainObject);
        }

        public static void RemoveAdapter(INakedObject nakedObject) {
            NakedObjectsContext.ObjectPersistor.RemoveAdapter(nakedObject);
        }

        public static INakedObject CreateAggregatedAdapter(INakedObject parent, INakedObjectAssociation field, object obj) {
            return CreateAggregatedAdapter(parent, field.Id, obj);
        }

        public static INakedObject CreateAggregatedAdapterClone(INakedObject parent, INakedObjectAssociation field, object obj) {
            return CreateAggregatedAdapter(parent, field.Id + ":CLONE", obj);
        }

        public static INakedObject CreateAggregatedAdapter(INakedObject parent, PropertyInfo property, object obj) {
            return CreateAggregatedAdapter(parent, parent.Specification.GetProperty(property.Name), obj);
        }

        public static void Abort(INakedObjectPersistor objectManager, IFacetHolder holder) {
            Log.Info("exception executing " + holder + ", aborting transaction");
            try {
                objectManager.AbortTransaction();
            }
            catch (Exception e2) {
                Log.Error("failure during abort", e2);
            }
        }

        public static IOid RestoreGenericOid(string[] encodedData) {
            string typeName = TypeNameUtils.DecodeTypeName(HttpUtility.UrlDecode(encodedData.First()));
            INakedObjectSpecification spec = NakedObjectsContext.Reflector.LoadSpecification(typeName);

            if (spec.IsCollection) {
                return new CollectionMemento(NakedObjectsContext.ObjectPersistor, encodedData);
            }

            if (spec.ContainsFacet<IViewModelFacet>()) {
                return new ViewModelOid(encodedData);
            }

            return spec.ContainsFacet<IComplexTypeFacet>() ? new AggregateOid(NakedObjectsContext.Reflector, encodedData) : null;
        }

        public static INakedObject GetViewModel(ViewModelOid oid) {
            return NakedObjectsContext.ObjectPersistor.GetViewModel(oid);
        }

        public static void PopulateViewModelKeys(INakedObject nakedObject) {
            var vmoid = (ViewModelOid) nakedObject.Oid;

            if (!vmoid.IsFinal) {
                vmoid.UpdateKeys(nakedObject.Specification.GetFacet<IViewModelFacet>().Derive(nakedObject), true);
            }
        }

        #region private 

        private static INakedObject CreateAdapter(object domainObject, IOid oid, IVersion version) {
            return NakedObjectsContext.ObjectPersistor.CreateAdapter(domainObject, oid, version);
        }

        private static INakedObject CreateAggregatedAdapter(INakedObject parent, string fieldId, object obj) {
            NakedObjectsContext.ObjectPersistor.GetAdapterFor(obj);

            IOid oid = new AggregateOid(NakedObjectsContext.Reflector,  parent.Oid, fieldId, obj.GetType().FullName);
            INakedObject adapterFor = NakedObjectsContext.ObjectPersistor.GetAdapterFor(oid);
            if (adapterFor == null || adapterFor.Object != obj) {
                if (adapterFor != null) {
                    NakedObjectsContext.ObjectPersistor.RemoveAdapter(adapterFor);
                }
                adapterFor = CreateAdapter(oid, obj);
                adapterFor.OptimisticLock = new NullVersion();
            }
            Assert.AssertNotNull(adapterFor);
            return adapterFor;
        }

        #endregion
    }

    // Copyright (c) Naked Objects Group Ltd.
}
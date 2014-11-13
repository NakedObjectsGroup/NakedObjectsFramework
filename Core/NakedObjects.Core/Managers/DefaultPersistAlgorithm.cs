// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using Common.Logging;
using NakedObjects.Architecture;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Resolve;
using NakedObjects.Architecture.Spec;
using NakedObjects.Core.Resolve;
using NakedObjects.Core.Util;

namespace NakedObjects.Managers {
    public class DefaultPersistAlgorithm : IPersistAlgorithm {
        private readonly IObjectPersistor persistor;
        private readonly INakedObjectManager manager;
        private readonly IServicesManager services;
        private static readonly ILog Log;

        static DefaultPersistAlgorithm() {
            Log = LogManager.GetLogger(typeof (DefaultPersistAlgorithm));
        }

        public DefaultPersistAlgorithm(IObjectPersistor persistor, INakedObjectManager manager, IServicesManager services) {
            this.persistor = persistor;
            this.manager = manager;
            this.services = services;
        }


        #region IPersistAlgorithm Members

       
        public virtual void MakePersistent(INakedObject nakedObject,  ISession session) {
            if (nakedObject.Spec.IsCollection) {
                Log.Info("Persist " + nakedObject);

                nakedObject.GetAsEnumerable(manager).ForEach(no => Persist(no, session));

                if (nakedObject.ResolveState.IsGhost()) {
                    nakedObject.ResolveState.Handle(Events.StartResolvingEvent);
                    nakedObject.ResolveState.Handle(Events.EndResolvingEvent);
                }
            }
            else {
                if (nakedObject.ResolveState.IsPersistent()) {
                    throw new NotPersistableException("can't make object persistent as it is already persistent: " + nakedObject);
                }
                if (nakedObject.Spec.Persistable == PersistableType.Transient) {
                    throw new NotPersistableException("can't make object persistent as it is not persistable: " + nakedObject);
                }
                Persist(nakedObject, session);
            }
        }

        public virtual string Name {
            get { return "Simple Bottom Up Persistence Walker"; }
        }

        

        #endregion

        protected void Persist(INakedObject nakedObject, ISession session) {
            if (nakedObject.ResolveState.IsAggregated() ||
                (nakedObject.ResolveState.IsTransient() &&
                 nakedObject.Spec.Persistable != PersistableType.Transient)) {
                IAssociationSpec[] fields = nakedObject.Spec.Properties;
                if (!nakedObject.Spec.IsEncodeable && fields.Length > 0) {
                    Log.Info("make persistent " + nakedObject);
                    nakedObject.Persisting(session);
                    if (!nakedObject.Spec.ContainsFacet(typeof (IComplexTypeFacet))) {
                        manager.MadePersistent(nakedObject);
                    }

                    foreach (IAssociationSpec field in fields) {
                        if (!field.IsPersisted) {
                            continue;
                        }
                        if (field is IOneToManyAssociationSpec) {
                            INakedObject collection = field.GetNakedObject(nakedObject);
                            if (collection == null) {
                                throw new NotPersistableException("Collection " + field.GetName() + " does not exist in " + nakedObject.Spec.FullName);
                            }
                            MakePersistent(collection,  session);
                        }
                        else {
                            INakedObject fieldValue = field.GetNakedObject(nakedObject);
                            if (fieldValue == null) {
                                continue;
                            }
                            Persist(fieldValue, session);
                        }
                    }
                    persistor.AddPersistedObject(nakedObject);
                }
            }
        }

        public override string ToString() {
            return new AsString(this).ToString();
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}
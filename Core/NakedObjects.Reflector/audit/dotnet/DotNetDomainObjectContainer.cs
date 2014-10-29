// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Resolve;
using NakedObjects.Architecture.Spec;
using NakedObjects.Services;
using NakedObjects.UtilInternal;

namespace NakedObjects.Reflect.DotNet {
    public class DotNetDomainObjectContainer : IDomainObjectContainer, IInternalAccess {
        private readonly INakedObjectsFramework framework;

        public DotNetDomainObjectContainer(INakedObjectsFramework framework) {
            this.framework = framework;
        }

        #region IDomainObjectContainer Members

        public IQueryable<T> Instances<T>() where T : class {
            return framework.Persistor.Instances<T>();
        }

        public IQueryable Instances(Type type) {
            return framework.Persistor.Instances(type);
        }

        public void DisposeInstance(object persistentObject) {
            if (persistentObject == null) {
                throw new ArgumentException(Resources.NakedObjects.DisposeReferenceError);
            }
            INakedObject adapter = framework.Manager.GetAdapterFor(persistentObject);
            if (!IsPersistent(persistentObject)) {
                throw new DisposeFailedException(string.Format(Resources.NakedObjects.NotPersistentMessage, adapter));
            }
            framework.UpdateNotifier.AddDisposedObject(adapter);
            framework.Persistor.DestroyObject(adapter);
        }

        public IPrincipal Principal {
            get { return framework.Session.Principal; }
        }

        public void InformUser(string message) {
            framework.MessageBroker.AddMessage(message);
        }

        public bool IsPersistent(object obj) {
            return !AdapterFor(obj).Oid.IsTransient;
        }

        public void Persist<T>(ref T transientObject) {
            INakedObject adapter = framework.Manager.GetAdapterFor(transientObject);
            if (IsPersistent(transientObject)) {
                throw new PersistFailedException(string.Format(Resources.NakedObjects.AlreadyPersistentMessage, adapter));
            }
            Validate(adapter);
            framework.LifecycleManager.MakePersistent(adapter);
            transientObject = adapter.GetDomainObject<T>();
        }

        public T NewTransientInstance<T>() where T : new() {
            return (T) NewTransientInstance(typeof (T));
        }

        public T NewViewModel<T>() where T : IViewModel, new() {
            return (T) NewViewModel(typeof (T));
        }

        public IViewModel NewViewModel(Type type) {
            IObjectSpec spec = framework.Metamodel.GetSpecification(type);
            if (spec.IsViewModel) {
                return framework.LifecycleManager.CreateViewModel(spec).GetDomainObject<IViewModel>();
            }
            return null;
        }

        public object NewTransientInstance(Type type) {
            IObjectSpec spec = framework.Metamodel.GetSpecification(type);
            return framework.LifecycleManager.CreateInstance(spec).Object;
        }

        public void ObjectChanged(object obj) {
            if (obj != null) {
                INakedObject adapter = AdapterFor(obj);
                Validate(adapter);
                framework.Persistor.ObjectChanged(adapter);
            }
        }

        public void RaiseError(string message) {
            throw new DomainException(message);
        }

        public void Refresh(object obj) {
            INakedObject nakedObject = AdapterFor(obj);
            framework.Persistor.Refresh(nakedObject);
            ObjectChanged(obj);
        }

        public void Resolve(object parent) {
            INakedObject adapter = AdapterFor(parent);
            if (adapter.ResolveState.IsResolvable()) {
                framework.Persistor.ResolveImmediately(adapter);
            }
        }

        public void Resolve(object parent, object field) {
            if (field == null) {
                Resolve(parent);
            }
        }

        public void WarnUser(string message) {
            framework.MessageBroker.AddWarning(message);
        }

        public void AbortCurrentTransaction() {
            framework.TransactionManager.UserAbortTransaction();
        }

        #endregion

        #region IInternalAccess Members

        public PropertyInfo[] GetKeys(Type type) {
            return framework.Persistor.GetKeys(type);
        }

        public object FindByKeys(Type type, object[] keys) {
            return framework.Persistor.FindByKeys(type, keys).GetDomainObject();
        }

        #endregion

        private void Validate(INakedObject adapter) {
            if (adapter.Spec.ContainsFacet<IValidateProgrammaticUpdatesFacet>()) {
                string state = adapter.ValidToPersist();
                if (state != null) {
                    throw new PersistFailedException(string.Format(Resources.NakedObjects.PersistStateError, adapter.Spec.ShortName, adapter.TitleString(), state));
                }
            }
        }

        private INakedObject AdapterFor(object obj) {
            return framework.Manager.CreateAdapter(obj, null, null);
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}
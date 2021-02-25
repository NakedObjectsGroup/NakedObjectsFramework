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
using Microsoft.Extensions.Logging;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Framework;
using NakedFramework.Architecture.Spec;
using NakedFramework.Core.Exception;
using NakedFramework.Core.Resolve;
using NakedFramework.Core.Util;
using NakedObjects;
using NakedObjects.UtilInternal;

namespace NakedFramework.Core.Container {
    public sealed class DomainObjectContainer : IDomainObjectContainer, IInternalAccess {
        private readonly INakedObjectsFramework framework;
        private readonly ILogger<DomainObjectContainer> logger;

        public DomainObjectContainer(INakedObjectsFramework framework,
                                     ILogger<DomainObjectContainer> logger) {
            this.framework = framework;
            this.logger = logger;
        }

        private void Validate(INakedObjectAdapter adapter) {
            if (adapter.Spec.ContainsFacet<IValidateProgrammaticUpdatesFacet>()) {
                var state = adapter.ValidToPersist();
                if (state != null) {
                    throw new PersistFailedException(logger.LogAndReturn(string.Format(NakedObjects.Resources.NakedObjects.PersistStateError, adapter.Spec.ShortName, adapter.TitleString(), state)));
                }
            }
        }

        private INakedObjectAdapter AdapterFor(object obj) => framework.NakedObjectManager.CreateAdapter(obj, null, null);

        #region IDomainObjectContainer Members

        public IQueryable<T> Instances<T>() where T : class => framework.Persistor.Instances<T>();

        public IQueryable Instances(Type type) => framework.Persistor.Instances(type);

        public void DisposeInstance(object persistentObject) {
            if (persistentObject == null) {
                throw new ArgumentException(logger.LogAndReturn(NakedObjects.Resources.NakedObjects.DisposeReferenceError));
            }

            var adapter = framework.NakedObjectManager.GetAdapterFor(persistentObject);
            if (!IsPersistent(persistentObject)) {
                throw new DisposeFailedException(logger.LogAndReturn(string.Format(NakedObjects.Resources.NakedObjects.NotPersistentMessage, adapter)));
            }

            framework.Persistor.DestroyObject(adapter);
        }

        public T GetService<T>() => framework.ServicesManager.GetServices().Select(no => no.Object).OfType<T>().SingleOrDefault();

        public IPrincipal Principal => framework.Session.Principal;

        public void InformUser(string message) => framework.MessageBroker.AddMessage(message);

        public bool IsPersistent(object obj) => !AdapterFor(obj).Oid.IsTransient;

        public void Persist<T>(ref T transientObject) {
            var adapter = framework.NakedObjectManager.GetAdapterFor(transientObject);
            if (IsPersistent(transientObject)) {
                throw new PersistFailedException(logger.LogAndReturn(string.Format(NakedObjects.Resources.NakedObjects.AlreadyPersistentMessage, adapter)));
            }

            Validate(adapter);
            framework.LifecycleManager.MakePersistent(adapter);
            transientObject = adapter.GetDomainObject<T>();
        }

        public T NewTransientInstance<T>() where T : new() => (T) NewTransientInstance(typeof(T));

        public T NewViewModel<T>() where T : IViewModel, new() => (T) NewViewModel(typeof(T));

        public IViewModel NewViewModel(Type type) {
            var spec = (IObjectSpec) framework.MetamodelManager.GetSpecification(type);
            return spec.IsViewModel ? framework.LifecycleManager.CreateViewModel(spec).GetDomainObject<IViewModel>() : null;
        }

        public object NewTransientInstance(Type type) {
            var spec = (IObjectSpec) framework.MetamodelManager.GetSpecification(type);
            return framework.LifecycleManager.CreateInstance(spec).Object;
        }

        public void ObjectChanged(object obj) {
            if (obj != null) {
                var adapter = AdapterFor(obj);
                Validate(adapter);
                framework.Persistor.ObjectChanged(adapter, framework.LifecycleManager, framework.MetamodelManager);
            }
        }

        public void RaiseError(string message) => throw new DomainException(logger.LogAndReturn(message));

        public void Refresh(object obj) {
            var nakedObjectAdapter = AdapterFor(obj);
            framework.Persistor.Refresh(nakedObjectAdapter);
            ObjectChanged(obj);
        }

        public void Resolve(object parent) {
            var adapter = AdapterFor(parent);
            if (adapter.ResolveState.IsResolvable()) {
                framework.Persistor.ResolveImmediately(adapter);
            }
        }

        public void Resolve(object parent, object field) {
            if (field == null) {
                Resolve(parent);
            }
        }

        public void WarnUser(string message) => framework.MessageBroker.AddWarning(message);

        public void AbortCurrentTransaction() => framework.TransactionManager.UserAbortTransaction();

        #endregion

        #region IInternalAccess Members

        public PropertyInfo[] GetKeys(Type type) => framework.Persistor.GetKeys(type);

        public object FindByKeys(Type type, object[] keys) => framework.Persistor.FindByKeys(type, keys).GetDomainObject();

        #endregion

        #region Titles

        public ITitleBuilder NewTitleBuilder() => new TitleBuilderImpl(this);

        public ITitleBuilder NewTitleBuilder(object obj, string defaultTitle = null) => new TitleBuilderImpl(this, obj, defaultTitle);

        public ITitleBuilder NewTitleBuilder(string text) => new TitleBuilderImpl(this, text);

        public string TitleOf(object obj, string format = null) {
            var adapter = AdapterFor(obj);
            return format == null ? adapter.TitleString() : adapter.Spec.GetFacet<ITitleFacet>().GetTitleWithMask(format, adapter, framework);
        }

        #endregion
    }

    // Copyright (c) Naked Objects Group Ltd.
}
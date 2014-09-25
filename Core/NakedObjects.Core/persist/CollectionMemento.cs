// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets.Actcoll.Typeof;
using NakedObjects.Architecture.Persist;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Security;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.Util;
using NakedObjects.Core.Adapter;
using NakedObjects.Core.Util;
using NakedObjects.Util;

namespace NakedObjects.Core.Persist {
    public class CollectionMemento : IEncodedToStrings, IOid {
        private readonly ILifecycleManager lifecycleManager;
        private readonly IObjectPersistor persistor;
        private readonly INakedObjectReflector reflector;
        private readonly ISession session;

        public enum ParameterType {
            Value,
            Object,
            ValueCollection,
            ObjectCollection
        }

        public CollectionMemento(ILifecycleManager lifecycleManager, IObjectPersistor persistor,  INakedObjectReflector reflector, ISession session,  CollectionMemento otherMemento, object[] selectedObjects) {
            Assert.AssertNotNull(lifecycleManager);
            Assert.AssertNotNull(reflector);
            Assert.AssertNotNull(otherMemento);

            this.lifecycleManager = lifecycleManager;
            this.persistor = persistor;
            this.reflector = reflector;
            this.session = session;
            IsPaged = otherMemento.IsPaged;
            IsNotQueryable = otherMemento.IsNotQueryable;
            Target = otherMemento.Target;
            Action = otherMemento.Action;
            Parameters = otherMemento.Parameters;
            SelectedObjects = selectedObjects;
        }

        public CollectionMemento(ILifecycleManager lifecycleManager, IObjectPersistor persistor, INakedObjectReflector reflector, ISession session, INakedObject target, INakedObjectAction action, INakedObject[] parameters) {
            Assert.AssertNotNull(lifecycleManager);
            Assert.AssertNotNull(reflector);
            this.lifecycleManager = lifecycleManager;
            this.persistor = persistor;
            this.reflector = reflector;
            this.session = session;
            Target = target;
            Action = action;
            Parameters = parameters;

            if (Target.Specification.IsViewModel) {
                lifecycleManager.PopulateViewModelKeys(Target);
            }
        }

        public CollectionMemento(ILifecycleManager lifecycleManager, IObjectPersistor persistor, INakedObjectReflector reflector, ISession session, string[] strings) {
            Assert.AssertNotNull(lifecycleManager);
            Assert.AssertNotNull(reflector);
            this.lifecycleManager = lifecycleManager;
            this.persistor = persistor;
            this.reflector = reflector;
            this.session = session;
            var helper = new StringDecoderHelper(reflector, strings, true);
            string specName = helper.GetNextString();
            string actionId = helper.GetNextString();
            var targetOid = (IOid) helper.GetNextEncodedToStrings();

            Target = RestoreObject(targetOid);
            Action = Target.GetActionLeafNode(actionId);

            var parameters = new List<INakedObject>();

            while (helper.HasNext) {
                var parmType = helper.GetNextEnum<ParameterType>();

                switch (parmType) {
                    case ParameterType.Value:
                        object obj = helper.GetNextObject();
                        parameters.Add(lifecycleManager.CreateAdapter(obj, null, null));
                        break;
                    case ParameterType.Object:
                        var oid = (IOid) helper.GetNextEncodedToStrings();
                        INakedObject nakedObject = RestoreObject(oid);
                        parameters.Add(nakedObject);
                        break;
                    case ParameterType.ValueCollection:
                        Type vInstanceType;
                        IList<object> vColl = helper.GetNextValueCollection(out vInstanceType);
                        IList typedVColl = CollectionUtils.ToTypedIList(vColl, vInstanceType);
                        parameters.Add(lifecycleManager.CreateAdapter(typedVColl, null, null));
                        break;
                    case ParameterType.ObjectCollection:
                        Type oInstanceType;
                        List<object> oColl = helper.GetNextObjectCollection(out oInstanceType).Cast<IOid>().Select(o => RestoreObject(o).Object).ToList();
                        IList typedOColl = CollectionUtils.ToTypedIList(oColl, oInstanceType);
                        parameters.Add(lifecycleManager.CreateAdapter(typedOColl, null, null));
                        break;
                    default:
                        throw new ArgumentException(string.Format("Unexpected parameter type value: {0}", parmType));
                }
            }

            Parameters = parameters.ToArray();
        }

        public INakedObject Target { get; private set; }
        public INakedObjectAction Action { get; private set; }
        public INakedObject[] Parameters { get; private set; }

        public bool IsPaged { get; set; }
        public bool IsNotQueryable { get; set; }

        #region IEncodedToStrings Members

        public string[] ToEncodedStrings() {
            var helper = new StringEncoderHelper {Encode = true};
            helper.Add(Action.ReturnType.EncodeTypeName());
            helper.Add(Action.Id);
            helper.Add(Target.Oid as IEncodedToStrings);

            foreach (INakedObject parameter in Parameters) {
                if (parameter == null) {
                    helper.Add(ParameterType.Value);
                    helper.Add((object) null);
                }
                else if (parameter.Specification.IsParseable) {
                    helper.Add(ParameterType.Value);
                    helper.Add(parameter.Object);
                }
                else if (parameter.Specification.IsCollection) {
                    INakedObjectSpecification instanceSpec = parameter.Specification.GetFacet<ITypeOfFacet>().ValueSpec;
                    Type instanceType = TypeUtils.GetType(instanceSpec.FullName);

                    if (instanceSpec.IsParseable) {
                        helper.Add(ParameterType.ValueCollection);
                        helper.Add((IEnumerable) parameter.Object, instanceType);
                    }
                    else {
                        helper.Add(ParameterType.ObjectCollection);
                        helper.Add(parameter.GetCollectionFacetFromSpec().AsEnumerable(parameter, lifecycleManager).Select(p => p.Oid).Cast<IEncodedToStrings>(), instanceType);
                    }
                }
                else {
                    helper.Add(ParameterType.Object);
                    helper.Add(parameter.Oid as IEncodedToStrings);
                }
            }

            return helper.ToArray();
        }

        public string[] ToShortEncodedStrings() {
            return ToEncodedStrings();
        }

        #endregion

        #region IOid Members

        private object[] selectedObjects;

        public object[] SelectedObjects {
            get { return selectedObjects ?? new object[] {}; }
            private set { selectedObjects = value; }
        }

        public IOid Previous {
            get { return null; }
        }

        public bool IsTransient {
            get { return true; }
        }

        public bool HasPrevious {
            get { return false; }
        }

        public void CopyFrom(IOid oid) {
            // do nothing
        }

        public INakedObjectSpecification Specification {
            get { return Target.Specification; }
        }

        #endregion

        private INakedObject RestoreObject(IOid oid) {
            if (oid.IsTransient) {
                return lifecycleManager.RecreateInstance(oid, oid.Specification);
            }

            if (oid is ViewModelOid) {
                return lifecycleManager.GetViewModel(oid as ViewModelOid);
            }

            return persistor.LoadObject(oid, oid.Specification);
        }

        public INakedObject RecoverCollection() {
            INakedObject nakedObject = Action.Execute(Target, Parameters, lifecycleManager, session);

            if (selectedObjects != null) {
                IEnumerable<object> selected = nakedObject.GetDomainObject<IEnumerable>().Cast<object>().Where(x => selectedObjects.Contains(x));
                IList newResult = CollectionUtils.CloneCollectionAndPopulate(nakedObject.Object, selected);
                nakedObject = lifecycleManager.CreateAdapter(newResult, null, null);
            }

            nakedObject.SetATransientOid(this);
            return nakedObject;
        }
    }
}
// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Common.Logging;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Core.Util;
using NakedObjects.Util;

namespace NakedObjects.Core.Adapter {
    public sealed class CollectionMemento : IEncodedToStrings, ICollectionMemento {
        #region ParameterType enum

        public enum ParameterType {
            Value,
            Object,
            ValueCollection,
            ObjectCollection
        }

        #endregion

        private static readonly ILog Log = LogManager.GetLogger(typeof(CollectionMemento));
        private readonly ILifecycleManager lifecycleManager;
        private readonly IMetamodelManager metamodel;
        private readonly INakedObjectManager nakedObjectManager;
        private object[] selectedObjects;

        private CollectionMemento(ILifecycleManager lifecycleManager, INakedObjectManager nakedObjectManager, IMetamodelManager metamodel) {
            Assert.AssertNotNull(lifecycleManager);
            Assert.AssertNotNull(nakedObjectManager);
            this.lifecycleManager = lifecycleManager;
            this.nakedObjectManager = nakedObjectManager;
            this.metamodel = metamodel;
        }

        public CollectionMemento(ILifecycleManager lifecycleManager, INakedObjectManager nakedObjectManager, IMetamodelManager metamodel, CollectionMemento otherMemento, object[] selectedObjects)
            : this(lifecycleManager, nakedObjectManager, metamodel) {
            Assert.AssertNotNull(otherMemento);

            IsPaged = otherMemento.IsPaged;
            IsNotQueryable = otherMemento.IsNotQueryable;
            Target = otherMemento.Target;
            Action = otherMemento.Action;
            Parameters = otherMemento.Parameters;
            SelectedObjects = selectedObjects;
        }

        public CollectionMemento(ILifecycleManager lifecycleManager, INakedObjectManager nakedObjectManager, IMetamodelManager metamodel, INakedObjectAdapter target, IActionSpec actionSpec, INakedObjectAdapter[] parameters)
            : this(lifecycleManager, nakedObjectManager, metamodel) {
            Target = target;
            Action = actionSpec;
            Parameters = parameters;

            if (Target.Spec.IsViewModel) {
                lifecycleManager.PopulateViewModelKeys(Target);
            }
        }

        public CollectionMemento(ILifecycleManager lifecycleManager, INakedObjectManager nakedObjectManager, IMetamodelManager metamodel, string[] strings)
            : this(lifecycleManager, nakedObjectManager, metamodel) {
            var helper = new StringDecoderHelper(metamodel, strings, true);
            // ReSharper disable once UnusedVariable
            string specName = helper.GetNextString();
            string actionId = helper.GetNextString();
            var targetOid = (IOid) helper.GetNextEncodedToStrings();

            Target = RestoreObject(targetOid);
            Action = Target.GetActionLeafNode(actionId);

            var parameters = new List<INakedObjectAdapter>();

            while (helper.HasNext) {
                var parmType = helper.GetNextEnum<ParameterType>();

                switch (parmType) {
                    case ParameterType.Value:
                        object obj = helper.GetNextObject();
                        parameters.Add(nakedObjectManager.CreateAdapter(obj, null, null));
                        break;
                    case ParameterType.Object:
                        var oid = (IOid) helper.GetNextEncodedToStrings();
                        INakedObjectAdapter nakedObjectAdapter = RestoreObject(oid);
                        parameters.Add(nakedObjectAdapter);
                        break;
                    case ParameterType.ValueCollection:
                        Type vInstanceType;
                        IList<object> vColl = helper.GetNextValueCollection(out vInstanceType);
                        IList typedVColl = CollectionUtils.ToTypedIList(vColl, vInstanceType);
                        parameters.Add(nakedObjectManager.CreateAdapter(typedVColl, null, null));
                        break;
                    case ParameterType.ObjectCollection:
                        Type oInstanceType;
                        List<object> oColl = helper.GetNextObjectCollection(out oInstanceType).Cast<IOid>().Select(o => RestoreObject(o).Object).ToList();
                        IList typedOColl = CollectionUtils.ToTypedIList(oColl, oInstanceType);
                        parameters.Add(nakedObjectManager.CreateAdapter(typedOColl, null, null));
                        break;
                    default:
                        throw new ArgumentException(Log.LogAndReturn($"Unexpected parameter type value: {parmType}"));
                }
            }

            Parameters = parameters.ToArray();
        }

        public INakedObjectAdapter Target { get; }
        public IActionSpec Action { get; }
        public INakedObjectAdapter[] Parameters { get; }

        public bool IsPaged { get; set; }
        public bool IsNotQueryable { get; set; }

        public object[] SelectedObjects {
            get { return selectedObjects ?? new object[] {}; }
            private set { selectedObjects = value; }
        }

        #region IEncodedToStrings Members

        public string[] ToEncodedStrings() {
            var helper = new StringEncoderHelper {Encode = true};
            helper.Add(Action.ReturnSpec.EncodeTypeName(Action.ReturnSpec.IsCollection ? new[] {Action.ElementSpec} : new IObjectSpec[] {}));
            helper.Add(Action.Id);
            helper.Add(Target.Oid as IEncodedToStrings);

            foreach (INakedObjectAdapter parameter in Parameters) {
                if (parameter == null) {
                    helper.Add(ParameterType.Value);
                    helper.Add((object) null);
                }
                else if (parameter.Spec.IsParseable) {
                    helper.Add(ParameterType.Value);
                    helper.Add(parameter.Object);
                }
                else if (parameter.Spec.IsCollection) {
                    IObjectSpecImmutable instanceSpec = parameter.Spec.GetFacet<ITypeOfFacet>().GetValueSpec(parameter, metamodel.Metamodel);
                    Type instanceType = TypeUtils.GetType(instanceSpec.FullName);

                    if (instanceSpec.IsParseable) {
                        helper.Add(ParameterType.ValueCollection);
                        helper.Add((IEnumerable) parameter.Object, instanceType);
                    }
                    else {
                        helper.Add(ParameterType.ObjectCollection);
                        helper.Add(parameter.GetCollectionFacetFromSpec().AsEnumerable(parameter, nakedObjectManager).Select(p => p.Oid).Cast<IEncodedToStrings>(), instanceType);
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

        public IOid Previous => null;

        public bool IsTransient => true;

        public bool HasPrevious => false;

        public void CopyFrom(IOid oid) {
            // do nothing
        }

        public ITypeSpec Spec => Target.Spec;

        #endregion

        public ICollectionMemento NewSelectionMemento(object[] objects, bool isPaged) {
            return new CollectionMemento(lifecycleManager, nakedObjectManager, metamodel, this, objects) {IsPaged = isPaged};
        }

        private INakedObjectAdapter RestoreObject(IOid oid) {
            if (oid.IsTransient) {
                return lifecycleManager.RecreateInstance(oid, oid.Spec);
            }

            if (oid is IViewModelOid) {
                return lifecycleManager.GetViewModel(oid);
            }

            return lifecycleManager.LoadObject(oid, oid.Spec);
        }

        public INakedObjectAdapter RecoverCollection() {
            INakedObjectAdapter nakedObjectAdapter = Action.Execute(Target, Parameters);

            if (selectedObjects != null) {
                IEnumerable<object> selected = nakedObjectAdapter.GetDomainObject<IEnumerable>().Cast<object>().Where(x => selectedObjects.Contains(x));
                IList newResult = CollectionUtils.CloneCollectionAndPopulate(nakedObjectAdapter.Object, selected);
                nakedObjectAdapter = nakedObjectManager.CreateAdapter(newResult, null, null);
            }

            nakedObjectAdapter.SetATransientOid(this);
            return nakedObjectAdapter;
        }
    }
}
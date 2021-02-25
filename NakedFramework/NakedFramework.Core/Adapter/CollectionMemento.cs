﻿// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using NakedFramework.Architecture.Adapter;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Spec;
using NakedObjects.Core.Util;

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

        private readonly ILifecycleManager lifecycleManager;
        private readonly INakedObjectsFramework framework;
        private readonly ILogger<CollectionMemento> logger;
        private readonly IMetamodelManager metamodel;
        private readonly INakedObjectManager nakedObjectManager;
        private object[] selectedObjects;

        private CollectionMemento(INakedObjectsFramework framework, ILogger<CollectionMemento> logger) {
            this.lifecycleManager = framework.LifecycleManager ?? throw new InitialisationException($"{nameof(framework.LifecycleManager)} is null");
            this.nakedObjectManager = framework.NakedObjectManager ?? throw new InitialisationException($"{nameof(framework.NakedObjectManager)} is null");
            this.metamodel = framework.MetamodelManager ?? throw new InitialisationException($"{nameof(framework.MetamodelManager)} is null");
            this.framework = framework;
            this.logger = logger ?? throw new InitialisationException($"{nameof(logger)} is null");
        }

        public CollectionMemento(INakedObjectsFramework framework, ILogger<CollectionMemento> logger, CollectionMemento otherMemento, object[] selectedObjects)
            : this(framework, logger) {
            IsPaged = otherMemento.IsPaged;
            IsNotQueryable = otherMemento.IsNotQueryable;
            Target = otherMemento.Target;
            Action = otherMemento.Action;
            Parameters = otherMemento.Parameters;
            SelectedObjects = selectedObjects;
        }

        public CollectionMemento(INakedObjectsFramework framework, ILogger<CollectionMemento> logger, INakedObjectAdapter target, IActionSpec actionSpec, INakedObjectAdapter[] parameters)
            : this(framework, logger) {
            Target = target;
            Action = actionSpec;
            Parameters = parameters;

            if (Target?.Spec.IsViewModel == true) {
                lifecycleManager.PopulateViewModelKeys(Target, framework);
            }
        }

        public CollectionMemento(INakedObjectsFramework framework, ILoggerFactory loggerFactory, ILogger<CollectionMemento> logger, string[] strings)
            : this(framework, logger) {
            var helper = new StringDecoderHelper(metamodel, loggerFactory, loggerFactory.CreateLogger<StringDecoderHelper>(), strings, true);
            // ReSharper disable once UnusedVariable
            var specName = helper.GetNextString();
            var actionId = helper.GetNextString();
            var targetOid = (IOid) helper.GetNextEncodedToStrings();

            Target = RestoreObject(targetOid);
            Action = Target.GetActionLeafNode(actionId);

            var parameters = new List<INakedObjectAdapter>();

            while (helper.HasNext) {
                var parmType = helper.GetNextEnum<ParameterType>();

                switch (parmType) {
                    case ParameterType.Value:
                        var obj = helper.GetNextObject();
                        parameters.Add(nakedObjectManager.CreateAdapter(obj, null, null));
                        break;
                    case ParameterType.Object:
                        var oid = (IOid) helper.GetNextEncodedToStrings();
                        var nakedObjectAdapter = RestoreObject(oid);
                        parameters.Add(nakedObjectAdapter);
                        break;
                    case ParameterType.ValueCollection:
                        var vColl = helper.GetNextValueCollection(out var vInstanceType);
                        var typedVColl = CollectionUtils.ToTypedIList(vColl, vInstanceType);
                        parameters.Add(nakedObjectManager.CreateAdapter(typedVColl, null, null));
                        break;
                    case ParameterType.ObjectCollection:
                        var oColl = helper.GetNextObjectCollection(out var oInstanceType).Cast<IOid>().Select(o => RestoreObject(o).Object).ToList();
                        var typedOColl = CollectionUtils.ToTypedIList(oColl, oInstanceType);
                        parameters.Add(nakedObjectManager.CreateAdapter(typedOColl, null, null));
                        break;
                    default:
                        throw new ArgumentException(logger.LogAndReturn($"Unexpected parameter type value: {parmType}"));
                }
            }

            Parameters = parameters.ToArray();
        }

        #region ICollectionMemento Members

        public INakedObjectAdapter Target { get; }
        public IActionSpec Action { get; }
        public INakedObjectAdapter[] Parameters { get; }

        public bool IsPaged { get; set; }
        public bool IsNotQueryable { get; set; }

        public object[] SelectedObjects {
            get { return selectedObjects ?? new object[] { }; }
            private set => selectedObjects = value;
        }

        public IOid Previous => null;

        public bool IsTransient => true;

        public bool HasPrevious => false;

        public void CopyFrom(IOid oid) {
            // do nothing
        }

        public ITypeSpec Spec => Target.Spec;

        public ICollectionMemento NewSelectionMemento(object[] objects, bool isPaged) => new CollectionMemento(framework, logger, this, objects) {IsPaged = isPaged};

        public INakedObjectAdapter RecoverCollection() {
            var nakedObjectAdapter = Action.Execute(Target, Parameters);

            if (selectedObjects != null) {
                var selected = nakedObjectAdapter.GetDomainObject<IEnumerable>().Cast<object>().Where(x => selectedObjects.Contains(x));
                var newResult = CollectionUtils.CloneCollectionAndPopulate(nakedObjectAdapter.Object, selected);
                nakedObjectAdapter = nakedObjectManager.CreateAdapter(newResult, null, null);
            }

            nakedObjectAdapter.SetATransientOid(this);
            return nakedObjectAdapter;
        }

        #endregion

        #region IEncodedToStrings Members

        public string[] ToEncodedStrings() {
            var helper = new StringEncoderHelper {Encode = true};
            helper.Add(Action.ReturnSpec.EncodeTypeName(Action.ReturnSpec.IsCollection ? new[] {Action.ElementSpec} : new IObjectSpec[] { }));
            helper.Add(Action.Id);
            helper.Add(Target.Oid as IEncodedToStrings);

            foreach (var parameter in Parameters) {
                if (parameter == null) {
                    helper.Add(ParameterType.Value);
                    helper.Add((object) null);
                }
                else if (parameter.Spec.IsParseable) {
                    helper.Add(ParameterType.Value);
                    helper.Add(parameter.Object);
                }
                else if (parameter.Spec.IsCollection) {
                    var instanceSpec = parameter.Spec.GetFacet<ITypeOfFacet>().GetValueSpec(parameter, metamodel.Metamodel);
                    var instanceType = TypeUtils.GetType(instanceSpec.FullName);

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

        public string[] ToShortEncodedStrings() => ToEncodedStrings();

        #endregion

        private INakedObjectAdapter RestoreObject(IOid oid) =>
            oid switch {
                _ when oid.IsTransient => lifecycleManager.RecreateInstance(oid, oid.Spec),
                IViewModelOid _ => lifecycleManager.GetViewModel(oid, framework),
                _ => lifecycleManager.LoadObject(oid, oid.Spec)
            };
    }
}
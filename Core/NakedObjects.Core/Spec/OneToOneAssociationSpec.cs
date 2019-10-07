// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Interactions;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Core.Interactions;
using NakedObjects.Core.Resolve;
using NakedObjects.Core.Util;

namespace NakedObjects.Core.Spec {
    public sealed class OneToOneAssociationSpec : AssociationSpecAbstract, IOneToOneAssociationSpec {
        private readonly IObjectPersistor persistor;
        private readonly ITransactionManager transactionManager;
        private bool? isFindMenuEnabled;

        public OneToOneAssociationSpec(IMetamodelManager metamodel, IOneToOneAssociationSpecImmutable association, ISession session, ILifecycleManager lifecycleManager, INakedObjectManager manager, IObjectPersistor persistor, ITransactionManager transactionManager)
            : base(metamodel, association, session, lifecycleManager, manager) {
            this.persistor = persistor;
            this.transactionManager = transactionManager;
        }

        public override IObjectSpec ElementSpec {
            get { return null; }
        }

        #region IOneToOneAssociationSpec Members

        public override bool IsChoicesEnabled {
            get { return ReturnSpec.IsBoundedSet() || ContainsFacet<IPropertyChoicesFacet>() || ContainsFacet<IEnumFacet>(); }
        }

        public override bool IsMandatory {
            get {
                var mandatoryFacet = GetFacet<IMandatoryFacet>();
                return mandatoryFacet.IsMandatory;
            }
        }

        public bool IsFindMenuEnabled {
            get {
                if (!isFindMenuEnabled.HasValue) {
                    isFindMenuEnabled = ContainsFacet<IFindMenuFacet>();
                }

                return isFindMenuEnabled.Value;
            }
        }

        public override INakedObjectAdapter GetNakedObject(INakedObjectAdapter fromObjectAdapter) {
            return GetAssociation(fromObjectAdapter);
        }

        public override Tuple<string, IObjectSpec>[] GetChoicesParameters() {
            var propertyChoicesFacet = GetFacet<IPropertyChoicesFacet>();
            return propertyChoicesFacet == null ? new Tuple<string, IObjectSpec>[] { } : propertyChoicesFacet.ParameterNamesAndTypes.Select(t => new Tuple<string, IObjectSpec>(t.Item1, MetamodelManager.GetSpecification(t.Item2))).ToArray();
        }

        public override INakedObjectAdapter[] GetChoices(INakedObjectAdapter target, IDictionary<string, INakedObjectAdapter> parameterNameValues) {
            var propertyChoicesFacet = GetFacet<IPropertyChoicesFacet>();
            var enumFacet = GetFacet<IEnumFacet>();

            object[] objectOptions = propertyChoicesFacet == null ? null : propertyChoicesFacet.GetChoices(target, parameterNameValues);
            if (objectOptions != null) {
                if (enumFacet == null) {
                    return Manager.GetCollectionOfAdaptedObjects(objectOptions).ToArray();
                }

                return Manager.GetCollectionOfAdaptedObjects(enumFacet.GetChoices(target, objectOptions)).ToArray();
            }

            objectOptions = enumFacet == null ? null : enumFacet.GetChoices(target);
            if (objectOptions != null) {
                return Manager.GetCollectionOfAdaptedObjects(objectOptions).ToArray();
            }

            if (ReturnSpec.IsBoundedSet()) {
                return Manager.GetCollectionOfAdaptedObjects(persistor.GetBoundedSet(ReturnSpec)).ToArray();
            }

            return null;
        }

        public override INakedObjectAdapter[] GetCompletions(INakedObjectAdapter target, string autoCompleteParm) {
            var propertyAutoCompleteFacet = GetFacet<IAutoCompleteFacet>();
            return propertyAutoCompleteFacet == null ? null : Manager.GetCollectionOfAdaptedObjects(propertyAutoCompleteFacet.GetCompletions(target, autoCompleteParm)).ToArray();
        }

        public void InitAssociation(INakedObjectAdapter inObjectAdapter, INakedObjectAdapter associate) {
            var initializerFacet = GetFacet<IPropertyInitializationFacet>();
            if (initializerFacet != null) {
                initializerFacet.InitProperty(inObjectAdapter, associate);
            }
        }

        public IConsent IsAssociationValid(INakedObjectAdapter inObjectAdapter, INakedObjectAdapter reference) {
            if (reference != null && !reference.Spec.IsOfType(ReturnSpec)) {
                return GetConsent(string.Format(Resources.NakedObjects.TypeMismatchError, ReturnSpec.SingularName));
            }

            if (!inObjectAdapter.ResolveState.IsNotPersistent()) {
                if (reference != null && !reference.Spec.IsParseable && reference.ResolveState.IsNotPersistent()) {
                    return GetConsent(Resources.NakedObjects.TransientFieldMessage);
                }
            }

            var buf = new InteractionBuffer();
            IInteractionContext ic = InteractionContext.ModifyingPropParam(Session, false, inObjectAdapter, Identifier, reference);
            InteractionUtils.IsValid(this, ic, buf);
            return InteractionUtils.IsValid(buf);
        }

        public override bool IsEmpty(INakedObjectAdapter inObjectAdapter) {
            return GetAssociation(inObjectAdapter) == null;
        }

        public override bool IsInline {
            get { return ReturnSpec.ContainsFacet(typeof(IComplexTypeFacet)); }
        }

        public override INakedObjectAdapter GetDefault(INakedObjectAdapter fromObjectAdapter) {
            return GetDefaultObject(fromObjectAdapter).Item1;
        }

        public override TypeOfDefaultValue GetDefaultType(INakedObjectAdapter fromObjectAdapter) {
            return GetDefaultObject(fromObjectAdapter).Item2;
        }

        public override void ToDefault(INakedObjectAdapter inObjectAdapter) {
            INakedObjectAdapter defaultValue = GetDefault(inObjectAdapter);
            if (defaultValue != null) {
                InitAssociation(inObjectAdapter, defaultValue);
            }
        }

        public void SetAssociation(INakedObjectAdapter inObjectAdapter, INakedObjectAdapter associate) {
            INakedObjectAdapter currentValue = GetAssociation(inObjectAdapter);
            if (currentValue != associate) {
                var setterFacet = GetFacet<IPropertySetterFacet>();
                if (setterFacet != null) {
                    inObjectAdapter.ResolveState.CheckCanAssociate(associate);
                    setterFacet.SetProperty(inObjectAdapter, associate, transactionManager, Session, LifecycleManager);
                }
            }
        }

        #endregion

        private INakedObjectAdapter GetAssociation(INakedObjectAdapter fromObjectAdapter) {
            object obj = GetFacet<IPropertyAccessorFacet>().GetProperty(fromObjectAdapter);
            if (obj == null) {
                return null;
            }

            var spec = (IObjectSpec) MetamodelManager.GetSpecification(obj.GetType());
            if (spec.ContainsFacet(typeof(IComplexTypeFacet))) {
                return Manager.CreateAggregatedAdapter(fromObjectAdapter, ((IAssociationSpec) this).Id, obj);
            }

            return Manager.CreateAdapter(obj, null, null);
        }

        private Tuple<INakedObjectAdapter, TypeOfDefaultValue> GetDefaultObject(INakedObjectAdapter fromObjectAdapter) {
            Tuple<object, TypeOfDefaultValue> defaultValue = null;

            // Check Facet on property, then facet on type finally fall back on type; 

            var defaultsFacet = GetFacet<IPropertyDefaultFacet>();
            if (defaultsFacet != null && !defaultsFacet.IsNoOp) {
                defaultValue = new Tuple<object, TypeOfDefaultValue>(defaultsFacet.GetDefault(fromObjectAdapter), TypeOfDefaultValue.Explicit);
            }

            // only use the default from the DefaultedFacet if not nullable
            if (defaultValue == null && !IsNullable) {
                var defaultFacet = ReturnSpec.GetFacet<IDefaultedFacet>();
                if (defaultFacet != null && !defaultFacet.IsNoOp) {
                    defaultValue = new Tuple<object, TypeOfDefaultValue>(defaultFacet.Default, TypeOfDefaultValue.Implicit);
                }
            }

            if (defaultValue == null) {
                object rawValue = fromObjectAdapter == null ? null : fromObjectAdapter.Object.GetType().IsValueType ? (object) 0 : null;
                defaultValue = new Tuple<object, TypeOfDefaultValue>(rawValue, TypeOfDefaultValue.Implicit);
            }

            return new Tuple<INakedObjectAdapter, TypeOfDefaultValue>(Manager.CreateAdapter(defaultValue.Item1, null, null), defaultValue.Item2);
        }

        public override string ToString() {
            var str = new AsString(this);
            str.Append(base.ToString());
            str.AddComma();
            str.Append("persisted", IsPersisted);
            str.Append("type", ReturnSpec.ShortName);
            return str.ToString();
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}
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
using NakedObjects.Core.Resolve;
using NakedObjects.Core.Util;

namespace NakedObjects.Core.Spec {
    internal class OneToOneAssociationSpec : AssociationSpecAbstract, IOneToOneAssociationSpec {
        private readonly IObjectPersistor persistor;
        private readonly ITransactionManager transactionManager;

        public OneToOneAssociationSpec(IMetamodelManager metamodel, IOneToOneAssociationSpecImmutable association, ISession session, ILifecycleManager lifecycleManager, INakedObjectManager manager, IObjectPersistor persistor, ITransactionManager transactionManager)
            : base(metamodel, association, session, lifecycleManager, manager) {
            this.persistor = persistor;
            this.transactionManager = transactionManager;
        }

        #region IOneToOneAssociationSpec Members

        public override IObjectSpec ElementSpec {
            get { return null; }
        }

        public override bool IsChoicesEnabled {
            get { return ReturnSpec.IsBoundedSet() || ContainsFacet<IPropertyChoicesFacet>() || ContainsFacet<IEnumFacet>(); }
        }

        public override bool IsMandatory {
            get {
                var mandatoryFacet = GetFacet<IMandatoryFacet>();
                return mandatoryFacet.IsMandatory;
            }
        }

        public override INakedObject GetNakedObject(INakedObject fromObject) {
            return GetAssociation(fromObject);
        }

        public override Tuple<string, IObjectSpec>[] GetChoicesParameters() {
            var propertyChoicesFacet = GetFacet<IPropertyChoicesFacet>();
            return propertyChoicesFacet == null ? new Tuple<string, IObjectSpec>[] {} :
                propertyChoicesFacet.ParameterNamesAndTypes.Select(t => new Tuple<string, IObjectSpec>(t.Item1, (IObjectSpec) MetamodelManager.GetSpecification(t.Item2))).ToArray();
        }

        public override INakedObject[] GetChoices(INakedObject target, IDictionary<string, INakedObject> parameterNameValues) {
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

        public override INakedObject[] GetCompletions(INakedObject target, string autoCompleteParm) {
            var propertyAutoCompleteFacet = GetFacet<IAutoCompleteFacet>();
            return propertyAutoCompleteFacet == null ? null : Manager.GetCollectionOfAdaptedObjects(propertyAutoCompleteFacet.GetCompletions(target, autoCompleteParm)).ToArray();
        }

        public virtual void InitAssociation(INakedObject inObject, INakedObject associate) {
            var initializerFacet = GetFacet<IPropertyInitializationFacet>();
            if (initializerFacet != null) {
                initializerFacet.InitProperty(inObject, associate);
            }
        }

        public virtual IConsent IsAssociationValid(INakedObject inObject, INakedObject reference) {
            if (reference != null && !reference.Spec.IsOfType(ReturnSpec)) {
                return GetConsent(string.Format(Resources.NakedObjects.TypeMismatchError, ReturnSpec.SingularName));
            }

            if (!inObject.ResolveState.IsNotPersistent()) {
                if (reference != null && !reference.Spec.IsParseable && reference.ResolveState.IsNotPersistent()) {
                    return GetConsent(Resources.NakedObjects.TransientFieldMessage);
                }
            }

            var buf = new InteractionBuffer();
            InteractionContext ic = InteractionContext.ModifyingPropParam(Session, false, inObject, Identifier, reference);
            InteractionUtils.IsValid(this, ic, buf);
            return InteractionUtils.IsValid(buf);
        }

        public override bool IsEmpty(INakedObject inObject) {
            return GetAssociation(inObject) == null;
        }

        public override bool IsInline {
            get { return ReturnSpec.ContainsFacet(typeof (IComplexTypeFacet)); }
        }

        public override INakedObject GetDefault(INakedObject fromObject) {
            return GetDefaultObject(fromObject).Item1;
        }

        public override TypeOfDefaultValue GetDefaultType(INakedObject fromObject) {
            return GetDefaultObject(fromObject).Item2;
        }

        public override void ToDefault(INakedObject inObject) {
            INakedObject defaultValue = GetDefault(inObject);
            if (defaultValue != null) {
                InitAssociation(inObject, defaultValue);
            }
        }

        public virtual void SetAssociation(INakedObject inObject, INakedObject associate) {
            INakedObject currentValue = GetAssociation(inObject);
            if (currentValue != associate) {
                if (associate == null && ContainsFacet<IPropertyClearFacet>()) {
                    GetFacet<IPropertyClearFacet>().ClearProperty(inObject, transactionManager);
                }
                else {
                    var setterFacet = GetFacet<IPropertySetterFacet>();
                    if (setterFacet != null) {
                        inObject.ResolveState.CheckCanAssociate(associate);
                        setterFacet.SetProperty(inObject, associate, transactionManager);
                    }
                }
            }
        }

        #endregion

        private INakedObject GetAssociation(INakedObject fromObject) {
            object obj = GetFacet<IPropertyAccessorFacet>().GetProperty(fromObject);
            if (obj == null) {
                return null;
            }
            var spec = (IObjectSpec) MetamodelManager.GetSpecification(obj.GetType());
            if (spec.ContainsFacet(typeof (IComplexTypeFacet))) {
                return Manager.CreateAggregatedAdapter(fromObject, ((IAssociationSpec) this).Id, obj);
            }
            return Manager.CreateAdapter(obj, null, null);
        }

        public virtual Tuple<INakedObject, TypeOfDefaultValue> GetDefaultObject(INakedObject fromObject) {
            Tuple<object, TypeOfDefaultValue> defaultValue = null;

            // Check Facet on property, then facet on type finally fall back on type; 

            var defaultsFacet = GetFacet<IPropertyDefaultFacet>();
            if (defaultsFacet != null && !defaultsFacet.IsNoOp) {
                defaultValue = new Tuple<object, TypeOfDefaultValue>(defaultsFacet.GetDefault(fromObject), TypeOfDefaultValue.Explicit);
            }

            if (defaultValue == null) {
                var defaultFacet = ReturnSpec.GetFacet<IDefaultedFacet>();
                if (defaultFacet != null && !defaultFacet.IsNoOp) {
                    defaultValue = new Tuple<object, TypeOfDefaultValue>(defaultFacet.Default, TypeOfDefaultValue.Implicit);
                }
            }

            if (defaultValue == null) {
                object rawValue = fromObject == null ? null : fromObject.Object.GetType().IsValueType ? (object) 0 : null;
                defaultValue = new Tuple<object, TypeOfDefaultValue>(rawValue, TypeOfDefaultValue.Implicit);
            }

            return new Tuple<INakedObject, TypeOfDefaultValue>(Manager.CreateAdapter(defaultValue.Item1, null, null), defaultValue.Item2);
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
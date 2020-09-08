// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

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
        private readonly ITransactionManager transactionManager;
        private bool? isFindMenuEnabled;

        public OneToOneAssociationSpec(IMetamodelManager metamodel, IOneToOneAssociationSpecImmutable association, ISession session, ILifecycleManager lifecycleManager, INakedObjectManager manager, IObjectPersistor persistor, ITransactionManager transactionManager)
            : base(metamodel, association, session, lifecycleManager, manager, persistor) {
            this.transactionManager = transactionManager;
        }

        public override IObjectSpec ElementSpec => null;

        #region IOneToOneAssociationSpec Members

        public bool IsChoicesEnabled => ReturnSpec.IsBoundedSet() || ContainsFacet<IPropertyChoicesFacet>() || ContainsFacet<IEnumFacet>();

        public override bool IsMandatory {
            get {
                var mandatoryFacet = GetFacet<IMandatoryFacet>();
                return mandatoryFacet.IsMandatory;
            }
        }

        public bool IsFindMenuEnabled {
            get {
                isFindMenuEnabled ??= ContainsFacet<IFindMenuFacet>();

                return isFindMenuEnabled.Value;
            }
        }

        public override INakedObjectAdapter GetNakedObject(INakedObjectAdapter fromObjectAdapter) => GetAssociation(fromObjectAdapter);

        public (string, IObjectSpec)[] GetChoicesParameters() {
            var propertyChoicesFacet = GetFacet<IPropertyChoicesFacet>();
            return propertyChoicesFacet == null
                ? new (string, IObjectSpec)[] { }
                : propertyChoicesFacet.ParameterNamesAndTypes.Select(t => {
                    var (pName, pSpec) = t;
                    return (pName, MetamodelManager.GetSpecification(pSpec));
                }).ToArray();
        }

        public INakedObjectAdapter[] GetChoices(INakedObjectAdapter target, IDictionary<string, INakedObjectAdapter> parameterNameValues) {
            var propertyChoicesFacet = GetFacet<IPropertyChoicesFacet>();
            var enumFacet = GetFacet<IEnumFacet>();

            var objectOptions = propertyChoicesFacet == null ? null : propertyChoicesFacet.GetChoices(target, parameterNameValues);
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
                return Manager.GetCollectionOfAdaptedObjects(Persistor.GetBoundedSet(ReturnSpec)).ToArray();
            }

            return null;
        }

        public override INakedObjectAdapter[] GetCompletions(INakedObjectAdapter target, string autoCompleteParm) {
            var propertyAutoCompleteFacet = GetFacet<IAutoCompleteFacet>();
            return propertyAutoCompleteFacet == null ? null : Manager.GetCollectionOfAdaptedObjects(propertyAutoCompleteFacet.GetCompletions(target, autoCompleteParm, Session, Persistor)).ToArray();
        }

        public void InitAssociation(INakedObjectAdapter inObjectAdapter, INakedObjectAdapter associate) {
            var initializerFacet = GetFacet<IPropertyInitializationFacet>();
            initializerFacet?.InitProperty(inObjectAdapter, associate);
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
            IInteractionContext ic = InteractionContext.ModifyingPropParam(Session, Persistor,false, inObjectAdapter, Identifier, reference);
            InteractionUtils.IsValid(this, ic, buf);
            return InteractionUtils.IsValid(buf);
        }

        public override bool IsEmpty(INakedObjectAdapter inObjectAdapter) => GetAssociation(inObjectAdapter) == null;

        public override bool IsInline => ReturnSpec.ContainsFacet(typeof(IComplexTypeFacet));

        public override INakedObjectAdapter GetDefault(INakedObjectAdapter fromObjectAdapter) => GetDefaultObject(fromObjectAdapter).value;

        public override TypeOfDefaultValue GetDefaultType(INakedObjectAdapter fromObjectAdapter) => GetDefaultObject(fromObjectAdapter).type;

        public override void ToDefault(INakedObjectAdapter inObjectAdapter) {
            var defaultValue = GetDefault(inObjectAdapter);
            if (defaultValue != null) {
                InitAssociation(inObjectAdapter, defaultValue);
            }
        }

        public void SetAssociation(INakedObjectAdapter inObjectAdapter, INakedObjectAdapter associate) {
            var currentValue = GetAssociation(inObjectAdapter);
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
            var obj = GetFacet<IPropertyAccessorFacet>().GetProperty(fromObjectAdapter);
            if (obj == null) {
                return null;
            }

            var spec = (IObjectSpec) MetamodelManager.GetSpecification(obj.GetType());
            return spec.ContainsFacet(typeof(IComplexTypeFacet))
                ? Manager.CreateAggregatedAdapter(fromObjectAdapter, Id, obj)
                : Manager.CreateAdapter(obj, null, null);
        }

        private (INakedObjectAdapter value, TypeOfDefaultValue type) GetDefaultObject(INakedObjectAdapter fromObjectAdapter) {
            var facet = this.GetOpFacet<IPropertyDefaultFacet>() ?? ReturnSpec.GetOpFacet<IDefaultedFacet>();

            var (domainObject, typeOfDefaultValue) = facet switch {
                IPropertyDefaultFacet pdf => (pdf.GetDefault(fromObjectAdapter), TypeOfDefaultValue.Explicit),
                IDefaultedFacet df when !IsNullable => (df.Default, TypeOfDefaultValue.Implicit),
                _ when fromObjectAdapter == null => (null, TypeOfDefaultValue.Implicit),
                _ when fromObjectAdapter.Object.GetType().IsValueType => (0, TypeOfDefaultValue.Implicit),
                _ => (null, TypeOfDefaultValue.Implicit)
            };

            return (Manager.CreateAdapter(domainObject, null, null), typeOfDefaultValue);
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
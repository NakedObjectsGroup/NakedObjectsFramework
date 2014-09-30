// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections.Generic;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets.AutoComplete;
using NakedObjects.Architecture.Facets.Objects.Aggregated;
using NakedObjects.Architecture.Facets.Properties.Access;
using NakedObjects.Architecture.Facets.Properties.Choices;
using NakedObjects.Architecture.Facets.Properties.Defaults;
using NakedObjects.Architecture.Facets.Properties.Enums;
using NakedObjects.Architecture.Facets.Properties.Modify;
using NakedObjects.Architecture.Facets.Propparam.Validate.Mandatory;
using NakedObjects.Architecture.Interactions;
using NakedObjects.Architecture.Persist;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Resolve;
using NakedObjects.Architecture.Security;
using NakedObjects.Architecture.Spec;
using NakedObjects.Core.Util;
using NakedObjects.Reflector.Peer;

namespace NakedObjects.Reflector.Spec {
    public class OneToOneAssociationImpl : NakedObjectAssociationAbstract, IOneToOneAssociation {
        private readonly INakedObjectReflector reflector;
        private readonly INakedObjectAssociationPeer reflectiveAdapter;

        public OneToOneAssociationImpl(INakedObjectReflector reflector, INakedObjectAssociationPeer association)
            : base(association.Identifier.MemberName, association.Specification, association) {
            this.reflector = reflector;
            reflectiveAdapter = association;
        }

        #region IOneToOneAssociation Members

        public override bool IsObject {
            get { return true; }
        }

        public override bool IsChoicesEnabled {
            get { return Specification.IsBoundedSet() || ContainsFacet<IPropertyChoicesFacet>() || ContainsFacet<IEnumFacet>(); }
        }

        public override bool IsMandatory {
            get {
                var mandatoryFacet = GetFacet<IMandatoryFacet>();
                return mandatoryFacet.IsMandatory;
            }
        }

        public override INakedObject GetNakedObject(INakedObject fromObject, INakedObjectManager manager) {
            return GetAssociation(fromObject, manager);
        }

        public override Tuple<string, INakedObjectSpecification>[] GetChoicesParameters() {
            var propertyChoicesFacet = GetFacet<IPropertyChoicesFacet>();
            return propertyChoicesFacet != null ? propertyChoicesFacet.ParameterNamesAndTypes : new Tuple<string, INakedObjectSpecification>[] {};
        }

        public override INakedObject[] GetChoices(INakedObject target, IDictionary<string, INakedObject> parameterNameValues, ILifecycleManager persistor) {
            var propertyChoicesFacet = GetFacet<IPropertyChoicesFacet>();
            var enumFacet = GetFacet<IEnumFacet>();

            object[] objectOptions = propertyChoicesFacet == null ? null : propertyChoicesFacet.GetChoices(target, parameterNameValues);
            if (objectOptions != null) {
                if (enumFacet == null) {
                    return persistor .GetCollectionOfAdaptedObjects(objectOptions).ToArray();
                }
                return persistor.GetCollectionOfAdaptedObjects(enumFacet.GetChoices(target, objectOptions)).ToArray();
            }

            objectOptions = enumFacet == null ? null : enumFacet.GetChoices(target);
            if (objectOptions != null) {
                return persistor.GetCollectionOfAdaptedObjects(objectOptions).ToArray();
            }

            if (Specification.IsBoundedSet()) {
                return persistor.GetCollectionOfAdaptedObjects(Specification.GetBoundedSet(persistor)).ToArray();
            }
            return null;
        }

        public override INakedObject[] GetCompletions(INakedObject target, string autoCompleteParm, ILifecycleManager persistor) {
            var propertyAutoCompleteFacet = GetFacet<IAutoCompleteFacet>();
            return propertyAutoCompleteFacet == null ? null : persistor.GetCollectionOfAdaptedObjects(propertyAutoCompleteFacet.GetCompletions(target, autoCompleteParm)).ToArray();
        }

        public virtual void InitAssociation(INakedObject inObject, INakedObject associate) {
            var initializerFacet = GetFacet<IPropertyInitializationFacet>();
            if (initializerFacet != null) {
                initializerFacet.InitProperty(inObject, associate);
            }
        }

        public virtual IConsent IsAssociationValid(INakedObject inObject, INakedObject reference, ISession session) {
            if (reference != null && !reference.Specification.IsOfType(Specification)) {
                return GetConsent(string.Format(Resources.NakedObjects.TypeMismatchError, Specification.SingularName));
            }

            if (!inObject.ResolveState.IsNotPersistent()) {
                if (reference != null && !reference.Specification.IsParseable && reference.ResolveState.IsNotPersistent()) {
                    return GetConsent(Resources.NakedObjects.TransientFieldMessage);
                }
            }

            var buf = new InteractionBuffer();
            InteractionContext ic = InteractionContext.ModifyingPropParam(session, false, inObject, Identifier, reference);
            InteractionUtils.IsValid(this, ic, buf);
            return InteractionUtils.IsValid(buf);
        }

        public override bool IsEmpty(INakedObject inObject, INakedObjectManager manager, IObjectPersistor persistor) {
            return GetAssociation(inObject, manager) == null;
        }

        public override bool IsInline {
            get { return Specification.ContainsFacet(typeof (IComplexTypeFacet)); }
        }

        public override INakedObject GetDefault(INakedObject fromObject, INakedObjectManager manager) {
            return GetDefaultObject(fromObject, manager).Item1;
        }

        public override TypeOfDefaultValue GetDefaultType(INakedObject fromObject, INakedObjectManager manager) {
            return GetDefaultObject(fromObject, manager).Item2;
        }

        public override void ToDefault(INakedObject inObject, INakedObjectManager manager) {
            INakedObject defaultValue = GetDefault(inObject, manager);
            if (defaultValue != null) {
                InitAssociation(inObject, defaultValue);
            }
        }

        public virtual void SetAssociation(INakedObject inObject, INakedObject associate, ILifecycleManager persistor) {
            INakedObject currentValue = GetAssociation(inObject, persistor);
            if (currentValue != associate) {
                if (associate == null && ContainsFacet<IPropertyClearFacet>()) {
                    GetFacet<IPropertyClearFacet>().ClearProperty(inObject, persistor);
                }
                else {
                    var setterFacet = GetFacet<IPropertySetterFacet>();
                    if (setterFacet != null) {
                        inObject.ResolveState.CheckCanAssociate(associate);
                        setterFacet.SetProperty(inObject, associate, persistor);
                    }
                }
            }
        }

        #endregion

        private INakedObject GetAssociation(INakedObject fromObject, INakedObjectManager manager) {
            object obj = GetFacet<IPropertyAccessorFacet>().GetProperty(fromObject);
            if (obj == null) {
                return null;
            }
            INakedObjectSpecification specification = reflector.LoadSpecification(obj.GetType());
            if (specification.ContainsFacet(typeof (IComplexTypeFacet))) {
                return manager.CreateAggregatedAdapter(fromObject, ((INakedObjectAssociation)this).Id, obj);
            }
            return manager.CreateAdapter(obj, null, null);
        }

        public virtual Tuple<INakedObject, TypeOfDefaultValue> GetDefaultObject(INakedObject fromObject, INakedObjectManager manager) {
            var typeofDefaultValue = TypeOfDefaultValue.Explicit;

            // look for a default on the association ...
            var propertyDefaultFacet = GetFacet<IPropertyDefaultFacet>();
            // ... if none, attempt to find a default on the specification (eg an int should default to 0).
            if (propertyDefaultFacet == null || propertyDefaultFacet.IsNoOp) {
                typeofDefaultValue = TypeOfDefaultValue.Implicit;
                propertyDefaultFacet = Specification.GetFacet<IPropertyDefaultFacet>();
            }
            if (propertyDefaultFacet == null) {
                return new Tuple<INakedObject, TypeOfDefaultValue>(null, TypeOfDefaultValue.Implicit);
            }
            object obj = propertyDefaultFacet.GetDefault(fromObject);
            return new Tuple<INakedObject, TypeOfDefaultValue>(manager.CreateAdapter(obj, null, null), typeofDefaultValue);
        }

        public override string ToString() {
            var str = new AsString(this);
            str.Append(base.ToString());
            str.AddComma();
            str.Append("persisted", IsPersisted);
            str.Append("type", Specification.ShortName);
            return str.ToString();
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}
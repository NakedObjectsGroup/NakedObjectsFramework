// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Actcoll.Typeof;
using NakedObjects.Architecture.Facets.Actions.Choices;
using NakedObjects.Architecture.Facets.Actions.Defaults;
using NakedObjects.Architecture.Facets.AutoComplete;
using NakedObjects.Architecture.Facets.Naming.DescribedAs;
using NakedObjects.Architecture.Facets.Naming.Named;
using NakedObjects.Architecture.Facets.Properties.Enums;
using NakedObjects.Architecture.Facets.Propparam.Modify;
using NakedObjects.Architecture.Facets.Propparam.Validate.Mandatory;
using NakedObjects.Architecture.Interactions;
using NakedObjects.Architecture.Persist;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Security;
using NakedObjects.Architecture.Spec;
using NakedObjects.Reflector.Peer;

namespace NakedObjects.Reflector.Spec {
    public abstract class NakedObjectActionParameterAbstract : INakedObjectActionParameter {
        private readonly IMetamodel metamodel;
        private readonly int number;
        private readonly INakedObjectAction parentAction;
        private readonly INakedObjectActionParamPeer peer;

        protected internal NakedObjectActionParameterAbstract(IMetamodel metamodel, int number, INakedObjectAction nakedObjectAction, INakedObjectActionParamPeer peer) {
            this.metamodel = metamodel;
            this.number = number;
            parentAction = nakedObjectAction;
            this.peer = peer;
        }

        public bool IsAutoCompleteEnabled {
            get { return ContainsFacet<IAutoCompleteFacet>(); }
        }

        public bool IsChoicesEnabled {
            get { return !IsMultipleChoicesEnabled && (Specification.IsBoundedSet() || ContainsFacet<IActionChoicesFacet>() || ContainsFacet<IEnumFacet>()); }
        }

        public bool IsMultipleChoicesEnabled {
            get { return Specification.IsCollectionOfBoundedSet() || Specification.IsCollectionOfEnum() || (ContainsFacet<IActionChoicesFacet>() && GetFacet<IActionChoicesFacet>().IsMultiple); }
        }

        #region INakedObjectActionParameter Members

        /// <summary>
        ///     Subclasses should override either <see cref="IsObject" /> or <see cref="IsCollection" />
        /// </summary>
        public virtual bool IsObject {
            get { return false; }
        }

        /// <summary>
        ///     Subclasses should override either <see cref="IsObject" /> or <see cref="IsCollection" />
        /// </summary>
        public virtual bool IsCollection {
            get { return false; }
        }

        public virtual int Number {
            get { return number; }
        }

        public virtual INakedObjectAction Action {
            get { return parentAction; }
        }

        public virtual INakedObjectSpecification Specification {
            get {
                return  metamodel.GetSpecification(peer.Specification);
            }
        }

        public string GetName(IServicesManager services) {
            var facet = GetFacet<INamedFacet>();
            string name = facet == null ? null : facet.Value;
            return name ?? Specification.SingularName;
        }

        public virtual string Description {
            get { return GetFacet<IDescribedAsFacet>().Value ?? ""; }
        }

        public virtual bool IsMandatory {
            get {
                var mandatoryFacet = GetFacet<IMandatoryFacet>();
                return mandatoryFacet.IsMandatory;
            }
        }

        public virtual Type[] FacetTypes {
            get { return peer != null ? peer.FacetTypes : new Type[] {}; }
        }

        public virtual IIdentifier Identifier {
            get { return parentAction.Identifier; }
        }

        public virtual bool ContainsFacet(Type facetType) {
            return peer != null && peer.ContainsFacet(facetType);
        }

        public virtual bool ContainsFacet<T>() where T : IFacet {
            return peer != null && peer.ContainsFacet<T>();
        }

        public virtual IFacet GetFacet(Type type) {
            return peer != null ? peer.GetFacet(type) : null;
        }

        public virtual T GetFacet<T>() where T : IFacet {
            return peer != null ? peer.GetFacet<T>() : default(T);
        }

        public virtual IFacet[] GetFacets(IFacetFilter filter) {
            return peer != null ? peer.GetFacets(filter) : new IFacet[] {};
        }

        public virtual void AddFacet(IFacet facet) {
            if (peer != null) {
                peer.AddFacet(facet);
            }
        }

        public virtual void AddFacet(IMultiTypedFacet facet) {
            if (peer != null) {
                peer.AddFacet(facet);
            }
        }

        public virtual void RemoveFacet(IFacet facet) {
            if (peer != null) {
                peer.RemoveFacet(facet);
            }
        }

        public virtual void RemoveFacet(Type facetType) {
            if (peer != null) {
                peer.RemoveFacet(facetType);
            }
        }

        public IConsent IsValid(INakedObject nakedObject, INakedObject proposedValue, ILifecycleManager persistor, ISession session) {
            if (proposedValue != null && !proposedValue.Specification.IsOfType(Specification)) {
                return GetConsent("Not a suitable type; must be a " + Specification.SingularName);
            }

            var buf = new InteractionBuffer();
            InteractionContext ic = InteractionContext.ModifyingPropParam(session, false, parentAction.RealTarget(nakedObject, persistor), Identifier, proposedValue);
            InteractionUtils.IsValid(this, ic, buf);
            return InteractionUtils.IsValid(buf);
        }

        public virtual IConsent IsUsable(ISession session, INakedObject target, ILifecycleManager persistor) {
            return Allow.Default;
        }

        public bool IsNullable {
            get { return ContainsFacet(typeof (INullableFacet)); }
        }

        public Tuple<string, INakedObjectSpecification>[] GetChoicesParameters() {
            var choicesFacet = GetFacet<IActionChoicesFacet>();
            return choicesFacet == null ? new Tuple<string, INakedObjectSpecification>[] {} :
                choicesFacet.ParameterNamesAndTypes.Select(t => new Tuple<string, INakedObjectSpecification>(t.Item1, metamodel.GetSpecification(t.Item2))).ToArray();
        }

        public INakedObject[] GetChoices(INakedObject nakedObject, IDictionary<string, INakedObject> parameterNameValues, ILifecycleManager persistor) {
            var choicesFacet = GetFacet<IActionChoicesFacet>();
            var enumFacet = GetFacet<IEnumFacet>();

            if (choicesFacet != null) {
                object[] options = choicesFacet.GetChoices(parentAction.RealTarget(nakedObject, persistor), parameterNameValues);
                if (enumFacet == null) {
                    return persistor.GetCollectionOfAdaptedObjects(options).ToArray();
                }

                return persistor.GetCollectionOfAdaptedObjects(enumFacet.GetChoices(parentAction.RealTarget(nakedObject, persistor), options)).ToArray();
            }


            if (enumFacet != null) {
                return persistor.GetCollectionOfAdaptedObjects(enumFacet.GetChoices(parentAction.RealTarget(nakedObject, persistor))).ToArray();
            }

            if (Specification.IsBoundedSet()) {
                return persistor.GetCollectionOfAdaptedObjects(persistor.Instances(Specification)).ToArray();
            }

            if (Specification.IsCollectionOfBoundedSet() || Specification.IsCollectionOfEnum()) {
                var instanceSpec =  metamodel.GetSpecification(Specification.GetFacet<ITypeOfFacet>().ValueSpec);

                var instanceEnumFacet = instanceSpec.GetFacet<IEnumFacet>();

                if (instanceEnumFacet != null) {
                    return persistor.GetCollectionOfAdaptedObjects(instanceEnumFacet.GetChoices(parentAction.RealTarget(nakedObject, persistor))).ToArray();
                }

                return persistor.GetCollectionOfAdaptedObjects(persistor.Instances(instanceSpec)).ToArray();
            }

            return null;
        }


        public INakedObject[] GetCompletions(INakedObject nakedObject, string autoCompleteParm, ILifecycleManager persistor) {
            var autoCompleteFacet = GetFacet<IAutoCompleteFacet>();
            return autoCompleteFacet == null ? null : persistor.GetCollectionOfAdaptedObjects(autoCompleteFacet.GetCompletions(parentAction.RealTarget(nakedObject, persistor), autoCompleteParm)).ToArray();
        }

        public INakedObject GetDefault(INakedObject nakedObject, ILifecycleManager persistor) {
            return GetDefaultValueAndType(nakedObject, persistor).Item1;
        }

        public TypeOfDefaultValue GetDefaultType(INakedObject nakedObject, ILifecycleManager persistor) {
            return GetDefaultValueAndType(nakedObject, persistor).Item2;
        }


        public string Id {
            get { return Identifier.MemberParameterNames[Number]; }
        }

        private Tuple<INakedObject, TypeOfDefaultValue> GetDefaultValueAndType(INakedObject nakedObject, ILifecycleManager persistor) {
            if (parentAction.IsContributedMethod && nakedObject != null) {
                IEnumerable<INakedObjectActionParameter> matchingParms = parentAction.Parameters.Where(p => nakedObject.Specification.IsOfType(p.Specification));

                if (matchingParms.Any() && matchingParms.First() == this) {
                    return new Tuple<INakedObject, TypeOfDefaultValue>(nakedObject, TypeOfDefaultValue.Explicit);
                }
            }
            var defaultsFacet = GetFacet<IActionDefaultsFacet>();
            if (defaultsFacet != null) {
                Tuple<object, TypeOfDefaultValue> defaultvalue = defaultsFacet.GetDefault(parentAction.RealTarget(nakedObject, persistor));
                return new Tuple<INakedObject, TypeOfDefaultValue>(persistor.CreateAdapter(defaultvalue.Item1, null, null), defaultvalue.Item2);
            }
            return new Tuple<INakedObject, TypeOfDefaultValue>(null, TypeOfDefaultValue.Implicit);
        }

        #endregion

        protected internal virtual IConsent GetConsent(string message) {
            return message == null ? (IConsent) Allow.Default : new Veto(message);
        }
    }
}
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
using NakedObjects.Core.Util;
using NakedObjects.Reflector.Peer;

namespace NakedObjects.Reflector.Spec {
    public abstract class NakedObjectActionParameterAbstract : INakedObjectActionParameter {
        private readonly INakedObjectManager manager;
        private readonly IMetamodelManager metamodel;
        private readonly int number;
        private readonly INakedObjectAction parentAction;
        private readonly INakedObjectActionParamPeer peer;
        private readonly IObjectPersistor persistor;
        private readonly ISession session;

        protected internal NakedObjectActionParameterAbstract(IMetamodelManager metamodel, int number, INakedObjectAction nakedObjectAction, INakedObjectActionParamPeer peer, INakedObjectManager manager, ISession session, IObjectPersistor persistor) {
            Assert.AssertNotNull(metamodel);
            Assert.AssertNotNull(nakedObjectAction);
            Assert.AssertNotNull(peer);
            Assert.AssertNotNull(manager);
            Assert.AssertNotNull(session);
            Assert.AssertNotNull(persistor);

            this.metamodel = metamodel;
            this.number = number;
            parentAction = nakedObjectAction;
            this.peer = peer;
            this.manager = manager;
            this.session = session;
            this.persistor = persistor;
        }

        public INakedObjectManager Manager {
            get { return manager; }
        }

        #region INakedObjectActionParameter Members

        public bool IsAutoCompleteEnabled {
            get { return ContainsFacet<IAutoCompleteFacet>(); }
        }

        public bool IsChoicesEnabled {
            get { return !IsMultipleChoicesEnabled && (Specification.IsBoundedSet() || ContainsFacet<IActionChoicesFacet>() || ContainsFacet<IEnumFacet>()); }
        }

        public bool IsMultipleChoicesEnabled {
            get { return Specification.IsCollectionOfBoundedSet() || Specification.IsCollectionOfEnum() || (ContainsFacet<IActionChoicesFacet>() && GetFacet<IActionChoicesFacet>().IsMultiple); }
        }

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
            get { return metamodel.GetSpecification(peer.Specification); }
        }

        public string GetName() {
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

        public virtual IEnumerable<IFacet> GetFacets() {
            return peer != null ? peer.GetFacets() : new IFacet[] {};
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

        public IConsent IsValid(INakedObject nakedObject, INakedObject proposedValue) {
            if (proposedValue != null && !proposedValue.Specification.IsOfType(Specification)) {
                return GetConsent("Not a suitable type; must be a " + Specification.SingularName);
            }

            var buf = new InteractionBuffer();
            InteractionContext ic = InteractionContext.ModifyingPropParam(session, false, parentAction.RealTarget(nakedObject), Identifier, proposedValue);
            InteractionUtils.IsValid(this, ic, buf);
            return InteractionUtils.IsValid(buf);
        }

        public virtual IConsent IsUsable(INakedObject target) {
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

        public INakedObject[] GetChoices(INakedObject nakedObject, IDictionary<string, INakedObject> parameterNameValues) {
            var choicesFacet = GetFacet<IActionChoicesFacet>();
            var enumFacet = GetFacet<IEnumFacet>();

            if (choicesFacet != null) {
                object[] options = choicesFacet.GetChoices(parentAction.RealTarget(nakedObject), parameterNameValues);
                if (enumFacet == null) {
                    return Manager.GetCollectionOfAdaptedObjects(options).ToArray();
                }

                return Manager.GetCollectionOfAdaptedObjects(enumFacet.GetChoices(parentAction.RealTarget(nakedObject), options)).ToArray();
            }


            if (enumFacet != null) {
                return Manager.GetCollectionOfAdaptedObjects(enumFacet.GetChoices(parentAction.RealTarget(nakedObject))).ToArray();
            }

            if (Specification.IsBoundedSet()) {
                return Manager.GetCollectionOfAdaptedObjects(persistor.Instances(Specification)).ToArray();
            }

            if (Specification.IsCollectionOfBoundedSet() || Specification.IsCollectionOfEnum()) {
                var instanceSpec = metamodel.GetSpecification(Specification.GetFacet<ITypeOfFacet>().ValueSpec);

                var instanceEnumFacet = instanceSpec.GetFacet<IEnumFacet>();

                if (instanceEnumFacet != null) {
                    return Manager.GetCollectionOfAdaptedObjects(instanceEnumFacet.GetChoices(parentAction.RealTarget(nakedObject))).ToArray();
                }

                return Manager.GetCollectionOfAdaptedObjects(persistor.Instances(instanceSpec)).ToArray();
            }

            return null;
        }


        public INakedObject[] GetCompletions(INakedObject nakedObject, string autoCompleteParm) {
            var autoCompleteFacet = GetFacet<IAutoCompleteFacet>();
            return autoCompleteFacet == null ? null : Manager.GetCollectionOfAdaptedObjects(autoCompleteFacet.GetCompletions(parentAction.RealTarget(nakedObject), autoCompleteParm)).ToArray();
        }

        public INakedObject GetDefault(INakedObject nakedObject) {
            return GetDefaultValueAndType(nakedObject).Item1;
        }

        public TypeOfDefaultValue GetDefaultType(INakedObject nakedObject) {
            return GetDefaultValueAndType(nakedObject).Item2;
        }


        public string Id {
            get { return Identifier.MemberParameterNames[Number]; }
        }

        #endregion

        private Tuple<INakedObject, TypeOfDefaultValue> GetDefaultValueAndType(INakedObject nakedObject) {
            if (parentAction.IsContributedMethod && nakedObject != null) {
                IEnumerable<INakedObjectActionParameter> matchingParms = parentAction.Parameters.Where(p => nakedObject.Specification.IsOfType(p.Specification));

                if (matchingParms.Any() && matchingParms.First() == this) {
                    return new Tuple<INakedObject, TypeOfDefaultValue>(nakedObject, TypeOfDefaultValue.Explicit);
                }
            }
            var defaultsFacet = GetFacet<IActionDefaultsFacet>();
            if (defaultsFacet != null) {
                Tuple<object, TypeOfDefaultValue> defaultvalue = defaultsFacet.GetDefault(parentAction.RealTarget(nakedObject));
                return new Tuple<INakedObject, TypeOfDefaultValue>(Manager.CreateAdapter(defaultvalue.Item1, null, null), defaultvalue.Item2);
            }
            return new Tuple<INakedObject, TypeOfDefaultValue>(null, TypeOfDefaultValue.Implicit);
        }

        protected internal virtual IConsent GetConsent(string message) {
            return message == null ? (IConsent) Allow.Default : new Veto(message);
        }
    }
}
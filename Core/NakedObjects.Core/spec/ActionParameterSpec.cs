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
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Interactions;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Core.Reflect;
using NakedObjects.Core.Util;

namespace NakedObjects.Core.Spec {
    public abstract class ActionParameterSpec : IActionParameterSpec {
        private readonly IActionParameterSpecImmutable actionParameterSpecImmutable;
        private readonly INakedObjectManager manager;
        private readonly IMetamodelManager metamodel;
        private readonly int number;
        private readonly IActionSpec parentAction;
        private readonly IObjectPersistor persistor;
        private readonly ISession session;

        protected internal ActionParameterSpec(IMetamodelManager metamodel, int number, IActionSpec actionSpec, IActionParameterSpecImmutable actionParameterSpecImmutable, INakedObjectManager manager, ISession session, IObjectPersistor persistor) {
            Assert.AssertNotNull(metamodel);
            Assert.AssertNotNull(actionSpec);
            Assert.AssertNotNull(actionParameterSpecImmutable);
            Assert.AssertNotNull(manager);
            Assert.AssertNotNull(session);
            Assert.AssertNotNull(persistor);

            this.metamodel = metamodel;
            this.number = number;
            parentAction = actionSpec;
            this.actionParameterSpecImmutable = actionParameterSpecImmutable;
            this.manager = manager;
            this.session = session;
            this.persistor = persistor;
        }

        protected INakedObjectManager Manager {
            get { return manager; }
        }

        #region IActionParameterSpec Members

        public bool IsAutoCompleteEnabled {
            get { return ContainsFacet<IAutoCompleteFacet>(); }
        }

        public bool IsChoicesEnabled {
            get { return !IsMultipleChoicesEnabled && (Spec.IsBoundedSet() || ContainsFacet<IActionChoicesFacet>() || ContainsFacet<IEnumFacet>()); }
        }

        public bool IsMultipleChoicesEnabled {
            get {
                return Spec.IsCollectionOfBoundedSet(ElementSpec) ||
                       Spec.IsCollectionOfEnum(ElementSpec) ||
                       (ContainsFacet<IActionChoicesFacet>() && GetFacet<IActionChoicesFacet>().IsMultiple);
            }
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

        public virtual IActionSpec Action {
            get { return parentAction; }
        }

        public virtual IObjectSpec Spec {
            get { return metamodel.GetSpecification(actionParameterSpecImmutable.Specification); }
        }

        public virtual IObjectSpec ElementSpec {
            get {
                var facet = GetFacet<IElementTypeFacet>();
                var spec = facet != null ? facet.ValueSpec : null;
                return spec == null ? null : metamodel.GetSpecification(spec);
            }
        }

        public string GetName() {
            var facet = GetFacet<INamedFacet>();
            string name = facet == null ? null : facet.Value;
            return name ?? Spec.SingularName;
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
            get { return actionParameterSpecImmutable != null ? actionParameterSpecImmutable.FacetTypes : new Type[] {}; }
        }

        public virtual IIdentifier Identifier {
            get { return parentAction.Identifier; }
        }

        public virtual bool ContainsFacet(Type facetType) {
            return actionParameterSpecImmutable != null && actionParameterSpecImmutable.ContainsFacet(facetType);
        }

        public virtual bool ContainsFacet<T>() where T : IFacet {
            return actionParameterSpecImmutable != null && actionParameterSpecImmutable.ContainsFacet<T>();
        }

        public virtual IFacet GetFacet(Type type) {
            return actionParameterSpecImmutable != null ? actionParameterSpecImmutable.GetFacet(type) : null;
        }

        public virtual T GetFacet<T>() where T : IFacet {
            return actionParameterSpecImmutable != null ? actionParameterSpecImmutable.GetFacet<T>() : default(T);
        }

        public virtual IEnumerable<IFacet> GetFacets() {
            return actionParameterSpecImmutable != null ? actionParameterSpecImmutable.GetFacets() : new IFacet[] {};
        }

        public IConsent IsValid(INakedObject nakedObject, INakedObject proposedValue) {
            if (proposedValue != null && !proposedValue.Spec.IsOfType(Spec)) {
                return GetConsent("Not a suitable type; must be a " + Spec.SingularName);
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

        public Tuple<string, IObjectSpec>[] GetChoicesParameters() {
            var choicesFacet = GetFacet<IActionChoicesFacet>();
            return choicesFacet == null ? new Tuple<string, IObjectSpec>[] {} :
                choicesFacet.ParameterNamesAndTypes.Select(t => new Tuple<string, IObjectSpec>(t.Item1, metamodel.GetSpecification(t.Item2))).ToArray();
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

            if (Spec.IsBoundedSet()) {
                return Manager.GetCollectionOfAdaptedObjects(persistor.Instances(Spec)).ToArray();
            }

            if (Spec.IsCollectionOfBoundedSet(ElementSpec) || Spec.IsCollectionOfEnum(ElementSpec)) {
                var elementEnumFacet = ElementSpec.GetFacet<IEnumFacet>();
                IEnumerable domainObjects = elementEnumFacet != null ? (IEnumerable) elementEnumFacet.GetChoices(parentAction.RealTarget(nakedObject)) : persistor.Instances(ElementSpec);
                return Manager.GetCollectionOfAdaptedObjects(domainObjects).ToArray();
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
                IEnumerable<IActionParameterSpec> matchingParms = parentAction.Parameters.Where(p => nakedObject.Spec.IsOfType(p.Spec));

                if (matchingParms.Any() && matchingParms.First() == this) {
                    return new Tuple<INakedObject, TypeOfDefaultValue>(nakedObject, TypeOfDefaultValue.Explicit);
                }
            }

            Tuple<object, TypeOfDefaultValue> defaultValue = null;

            // Check Facet on parm, then facet on type finally fall back on type; 

            var defaultsFacet = GetFacet<IActionDefaultsFacet>();
            if (defaultsFacet != null && !defaultsFacet.IsNoOp) {
                defaultValue = defaultsFacet.GetDefault(parentAction.RealTarget(nakedObject));
            }

            if (defaultValue == null) {
                var defaultFacet = Spec.GetFacet<IDefaultedFacet>();
                if (defaultFacet != null && !defaultFacet.IsNoOp) {
                    defaultValue = new Tuple<object, TypeOfDefaultValue>(defaultFacet.Default, TypeOfDefaultValue.Implicit);
                }
            }

            if (defaultValue == null) {
                var rawValue = nakedObject == null ? null : nakedObject.Object.GetType().IsValueType ? (object) 0 : null;
                defaultValue = new Tuple<object, TypeOfDefaultValue>(rawValue, TypeOfDefaultValue.Implicit);
            }

            return new Tuple<INakedObject, TypeOfDefaultValue>(Manager.CreateAdapter(defaultValue.Item1, null, null), defaultValue.Item2);
        }

        protected internal virtual IConsent GetConsent(string message) {
            return message == null ? (IConsent) Allow.Default : new Veto(message);
        }
    }
}
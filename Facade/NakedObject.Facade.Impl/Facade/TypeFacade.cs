// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Spec;
using NakedObjects.Core.Util;
using NakedObjects.Facade.Impl.Utility;
using NakedObjects.Util;
using NakedObjects.Value;

namespace NakedObjects.Facade.Impl {
    public class TypeFacade : ITypeFacade {
        private readonly INakedObjectsFramework framework;

        public TypeFacade(ITypeSpec spec, IFrameworkFacade frameworkFacade, INakedObjectsFramework framework) {
            FacadeUtils.AssertNotNull(spec, "Spec is null");
            FacadeUtils.AssertNotNull(frameworkFacade, "FrameworkFacade is null");
            FacadeUtils.AssertNotNull(framework, "framework is null");

            FrameworkFacade = frameworkFacade;
            WrappedValue = spec;
            this.framework = framework;
        }

        public ITypeSpec WrappedValue { get; }

        #region ITypeFacade Members

        public bool IsParseable => WrappedValue.IsParseable;

        public bool IsQueryable => WrappedValue.IsQueryable;

        public bool IsService => WrappedValue is IServiceSpec;

        public bool IsVoid => WrappedValue.IsVoid;

        public bool IsASet => WrappedValue.IsASet;

        public bool IsAggregated => WrappedValue.IsAggregated;

        public bool IsImage {
            get {
                var imageSpec = framework.MetamodelManager.GetSpecification(typeof(Image));
                return WrappedValue.IsOfType(imageSpec);
            }
        }

        public bool IsFileAttachment {
            get {
                var fileSpec = framework.MetamodelManager.GetSpecification(typeof(FileAttachment));
                return WrappedValue.IsOfType(fileSpec);
            }
        }

        public bool IsFile => WrappedValue.IsFile(framework);

        public bool IsDateTime => FullName == "System.DateTime";

        public string FullName => WrappedValue.FullName;

        public string ShortName => WrappedValue.ShortName;

        public bool IsCollection => WrappedValue.IsCollection && !WrappedValue.IsParseable;

        public bool IsObject => WrappedValue.IsObject;

        public string SingularName => WrappedValue.SingularName;

        public string PluralName => WrappedValue.PluralName;

        public string Description => WrappedValue.Description;

        public bool IsEnum => WrappedValue.ContainsFacet<IEnumValueFacet>();

        public bool IsBoolean => WrappedValue.ContainsFacet<IBooleanValueFacet>();

        public bool IsAlwaysImmutable {
            get {
                IImmutableFacet facet = WrappedValue.GetFacet<IImmutableFacet>();
                return facet != null && facet.Value == WhenTo.Always;
            }
        }

        public bool IsImmutableOncePersisted {
            get {
                IImmutableFacet facet = WrappedValue.GetFacet<IImmutableFacet>();
                return facet != null && facet.Value == WhenTo.OncePersisted;
            }
        }

        public bool IsComplexType => WrappedValue.ContainsFacet<IComplexTypeFacet>();

        public IAssociationFacade[] Properties {
            get {
                var objectSpec = WrappedValue as IObjectSpec;
                return objectSpec == null ? new IAssociationFacade[] {} : objectSpec.Properties.Select(p => new AssociationFacade(p, FrameworkFacade, framework)).Cast<IAssociationFacade>().ToArray();
            }
        }

        public IMenuFacade Menu => new MenuFacade(WrappedValue.Menu, FrameworkFacade, framework);

        public bool IsImmutable(IObjectFacade objectFacade) {
            return WrappedValue.IsAlwaysImmutable() || (WrappedValue.IsImmutableOncePersisted() && !objectFacade.IsTransient);
        }

        public string GetIconName(IObjectFacade objectFacade) {
            return WrappedValue.GetIconName(objectFacade == null ? null : ((ObjectFacade) objectFacade).WrappedNakedObject);
        }

        public IActionFacade[] GetActionLeafNodes() {
            var actionsAndUid = FacadeUtils.GetActionsandUidFromSpec(WrappedValue);
            return actionsAndUid.Select(a => new ActionFacade(a.Item1, FrameworkFacade, framework, a.Item2 ?? "")).Cast<IActionFacade>().ToArray();
        }

        public ITypeFacade GetElementType(IObjectFacade objectFacade) {
            if (IsCollection) {
                var introspectableSpecification = WrappedValue.GetFacet<ITypeOfFacet>().GetValueSpec(((ObjectFacade) objectFacade).WrappedNakedObject, framework.MetamodelManager.Metamodel);
                var elementSpec = framework.MetamodelManager.GetSpecification(introspectableSpecification);
                return new TypeFacade(elementSpec, FrameworkFacade, framework);
            }
            return null;
        }

        public bool IsOfType(ITypeFacade otherSpec) {
            return WrappedValue.IsOfType(((TypeFacade) otherSpec).WrappedValue);
        }

        public Type GetUnderlyingType() {
            return TypeUtils.GetType(WrappedValue.FullName);
        }

        public IActionFacade[] GetCollectionContributedActions() {
            var objectSpec = WrappedValue as IObjectSpec;
            if (objectSpec != null) {
                return objectSpec.GetCollectionContributedActions().Select(a => new ActionFacade(a, FrameworkFacade, framework, "")).Cast<IActionFacade>().ToArray();
            }
            return new IActionFacade[] {};
        }

        public IActionFacade[] GetLocallyContributedActions(ITypeFacade typeFacade, string id) {
            var objectSpec = WrappedValue as IObjectSpec;
            if (objectSpec != null) {
                return objectSpec.GetLocallyContributedActions(((TypeFacade)typeFacade).WrappedValue, id).Select(a => new ActionFacade(a, FrameworkFacade, framework, "")).Cast<IActionFacade>().ToArray();
            }
            return new IActionFacade[] { };
        }


        public IFrameworkFacade FrameworkFacade { get; set; }

        public bool Equals(ITypeFacade other) {
            return Equals((object) other);
        }

        public string PresentationHint {
            get {
                var hintFacet = WrappedValue.GetFacet<IPresentationHintFacet>();
                return hintFacet == null ? "" : hintFacet.Value;
            }
        }

        public bool IsStream => WrappedValue.ContainsFacet<IFromStreamFacet>();

        #endregion

        public override bool Equals(object obj) {
            var nakedObjectSpecificationWrapper = obj as TypeFacade;
            if (nakedObjectSpecificationWrapper != null) {
                return Equals(nakedObjectSpecificationWrapper);
            }
            return false;
        }

        public bool Equals(TypeFacade other) {
            if (ReferenceEquals(null, other)) { return false; }
            if (ReferenceEquals(this, other)) { return true; }
            return Equals(other.WrappedValue, WrappedValue);
        }

        public override int GetHashCode() {
            return (WrappedValue != null ? WrappedValue.GetHashCode() : 0);
        }
    }
}
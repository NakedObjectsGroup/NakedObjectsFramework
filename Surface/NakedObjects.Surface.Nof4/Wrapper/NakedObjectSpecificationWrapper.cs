// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Spec;
using NakedObjects.Core.Util;
using NakedObjects.Surface.Nof4.Utility;
using NakedObjects.Surface.Utility;
using NakedObjects.Util;
using NakedObjects.Value;

namespace NakedObjects.Surface.Nof4.Wrapper {
    public class NakedObjectSpecificationWrapper : INakedObjectSpecificationSurface {
        private readonly INakedObjectsFramework framework;
        private readonly ITypeSpec spec;

        public NakedObjectSpecificationWrapper(ITypeSpec spec, INakedObjectsSurface surface, INakedObjectsFramework framework) {
            SurfaceUtils.AssertNotNull(spec, "Spec is null");
            SurfaceUtils.AssertNotNull(surface, "Surface is null");
            SurfaceUtils.AssertNotNull(framework, "framework is null");

            Surface = surface;
            this.spec = spec;
            this.framework = framework;
        }

        public ITypeSpec WrappedValue {
            get { return spec; }
        }

        #region INakedObjectSpecificationSurface Members

        public IDictionary<string, object> ExtensionData {
            get {
                var extData = new Dictionary<string, object>();

                if (spec.ContainsFacet<IPresentationHintFacet>()) {
                    extData[IdConstants.PresentationHint] = spec.GetFacet<IPresentationHintFacet>().Value;
                }

                return extData.Any() ? extData : null;
            }
        }

        public bool IsParseable {
            get { return spec.IsParseable; }
        }

        public bool IsQueryable {
            get { return spec.IsQueryable; }
        }

        public bool IsService {
            get { return spec is IServiceSpec; }
        }

        public bool IsVoid {
            get { return spec.IsVoid; }
        }

        public bool IsASet {
            get { return spec.IsASet; }
        }

        public bool IsAggregated {
            get { return spec.IsAggregated; }
        }

        public bool IsImage {
            get {
                var imageSpec = framework.MetamodelManager.GetSpecification(typeof (Image));
                return spec.IsOfType(imageSpec);
            }
        }

        public bool IsFileAttachment {
            get {
                var fileSpec = framework.MetamodelManager.GetSpecification(typeof (FileAttachment));
                return spec.IsOfType(fileSpec);
            }
        }

        public bool IsFile {
            get { return spec.IsFile(framework); }
        }

        public bool IsDateTime {
            get { return FullName == "System.DateTime"; }
        }

        public string FullName {
            get { return spec.FullName; }
        }

        public string ShortName
        {
            get { return spec.ShortName; }
        }

        public bool IsCollection {
            get { return spec.IsCollection && !spec.IsParseable; }
        }

        public bool IsObject {
            get { return spec.IsObject; }
        }

        public string SingularName {
            get { return spec.SingularName; }
        }

        public string PluralName {
            get { return spec.PluralName; }
        }

        public string Description {
            get { return spec.Description; }
        }

        public bool IsEnum {
            get { return spec.ContainsFacet<IEnumValueFacet>(); }
        }

        public bool IsBoolean {
            get { return spec.ContainsFacet<IBooleanValueFacet>(); }
        }

        public bool IsAlwaysImmutable {
            get {
                IImmutableFacet facet = spec.GetFacet<IImmutableFacet>();
                return facet != null && facet.Value == WhenTo.Always;
            }
        }

        public bool IsImmutableOncePersisted {
            get {
                IImmutableFacet facet = spec.GetFacet<IImmutableFacet>();
                return facet != null && facet.Value == WhenTo.OncePersisted;
            }
        }

        public bool IsComplexType {
            get { return spec.ContainsFacet<IComplexTypeFacet>(); }
        }

        public INakedObjectAssociationSurface[] Properties {
            get {
                var objectSpec = spec as IObjectSpec;
                return objectSpec == null ? new INakedObjectAssociationSurface[] {} : objectSpec.Properties.Select(p => new NakedObjectAssociationWrapper(p, Surface, framework)).Cast<INakedObjectAssociationSurface>().ToArray();
            }
        }

        public IMenuFacade Menu {
            get { return new MenuWrapper(spec.Menu, Surface, framework); }
        }

        public bool IsImmutable(IObjectFacade nakedObject) {
            return spec.IsAlwaysImmutable() || (spec.IsImmutableOncePersisted() && !nakedObject.IsTransient);
        }

        public string GetIconName(IObjectFacade nakedObject) {
            return spec.GetIconName(nakedObject == null ? null : ((NakedObjectWrapper) nakedObject).WrappedNakedObject);
        }

        public INakedObjectActionSurface[] GetActionLeafNodes() {
            var actionsAndUid = SurfaceUtils.GetActionsandUidFromSpec(spec);
            return actionsAndUid.Select(a => new NakedObjectActionWrapper(a.Item1, Surface, framework, a.Item2 ?? "")).Cast<INakedObjectActionSurface>().ToArray();
        }

        public INakedObjectSpecificationSurface GetElementType(IObjectFacade nakedObject) {
            if (IsCollection) {
                var introspectableSpecification = spec.GetFacet<ITypeOfFacet>().GetValueSpec(((NakedObjectWrapper) nakedObject).WrappedNakedObject, framework.MetamodelManager.Metamodel);
                var elementSpec = framework.MetamodelManager.GetSpecification(introspectableSpecification);
                return new NakedObjectSpecificationWrapper(elementSpec, Surface, framework);
            }
            return null;
        }

        public bool IsOfType(INakedObjectSpecificationSurface otherSpec) {
            return spec.IsOfType(((NakedObjectSpecificationWrapper) otherSpec).spec);
        }

        public Type GetUnderlyingType() {
            return TypeUtils.GetType(spec.FullName);
        }

        public INakedObjectActionSurface[] GetCollectionContributedActions() {
            var objectSpec = spec as IObjectSpec;
            if (objectSpec != null) {
                return objectSpec.GetCollectionContributedActions().Select(a => new NakedObjectActionWrapper(a, Surface, framework, "")).Cast<INakedObjectActionSurface>().ToArray();
            }
            return new INakedObjectActionSurface[] {};
        }

        public INakedObjectActionSurface[] GetFinderActions() {
            var objectSpec = spec as IObjectSpec;
            if (objectSpec != null) {
                return objectSpec.GetFinderActions().Select(a => new NakedObjectActionWrapper(a, Surface, framework, "")).Cast<INakedObjectActionSurface>().ToArray();
            }
            return new INakedObjectActionSurface[] {};
        }

        public INakedObjectsSurface Surface { get; set; }

        public bool Equals(INakedObjectSpecificationSurface other) {
            return Equals((object) other);
        }

        public string PresentationHint {
            get {
                var hintFacet = spec.GetFacet<IPresentationHintFacet>();
                return hintFacet == null ? "" : hintFacet.Value;
            }
        }

        public bool IsStream {
            get { return spec.ContainsFacet<IFromStreamFacet>(); }
        }

        public string UntitledName {
            get { return spec.UntitledName; }
        }

        #endregion

        public override bool Equals(object obj) {
            var nakedObjectSpecificationWrapper = obj as NakedObjectSpecificationWrapper;
            if (nakedObjectSpecificationWrapper != null) {
                return Equals(nakedObjectSpecificationWrapper);
            }
            return false;
        }

        public bool Equals(NakedObjectSpecificationWrapper other) {
            if (ReferenceEquals(null, other)) { return false; }
            if (ReferenceEquals(this, other)) { return true; }
            return Equals(other.spec, spec);
        }

        public override int GetHashCode() {
            return (spec != null ? spec.GetHashCode() : 0);
        }
    }
}
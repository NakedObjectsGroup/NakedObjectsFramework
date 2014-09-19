// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections.Generic;
using System.Linq;
using NakedObjects.Architecture.Facets.Actcoll.Typeof;
using NakedObjects.Architecture.Facets.Presentation;
using NakedObjects.Architecture.Services;
using NakedObjects.Architecture.Spec;
using NakedObjects.Surface.Nof4.Utility;
using NakedObjects.Surface.Utility;
using NakedObjects.Util;
using NakedObjects.Value;

namespace NakedObjects.Surface.Nof4.Wrapper {
    public class NakedObjectSpecificationWrapper : ScalarPropertyHolder, INakedObjectSpecificationSurface {
        private readonly INakedObjectSpecification spec;
        private readonly INakedObjectsFramework framework;

        public NakedObjectSpecificationWrapper(INakedObjectSpecification spec, INakedObjectsSurface surface, INakedObjectsFramework framework) {
            Surface = surface;
            this.spec = spec;
            this.framework = framework;
        }

        public INakedObjectSpecification WrappedValue {
            get { return spec; }
        }

        protected IDictionary<string, object> ExtensionData {
            get {
                var extData = new Dictionary<string, object>();
                
                if (spec.IsService) {
                    ServiceTypes st = framework.ObjectPersistor.GetServiceType(spec);
                    extData[ServiceType] = st.ToString();
                }

                if (spec.ContainsFacet<IPresentationHintFacet>()) {
                    extData[PresentationHint] = spec.GetFacet<IPresentationHintFacet>().Value;
                }

                return extData.Any() ? extData : null;
            }
        }

        #region INakedObjectSpecificationSurface Members

        public bool IsParseable {
            get { return spec.IsParseable; }
        }

        public bool IsQueryable {
            get { return spec.IsQueryable; }
        }

        public bool IsService {
            get { return spec.IsService; }
        }

        public bool IsVoid {
            get { return spec.IsVoid; }
        }

        public bool IsASet {
            get { return spec.IsASet; }
        }

        protected bool IsAggregated {
            get { return spec.IsAggregated; }
        }

        protected bool IsImage {
            get {
                var imageSpec = framework.Reflector.LoadSpecification(typeof(Image));
                return spec.IsOfType(imageSpec);
            }
        }

        protected bool IsFileAttachment {
            get {
                var fileSpec = framework.Reflector.LoadSpecification(typeof(FileAttachment));
                return spec.IsOfType(fileSpec);
            }
        }

        public bool IsDateTime {
            get { return FullName == "System.DateTime"; }
        }

        public string FullName {
            get { return spec.FullName; }
        }

        public bool IsCollection {
            get { return spec.IsCollection; }
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

        public INakedObjectAssociationSurface[] Properties {
            get { return spec.Properties.Select(p => new NakedObjectAssociationWrapper(p, Surface, framework)).Cast<INakedObjectAssociationSurface>().ToArray(); }
        }

        public bool IsImmutable(INakedObjectSurface nakedObject) {
            return spec.IsAlwaysImmutable() || (spec.IsImmutableOncePersisted() && !nakedObject.IsTransient());
        }

        public string GetIconName(INakedObjectSurface nakedObject) {
            return spec.GetIconName(nakedObject == null ? null : ((NakedObjectWrapper) nakedObject).WrappedNakedObject);
        }

        public INakedObjectActionSurface[] GetActionLeafNodes() {
            var actionsAndUid = SurfaceUtils.GetActionsandUidFromSpec(spec);
            return actionsAndUid.Select(a => new NakedObjectActionWrapper(a.Item1, Surface, framework, a.Item2)).Cast<INakedObjectActionSurface>().ToArray();
        }

        public INakedObjectSpecificationSurface ElementType {
            get {
                if (IsCollection) {
                    return new NakedObjectSpecificationWrapper(spec.GetFacet<ITypeOfFacet>().ValueSpec, Surface, framework);
                }
                return null;
            }
        }

        public bool IsOfType(INakedObjectSpecificationSurface otherSpec) {
            return spec.IsOfType(((NakedObjectSpecificationWrapper) otherSpec).spec);
        }

        public Type GetUnderlyingType() {
            return TypeUtils.GetType(spec.FullName);
        }

        #endregion

        public INakedObjectsSurface Surface { get; set; }

        public override bool Equals(object obj) {
            var nakedObjectSpecificationWrapper = obj as NakedObjectSpecificationWrapper;
            if (nakedObjectSpecificationWrapper != null) {
                return Equals(nakedObjectSpecificationWrapper);
            }
            return false;
        }

        public bool Equals(NakedObjectSpecificationWrapper other) {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.spec, spec);
        }

        public override int GetHashCode() {
            return (spec != null ? spec.GetHashCode() : 0);
        }

        public override object GetScalarProperty(ScalarProperty name) {
            switch (name) {
                case (ScalarProperty.FullName):
                    return FullName;
                case (ScalarProperty.SingularName):
                    return SingularName;
                case (ScalarProperty.PluralName):
                    return PluralName;
                case (ScalarProperty.Description):
                    return Description;
                case (ScalarProperty.IsParseable):
                    return IsParseable;
                case (ScalarProperty.IsQueryable):
                    return IsQueryable;
                case (ScalarProperty.IsService):
                    return IsService;
                case (ScalarProperty.IsVoid):
                    return IsVoid;
                case (ScalarProperty.IsDateTime):
                    return IsDateTime;
                case (ScalarProperty.IsCollection):
                    return IsCollection;
                case (ScalarProperty.IsObject):
                    return IsObject;
                case (ScalarProperty.IsASet):
                    return IsASet;
                case (ScalarProperty.IsAggregated):
                    return IsAggregated;
                case (ScalarProperty.IsImage):
                    return IsImage;
                case (ScalarProperty.IsFileAttachment):
                    return IsFileAttachment;
                case (ScalarProperty.ExtensionData):
                    return ExtensionData;
                default:
                    throw new NotImplementedException(string.Format("{0} doesn't support {1}", GetType(), name));
            }
        }

        
    }
}
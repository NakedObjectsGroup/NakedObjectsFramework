// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using NakedObjects.Surface;
using NakedObjects.Surface.Nof2.Utility;
using org.nakedobjects.@object;
using sdm.systems.application.container;

namespace NakedObjects.Surface.Nof2.Wrapper {
    public class NakedObjectSpecificationWrapper : ScalarPropertyHolder, INakedObjectSpecificationSurface {
        private readonly NakedObjectSpecification spec;
        private readonly Naked target;

        public NakedObjectSpecificationWrapper(NakedObjectSpecification spec, Naked target, INakedObjectsSurface surface) {
            this.spec = spec;
            this.target = target;
            Surface = surface;
        }

        #region INakedObjectSpecificationSurface Members

        public bool IsParseable {
            get { return spec.isValue(); }
        }

        public bool IsQueryable {
            get { return false; }
        }

        public bool IsService {
            get {
                if (target != null) {
                    return typeof (IRepository).IsAssignableFrom(target.getObject().GetType());
                }
                return false;
            }
        }

        public bool IsVoid {
            get { return false; }
        }

        public bool IsDateTime {
            get { return false; }
        }

        public string FullName {
            get { return spec.getFullName(); }
        }

        public bool IsCollection {
            get { return spec.isCollection() || (target != null && target.getObject() is ICollection) || spec.getFullName() == "System.Collections.ArrayList"; }
        }

        public bool IsObject {
            get { return spec.isObject(); }
        }

        protected bool IsASet {
            get { return false; }
        }

        public INakedObjectAssociationSurface[] Properties {
            get { return spec.getFields().Select(p => new NakedObjectAssociationWrapper(p, target, Surface)).Cast<INakedObjectAssociationSurface>().OrderBy(a => a.Id).ToArray(); }
        }

        public IMenu Menu { get; private set; }

        public string SingularName {
            get { return spec.getSingularName(); }
        }

        public string PluralName {
            get { return SurfaceUtils.MakeSpaced(spec.getPluralName()); }
        }

        public string Description {
            get { return ""; }
        }

        public INakedObjectSpecificationSurface GetElementType(INakedObjectSurface nakedObject) {
            return ElementType;
        }

        bool INakedObjectSpecificationSurface.IsImmutable(INakedObjectSurface nakedObject) {
            return IsService;
        }

        public string GetIconName(INakedObjectSurface nakedObject) {
            string iconName = nakedObject == null ? "" : ((NakedReference) ((NakedObjectWrapper) nakedObject).NakedObject).getIconName();
            return string.IsNullOrEmpty(iconName) ? "Default" : iconName;
        }

        public INakedObjectActionSurface[] GetActionLeafNodes() {
            return spec.GetActionLeafNodes().Select(a => new NakedObjectActionWrapper(a, target, Surface)).Cast<INakedObjectActionSurface>().OrderBy(a => a.Id).ToArray();
        }

        public INakedObjectSpecificationSurface ElementType {
            get {
                if (IsCollection) {
                    return new NakedObjectSpecificationWrapper(org.nakedobjects.@object.NakedObjects.getSpecificationLoader().loadSpecification(typeof (object).FullName), null, Surface);
                }
                return null;
            }
        }

        public bool IsOfType(INakedObjectSpecificationSurface otherSpec) {
            return spec.isOfType(((NakedObjectSpecificationWrapper) otherSpec).spec);
        }

        public Type GetUnderlyingType() {
            var typeName = spec.getFullName();
            // todo this is not efficient use TypeUtils or clone of
            return AppDomain.CurrentDomain.GetAssemblies().Select(assembly => assembly.GetType(typeName)).FirstOrDefault(type => type != null);
        }

        public INakedObjectActionSurface[] GetCollectionContributedActions() {
            throw new NotImplementedException();
        }

        public INakedObjectActionSurface[] GetFinderActions() {
            throw new NotImplementedException();
        }

        #endregion

        public bool Equals(INakedObjectSpecificationSurface other) {
            throw new NotImplementedException();
        }

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
                case (ScalarProperty.UntitledName):
                    return "";
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
                    return false;
                case (ScalarProperty.IsImage):
                    return false;
                case (ScalarProperty.IsFileAttachment):
                    return false;
                case (ScalarProperty.IsFile):
                    return false;
                case (ScalarProperty.IsBoolean):
                    return false;
                case (ScalarProperty.IsEnum):
                    return false;
                case (ScalarProperty.IsStream):
                    return false;
                case (ScalarProperty.IsAlwaysImmutable):
                    return false;
                case (ScalarProperty.IsImmutableOncePersisted):
                    return false;
                case (ScalarProperty.IsComplexType):
                    return false;
                case (ScalarProperty.PresentationHint):
                    return "";
                case (ScalarProperty.ExtensionData):
                    return null;
                default:
                    throw new NotImplementedException(string.Format("{0} doesn't support {1}", GetType(), name));
            }
        }


        public INakedObjectsSurface Surface { get; set; }
    }
}
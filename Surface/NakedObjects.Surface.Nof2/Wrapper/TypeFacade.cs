// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NakedObjects.Surface;
using NakedObjects.Surface.Nof2.Utility;
using org.nakedobjects.@object;
using sdm.systems.application.container;

namespace NakedObjects.Surface.Nof2.Wrapper {
    public class TypeFacade :  ITypeFacade {
        private readonly NakedObjectSpecification spec;
        private readonly Naked target;

        public TypeFacade(NakedObjectSpecification spec, Naked target, IFrameworkFacade surface) {
            this.spec = spec;
            this.target = target;
            Surface = surface;
        }

        #region ITypeFacade Members

        public bool IsComplexType { get; private set; }

        public bool IsParseable {
            get { return spec.isValue(); }
        }

        public bool IsStream { get; private set; }

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

        public string ShortName {
            get { return spec.getShortName(); }
        }

        public string UntitledName { get; private set; }

        public bool IsCollection {
            get { return spec.isCollection() || (target != null && target.getObject() is ICollection) || spec.getFullName() == "System.Collections.ArrayList"; }
        }

        public bool IsObject {
            get { return spec.isObject(); }
        }

        protected bool IsASet {
            get { return false; }
        }

        public bool IsAggregated { get; private set; }
        public bool IsImage { get; private set; }
        public bool IsFileAttachment { get; private set; }
        public bool IsFile { get; private set; }
        public IDictionary<string, object> ExtensionData { get; private set; }
        public bool IsBoolean { get; private set; }
        public bool IsEnum { get; private set; }

        public INakedObjectAssociationSurface[] Properties {
            get { return spec.getFields().Select(p => new NakedObjectAssociationWrapper(p, target, Surface)).Cast<INakedObjectAssociationSurface>().OrderBy(a => a.Id).ToArray(); }
        }

        public IMenuFacade Menu { get; private set; }
        public string PresentationHint { get; private set; }
        public bool IsAlwaysImmutable { get; private set; }
        public bool IsImmutableOncePersisted { get; private set; }

        public string SingularName {
            get { return spec.getSingularName(); }
        }

        public string PluralName {
            get { return SurfaceUtils.MakeSpaced(spec.getPluralName()); }
        }

        public string Description {
            get { return ""; }
        }

        bool ITypeFacade.IsASet {
            get { return IsASet; }
        }

        public ITypeFacade GetElementType(IObjectFacade nakedObject) {
            return ElementType;
        }

        bool ITypeFacade.IsImmutable(IObjectFacade nakedObject) {
            return IsService;
        }

        public string GetIconName(IObjectFacade nakedObject) {
            string iconName = nakedObject == null ? "" : ((NakedReference) ((NakedObjectWrapper) nakedObject).NakedObject).getIconName();
            return string.IsNullOrEmpty(iconName) ? "Default" : iconName;
        }

        public IActionFacade[] GetActionLeafNodes() {
            return spec.GetActionLeafNodes().Select(a => new ActionFacade(a, target, Surface)).Cast<IActionFacade>().OrderBy(a => a.Id).ToArray();
        }

        public ITypeFacade ElementType {
            get {
                if (IsCollection) {
                    return new TypeFacade(org.nakedobjects.@object.NakedObjects.getSpecificationLoader().loadSpecification(typeof (object).FullName), null, Surface);
                }
                return null;
            }
        }

        public bool IsOfType(ITypeFacade otherSpec) {
            return spec.isOfType(((TypeFacade) otherSpec).spec);
        }

        public Type GetUnderlyingType() {
            var typeName = spec.getFullName();
            // todo this is not efficient use TypeUtils or clone of
            return AppDomain.CurrentDomain.GetAssemblies().Select(assembly => assembly.GetType(typeName)).FirstOrDefault(type => type != null);
        }

        public IActionFacade[] GetCollectionContributedActions() {
            throw new NotImplementedException();
        }

        public IActionFacade[] GetFinderActions() {
            throw new NotImplementedException();
        }

        #endregion

        public bool Equals(ITypeFacade other) {
            throw new NotImplementedException();
        }

        public override bool Equals(object obj) {
            var nakedObjectSpecificationWrapper = obj as TypeFacade;
            if (nakedObjectSpecificationWrapper != null) {
                return Equals(nakedObjectSpecificationWrapper);
            }
            return false;
        }

        public bool Equals(TypeFacade other) {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.spec, spec);
        }

        public override int GetHashCode() {
            return (spec != null ? spec.GetHashCode() : 0);
        }

       


        public IFrameworkFacade Surface { get; set; }
    }
}
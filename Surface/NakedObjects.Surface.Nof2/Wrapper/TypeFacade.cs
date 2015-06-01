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
using NakedObjects.Surface.Nof2.Utility;
using org.nakedobjects.@object;
using sdm.systems.application.container;

namespace NakedObjects.Surface.Nof2.Wrapper {
    public class TypeFacade : ITypeFacade {
        private readonly NakedObjectSpecification spec;
        private readonly Naked target;

        public TypeFacade(NakedObjectSpecification spec, Naked target, IFrameworkFacade surface) {
            this.spec = spec;
            this.target = target;
            Surface = surface;
        }

        protected bool IsASet {
            get { return false; }
        }

        public ITypeFacade ElementType {
            get {
                if (IsCollection) {
                    return new TypeFacade(org.nakedobjects.@object.NakedObjects.getSpecificationLoader().loadSpecification(typeof (object).FullName), null, Surface);
                }
                return null;
            }
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

        public bool IsAggregated { get; private set; }
        public bool IsImage { get; private set; }
        public bool IsFileAttachment { get; private set; }
        public bool IsFile { get; private set; }
        public IDictionary<string, object> ExtensionData { get; private set; }
        public bool IsBoolean { get; private set; }
        public bool IsEnum { get; private set; }

        public IAssociationFacade[] Properties {
            get { return spec.getFields().Select(p => new AssociationFacade(p, target, Surface)).Cast<IAssociationFacade>().OrderBy(a => a.Id).ToArray(); }
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
            string iconName = nakedObject == null ? "" : ((NakedReference) ((ObjectFacade) nakedObject).NakedObject).getIconName();
            return string.IsNullOrEmpty(iconName) ? "Default" : iconName;
        }

        public IActionFacade[] GetActionLeafNodes() {
            return spec.GetActionLeafNodes().Select(a => new ActionFacade(a, target, Surface)).Cast<IActionFacade>().OrderBy(a => a.Id).ToArray();
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

        public bool Equals(ITypeFacade other) {
            throw new NotImplementedException();
        }

        public IFrameworkFacade Surface { get; set; }

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
            return Equals(other.spec, spec);
        }

        public override int GetHashCode() {
            return (spec != null ? spec.GetHashCode() : 0);
        }
    }
}
// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Common.Logging;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Exceptions;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Metamodel.Adapter;
using NakedObjects.Metamodel.Exception;
using NakedObjects.Metamodel.Facet;
using NakedObjects.Metamodel.Spec;
using NakedObjects.Metamodel.Utils;

namespace NakedObjects.Metamodel.SpecImmutable {
  
    public class ObjectSpecImmutable : Specification, IObjectSpecImmutable {
        private static readonly ILog Log = LogManager.GetLogger(typeof (ObjectSpecImmutable));

        private readonly IIdentifier identifier;

        public ObjectSpecImmutable(Type type, IMetamodel metamodel) {
            Type = type;
            identifier = new IdentifierImpl(metamodel, type.FullName);
            Interfaces = new IObjectSpecImmutable[] {};
            Subclasses = new IObjectSpecImmutable[] {};
            ValidationMethods = new INakedObjectValidation[] {};
            ContributedActions = new List<Tuple<string, string, IOrderSet<IActionSpecImmutable>>>();
            RelatedActions = new List<Tuple<string, string, IOrderSet<IActionSpecImmutable>>>();
        }

        private string SingularName {
            get { return GetFacet<INamedFacet>().Value; }
        }

        private string UntitledName {
            get { return Resources.NakedObjects.Untitled + SingularName; }
        }

        #region IObjectSpecImmutable Members

        public IObjectSpecImmutable Superclass { get; private set; }

        public override IIdentifier Identifier {
            get { return identifier; }
        }

        public Type Type { get; set; }

        public string FullName { get; set; }

        public string ShortName { get; set; }

        public IOrderSet<IActionSpecImmutable> ObjectActions { get; private set; }

        public IList<Tuple<string, string, IOrderSet<IActionSpecImmutable>>> ContributedActions { get; private set; }

        public IList<Tuple<string, string, IOrderSet<IActionSpecImmutable>>> RelatedActions { get; private set; }

        public IOrderSet<IAssociationSpecImmutable> Fields { get; set; }

        public IObjectSpecImmutable[] Interfaces { get; set; }

        public IObjectSpecImmutable[] Subclasses { get; set; }

        public bool Service { get; set; }

        public INakedObjectValidation[] ValidationMethods { get; set; }

        public override IFacet GetFacet(Type facetType) {
            IFacet facet = base.GetFacet(facetType);
            if (FacetUtils.IsNotANoopFacet(facet)) {
                return facet;
            }

            IFacet noopFacet = facet;

            if (Superclass != null) {
                IFacet superClassFacet = Superclass.GetFacet(facetType);
                if (FacetUtils.IsNotANoopFacet(superClassFacet)) {
                    return superClassFacet;
                }
                if (noopFacet == null) {
                    noopFacet = superClassFacet;
                }
            }
            if (Interfaces != null) {
                var interfaceSpecs = Interfaces;
                foreach (var interfaceSpec in interfaceSpecs) {
                    IFacet interfaceFacet = interfaceSpec.GetFacet(facetType);
                    if (FacetUtils.IsNotANoopFacet(interfaceFacet)) {
                        return interfaceFacet;
                    }
                    if (noopFacet == null) {
                        noopFacet = interfaceFacet;
                    }
                }
            }
            return noopFacet;
        }

        public void Introspect(IFacetDecoratorSet decorator, IIntrospector introspector) {
            introspector.IntrospectType(Type, this);
            FullName = introspector.FullName;
            ShortName = introspector.ShortName;
            Superclass = introspector.Superclass;
            Interfaces = introspector.Interfaces.ToArray();
            Fields = introspector.Fields;
            ValidationMethods = introspector.ValidationMethods;
            ObjectActions = introspector.ObjectActions;
            DecorateAllFacets(decorator);
        }


        public void MarkAsService() {
            if (Fields.Flattened.Any(field => field.Identifier.MemberName != "Id")) {
                string fieldNames = Fields.Flattened.Where(field => field.Identifier.MemberName != "Id").Aggregate("", (current, field) => current + (current.Length > 0 ? ", " : "") /*+ field.GetName(persistor)*/);
                throw new ModelException(string.Format(Resources.NakedObjects.ServiceObjectWithFieldsError, FullName, fieldNames));
            }
            Service = true;
        }

        public void AddSubclass(IObjectSpecImmutable subclass) {
            var subclassList = new List<IObjectSpecImmutable>(Subclasses) {subclass};
            Subclasses = subclassList.ToArray();
        }

        public string GetTitle(INakedObject nakedObject) {
            var titleFacet = GetFacet<ITitleFacet>();
            string title = titleFacet == null ? null : titleFacet.GetTitle(nakedObject);
            return title ?? DefaultTitle();
        }

        public virtual bool IsCollection {
            get { return ContainsFacet(typeof (ICollectionFacet)); }
        }

        public virtual bool IsParseable {
            get { return ContainsFacet(typeof (IParseableFacet)); }
        }

        public virtual bool IsObject {
            get { return !IsCollection; }
        }

        public bool IsOfType(IObjectSpecImmutable specification) {
            if (specification == this) {
                return true;
            }
            if (Interfaces.Any(interfaceSpec => interfaceSpec.IsOfType(specification))) {
                return true;
            }

            // match covariant generic types 
            if (Type.IsGenericType && IsCollection) {
                Type otherType = specification.Type;
                if (otherType.IsGenericType && Type.GetGenericArguments().Count() == 1 && otherType.GetGenericArguments().Count() == 1) {
                    if (Type.GetGenericTypeDefinition() == (typeof (IQueryable<>)) && Type.GetGenericTypeDefinition() == otherType.GetGenericTypeDefinition()) {
                        Type genericArgument = Type.GetGenericArguments().Single();
                        Type otherGenericArgument = otherType.GetGenericArguments().Single();
                        Type otherGenericParameter = otherType.GetGenericTypeDefinition().GetGenericArguments().Single();
                        if ((otherGenericParameter.GenericParameterAttributes & GenericParameterAttributes.Covariant) != 0) {
                            if (otherGenericArgument.IsAssignableFrom(genericArgument)) {
                                return true;
                            }
                        }
                    }
                }
            }

            return Superclass != null && Superclass.IsOfType(specification);
        }

        public string GetIconName(INakedObject forObject) {
            var iconFacet = GetFacet<IIconFacet>();
            string iconName = null;
            if (iconFacet != null) {
                iconName = forObject == null ? iconFacet.GetIconName() : iconFacet.GetIconName(forObject);
            }
            else if (IsCollection) {
                iconName = GetFacet<ITypeOfFacet>().ValueSpec.GetIconName(null);
            }

            return string.IsNullOrEmpty(iconName) ? "Default" : iconName;
        }

        #endregion

        private void DecorateAllFacets(IFacetDecoratorSet decorator) {
            decorator.DecorateAllHoldersFacets(this);
            foreach (IAssociationSpecImmutable field in Fields) {
                decorator.DecorateAllHoldersFacets(field);
            }
            foreach (IActionSpecImmutable action in ObjectActions.Flattened) {
                DecorateAction(decorator, action);
            }
        }

        private static void DecorateAction(IFacetDecoratorSet decorator, IActionSpecImmutable action) {
            decorator.DecorateAllHoldersFacets(action);
            foreach (IActionParameterSpecImmutable parm in action.Parameters) {
                decorator.DecorateAllHoldersFacets(parm);
            }
        }

        private string DefaultTitle() {
            return Service ? SingularName : UntitledName;
        }
    }
}
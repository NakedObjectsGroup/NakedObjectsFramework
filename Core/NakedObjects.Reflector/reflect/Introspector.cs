// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using Common.Logging;
using NakedObjects.Architecture;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Core.Util;
using NakedObjects.Meta.Adapter;
using NakedObjects.Meta.SpecImmutable;
using NakedObjects.Util;

namespace NakedObjects.Reflect {
    public class Introspector : IIntrospector {
        private static readonly ILog Log = LogManager.GetLogger(typeof (Introspector));

        private readonly IMetamodel metamodel;
        private readonly IReflector reflector;
        private Type introspectedType;
        private MethodInfo[] methods;
        private List<IAssociationSpecImmutable> orderedFields;
        private List<IActionSpecImmutable> orderedObjectActions;
        private PropertyInfo[] properties;

        public Introspector(IReflector reflector, IMetamodel metamodel) {
            Log.DebugFormat("Creating DotNetIntrospector");
            this.reflector = reflector;
            this.metamodel = metamodel;
        }

        private IClassStrategy ClassStrategy {
            get { return reflector.ClassStrategy; }
        }

        private IFacetFactorySet FacetFactorySet {
            get { return reflector.FacetFactorySet; }
        }

        private Type[] InterfacesTypes {
            get { return introspectedType.GetInterfaces().Where(i => i.IsPublic).ToArray(); }
        }

        private Type SuperclassType {
            get { return introspectedType.BaseType; }
        }

        #region IIntrospector Members

        public Type IntrospectedType {
            get { return introspectedType; }
        }

        /// <summary>
        ///     As per <see cref="MemberInfo.Name" />
        /// </summary>
        public string ClassName {
            get { return introspectedType.Name; }
        }

        public string FullName {
            get { return introspectedType.GetProxiedTypeFullName(); }
        }

        public string ShortName {
            get { return TypeNameUtils.GetShortName(introspectedType.Name); }
        }

        public IList<IAssociationSpecImmutable> Fields {
            get { return orderedFields.ToImmutableList(); }
        }

        public IList<IActionSpecImmutable> ObjectActions {
            get { return orderedObjectActions.ToImmutableList(); }
        }

        public ITypeSpecBuilder[] Interfaces { get; set; }

        public ITypeSpecBuilder Superclass { get; set; }

        public void IntrospectType(Type typeToIntrospect, ITypeSpecImmutable spec) {
            Log.InfoFormat("introspecting {0}: class-level details", typeToIntrospect.FullName);

            if (!TypeUtils.IsPublic(typeToIntrospect)) {
                throw new ReflectionException(string.Format(Resources.NakedObjects.DomainClassReflectionError, typeToIntrospect));
            }

            introspectedType = typeToIntrospect;
            properties = typeToIntrospect.GetProperties();
            methods = GetNonPropertyMethods();

            // Process facets at object level
            // this will also remove some methods, such as the superclass methods.
            var methodRemover = new IntrospectorMethodRemover(methods);
            FacetFactorySet.Process(reflector, introspectedType, methodRemover, spec);

            if (SuperclassType != null && ClassStrategy.IsTypeToBeIntrospected(SuperclassType)) {
                Superclass = reflector.LoadSpecification(SuperclassType);
            }
       
            AddAsSubclass(spec);

            var interfaces = new List<ITypeSpecBuilder>();
            foreach (Type interfaceType in InterfacesTypes) {
                if (interfaceType != null && ClassStrategy.IsTypeToBeIntrospected(interfaceType)) {
                    var interfaceSpec = reflector.LoadSpecification(interfaceType);
                    interfaceSpec.AddSubclass(spec);
                    interfaces.Add(interfaceSpec);
                }
            }
            Interfaces = interfaces.ToArray();
            IntrospectPropertiesAndCollections(spec);
            IntrospectActions(spec);
        }

        #endregion

        private void AddAsSubclass(ITypeSpecImmutable spec) {
            if (Superclass != null) {
                Log.DebugFormat("Superclass {0}", Superclass.FullName);
                Superclass.AddSubclass(spec);
            }
        }

        public void IntrospectPropertiesAndCollections(ITypeSpecImmutable spec) {
            Log.InfoFormat("introspecting {0}: properties and collections", ClassName);
            var objectSpec = spec as IObjectSpecImmutable;
            orderedFields = objectSpec != null ? CreateSortedListOfMembers(FindAndCreateFieldSpecs(objectSpec)) : new List<IAssociationSpecImmutable>();
        }

        public void IntrospectActions(ITypeSpecImmutable spec) {
            Log.InfoFormat("introspecting {0}: actions", ClassName);

            // find the actions ...
            IActionSpecImmutable[] findObjectActionMethods = FindActionMethods(spec);
            orderedObjectActions = CreateSortedListOfMembers(findObjectActionMethods);
        }

        private MethodInfo[] GetNonPropertyMethods() {
            // no better way to do this (ie no flag that indicates getter/setter)
            var allMethods = new List<MethodInfo>(introspectedType.GetMethods());
            foreach (PropertyInfo pInfo in properties) {
                allMethods.Remove(pInfo.GetGetMethod());
                allMethods.Remove(pInfo.GetSetMethod());
            }
            allMethods.Sort(new SortActionsFirst(FacetFactorySet));
            return allMethods.ToArray();
        }

        private IAssociationSpecImmutable[] FindAndCreateFieldSpecs(IObjectSpecImmutable spec) {
          
            Log.DebugFormat("Looking for fields for {0}", introspectedType);

            // now create fieldSpecs for value properties, for collections and for reference properties        
            IList<PropertyInfo> collectionProperties = FacetFactorySet.FindCollectionProperties(properties, ClassStrategy).Where(pi => !FacetFactorySet.Filters(pi, ClassStrategy)).ToList();
            IEnumerable<IAssociationSpecImmutable> collectionSpecs = CreateCollectionSpecs(collectionProperties, spec);

            // every other accessor is assumed to be a reference property.
            IList<PropertyInfo> allProperties = FacetFactorySet.FindProperties(properties, ClassStrategy).Where(pi => !FacetFactorySet.Filters(pi, ClassStrategy)).ToList();
            IEnumerable<PropertyInfo> refProperties = allProperties.Except(collectionProperties);
            IEnumerable<IAssociationSpecImmutable> refSpecs = CreateRefPropertySpecs(refProperties, spec);

            return collectionSpecs.Union(refSpecs).ToArray();
        }

        private IEnumerable<IAssociationSpecImmutable> CreateCollectionSpecs(IEnumerable<PropertyInfo> collectionProperties, IObjectSpecImmutable spec) {
            var specs = new List<IAssociationSpecImmutable>();

            foreach (PropertyInfo property in collectionProperties) {
                Log.DebugFormat("Identified one-many association method {0}", property);

                IIdentifier identifier = new IdentifierImpl(metamodel, FullName, property.Name);

                // create a collection property spec
                Type returnType = property.PropertyType;
                var returnSpec = reflector.LoadSpecification<IObjectSpecImmutable> (returnType);
                Type defaultType = typeof (object);
                var defaultSpec = reflector.LoadSpecification<IObjectSpecImmutable> (defaultType);
                var collection = new OneToManyAssociationSpecImmutable(identifier, spec, returnSpec, defaultSpec);

                FacetFactorySet.Process(reflector, property, new IntrospectorMethodRemover(methods), collection, FeatureType.Collections);
                specs.Add(collection);
            }
            return specs;
        }

        /// <summary>
        ///     Creates a list of Association fields for all the properties that use NakedObjects.
        /// </summary>
        private IEnumerable<IAssociationSpecImmutable> CreateRefPropertySpecs(IEnumerable<PropertyInfo> foundProperties, IObjectSpecImmutable spec) {
            var specs = new List<IAssociationSpecImmutable>();

            foreach (PropertyInfo property in foundProperties) {
                Log.DebugFormat("Identified 1-1 association method {0}", property);
                Log.DebugFormat("One-to-One association {0} -> {1}", property.Name, property);

                // create a reference property spec
                var identifier = new IdentifierImpl(metamodel, FullName, property.Name);
                Type propertyType = property.PropertyType;
                var propertySpec = reflector.LoadSpecification<IObjectSpecImmutable> (propertyType);
                var referenceProperty = new OneToOneAssociationSpecImmutable(identifier, spec, propertySpec);

                // Process facets for the property
                FacetFactorySet.Process(reflector, property, new IntrospectorMethodRemover(methods), referenceProperty, FeatureType.Property);
                specs.Add(referenceProperty);
            }

            return specs;
        }

        private IActionSpecImmutable[] FindActionMethods(ITypeSpecImmutable spec) {
 
            Log.Debug("Looking for action methods");

            var actionSpecs = new List<IActionSpecImmutable>();
         
            for (int i = 0; i < methods.Length; i++) {
                // careful in here - methods are being nulled out within the methods array as we iterate. 
                if (methods[i] != null) {
                    MethodInfo actionMethod = methods[i];

                    string fullMethodName = actionMethod.Name;
                    if (!FacetFactorySet.Filters(actionMethod, reflector.ClassStrategy)) {
                        Log.DebugFormat("Identified action {0}", actionMethod);
                        methods[i] = null;

                        Type[] parameterTypes = actionMethod.GetParameters().Select(parameterInfo => parameterInfo.ParameterType).ToArray();

                        // build action & its parameters          

                        IActionParameterSpecImmutable[] actionParams = parameterTypes.Select(pt => new ActionParameterSpecImmutable(reflector.LoadSpecification<IObjectSpecImmutable>(pt))).Cast<IActionParameterSpecImmutable>().ToArray();
                        IIdentifier identifier = new IdentifierImpl(metamodel, FullName, fullMethodName, actionMethod.GetParameters().ToArray());
                        var action = new ActionSpecImmutable(identifier, spec, actionParams);

                        // Process facets on the action & parameters
                        FacetFactorySet.Process(reflector, actionMethod, new IntrospectorMethodRemover(methods), action, FeatureType.Action);
                        for (int l = 0; l < actionParams.Length; l++) {
                            FacetFactorySet.ProcessParams(reflector, actionMethod, l, actionParams[l]);
                        }

                        if (actionMethod.ReturnType != typeof (void)) {
                            reflector.LoadSpecification(actionMethod.ReturnType);
                        }

                        actionSpecs.Add(action);
                    }
                }
            }

            return actionSpecs.ToArray();
        }

        private static List<T> CreateSortedListOfMembers<T>(T[] members) where T : IMemberSpecImmutable {
            var list = new List<T>(members);
            list.Sort(new MemberOrderComparator<T>());
            return list;
        }

        #region Nested Type: DotnetIntrospectorMethodRemover

        private class IntrospectorMethodRemover : IMethodRemover {
            private readonly MethodInfo[] methods;

            public IntrospectorMethodRemover(MethodInfo[] methods) {
                this.methods = methods;
            }

            #region IMethodRemover Members

            public void RemoveMethod(MethodInfo methodToRemove) {
                for (int i = 0; i < methods.Length; i++) {
                    if (methods[i] != null) {
                        if (methods[i].MemberInfoEquals(methodToRemove)) {
                            methods[i] = null;
                        }
                    }
                }
            }

            public void RemoveMethods(IList<MethodInfo> methodList) {
                for (int i = 0; i < methods.Length; i++) {
                    if (methods[i] != null) {
                        if (methodList.Any(methodToRemove => methods[i].MemberInfoEquals(methodToRemove))) {
                            methods[i] = null;
                        }
                    }
                }
            }

            #endregion
        }

        #endregion

        #region Nested type: SortActionsFirst

        private class SortActionsFirst : IComparer<MethodInfo> {
            private readonly IFacetFactorySet factories;

            public SortActionsFirst(IFacetFactorySet factories) {
                this.factories = factories;
            }

            #region IComparer<MethodInfo> Members

            public int Compare(MethodInfo x, MethodInfo y) {
                bool xIsRecognised = x != null && factories.Recognizes(x);
                bool yIsRecognised = y != null && factories.Recognizes(y);

                if (xIsRecognised == yIsRecognised) {
                    return 0;
                }

                return xIsRecognised ? 1 : -1;
            }

            #endregion
        }

        #endregion
    }

    // Copyright (c) Naked Objects Group Ltd.
}
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
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Core;
using NakedObjects.Core.Util;
using NakedObjects.Meta.Adapter;
using NakedObjects.Meta.SpecImmutable;
using NakedObjects.Util;

namespace NakedObjects.ParallelReflect {
    public sealed class Introspector : IIntrospector {
        private static readonly ILog Log = LogManager.GetLogger(typeof (Introspector));
        private readonly IReflector reflector;
        private MethodInfo[] methods;
        private IList<IAssociationSpecImmutable> orderedFields;
        private IList<IActionSpecImmutable> orderedObjectActions;
        private PropertyInfo[] properties;
        private Type introspectedType;
        private Type specificationType;
        private IIdentifier identifier;

        public Introspector(IReflector reflector) {
            Log.DebugFormat("Creating DotNetIntrospector");
            this.reflector = reflector;
        }

        private IClassStrategy ClassStrategy {
            get { return reflector.ClassStrategy; }
        }

        private IFacetFactorySet FacetFactorySet {
            get { return reflector.FacetFactorySet; }
        }

        private Type[] InterfacesTypes {
            get { return IntrospectedType.GetInterfaces().Where(i => i.IsPublic).ToArray(); }
        }

        private Type SuperclassType {
            get { return IntrospectedType.BaseType; }
        }

        #region IIntrospector Members

        public Type IntrospectedType {
            get { return introspectedType; }
            private set { introspectedType = value; }
        }

        public Type SpecificationType {
            get { return specificationType; }
            private set { specificationType = value; }
        }

        /// <summary>
        ///     As per <see cref="MemberInfo.Name" />
        /// </summary>
        public string ClassName {
            get { return IntrospectedType.Name; }
        }

        public IIdentifier Identifier {
            get { return identifier; }
            private set { identifier = value; }
        }

        public string FullName {
            get { return SpecificationType.GetProxiedTypeFullName(); }
        }

        public string ShortName {
            get { return TypeNameUtils.GetShortName(SpecificationType.Name); }
        }

        public IList<IAssociationSpecImmutable> Fields {
            get { return orderedFields.ToArray(); }
        }

        public IList<IActionSpecImmutable> ObjectActions {
            get { return orderedObjectActions.ToArray(); }
        }

        public ITypeSpecBuilder[] Interfaces { get; set; }
        public ITypeSpecBuilder Superclass { get; set; }


        private static bool IsGenericEnumerableOrSet(Type type) {
            return CollectionUtils.IsGenericType(type, typeof(IEnumerable<>)) ||
                   CollectionUtils.IsGenericType(type, typeof(ISet<>));
        }

        private Type GetSpecificationType(Type type) {
            var actualType = ClassStrategy.GetType(type);

            if (IsGenericEnumerableOrSet(type)) {
                return type.GetGenericTypeDefinition();
            }
            if (type.IsArray && !(type.GetElementType().IsValueType || type.GetElementType() == typeof(string))) {
                return typeof(System.Array);
            }

            return actualType;
        }


        public void IntrospectType(Type typeToIntrospect, ITypeSpecImmutable spec) {
            throw new NotImplementedException();
        }

        public ImmutableDictionary<string, ITypeSpecBuilder> IntrospectType(Type typeToIntrospect, ITypeSpecImmutable spec, ImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            if (!TypeUtils.IsPublic(typeToIntrospect)) {
                throw new ReflectionException(string.Format(Resources.NakedObjects.DomainClassReflectionError, typeToIntrospect));
            }

            IntrospectedType = typeToIntrospect;
            SpecificationType = GetSpecificationType(typeToIntrospect);

            properties = typeToIntrospect.GetProperties();
            methods = GetNonPropertyMethods();
            Identifier = new IdentifierImpl(FullName);

            // Process facets at object level
            // this will also remove some methods, such as the superclass methods.
            var methodRemover = new IntrospectorMethodRemover(methods);
            metamodel = FacetFactorySet.Process(reflector, IntrospectedType, methodRemover, spec, metamodel);

            if (SuperclassType != null && ClassStrategy.IsTypeToBeIntrospected(SuperclassType)) {
                var result = reflector.LoadSpecification(SuperclassType, metamodel);
                metamodel = result.Item2;
                Superclass = result.Item1;
            }

            AddAsSubclass(spec);

            var interfaces = new List<ITypeSpecBuilder>();
            foreach (Type interfaceType in InterfacesTypes) {
                if (interfaceType != null && ClassStrategy.IsTypeToBeIntrospected(interfaceType)) {
                    var result = reflector.LoadSpecification(interfaceType, metamodel);
                    metamodel = result.Item2;
                    var interfaceSpec = result.Item1;
                    interfaceSpec.AddSubclass(spec);
                    interfaces.Add(interfaceSpec);
                }
            }
            Interfaces = interfaces.ToArray();
            metamodel = IntrospectPropertiesAndCollections(spec, metamodel);
            metamodel = IntrospectActions(spec, metamodel);

            return metamodel;
        }

        #endregion

        private void AddAsSubclass(ITypeSpecImmutable spec) {
            if (Superclass != null) {
                Log.DebugFormat("Superclass {0}", Superclass.FullName);
                Superclass.AddSubclass(spec);
            }
        }

      

        public ImmutableDictionary<string, ITypeSpecBuilder> IntrospectPropertiesAndCollections(ITypeSpecImmutable spec, ImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            var objectSpec = spec as IObjectSpecImmutable;
            if (objectSpec != null) {
                var result = FindAndCreateFieldSpecs(objectSpec, metamodel);
                metamodel = result.Item2;
                orderedFields = CreateSortedListOfMembers(result.Item1);
            }
            else {
                orderedFields = new List<IAssociationSpecImmutable>();
            }

            return metamodel;
        }

    

        public ImmutableDictionary<string, ITypeSpecBuilder> IntrospectActions(ITypeSpecImmutable spec, ImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            // find the actions ...
            var result = FindActionMethods(spec, metamodel);
            IActionSpecImmutable[] findObjectActionMethods = result.Item1;
            metamodel = result.Item2;
            orderedObjectActions = CreateSortedListOfMembers(findObjectActionMethods);
            return metamodel;
        }


        private MethodInfo[] GetNonPropertyMethods() {
            // no better way to do this (ie no flag that indicates getter/setter)
            var allMethods = new List<MethodInfo>(IntrospectedType.GetMethods());
            foreach (PropertyInfo pInfo in properties) {
                allMethods.Remove(pInfo.GetGetMethod());
                allMethods.Remove(pInfo.GetSetMethod());
            }
            return allMethods.OrderBy(m => m, new SortActionsFirst(FacetFactorySet)).ToArray();
        }

        private Tuple<IAssociationSpecImmutable[], ImmutableDictionary<string, ITypeSpecBuilder>> FindAndCreateFieldSpecs(IObjectSpecImmutable spec, ImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            Log.DebugFormat("Looking for fields for {0}", IntrospectedType);

            // now create fieldSpecs for value properties, for collections and for reference properties
            IList<PropertyInfo> collectionProperties = FacetFactorySet.FindCollectionProperties(properties, ClassStrategy).Where(pi => !FacetFactorySet.Filters(pi, ClassStrategy)).ToArray();
            var result1 = CreateCollectionSpecs(collectionProperties, spec, metamodel);

            IEnumerable<IAssociationSpecImmutable> collectionSpecs = result1.Item1;
            metamodel = result1.Item2;

            // every other accessor is assumed to be a reference property.
            IList<PropertyInfo> allProperties = FacetFactorySet.FindProperties(properties, ClassStrategy).Where(pi => !FacetFactorySet.Filters(pi, ClassStrategy)).ToArray();
            IEnumerable<PropertyInfo> refProperties = allProperties.Except(collectionProperties);

            var result = CreateRefPropertySpecs(refProperties, spec, metamodel);
            var refSpecs = result.Item1;
            metamodel = result.Item2;

            return new Tuple<IAssociationSpecImmutable[], ImmutableDictionary<string, ITypeSpecBuilder>>(collectionSpecs.Union(refSpecs).ToArray(), metamodel);
        }

        private IEnumerable<IAssociationSpecImmutable> CreateCollectionSpecs(IEnumerable<PropertyInfo> collectionProperties, IObjectSpecImmutable spec) {
            var specs = new List<IAssociationSpecImmutable>();

            foreach (PropertyInfo property in collectionProperties) {
                Log.DebugFormat("Identified one-many association method {0}", property);

                IIdentifier identifier = new IdentifierImpl(FullName, property.Name);

                // create a collection property spec
                Type returnType = property.PropertyType;
                var returnSpec = reflector.LoadSpecification<IObjectSpecImmutable>(returnType);
                Type defaultType = typeof (object);
                var defaultSpec = reflector.LoadSpecification<IObjectSpecImmutable>(defaultType);

                var collection = ImmutableSpecFactory.CreateOneToManyAssociationSpecImmutable(identifier, spec, returnSpec, defaultSpec);

                FacetFactorySet.Process(reflector, property, new IntrospectorMethodRemover(methods), collection, FeatureType.Collections);
                specs.Add(collection);
            }
            return specs;
        }

        private Tuple<IEnumerable<IAssociationSpecImmutable>, ImmutableDictionary<string, ITypeSpecBuilder>> CreateCollectionSpecs(IEnumerable<PropertyInfo> collectionProperties, IObjectSpecImmutable spec, ImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            var specs = new List<IAssociationSpecImmutable>();

            foreach (PropertyInfo property in collectionProperties) {
                Log.DebugFormat("Identified one-many association method {0}", property);
                IIdentifier identifier = new IdentifierImpl(FullName, property.Name);

                // create a collection property spec
                Type returnType = property.PropertyType;
                Type defaultType = typeof(object);
                var result = reflector.LoadSpecification(returnType, metamodel);

                metamodel = result.Item2;
                var returnSpec = result.Item1 as IObjectSpecImmutable;
                result = reflector.LoadSpecification(defaultType, metamodel);
                metamodel = result.Item2;
                var defaultSpec = result.Item1 as IObjectSpecImmutable;

                var collection = ImmutableSpecFactory.CreateOneToManyAssociationSpecImmutable(identifier, spec, returnSpec, defaultSpec);

                metamodel = FacetFactorySet.Process(reflector, property, new IntrospectorMethodRemover(methods), collection, FeatureType.Collections, metamodel);
                specs.Add(collection);
            }
            return new Tuple<IEnumerable<IAssociationSpecImmutable>, ImmutableDictionary<string, ITypeSpecBuilder>>(specs, metamodel);
        }


        

        private Tuple<IEnumerable<IAssociationSpecImmutable>, ImmutableDictionary<string, ITypeSpecBuilder>> CreateRefPropertySpecs(IEnumerable<PropertyInfo> foundProperties, IObjectSpecImmutable spec, ImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            var specs = new List<IAssociationSpecImmutable>();

            foreach (PropertyInfo property in foundProperties) {
                Log.DebugFormat("Identified 1-1 association method {0}", property);
                Log.DebugFormat("One-to-One association {0} -> {1}", property.Name, property);

                // create a reference property spec
                var identifier = new IdentifierImpl(FullName, property.Name);
                Type propertyType = property.PropertyType;
                var result = reflector.LoadSpecification(propertyType, metamodel);
                metamodel = result.Item2;
                var propertySpec = result.Item1;
                if (propertySpec is IServiceSpecImmutable) {
                    throw new ReflectionException(string.Format(
                        "Type {0} is a service and cannot be used in public property {1} on type {2}." +
                        " If the property is intended to be an injected service it should have a protected get.",
                        propertyType.Name, property.Name, property.DeclaringType.Name
                    ));
                }
                var referenceProperty = ImmutableSpecFactory.CreateOneToOneAssociationSpecImmutable(identifier, spec, propertySpec as IObjectSpecImmutable);

                // Process facets for the property
                metamodel = FacetFactorySet.Process(reflector, property, new IntrospectorMethodRemover(methods), referenceProperty, FeatureType.Properties, metamodel);
                specs.Add(referenceProperty);
            }

            return new Tuple<IEnumerable<IAssociationSpecImmutable>, ImmutableDictionary<string, ITypeSpecBuilder>>(specs, metamodel);
        }



       

        private Tuple<IActionSpecImmutable[], ImmutableDictionary<string, ITypeSpecBuilder>> FindActionMethods(ITypeSpecImmutable spec, ImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            var actionSpecs = new List<IActionSpecImmutable>();
            var actions = FacetFactorySet.FindActions(methods.Where(m => m != null).ToArray(), reflector.ClassStrategy).Where(a => !FacetFactorySet.Filters(a, reflector.ClassStrategy)).ToArray();
            methods = methods.Except(actions).ToArray();

            // ReSharper disable once ForCanBeConvertedToForeach
            // kepp for look as actions are nulled out within loop
            for (int i = 0; i < actions.Length; i++) {
                MethodInfo actionMethod = actions[i];

                // actions are potentially being nulled within this loop
                if (actionMethod != null) {
                    string fullMethodName = actionMethod.Name;

                    Type[] parameterTypes = actionMethod.GetParameters().Select(parameterInfo => parameterInfo.ParameterType).ToArray();

                    // build action & its parameters

                    if (actionMethod.ReturnType != typeof(void)) {
                        metamodel = reflector.LoadSpecification(actionMethod.ReturnType, metamodel).Item2;
                    }

                    IIdentifier identifier = new IdentifierImpl(FullName, fullMethodName, actionMethod.GetParameters().ToArray());
                    //IActionParameterSpecImmutable[] actionParams = parameterTypes.
                    //    Select(pt => ImmutableSpecFactory.CreateActionParameterSpecImmutable(reflector.LoadSpecification<IObjectSpecImmutable>(pt, metamodel), identifier)).ToArray();

                    var actionParams = new List<IActionParameterSpecImmutable>();

                    foreach (var pt in parameterTypes) {
                        var result = reflector.LoadSpecification(pt, metamodel);
                        metamodel = result.Item2;
                        var ospec = result.Item1 as IObjectSpecImmutable;
                        var actionSpec = ImmutableSpecFactory.CreateActionParameterSpecImmutable(ospec, identifier);
                        actionParams.Add(actionSpec);
                    }

                    var action = ImmutableSpecFactory.CreateActionSpecImmutable(identifier, spec, actionParams.ToArray());

                    // Process facets on the action & parameters
                    metamodel = FacetFactorySet.Process(reflector, actionMethod, new IntrospectorMethodRemover(actions), action, FeatureType.Actions, metamodel);
                    for (int l = 0; l < actionParams.Count; l++) {
                        metamodel = FacetFactorySet.ProcessParams(reflector, actionMethod, l, actionParams[l], metamodel);
                    }

                    actionSpecs.Add(action);
                }
            }

            return new Tuple<IActionSpecImmutable[], ImmutableDictionary<string, ITypeSpecBuilder>>(actionSpecs.ToArray(), metamodel);
        }

        private static IList<T> CreateSortedListOfMembers<T>(T[] members) where T : IMemberSpecImmutable {
            return members.OrderBy(m => m, new MemberOrderComparator<T>()).ToArray();
        }

        #region Nested type: IntrospectorMethodRemover

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
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
using Microsoft.Extensions.Logging;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.FacetFactory;
using NakedFramework.Architecture.Reflect;
using NakedFramework.Architecture.SpecImmutable;
using NakedFramework.Core.Error;
using NakedFramework.Core.Util;
using NakedFramework.Metamodel.Adapter;
using NakedFramework.Metamodel.SpecImmutable;

namespace NakedFramework.ParallelReflector.Reflect {
    public abstract class Introspector : IIntrospector {
        private readonly ILogger logger;

        protected Introspector(IReflector reflector, ILogger logger) {
            Reflector = reflector;
            ClassStrategy = reflector.ClassStrategy;
            this.logger = logger;
            FacetFactorySet = reflector.FacetFactorySet;
        }

        private IList<IActionSpecImmutable> OrderedObjectActions { get; set; }
        private PropertyInfo[] Properties { get; set; }
        protected IReflector Reflector { get; }
        protected MethodInfo[] Methods { get; private set; }

        private IClassStrategy ClassStrategy { get; }

        protected IFacetFactorySet FacetFactorySet { get; }

        private Type[] InterfacesTypes => IntrospectedType.GetInterfaces().Where(i => i.IsPublic).ToArray();

        private Type SuperclassType => IntrospectedType.BaseType;

        public Type IntrospectedType { get; private set; }

        public Type SpecificationType { get; private set; }

        /// <summary>
        ///     As per <see cref="MemberInfo.Name" />
        /// </summary>
        public string ClassName => IntrospectedType.Name;

        public string FullName => FasterTypeUtils.GetProxiedTypeFullName(SpecificationType);

        public string ShortName => TypeNameUtils.GetShortName(SpecificationType.Name);

        public IIdentifier Identifier { get; private set; }

        public IList<IAssociationSpecImmutable> UnorderedFields { get; set; }

        public IList<IActionSpecImmutable> UnorderedObjectActions { get; set; }

        public ITypeSpecBuilder[] Interfaces { get; set; }
        public ITypeSpecBuilder Superclass { get; set; }

        public virtual IImmutableDictionary<string, ITypeSpecBuilder> IntrospectType(Type typeToIntrospect, ITypeSpecImmutable spec, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            if (!TypeUtils.IsPublic(typeToIntrospect)) {
                throw new ReflectionException(string.Format(NakedObjects.Resources.NakedObjects.DomainClassReflectionError, typeToIntrospect));
            }

            IntrospectedType = typeToIntrospect;
            SpecificationType = GetSpecificationType(typeToIntrospect);

            if (SpecificationType is null) {
                throw new ReflectionException($"Failed to get specification type for {typeToIntrospect}");
            }

            Properties = typeToIntrospect.GetProperties();
            Methods = GetNonPropertyMethods();
            Identifier = new IdentifierImpl(FullName);

            // Process facets at object level
            // this will also remove some methods, such as the superclass methods.
            metamodel = ProcessType(spec, metamodel);

            if (SuperclassType != null && ClassStrategy.IsTypeRecognized(SuperclassType)) {
                (Superclass, metamodel) = Reflector.LoadSpecification(SuperclassType, metamodel);
            }

            AddAsSubclass(spec);

            var interfaces = new List<ITypeSpecBuilder>();
            foreach (var interfaceType in InterfacesTypes) {
                if (interfaceType != null && ClassStrategy.IsTypeRecognized(interfaceType)) {
                    ITypeSpecBuilder interfaceSpec;
                    (interfaceSpec, metamodel) = Reflector.LoadSpecification(interfaceType, metamodel);
                    interfaceSpec.AddSubclass(spec);
                    interfaces.Add(interfaceSpec);
                }
            }

            Interfaces = interfaces.ToArray();
            metamodel = IntrospectPropertiesAndCollections(spec, metamodel);
            metamodel = IntrospectActions(spec, metamodel);

            return metamodel;
        }

        protected abstract IImmutableDictionary<string, ITypeSpecBuilder> ProcessType(ITypeSpecImmutable spec, IImmutableDictionary<string, ITypeSpecBuilder> metamodel);

        protected abstract IImmutableDictionary<string, ITypeSpecBuilder> ProcessCollection(PropertyInfo property, IOneToManyAssociationSpecImmutable collection, IImmutableDictionary<string, ITypeSpecBuilder> metamodel);

        protected abstract IImmutableDictionary<string, ITypeSpecBuilder> ProcessProperty(PropertyInfo property, IOneToOneAssociationSpecImmutable referenceProperty, IImmutableDictionary<string, ITypeSpecBuilder> metamodel);

        protected abstract IImmutableDictionary<string, ITypeSpecBuilder> ProcessAction(MethodInfo actionMethod, MethodInfo[] actions, IActionSpecImmutable action, IImmutableDictionary<string, ITypeSpecBuilder> metamodel);

        protected abstract IImmutableDictionary<string, ITypeSpecBuilder> ProcessParameter(MethodInfo actionMethod, int i, IActionParameterSpecImmutable param, IImmutableDictionary<string, ITypeSpecBuilder> metamodel);

        private void AddAsSubclass(ITypeSpecImmutable spec) => Superclass?.AddSubclass(spec);

        protected bool IsUnsupportedSystemType(Type type, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) => FasterTypeUtils.IsSystem(type) && !Reflector.FindSpecification(type, metamodel);

        private IImmutableDictionary<string, ITypeSpecBuilder> IntrospectPropertiesAndCollections(ITypeSpecImmutable spec, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            if (spec is IObjectSpecImmutable objectSpec) {
                (UnorderedFields, metamodel) = FindAndCreateFieldSpecs(objectSpec, metamodel);
            }
            else {
                UnorderedFields = new List<IAssociationSpecImmutable>();
            }

            return metamodel;
        }

        private IImmutableDictionary<string, ITypeSpecBuilder> IntrospectActions(ITypeSpecImmutable spec, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            // find the actions ...
            (UnorderedObjectActions, metamodel) = FindActionMethods(spec, metamodel);
            return metamodel;
        }

        private MethodInfo[] GetNonPropertyMethods() {
            // no better way to do this (ie no flag that indicates getter/setter)
            var allMethods = new List<MethodInfo>(IntrospectedType.GetMethods());
            foreach (var pInfo in Properties) {
                allMethods.Remove(pInfo.GetGetMethod());
                allMethods.Remove(pInfo.GetSetMethod());
            }

            return allMethods.OrderBy(m => m, new SortActionsFirst(FacetFactorySet)).ToArray();
        }

        private (IAssociationSpecImmutable[], IImmutableDictionary<string, ITypeSpecBuilder>) FindAndCreateFieldSpecs(IObjectSpecImmutable spec, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            // now create fieldSpecs for value properties, for collections and for reference properties
            var collectionProperties = FacetFactorySet.FindCollectionProperties(Properties, ClassStrategy).Where(pi => !FacetFactorySet.Filters(pi, ClassStrategy)).ToArray();
            IEnumerable<IAssociationSpecImmutable> collectionSpecs;
            (collectionSpecs, metamodel) = CreateCollectionSpecs(collectionProperties, spec, metamodel);

            // every other accessor is assumed to be a reference property.
            var allProperties = FacetFactorySet.FindProperties(Properties, ClassStrategy).Where(pi => !FacetFactorySet.Filters(pi, ClassStrategy)).ToArray();
            var refProperties = allProperties.Except(collectionProperties);

            IEnumerable<IAssociationSpecImmutable> refSpecs;
            (refSpecs, metamodel) = CreateRefPropertySpecs(refProperties, spec, metamodel);

            return (collectionSpecs.Union(refSpecs).ToArray(), metamodel);
        }

        private (IEnumerable<IAssociationSpecImmutable>, IImmutableDictionary<string, ITypeSpecBuilder>) CreateCollectionSpecs(IEnumerable<PropertyInfo> collectionProperties, IObjectSpecImmutable spec, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            var specs = new List<IAssociationSpecImmutable>();

            foreach (var property in collectionProperties) {
                var returnType = property.PropertyType;
                if (IsUnsupportedSystemType(returnType, metamodel)) {
                    logger.LogInformation($"Ignoring property: {property} on type: {property.DeclaringType} with return type: {returnType}");
                }
                else {
                    IIdentifier identifier = new IdentifierImpl(FullName, property.Name);

                    // create a collection property spec

                    IObjectSpecImmutable returnSpec;
                    (returnSpec, metamodel) = Reflector.LoadSpecification<IObjectSpecImmutable>(returnType, metamodel);

                    var defaultType = typeof(object);
                    IObjectSpecImmutable defaultSpec;
                    (defaultSpec, metamodel) = Reflector.LoadSpecification<IObjectSpecImmutable>(defaultType, metamodel);

                    var collection = ImmutableSpecFactory.CreateOneToManyAssociationSpecImmutable(identifier, spec, returnSpec, defaultSpec);

                    metamodel = ProcessCollection(property, collection, metamodel);
                    specs.Add(collection);
                }
            }

            return (specs, metamodel);
        }

        private (IEnumerable<IAssociationSpecImmutable>, IImmutableDictionary<string, ITypeSpecBuilder>) CreateRefPropertySpecs(IEnumerable<PropertyInfo> foundProperties, IObjectSpecImmutable spec, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            var specs = new List<IAssociationSpecImmutable>();

            foreach (var property in foundProperties) {
                var propertyType = property.PropertyType;

                if (IsUnsupportedSystemType(propertyType, metamodel)) {
                    if (!FasterTypeUtils.IsSystemOrNaked(property.DeclaringType)) {
                        logger.LogWarning($"Ignoring property: {property} on type: {property.DeclaringType} with return type: {propertyType}");
                    }
                }
                else {
                    // create a reference property spec
                    var identifier = new IdentifierImpl(FullName, property.Name);

                    IObjectSpecImmutable propertySpec;
                    (propertySpec, metamodel) = Reflector.LoadSpecification<IObjectSpecImmutable>(propertyType, metamodel);

                    if (propertySpec == null) {
                        throw new ReflectionException($"Type {propertyType.Name} is a service and cannot be used in public property {property.Name} on type {property.DeclaringType?.Name}. If the property is intended to be an injected service it should have a protected get.");
                    }

                    var referenceProperty = ImmutableSpecFactory.CreateOneToOneAssociationSpecImmutable(identifier, spec, propertySpec);

                    // Process facets for the property
                    metamodel = ProcessProperty(property, referenceProperty, metamodel);
                    specs.Add(referenceProperty);
                }
            }

            return (specs, metamodel);
        }

        private static Type GetSpecificationType(Type type) =>
            FasterTypeUtils.IsGenericCollection(type)
                ? type.GetGenericTypeDefinition()
                : FasterTypeUtils.IsObjectArray(type)
                    ? typeof(Array)
                    : TypeKeyUtils.FilterNullableAndProxies(type);

        protected virtual bool ReturnOrParameterTypesUnsupported(MethodInfo method, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            var types = method.GetParameters().Select(pi => pi.ParameterType).Append(method.ReturnType);
            return types.Any(t => IsUnsupportedSystemType(t, metamodel));
        }

        private (IActionSpecImmutable[], IImmutableDictionary<string, ITypeSpecBuilder>) FindActionMethods(ITypeSpecImmutable spec, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            var actionSpecs = new List<IActionSpecImmutable>();
            var actions = FacetFactorySet.FindActions(Methods.Where(m => m != null).ToArray(), ClassStrategy).Where(a => !FacetFactorySet.Filters(a, ClassStrategy)).ToArray();
            Methods = Methods.Except(actions).ToArray();

            // ReSharper disable once ForCanBeConvertedToForeach
            // keep a "for loop" as actions are nulled out within loop
            for (var i = 0; i < actions.Length; i++) {
                var actionMethod = actions[i];

                // actions are potentially being nulled within this loop
                if (actionMethod != null) {
                    if (ReturnOrParameterTypesUnsupported(actionMethod, metamodel)) {
                        logger.LogInformation($"Ignoring action: {actionMethod} on type: {actionMethod.DeclaringType}");
                    }
                    else {
                        var fullMethodName = actionMethod.Name;

                        var parameterTypes = actionMethod.GetParameters().Select(parameterInfo => parameterInfo.ParameterType).ToArray();

                        // build action & its parameters

                        if (ClassStrategy.LoadReturnType(actionMethod)) {
                            (_, metamodel) = Reflector.LoadSpecification(actionMethod.ReturnType, metamodel);
                        }

                        IIdentifier identifier = new IdentifierImpl(FullName, fullMethodName, actionMethod.GetParameters().ToArray());

                        var actionParams = new List<IActionParameterSpecImmutable>();

                        foreach (var pt in parameterTypes) {
                            IObjectSpecBuilder oSpec;
                            (oSpec, metamodel) = Reflector.LoadSpecification<IObjectSpecBuilder>(pt, metamodel);
                            var actionSpec = ImmutableSpecFactory.CreateActionParameterSpecImmutable(oSpec, identifier);
                            actionParams.Add(actionSpec);
                        }

                        var action = ImmutableSpecFactory.CreateActionSpecImmutable(identifier, spec, actionParams.ToArray());

                        // Process facets on the action & parameters
                        metamodel = ProcessAction(actionMethod, actions, action, metamodel);
                        for (var l = 0; l < actionParams.Count; l++) {
                            metamodel = ProcessParameter(actionMethod, l, actionParams[l], metamodel);
                        }

                        actionSpecs.Add(action);
                    }
                }
            }

            return (actionSpecs.ToArray(), metamodel);
        }

        #region Nested type: IntrospectorMethodRemover

        #region Nested Type: DotnetIntrospectorMethodRemover

        protected class IntrospectorMethodRemover : IMethodRemover {
            private readonly MethodInfo[] methods;

            public IntrospectorMethodRemover(MethodInfo[] methods) => this.methods = methods;

            #region IMethodRemover Members

            public void RemoveMethod(MethodInfo methodToRemove) {
                for (var i = 0; i < methods.Length; i++) {
                    if (methods[i] != null) {
                        if (methods[i].MemberInfoEquals(methodToRemove)) {
                            methods[i] = null;
                        }
                    }
                }
            }

            public void RemoveMethods(IList<MethodInfo> methodList) {
                for (var i = 0; i < methods.Length; i++) {
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

            public SortActionsFirst(IFacetFactorySet factories) => this.factories = factories;

            #region IComparer<MethodInfo> Members

            public int Compare(MethodInfo x, MethodInfo y) {
                var xIsRecognised = x != null && factories.Recognizes(x);
                var yIsRecognised = y != null && factories.Recognizes(y);

                if (xIsRecognised == yIsRecognised) {
                    return 0;
                }

                return xIsRecognised ? 1 : -1;
            }

            #endregion
        }

        #endregion
    }
}
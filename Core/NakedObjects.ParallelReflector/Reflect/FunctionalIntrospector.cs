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
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Core;
using NakedObjects.Core.Util;
using NakedObjects.Meta.Adapter;
using NakedObjects.Meta.SpecImmutable;
using NakedObjects.Util;

namespace NakedObjects.ParallelReflect {
    public sealed class FunctionalIntrospector : Introspector, IIntrospector {
        public static Type[] Functions;
        private readonly ILogger<FunctionalIntrospector> logger;


        public FunctionalIntrospector(IReflector reflector, Type[] functions, ILogger<FunctionalIntrospector> logger) {
            this.Reflector = reflector;
            ClassStrategy = reflector.ClassStrategy;
            FacetFactorySet = reflector.FacetFactorySet;
            Functions = functions;
            this.logger = logger;
        }

        protected override (IActionSpecImmutable[], IImmutableDictionary<string, ITypeSpecBuilder>) FindActionMethods(ITypeSpecImmutable spec, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            var actionSpecs = new List<IActionSpecImmutable>();
            var actions = FacetFactorySet.FindActions(Methods.Where(m => m != null).ToArray(), ClassStrategy).Where(a => !FacetFactorySet.Filters(a, ClassStrategy)).ToArray();
            Methods = Methods.Except(actions).ToArray();

            // ReSharper disable once ForCanBeConvertedToForeach
            // kepp for look as actions are nulled out within loop
            for (var i = 0; i < actions.Length; i++) {
                var actionMethod = actions[i];

                // actions are potentially being nulled within this loop
                if (actionMethod != null) {
                    var fullMethodName = actionMethod.Name;

                    var parameterTypes = actionMethod.GetParameters().Select(parameterInfo => parameterInfo.ParameterType).ToArray();

                    // build action & its parameters

                    // if static leave to facet to sort out return type
                    if (actionMethod.ReturnType != typeof(void) && !actionMethod.IsStatic) {
                        metamodel = Reflector.LoadSpecification(actionMethod.ReturnType, ClassStrategy, metamodel).Item2;
                    }

                    IIdentifier identifier = new IdentifierImpl(FullName, fullMethodName, actionMethod.GetParameters().ToArray());
                    //IActionParameterSpecImmutable[] actionParams = parameterTypes.
                    //    Select(pt => ImmutableSpecFactory.CreateActionParameterSpecImmutable(reflector.LoadSpecification<IObjectSpecImmutable>(pt, metamodel), identifier)).ToArray();

                    var actionParams = new List<IActionParameterSpecImmutable>();

                    foreach (var pt in parameterTypes) {
                        var result = Reflector.LoadSpecification(pt, ClassStrategy, metamodel);
                        metamodel = result.Item2;
                        var ospec = result.Item1 as IObjectSpecImmutable;
                        var actionSpec = ImmutableSpecFactory.CreateActionParameterSpecImmutable(ospec, identifier);
                        actionParams.Add(actionSpec);
                    }

                    var action = ImmutableSpecFactory.CreateActionSpecImmutable(identifier, spec, actionParams.ToArray());

                    // Process facets on the action & parameters
                    metamodel = FacetFactorySet.Process(Reflector, ClassStrategy, actionMethod, new IntrospectorMethodRemover(actions), action, FeatureType.Actions, metamodel);
                    for (var l = 0; l < actionParams.Count; l++) {
                        metamodel = FacetFactorySet.ProcessParams(Reflector, ClassStrategy, actionMethod, l, actionParams[l], metamodel);
                    }

                    actionSpecs.Add(action);
                }
            }

            return (actionSpecs.ToArray(), metamodel);
        }

        #region IIntrospector Members

        private static bool IsGenericEnumerableOrSet(Type type) =>
            CollectionUtils.IsGenericType(type, typeof(IEnumerable<>)) ||
            CollectionUtils.IsGenericType(type, typeof(ISet<>));

        private Type GetSpecificationType(Type type) {
            var actualType = ClassStrategy.GetType(type);

            if (IsGenericEnumerableOrSet(type)) {
                return type.GetGenericTypeDefinition();
            }

            if (type.IsArray && !(type.GetElementType().IsValueType || type.GetElementType() == typeof(string))) {
                return typeof(Array);
            }

            return actualType;
        }

        public override IImmutableDictionary<string, ITypeSpecBuilder> IntrospectType(Type typeToIntrospect, ITypeSpecImmutable spec, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            if (!TypeUtils.IsPublic(typeToIntrospect)) {
                throw new ReflectionException(string.Format(Resources.NakedObjects.DomainClassReflectionError, typeToIntrospect));
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
            var methodRemover = new IntrospectorMethodRemover(Methods);
            metamodel = FacetFactorySet.Process(Reflector, ClassStrategy, IntrospectedType, methodRemover, spec, metamodel);

            if (SuperclassType != null && ClassStrategy.IsTypeToBeIntrospected(SuperclassType)) {
                var result = Reflector.LoadSpecification(SuperclassType, ClassStrategy, metamodel);
                metamodel = result.Item2;
                Superclass = result.Item1;
            }

            AddAsSubclass(spec);

            var interfaces = new List<ITypeSpecBuilder>();
            foreach (var interfaceType in InterfacesTypes) {
                if (interfaceType != null && ClassStrategy.IsTypeToBeIntrospected(interfaceType)) {
                    var result = Reflector.LoadSpecification(interfaceType, ClassStrategy, metamodel);
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
    }

    // Copyright (c) Naked Objects Group Ltd.
}
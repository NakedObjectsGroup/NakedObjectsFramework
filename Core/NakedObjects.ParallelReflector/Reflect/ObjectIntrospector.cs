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
    public sealed class ObjectIntrospector : Introspector, IIntrospector {
        private readonly ILogger<ObjectIntrospector> logger;
     

        public ObjectIntrospector(IReflector reflector, ILogger<ObjectIntrospector> logger) {
            this.Reflector = reflector;
            ClassStrategy = reflector.ClassStrategy;
            this.logger = logger;
            FacetFactorySet = reflector.FacetFactorySet;
        }


        #region IIntrospector Members

        public override IImmutableDictionary<string, ITypeSpecBuilder> IntrospectType(Type typeToIntrospect, ITypeSpecImmutable spec, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            if (!TypeUtils.IsPublic(typeToIntrospect)) {
                throw new ReflectionException(logger.LogAndReturn(string.Format(Resources.NakedObjects.DomainClassReflectionError, typeToIntrospect)));
            }

            IntrospectedType = typeToIntrospect;
            SpecificationType = GetSpecificationType(typeToIntrospect);

            Properties = typeToIntrospect.GetProperties();
            Methods = GetNonPropertyMethods();
            Identifier = new IdentifierImpl(FullName);

            // Process facets at object level
            // this will also remove some methods, such as the superclass methods.
            var methodRemover = new IntrospectorMethodRemover(Methods);
            metamodel = FacetFactorySet.Process(Reflector, ClassStrategy, IntrospectedType, methodRemover, spec, metamodel);

            if (SuperclassType != null && ClassStrategy.IsTypeToBeIntrospected(SuperclassType)) {
                (Superclass, metamodel) = Reflector.LoadSpecification(SuperclassType, ClassStrategy, metamodel);
            }

            AddAsSubclass(spec);

            var interfaces = new List<ITypeSpecBuilder>();
            foreach (var interfaceType in InterfacesTypes) {
                if (interfaceType != null && ClassStrategy.IsTypeToBeIntrospected(interfaceType)) {
                    ITypeSpecBuilder interfaceSpec;
                    (interfaceSpec, metamodel) = Reflector.LoadSpecification(interfaceType, ClassStrategy, metamodel);
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

        private Type GetSpecificationType(Type type) =>
            FasterTypeUtils.IsGenericCollection(type)
                ? type.GetGenericTypeDefinition()
                : FasterTypeUtils.IsObjectArray(type)
                    ? typeof(Array)
                    : ClassStrategy.GetType(type);

        protected override (IActionSpecImmutable[], IImmutableDictionary<string, ITypeSpecBuilder>) FindActionMethods(ITypeSpecImmutable spec, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            var actionSpecs = new List<IActionSpecImmutable>();
            var actions = FacetFactorySet.FindActions(Methods.Where(m => m != null).ToArray(), ClassStrategy).Where(a => !FacetFactorySet.Filters(a, ClassStrategy)).ToArray();
            Methods = Methods.Except(actions).ToArray();

            // ReSharper disable once ForCanBeConvertedToForeach
            // keep a "for loop" as actions are nulled out within loop
            for (var i = 0; i < actions.Length; i++) {
                var actionMethod = actions[i];

                // actions are potentially being nulled within this loop
                if (actionMethod != null) {
                    var fullMethodName = actionMethod.Name;

                    var parameterTypes = actionMethod.GetParameters().Select(parameterInfo => parameterInfo.ParameterType).ToArray();

                    // build action & its parameters

                    if (actionMethod.ReturnType != typeof(void)) {
                        (_, metamodel) = Reflector.LoadSpecification(actionMethod.ReturnType, ClassStrategy, metamodel);
                    }

                    IIdentifier identifier = new IdentifierImpl(FullName, fullMethodName, actionMethod.GetParameters().ToArray());

                    var actionParams = new List<IActionParameterSpecImmutable>();

                    foreach (var pt in parameterTypes) {
                        IObjectSpecBuilder oSpec;
                        (oSpec, metamodel) = Reflector.LoadSpecification<IObjectSpecBuilder>(pt, ClassStrategy, metamodel);
                        var actionSpec = ImmutableSpecFactory.CreateActionParameterSpecImmutable(oSpec, identifier);
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
    }

    // Copyright (c) Naked Objects Group Ltd.
}
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
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Architecture.Util;
using NakedObjects.Meta.Adapter;
using NakedObjects.Meta.SpecImmutable;
using NakedObjects.Util;

namespace NakedObjects.Reflect {
    public class Introspector : IIntrospector {
        private static readonly ILog Log = LogManager.GetLogger(typeof (Introspector));

        private static readonly object[] NoParameters = new object[0];
        private readonly IMetamodel metamodel;
        private readonly IReflector reflector;
        private Type introspectedType;
        private MethodInfo[] methods;
        private IOrderSet<IActionSpecImmutable> orderedClassActions;
        private IOrderSet<IAssociationSpecImmutable> orderedFields;
        private IOrderSet<IActionSpecImmutable> orderedObjectActions;
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

        public string[] InterfacesNames {
            get { return introspectedType.GetInterfaces().Select(i => i.FullName ?? i.Namespace + "." + i.Name).ToArray(); }
        }

        public string SuperclassName {
            get { return TypeUtils.GetBaseType(introspectedType); }
        }

        public string ShortName {
            get { return TypeNameUtils.GetShortName(introspectedType.Name); }
        }

        public IList<IOrderableElement<IAssociationSpecImmutable>> Fields {
            get { return orderedFields.Cast<IOrderableElement<IAssociationSpecImmutable>>().ToImmutableList(); }
        }

        public IList<IOrderableElement<IActionSpecImmutable>> ClassActions {
            get { return orderedClassActions.Cast<IOrderableElement<IActionSpecImmutable>>().ToImmutableList(); }
        }

        public IList<IOrderableElement<IActionSpecImmutable>> ObjectActions {
            get { return orderedObjectActions.Cast<IOrderableElement<IActionSpecImmutable>>().ToImmutableList(); }
        }

        public IObjectSpecBuilder[] Interfaces { get; set; }

        public IObjectSpecBuilder Superclass { get; set; }

        public void IntrospectType(Type typeToIntrospect, IObjectSpecImmutable spec) {
            Log.InfoFormat("introspecting {0}: class-level details", typeToIntrospect.FullName);

            if (!TypeUtils.IsPublic(typeToIntrospect)) {
                throw new ReflectionException(string.Format(Resources.NakedObjects.DomainClassReflectionError, typeToIntrospect));
            }

            introspectedType = typeToIntrospect;
            properties = typeToIntrospect.GetProperties();
            methods = GetFilteredMethods();

            // Process facets at object level
            // this will also remove some methods, such as the superclass methods.
            var methodRemover = new IntrospectorMethodRemover(methods);
            FacetFactorySet.Process(introspectedType, methodRemover, spec);

            Action<string> addSubclass = s => {
                if (Superclass != null) {
                    Log.DebugFormat("Superclass {0}", s);
                    Superclass.AddSubclass(spec);
                }
            };

            var typeOfObj = typeof (object);
            if (SuperclassName != null && !TypeUtils.IsSystem(SuperclassName)) {
                Superclass = reflector.LoadSpecification(SuperclassName);
                addSubclass(SuperclassName);
            }
            else if (spec.Type != typeOfObj) {
                // always root in object (unless this is object!)            
                Superclass = reflector.LoadSpecification(typeOfObj);
                addSubclass(typeOfObj.Name);
            }

            var interfaces = new List<IObjectSpecBuilder>();
            foreach (string interfaceName in InterfacesNames) {
                var interfaceSpec = reflector.LoadSpecification(interfaceName);
                interfaceSpec.AddSubclass(spec);
                interfaces.Add(interfaceSpec);
            }
            Interfaces = interfaces.ToArray();
            IntrospectPropertiesAndCollections(spec);
            IntrospectActions(spec);
        }

        #endregion

        public void IntrospectPropertiesAndCollections(IObjectSpecImmutable spec) {
            Log.InfoFormat("introspecting {0}: properties and collections", ClassName);

            // find the properties and collections (fields) ...
            IAssociationSpecImmutable[] findFieldMethods = FindAndCreateFieldSpecs();

            // ... and the ordering of the properties and collections  
            var fieldOrderFacet = spec.GetFacet<IFieldOrderFacet>();

            // TODO: the calling of fieldOrder() should be a facet
            string fieldOrder = fieldOrderFacet == null ? InvokeSortOrderMethod("Field") : fieldOrderFacet.Value;
            orderedFields = CreateOrderSet(fieldOrder, findFieldMethods);
        }

        public void IntrospectActions(IObjectSpecImmutable spec) {
            Log.InfoFormat("introspecting {0}: actions", ClassName);

            // find the actions ...
            IActionSpecImmutable[] findObjectActionMethods = FindActionMethods(MethodType.Object, spec);

            // ... and the ordering of actions ...
            var actionOrderFacet = spec.GetFacet<IActionOrderFacet>();

            // TODO: the calling of actionOrder() should be a facet
            string actionOrder = actionOrderFacet == null ? InvokeSortOrderMethod("Action") : actionOrderFacet.Value;
            orderedObjectActions = CreateOrderSet(actionOrder, findObjectActionMethods);


            // find the class actions ...
            IActionSpecImmutable[] findClassActionMethods = FindActionMethods(MethodType.Class, spec);

            // ... and the ordering of class actions
            // TODO: the calling of classActionOrder() should be a facet
            actionOrder = InvokeSortOrderMethod("ClassAction");

            orderedClassActions = CreateOrderSet(actionOrder, findClassActionMethods);
        }

        private MethodInfo[] GetFilteredMethods() {
            var allMethods = new List<MethodInfo>(introspectedType.GetMethods());
            foreach (PropertyInfo pInfo in properties) {
                allMethods.Remove(pInfo.GetGetMethod());
                allMethods.Remove(pInfo.GetSetMethod());
            }
            return allMethods.ToArray();
        }

        private IObjectSpecImmutable GetSpecification(Type returnType) {
            return reflector.LoadSpecification(returnType);
        }

        private IAssociationSpecImmutable[] FindAndCreateFieldSpecs() {
            if (ClassStrategy.IsSystemClass(introspectedType)) {
                Log.DebugFormat("Skipping fields in {0} (system class according to ClassStrategy)", introspectedType.Name);
                return new IAssociationSpecImmutable[0];
            }

            Log.DebugFormat("Looking for fields for {0}", introspectedType);

            var candidates = new List<PropertyInfo>(properties);
            reflector.LoadSpecificationForReturnTypes(candidates, introspectedType);

            // now create fieldSpecs for value properties, for collections and for reference properties
            var fieldSpecs = new List<IAssociationSpecImmutable>();

            FindCollectionPropertiesAndCreateCorrespondingFieldSpecs(candidates, fieldSpecs);
            // every other accessor is assumed to be a reference property.
            FindPropertiesAndCreateCorrespondingFieldSpecs(candidates, fieldSpecs);

            return fieldSpecs.ToArray();
        }

        private void FindCollectionPropertiesAndCreateCorrespondingFieldSpecs(IList<PropertyInfo> candidates, ICollection<IAssociationSpecImmutable> fieldSpecs) {
            var collectionProperties = new List<PropertyInfo>();
            FacetFactorySet.FindCollectionProperties(candidates, collectionProperties);
            CreateCollectionSpecsFromAccessors(collectionProperties, fieldSpecs);
        }

        /// <summary>
        ///     Since the value properties and collections have already been processed, this will
        ///     pick up the remaining reference properties
        /// </summary>
        private void FindPropertiesAndCreateCorrespondingFieldSpecs(IList<PropertyInfo> candidates, ICollection<IAssociationSpecImmutable> fieldSpecs) {
            var foundProperties = new List<PropertyInfo>();
            FacetFactorySet.FindProperties(candidates, foundProperties);
            CreatePropertySpecsFromAccessors(foundProperties, fieldSpecs);
        }

        private void CreateCollectionSpecsFromAccessors(IEnumerable<PropertyInfo> collectionProperties, ICollection<IAssociationSpecImmutable> fieldsListToAppendto) {
            foreach (PropertyInfo property in collectionProperties) {
                Log.DebugFormat("Identified one-many association method {0}", property);

                IIdentifier identifier = new IdentifierImpl(metamodel, FullName, property.Name);

                // create property and add facets
                var returnType = property.PropertyType;
                var returnSpec = reflector.LoadSpecification(returnType);

                var collection = new OneToManyAssociationSpecImmutable(identifier, returnType, returnSpec, metamodel);
                FacetFactorySet.Process(property, new IntrospectorMethodRemover(methods), collection, FeatureType.Collections);
                fieldsListToAppendto.Add(collection);
            }
        }

        /// <summary>
        ///     Creates a list of Association fields for all the properties that use NakedObjects.
        /// </summary>
        private void CreatePropertySpecsFromAccessors(IEnumerable<PropertyInfo> foundProperties, ICollection<IAssociationSpecImmutable> fieldListToAppendto) {
            foreach (PropertyInfo property in foundProperties) {
                Log.DebugFormat("Identified 1-1 association method {0}", property);
                Log.DebugFormat("One-to-One association {0} -> {1}", property.Name, property);

                IIdentifier identifier = new IdentifierImpl(metamodel, FullName, property.Name);

                // create a reference property
                var propertyType = property.PropertyType;
                var propertySpec = reflector.LoadSpecification(propertyType);
                var referenceProperty = new OneToOneAssociationSpecImmutable(identifier, propertyType, propertySpec);

                // Process facets for the property
                FacetFactorySet.Process(property, new IntrospectorMethodRemover(methods), referenceProperty, FeatureType.Property);

                fieldListToAppendto.Add(referenceProperty);
            }
        }

        private IActionSpecImmutable[] FindActionMethods(MethodType methodType, IObjectSpecImmutable spec) {
            if (ClassStrategy.IsSystemClass(introspectedType)) {
                Log.DebugFormat("Skipping fields in {0}(system class according to ClassStrategy)", introspectedType.Name);
                return new IActionSpecImmutable[0];
            }

            Log.Debug("Looking for action methods");

            var actionSpecs = new List<IActionSpecImmutable>();

            Array.Sort(methods, new SortActionsFirst(FacetFactorySet));
            for (int i = 0; i < methods.Length; i++) {
                if (methods[i] == null) {
                    continue;
                }
                MethodInfo actionMethod = methods[i];
                if (actionMethod.IsStatic && methodType == MethodType.Object) {
                    continue;
                }

                string fullMethodName = actionMethod.Name;
                if (FacetFactorySet.Filters(actionMethod)) {
                    continue;
                }

                if (AttributeUtils.GetCustomAttribute<NakedObjectsIgnoreAttribute>(actionMethod) != null) {
                    continue;
                }

                Log.DebugFormat("Identified action {0}", actionMethod);
                methods[i] = null;

                Type[] parameterTypes = actionMethod.GetParameters().Select(parameterInfo => parameterInfo.ParameterType).ToArray();

                IIdentifier identifier = new IdentifierImpl(metamodel, FullName, fullMethodName, actionMethod.GetParameters().ToArray());

                // build action & its parameters          

                IActionParameterSpecImmutable[] actionParams = parameterTypes.Select(pt => new ActionParameterSpecImmutable(GetSpecification(pt))).Cast<IActionParameterSpecImmutable>().ToArray();

                var action = new ActionSpecImmutable(identifier, spec, actionParams);

                // Process facets on the action & parameters
                FacetFactorySet.Process(actionMethod, new IntrospectorMethodRemover(methods), action, FeatureType.Action);
                for (int l = 0; l < actionParams.Length; l++) {
                    FacetFactorySet.ProcessParams(actionMethod, l, actionParams[l]);
                }

                if (actionMethod.ReturnType != typeof (void)) {
                    reflector.LoadSpecification(actionMethod.ReturnType);
                }

                actionSpecs.Add(action);
            }

            return actionSpecs.ToArray();
        }

        /// <summary>
        ///     Searches for specific method and returns it, also removing it from the
        ///     array of methods <see cref="Methods" /> if found
        /// </summary>
        /// <seealso cref="MethodFinderUtils.RemoveMethod(MethodInfo[],MethodType,string,Type,Type[])" />
        private MethodInfo FindAndRemoveMethod(MethodType methodType, string name, Type returnType, Type[] paramTypes) {
            return MethodFinderUtils.RemoveMethod(methods, methodType, name, returnType, paramTypes);
        }

        private static OrderSet<T> CreateOrderSet<T>(string order, T[] members) where T : IOrderableElement<T>, ISpecification {
            if (order == null) {
                return DeweyOrderSet<T>.CreateOrderSet(members);
            }
            return SimpleOrderSet<T>.CreateOrderSet(order, members);
        }

        private string InvokeSortOrderMethod(string name) {
            MethodInfo method = FindAndRemoveMethod(MethodType.Class, name + "Order", typeof (string), Type.EmptyTypes);
            if (method == null) {
                return null;
            }
            if (!method.IsStatic) {
                Log.Warn("method " + ClassName + "." + name + "Order() must be declared as static");
                return null;
            }
            var s = (string) InvokeMethod(method, NoParameters);
            if (s.Trim().Length == 0) {
                return null;
            }
            return s;
        }

        private static object InvokeMethod(MethodInfo method, object[] parameters) {
            return method.Invoke(null, parameters);
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
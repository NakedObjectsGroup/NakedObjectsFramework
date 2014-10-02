// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections.Generic;
using System.Linq;
using Common.Logging;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Actcoll.Typeof;
using NakedObjects.Architecture.Facets.Objects.Facets;
using NakedObjects.Architecture.Facets.Ordering.MemberOrder;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.Util;
using NakedObjects.Reflector.DotNet.Facets.Ordering;
using NakedObjects.Reflector.DotNet.Facets.Ordering.MemberOrder;
using NakedObjects.Reflector.DotNet.Reflect.Actions;
using NakedObjects.Reflector.DotNet.Reflect.Collections;
using NakedObjects.Reflector.DotNet.Reflect.Properties;
using NakedObjects.Reflector.Peer;
using NakedObjects.Reflector.spec;
using NakedObjects.Reflector.Spec;
using NakedObjects.Util;
using MemberInfo = System.Reflection.MemberInfo;
using MethodInfo = System.Reflection.MethodInfo;
using PropertyInfo = System.Reflection.PropertyInfo;
using ParameterInfo = System.Reflection.ParameterInfo;

namespace NakedObjects.Reflector.DotNet.Reflect {
    internal class DotNetIntrospector {
        private static readonly ILog Log = LogManager.GetLogger(typeof (DotNetIntrospector));

        private static readonly object[] NoParameters = new object[0];
        private readonly IIntrospectableSpecification specification;
        private readonly Type introspectedType;

        private readonly MethodInfo[] methods;
        private readonly PropertyInfo[] properties;
        private readonly INakedObjectReflector reflector;

        private OrderSet orderedClassActions;
        private OrderSet orderedFields;
        private OrderSet orderedObjectActions;

        public DotNetIntrospector(Type typeToIntrospect,
                                  IIntrospectableSpecification specification,
                                  INakedObjectReflector reflector) {
            Log.DebugFormat("Creating DotNetIntrospector for {0}", typeToIntrospect);

            if (!TypeUtils.IsPublic(typeToIntrospect)) {
                throw new ReflectionException(string.Format(Resources.NakedObjects.DomainClassReflectionError, typeToIntrospect));
            }

            introspectedType = typeToIntrospect;
            this.specification = specification;
            this.reflector = reflector;
            properties = typeToIntrospect.GetProperties();
            methods = GetFilteredMethods();
        }

        private IClassStrategy ClassStrategy {
            get { return reflector.ClassStrategy; }
        }

        private IFacetFactorySet FacetFactorySet {
            get { return reflector.FacetFactorySet; }
        }

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
            get { return TypeUtils.GetInterfaces(introspectedType); }
        }

        public string SuperclassName {
            get { return TypeUtils.GetBaseType(introspectedType); }
        }

        public string ShortName {
            get {
                return TypeNameUtils.GetShortName(introspectedType.Name);
            }
        }

        public OrderSet Fields {
            get { return orderedFields; }
        }

        public OrderSet ClassActions {
            get { return orderedClassActions; }
        }

        public OrderSet ObjectActions {
            get { return orderedObjectActions; }
        }

        public bool IsAbstract {
            get { return introspectedType.IsAbstract; }
        }

        public bool IsInterface {
            get { return introspectedType.IsInterface; }
        }

        public bool IsSealed {
            get { return introspectedType.IsSealed; }
        }

        public bool IsVoid {
            get { return introspectedType == typeof (void); }
        }

        private MethodInfo[] GetFilteredMethods() {
            var allMethods = new List<MethodInfo>(introspectedType.GetMethods());
            foreach (PropertyInfo pInfo in properties) {
                allMethods.Remove(pInfo.GetGetMethod());
                allMethods.Remove(pInfo.GetSetMethod());
            }
            return allMethods.ToArray();
        }

        private IIntrospectableSpecification GetSpecification(Type returnType) {
            return reflector.LoadSpecification(returnType);
        }

        public void IntrospectClass() {
            Log.InfoFormat("introspecting {0}: class-level details", ClassName);
            // Process facets at object level
            // this will also remove some methods, such as the superclass methods.
            var methodRemover = new DotnetIntrospectorMethodRemover(methods);
            FacetFactorySet.Process(introspectedType, methodRemover, specification);
            // if this class has additional facets then Process them.
            var facetsFacet = specification.GetFacet<IFacetsFacet>();
            if (facetsFacet != null) {
                foreach (Type facetFactory in facetsFacet.FacetFactories) {
                    var facetFactoryInstance = (IFacetFactory) Activator.CreateInstance(facetFactory, reflector);
                    facetFactoryInstance.Process(introspectedType, methodRemover, specification);
                }
            }
        }

        public void IntrospectPropertiesAndCollections() {
            Log.InfoFormat("introspecting {0}: properties and collections", ClassName);

            // find the properties and collections (fields) ...
            INakedObjectMemberPeer[] findFieldMethods = FindAndCreateFieldPeers();

            // ... and the ordering of the properties and collections  
            var fieldOrderFacet = specification.GetFacet<IFieldOrderFacet>();

            // TODO: the calling of fieldOrder() should be a facet
            string fieldOrder = fieldOrderFacet == null ? InvokeSortOrderMethod("Field") : fieldOrderFacet.Value;
            orderedFields = CreateOrderSet(fieldOrder, findFieldMethods);
        }

        public INakedObjectValidation[] IntrospectObjectValidationMethods() {
            Log.InfoFormat("introspecting {0}: object validation methods", ClassName);
            return FindAndCreateValidationPeers();
        }

        public void IntrospectActions() {
            Log.InfoFormat("introspecting {0}: actions", ClassName);

            // find the actions ...
            INakedObjectActionPeer[] findObjectActionMethods = FindActionMethods(MethodType.Object);

            // ... and the ordering of actions ...
            var actionOrderFacet = specification.GetFacet<IActionOrderFacet>();

            // TODO: the calling of actionOrder() should be a facet
            string actionOrder = actionOrderFacet == null ? InvokeSortOrderMethod("Action") : actionOrderFacet.Value;
            orderedObjectActions = CreateOrderSet(actionOrder, findObjectActionMethods);

            // find the class actions ...
            INakedObjectActionPeer[] findClassActionMethods = FindActionMethods(MethodType.Class);

            // ... and the ordering of class actions
            // TODO: the calling of classActionOrder() should be a facet
            actionOrder = InvokeSortOrderMethod("ClassAction");

            orderedClassActions = CreateOrderSet(actionOrder, findClassActionMethods);
        }

        private INakedObjectMemberPeer[] FindAndCreateFieldPeers() {
            if (ClassStrategy.IsSystemClass(introspectedType)) {
                Log.DebugFormat("Skipping fields in {0} (system class according to ClassStrategy)", introspectedType.Name);
                return new INakedObjectMemberPeer[0];
            }

            Log.DebugFormat("Looking for fields for {0}", introspectedType);

            var candidates = new List<PropertyInfo>(properties);
            reflector.LoadSpecificationForReturnTypes(candidates, introspectedType);

            // now create FieldPeers for value properties, for collections and for reference properties
            var fieldPeers = new List<INakedObjectMemberPeer>();

            FindCollectionPropertiesAndCreateCorrespondingFieldPeers(candidates, fieldPeers);
            // every other accessor is assumed to be a reference property.
            FindPropertiesAndCreateCorrespondingFieldPeers(candidates, fieldPeers);

            return fieldPeers.ToArray();
        }

        private void FindCollectionPropertiesAndCreateCorrespondingFieldPeers(IList<PropertyInfo> candidates, ICollection<INakedObjectMemberPeer> fieldPeers) {
            var collectionProperties = new List<PropertyInfo>();
            FacetFactorySet.FindCollectionProperties(candidates, collectionProperties);
            CreateCollectionPeersFromAccessors(collectionProperties, fieldPeers);
        }

        /// <summary>
        ///     Since the value properties and collections have already been processed, this will
        ///     pick up the remaining reference properties
        /// </summary>
        private void FindPropertiesAndCreateCorrespondingFieldPeers(IList<PropertyInfo> candidates, ICollection<INakedObjectMemberPeer> fieldPeers) {
            var foundProperties = new List<PropertyInfo>();
            FacetFactorySet.FindProperties(candidates, foundProperties);
            CreatePropertyPeersFromAccessors(foundProperties, fieldPeers);
        }

        private void CreateCollectionPeersFromAccessors(IEnumerable<PropertyInfo> collectionProperties, ICollection<INakedObjectMemberPeer> fieldsListToAppendto) {
            foreach (PropertyInfo property in collectionProperties) {
                Log.DebugFormat("Identified one-many association method {0}", property);

                IIdentifier identifier = new IdentifierImpl((IMetadata)reflector, FullName, property.Name);

                // create property and add facets
                var collection = new DotNetOneToManyAssociationPeer((IMetadata)reflector, identifier, property.PropertyType);
                FacetFactorySet.Process(property, new DotnetIntrospectorMethodRemover(methods), collection, NakedObjectFeatureType.Collection);

                // figure out what the Type is
                var typeOfFacet = collection.GetFacet<ITypeOfFacet>();
                collection.ElementType = typeOfFacet != null ? typeOfFacet.Value : typeof (object);
                fieldsListToAppendto.Add(collection);
            }
        }

        /// <summary>
        ///     Creates a list of Association fields for all the properties that use NakedObjects.
        /// </summary>
        private void CreatePropertyPeersFromAccessors(IEnumerable<PropertyInfo> foundProperties, ICollection<INakedObjectMemberPeer> fieldListToAppendto) {
            foreach (PropertyInfo property in foundProperties) {
                Log.DebugFormat("Identified 1-1 association method {0}", property);
                Log.DebugFormat("One-to-One association {0} -> {1}", property.Name, property);

                IIdentifier identifier = new IdentifierImpl((IMetadata)reflector, FullName, property.Name);

                // create a reference property
                var referenceProperty = new DotNetOneToOneAssociationPeer((IMetadata)reflector, identifier, property.PropertyType);

                // Process facets for the property
                FacetFactorySet.Process(property, new DotnetIntrospectorMethodRemover(methods), referenceProperty, NakedObjectFeatureType.Property);

                fieldListToAppendto.Add(referenceProperty);
            }
        }

        private INakedObjectValidation[] FindAndCreateValidationPeers() {
            if (ClassStrategy.IsSystemClass(introspectedType)) {
                Log.DebugFormat("Skipping methods in {0} (system class according to ClassStrategy)", introspectedType.Name);
                return new INakedObjectValidation[0];
            }

            Log.DebugFormat("Looking for validate methods for {0}", introspectedType);

            var methodPeers = new List<INakedObjectValidation>();

            for (int i = 0; i < methods.Length; i++) {
                if (methods[i] == null) {
                    continue;
                }
                MethodInfo validateMethod = methods[i];
                if (!validateMethod.Name.Equals("Validate")) {
                    continue;
                }
                if (validateMethod.IsStatic) {
                    continue;
                }
                if (!validateMethod.ReturnType.Equals(typeof (string))) {
                    continue;
                }
                ParameterInfo[] parameters = validateMethod.GetParameters();
                if (parameters.Length < 2) {
                    continue;
                }
                bool parametersMatch = true;
                foreach (ParameterInfo parameter in parameters) {
                    string name = parameter.Name;
                    name = name[0].ToString().ToUpper() + name.Substring(1);
                    if (!ContainsField(name)) {
                        parametersMatch = false;
                        break;
                    }
                }
                if (!parametersMatch) {
                    continue;
                }

                methods[i] = null;
                methodPeers.Add(new NakedObjectValidationMethod(validateMethod));
            }
            return methodPeers.ToArray();
        }

        private bool ContainsField(string name) {
            foreach (IOrderableElement field in Fields) {
                var field1 = (INakedObjectAssociationPeer) field;
                if (field1.IsOneToOne && ((DotNetOneToOneAssociationPeer) field1).Identifier.MemberName.Equals(name)) {
                    return true;
                }
            }
            return false;
        }

        private INakedObjectActionPeer[] FindActionMethods(MethodType methodType) {
            if (ClassStrategy.IsSystemClass(introspectedType)) {
                Log.DebugFormat("Skipping fields in {0}(system class according to ClassStrategy)", introspectedType.Name);
                return new INakedObjectActionPeer[0];
            }

            Log.Debug("Looking for action methods");

            var actionPeers = new List<INakedObjectActionPeer>();

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

                if (actionMethod.GetCustomAttribute<NakedObjectsIgnoreAttribute>() != null) {
                    continue;
                }

                Log.DebugFormat("Identified action {0}", actionMethod);
                methods[i] = null;

                Type[] parameterTypes = actionMethod.GetParameters().Select(parameterInfo => parameterInfo.ParameterType).ToArray();

                IIdentifier identifier = new IdentifierImpl((IMetadata)reflector, FullName, fullMethodName, actionMethod.GetParameters().ToArray());

                // build action & its parameters

                DotNetNakedObjectActionParamPeer[] actionParams = parameterTypes.Select(pt => new DotNetNakedObjectActionParamPeer(GetSpecification(pt))).ToArray();
                var action = new DotNetNakedObjectActionPeer(identifier, actionParams);

                // Process facets on the action & parameters
                FacetFactorySet.Process(actionMethod, new DotnetIntrospectorMethodRemover(methods), action, NakedObjectFeatureType.Action);
                for (int l = 0; l < actionParams.Length; l++) {
                    FacetFactorySet.ProcessParams(actionMethod, l, actionParams[l]);
                }

                if (actionMethod.ReturnType != typeof (void)) {
                    reflector.LoadSpecification(actionMethod.ReturnType);
                }

                actionPeers.Add(action);
            }

            return actionPeers.ToArray();
        }

        /// <summary>
        ///     Searches for specific method and returns it, also removing it from the
        ///     array of methods <see cref="Methods" /> if found
        /// </summary>
        /// <seealso cref="MethodFinderUtils.RemoveMethod(MethodInfo[],MethodType,string,Type,Type[])" />
        private MethodInfo FindAndRemoveMethod(MethodType methodType, string name, Type returnType, Type[] paramTypes) {
            return MethodFinderUtils.RemoveMethod(methods, methodType, name, returnType, paramTypes);
        }

        private static OrderSet CreateOrderSet(string order, INakedObjectMemberPeer[] members) {
            if (order != null) {
                return SimpleOrderSet.CreateOrderSet(order, members);
            }
            return DeweyOrderSet.CreateOrderSet(members);
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

        private class DotnetIntrospectorMethodRemover : IMethodRemover {
            private readonly MethodInfo[] methods;

            public DotnetIntrospectorMethodRemover(MethodInfo[] methods) {
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
                        foreach (MethodInfo methodToRemove in methodList) {
                            if (methods[i].MemberInfoEquals(methodToRemove)) {
                                methods[i] = null;
                                break;
                            }
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
                bool xIsRecognised = x == null ? false : factories.Recognizes(x);
                bool yIsRecognised = y == null ? false : factories.Recognizes(y);

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
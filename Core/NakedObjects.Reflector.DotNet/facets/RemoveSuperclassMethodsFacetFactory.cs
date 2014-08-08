// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections.Generic;
using System.Reflection;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Util;

namespace NakedObjects.Reflector.DotNet.Facets {
    /// <summary>
    ///     Removes all methods inherited from <see cref="object" />
    /// </summary>
    /// <para>
    ///     Implementation - .Net fails to find methods properly for root class, so we used the saved set.
    /// </para>
    public class RemoveSuperclassMethodsFacetFactory : FacetFactoryAbstract {
        private static readonly IDictionary<Type, MethodInfo[]> typeToMethods;

        static RemoveSuperclassMethodsFacetFactory() {
            typeToMethods = new Dictionary<Type, MethodInfo[]>();
        }

        public RemoveSuperclassMethodsFacetFactory(INakedObjectReflector reflector)
            : base(reflector, NakedObjectFeatureType.ObjectsOnly) { }

        private static void InitForType(Type type) {
            if (!typeToMethods.ContainsKey(type)) {
                typeToMethods.Add(type, type.GetMethods());
            }
        }

        public void ProcessSystemType(Type type, IMethodRemover methodRemover, IFacetHolder holder) {
            InitForType(type);
            foreach (MethodInfo method in typeToMethods[type]) {
                if (methodRemover != null && method != null) {
                    methodRemover.RemoveMethod(method);
                }
            }
        }

        public override bool Process(Type type, IMethodRemover methodRemover, IFacetHolder holder) {
            Type currentType = type;
            while (currentType != null) {
                if (TypeUtils.IsSystem(currentType)) {
                    ProcessSystemType(currentType, methodRemover, holder);
                }
                currentType = currentType.BaseType;
            }
            return false;
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}
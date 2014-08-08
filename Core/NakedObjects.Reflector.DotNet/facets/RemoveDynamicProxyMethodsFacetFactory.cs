// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Reflector.DotNet.Facets.Hide;

namespace NakedObjects.Reflector.DotNet.Facets {
    public class RemoveDynamicProxyMethodsFacetFactory : FacetFactoryAbstract {
        private static readonly IList<string> methodsToRemove;

        static RemoveDynamicProxyMethodsFacetFactory() {
            methodsToRemove = new List<string> {"GetBasePropertyValue", "SetBasePropertyValue", "SetChangeTracker"};
        }

        public RemoveDynamicProxyMethodsFacetFactory(INakedObjectReflector reflector)
            : base(reflector, NakedObjectFeatureType.ObjectsAndProperties) {}

        private static bool IsDynamicProxyType(Type type) {
            return type.FullName.StartsWith("System.Data.Entity.DynamicProxies");
        }

        public override bool Process(Type type, IMethodRemover methodRemover, IFacetHolder holder) {
            if (IsDynamicProxyType(type)) {
                foreach (MethodInfo method in type.GetMethods().Join(methodsToRemove, mi => mi.Name, s => s, (mi, s) => mi)) {
                    if (methodRemover != null && method != null) {
                        methodRemover.RemoveMethod(method);
                    }
                }
            }

            return false;
        }

        public override bool Process(PropertyInfo property, IMethodRemover methodRemover, IFacetHolder holder) {
            if (IsDynamicProxyType(property.DeclaringType) && property.Name == "RelationshipManager") {
                return FacetUtils.AddFacet(new HiddenFacetAnnotation(When.Always, holder));
            }
            return false;
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}
// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Reflection;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Reflect;

namespace NakedObjects.Reflector.DotNet.Facets {
    /// <summary>
    ///     Removes any calls to <c>Init</c>
    /// </summary>
    public class RemoveInitMethodFacetFactory : MethodPrefixBasedFacetFactoryAbstract {
        public RemoveInitMethodFacetFactory(INakedObjectReflector reflector)
            :base(reflector, FeatureType.ObjectsOnly) { }

        public override string[] Prefixes {
            get { return new string[] {}; }
        }

        public override bool Process(Type type, IMethodRemover methodRemover, ISpecification specification) {
            FindAndRemoveInitMethod(type, methodRemover);
            return false;
        }

        private void FindAndRemoveInitMethod(Type type, IMethodRemover methodRemover) {
            MethodInfo method = FindMethod(type, MethodType.Object, "Init", typeof (void), Type.EmptyTypes);
            if (method != null) {
                RemoveMethod(methodRemover, method);
            }
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}
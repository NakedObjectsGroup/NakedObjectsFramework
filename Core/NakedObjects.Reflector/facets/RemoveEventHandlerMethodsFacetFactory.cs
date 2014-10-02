// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Reflection;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Util;

namespace NakedObjects.Reflector.DotNet.Facets {
    /// <summary>
    ///     Removes any calls to <c>Init</c>
    /// </summary>
    public class RemoveEventHandlerMethodsFacetFactory : MethodPrefixBasedFacetFactoryAbstract {
        public RemoveEventHandlerMethodsFacetFactory(IMetadata metadata)
            : base(metadata, NakedObjectFeatureType.ObjectsOnly) {}

        public override string[] Prefixes {
            get { return new string[] {}; }
        }

        public override bool Process(Type type, IMethodRemover methodRemover, IFacetHolder holder) {
            FindAndRemoveEventHandlerMethods(type, methodRemover);
            return false;
        }

        private void RemoveIfNotNull(IMethodRemover methodRemover, MethodInfo mi) {
            if (mi != null) {
                RemoveMethod(methodRemover, mi);
            }
        }

        private void FindAndRemoveEventHandlerMethods(Type type, IMethodRemover methodRemover) {
            foreach (EventInfo eInfo in type.GetEvents()) {
                RemoveIfNotNull(methodRemover, eInfo.GetAddMethod());
                RemoveIfNotNull(methodRemover, eInfo.GetRaiseMethod());
                RemoveIfNotNull(methodRemover, eInfo.GetRemoveMethod());

                eInfo.GetOtherMethods().ForEach(mi => RemoveIfNotNull(methodRemover, mi));
            }
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}
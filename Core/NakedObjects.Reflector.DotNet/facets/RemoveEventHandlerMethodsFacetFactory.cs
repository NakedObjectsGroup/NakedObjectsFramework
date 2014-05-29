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
        public RemoveEventHandlerMethodsFacetFactory()
            : base(NakedObjectFeatureType.ObjectsOnly) {}

        public override string[] Prefixes {
            get { return new string[] {}; }
        }

        public override bool Process(Type type, IMethodRemover methodRemover, IFacetHolder holder) {
            FindAndRemoveEventHandlerMethods(type, methodRemover);
            return false;
        }

        private void FindAndRemoveEventHandlerMethods(Type type, IMethodRemover methodRemover) {
            foreach (EventInfo eInfo in type.GetEvents()) {
                RemoveMethod(methodRemover, eInfo.GetAddMethod());
                RemoveMethod(methodRemover, eInfo.GetRaiseMethod());
                RemoveMethod(methodRemover, eInfo.GetRemoveMethod());
                RemoveMethod(methodRemover, eInfo.GetAddMethod());

                eInfo.GetOtherMethods().ForEach(mi => RemoveMethod(methodRemover, mi));
            }
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}
// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections;
using System.Reflection;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Util;

namespace NakedObjects.Reflector.DotNet.Facets.Actions {
    /// <summary>
    ///     Designed to simply filter out <see cref="IEnumerable.GetEnumerator" /> method if it exists.
    /// </summary>
    /// <para>
    ///     Does not add any <see cref="IFacet" />s
    /// </para>
    public class IteratorFilteringFacetFactory : MethodPrefixBasedFacetFactoryAbstract {
        private static readonly string[] FixedPrefixes;

        static IteratorFilteringFacetFactory() {
            FixedPrefixes = new[] {PrefixesAndRecognisedMethods.GetEnumeratorMethod};
        }

        public IteratorFilteringFacetFactory(IMetadata metadata)
            : base(metadata, NakedObjectFeatureType.ObjectsOnly) { }

        public override string[] Prefixes {
            get { return FixedPrefixes; }
        }

        public override bool Process(Type type, IMethodRemover methodRemover, IFacetHolder holder) {
            if (typeof (IEnumerable).IsAssignableFrom(type) && !TypeUtils.IsSystem(type)) {
                MethodInfo method = FindMethod(type, MethodType.Object, PrefixesAndRecognisedMethods.GetEnumeratorMethod, null, Type.EmptyTypes);
                if (method != null) {
                    methodRemover.RemoveMethod(method);
                }
            }
            return false;
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}
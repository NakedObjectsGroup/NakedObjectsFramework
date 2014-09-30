// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Reflection;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Reflect;

namespace NakedObjects.Reflector.DotNet.Facets.Hide {
    /// <summary>
    ///     Note - this factory simply removes the class level attribute from the list of methods.  The action and properties look up this attribute directly
    /// </summary>
    internal class DisableDefaultMethodFacetFactory : MethodPrefixBasedFacetFactoryAbstract {
        private static readonly string[] FixedPrefixes;

        static DisableDefaultMethodFacetFactory() {
            FixedPrefixes = new[] {
                PrefixesAndRecognisedMethods.DisablePrefix + "Action" + PrefixesAndRecognisedMethods.DefaultPrefix,
                PrefixesAndRecognisedMethods.DisablePrefix + "Property" + PrefixesAndRecognisedMethods.DefaultPrefix,
            };
        }

        public DisableDefaultMethodFacetFactory(INakedObjectReflector reflector)
            : base(reflector, NakedObjectFeatureType.ObjectsOnly) { }


        public override string[] Prefixes {
            get { return FixedPrefixes; }
        }

        public override bool Process(Type type, IMethodRemover methodRemover, IFacetHolder facetHolder) {
            try {
                foreach (string methodName in FixedPrefixes) {
                    MethodInfo methodInfo = FindMethod(type, MethodType.Object, methodName, typeof (string), Type.EmptyTypes);
                    if (methodInfo != null) {
                        methodRemover.RemoveMethod(methodInfo);
                    }
                }
                return false;
            }
            catch {
                return false;
            }
        }
    }
}
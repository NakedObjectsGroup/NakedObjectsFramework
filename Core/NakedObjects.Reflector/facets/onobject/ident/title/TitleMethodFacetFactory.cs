// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections.Generic;
using System.Linq;
using Common.Logging;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Util;
using MethodInfo = System.Reflection.MethodInfo;
using PropertyInfo = System.Reflection.PropertyInfo;
using BindingFlags = System.Reflection.BindingFlags;

namespace NakedObjects.Reflector.DotNet.Facets.Objects.Ident.Title {
    public class TitleMethodFacetFactory : MethodPrefixBasedFacetFactoryAbstract {
        private static readonly ILog Log = LogManager.GetLogger(typeof (TitleMethodFacetFactory));
        private static readonly string[] FixedPrefixes;

        static TitleMethodFacetFactory() {
            FixedPrefixes = new[] {
                PrefixesAndRecognisedMethods.ToStringMethod,
                PrefixesAndRecognisedMethods.TitleMethod
            };
        }

        public TitleMethodFacetFactory(INakedObjectReflector reflector)
            :base(reflector, NakedObjectFeatureType.ObjectsOnly) { }

        public override string[] Prefixes {
            get { return FixedPrefixes; }
        }

        /// <summary>
        ///     If no title or ToString can be used then will use Facets provided by
        ///     <see cref="FallbackFacetFactory" /> instead.
        /// </summary>
        public override bool Process(Type type, IMethodRemover methodRemover, IFacetHolder facetHolder) {
            IList<MethodInfo> attributedMethods = new List<MethodInfo>();
            foreach (PropertyInfo propertyInfo in type.GetProperties(BindingFlags.Public | BindingFlags.Instance)) {
                if (propertyInfo.GetCustomAttribute<TitleAttribute>() != null) {
                    if (attributedMethods.Count > 0) {
                        Log.Warn("Title annotation is used more than once in " + type.Name + ", this time on property " + propertyInfo.Name + "; this will be ignored");
                    }
                    attributedMethods.Add(propertyInfo.GetGetMethod());
                }
            }

            if (attributedMethods.Count > 0) {
                return FacetUtils.AddFacet(new TitleFacetViaProperty(attributedMethods.First(), facetHolder));
            }

            try {
                MethodInfo titleMethod = FindMethod(type, MethodType.Object, PrefixesAndRecognisedMethods.TitleMethod, typeof (string), Type.EmptyTypes);
                IFacet titleFacet = null;

                if (titleMethod != null) {
                    methodRemover.RemoveMethod(titleMethod);
                    titleFacet = new TitleFacetViaTitleMethod(titleMethod, facetHolder);
                }

                MethodInfo toStringMethod = FindMethod(type, MethodType.Object, PrefixesAndRecognisedMethods.ToStringMethod, typeof (string), Type.EmptyTypes);
                if (toStringMethod != null && !toStringMethod.DeclaringType.Equals(typeof (object))) {
                    methodRemover.RemoveMethod(toStringMethod);
                }
                else {
                    // on object do not use 
                    toStringMethod = null;
                }

                MethodInfo maskMethod = FindMethod(type, MethodType.Object, PrefixesAndRecognisedMethods.ToStringMethod, typeof (string), new[] {typeof (string)});

                if (maskMethod != null) {
                    methodRemover.RemoveMethod(maskMethod);
                }

                if (titleFacet == null && toStringMethod == null) {
                    // nothing to use 
                    return false;
                }

                if (titleFacet == null) {
                    titleFacet = new TitleFacetViaToStringMethod(toStringMethod, maskMethod, facetHolder);
                }

                return FacetUtils.AddFacet(titleFacet);
            }
            catch {
                return false;
            }
        }
    }
}
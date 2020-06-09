// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Common.Logging;
using Microsoft.Extensions.Logging;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Meta.Facet;
using NakedObjects.Meta.Utils;

namespace NakedObjects.Reflect.FacetFactory {
    public sealed class TitleMethodFacetFactory : MethodPrefixBasedFacetFactoryAbstract {
        private static readonly ILog Log = LogManager.GetLogger(typeof(TitleMethodFacetFactory));

        private static readonly string[] FixedPrefixes = {
            RecognisedMethodsAndPrefixes.ToStringMethod,
            RecognisedMethodsAndPrefixes.TitleMethod
        };

        public TitleMethodFacetFactory(int numericOrder, ILoggerFactory loggerFactory)
            : base(numericOrder, loggerFactory, FeatureType.ObjectsAndInterfaces) { }

        public override string[] Prefixes => FixedPrefixes;

        /// <summary>
        ///     If no title or ToString can be used then will use Facets provided by
        ///     <see cref="FallbackFacetFactory" /> instead.
        /// </summary>
        public override void Process(IReflector reflector, Type type, IMethodRemover methodRemover, ISpecificationBuilder specification) {
            IList<MethodInfo> attributedMethods = new List<MethodInfo>();
            foreach (var propertyInfo in type.GetProperties(BindingFlags.Public | BindingFlags.Instance)) {
                if (propertyInfo.GetCustomAttribute<TitleAttribute>() != null) {
                    if (attributedMethods.Count > 0) {
                        Log.Warn("Title annotation is used more than once in " + type.Name + ", this time on property " + propertyInfo.Name + "; this will be ignored");
                    }

                    attributedMethods.Add(propertyInfo.GetGetMethod());
                }
            }

            if (attributedMethods.Count > 0) {
                // attributes takes priority
                FacetUtils.AddFacet(new TitleFacetViaProperty(attributedMethods.First(), specification));
                return;
            }

            try {
                var titleMethod = FindMethod(reflector, type, MethodType.Object, RecognisedMethodsAndPrefixes.TitleMethod, typeof(string), Type.EmptyTypes);
                IFacet titleFacet = null;

                if (titleMethod != null) {
                    methodRemover.RemoveMethod(titleMethod);
                    titleFacet = new TitleFacetViaTitleMethod(titleMethod, specification);
                }

                var toStringMethod = FindMethod(reflector, type, MethodType.Object, RecognisedMethodsAndPrefixes.ToStringMethod, typeof(string), Type.EmptyTypes);
                if (toStringMethod != null && !(toStringMethod.DeclaringType == typeof(object))) {
                    methodRemover.RemoveMethod(toStringMethod);
                }
                else {
                    // on object do not use 
                    toStringMethod = null;
                }

                var maskMethod = FindMethod(reflector, type, MethodType.Object, RecognisedMethodsAndPrefixes.ToStringMethod, typeof(string), new[] {typeof(string)});

                if (maskMethod != null) {
                    methodRemover.RemoveMethod(maskMethod);
                }

                if (titleFacet == null && toStringMethod != null) {
                    titleFacet = new TitleFacetViaToStringMethod(maskMethod, specification);
                }

                FacetUtils.AddFacet(titleFacet);
            }
            catch (Exception e) {
                Log.Error("Unexpected Exception", e);
            }
        }
    }
}
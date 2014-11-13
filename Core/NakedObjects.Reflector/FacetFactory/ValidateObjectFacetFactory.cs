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
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Core.Util;
using NakedObjects.Meta.Facet;
using NakedObjects.Meta.Utils;
using NakedObjects.Util;

namespace NakedObjects.Reflect.FacetFactory {
    public class ValidateObjectFacetFactory : MethodPrefixBasedFacetFactoryAbstract {
        private static readonly ILog Log = LogManager.GetLogger(typeof (ValidateObjectFacetFactory));
        private static readonly string[] FixedPrefixes;

        static ValidateObjectFacetFactory() {
            FixedPrefixes = new[] {
                PrefixesAndRecognisedMethods.ValidatePrefix
            };
        }

        public ValidateObjectFacetFactory(IReflector reflector)
            : base(reflector, FeatureType.Objects) {}

        public override string[] Prefixes {
            get { return FixedPrefixes; }
        }

        private bool ContainsField(string name, Type type) {
            var properties = type.GetProperties();

            return properties.Any(p => p.Name == name &&
                                       p.GetGetMethod() != null &&
                                       AttributeUtils.GetCustomAttribute<NakedObjectsIgnoreAttribute>(p) == null &&
                                       !CollectionUtils.IsCollection(p.PropertyType) &&
                                       !CollectionUtils.IsQueryable(p.PropertyType));
        }

        public override bool Process(Type type, IMethodRemover methodRemover, ISpecificationBuilder specification) {
            Log.DebugFormat("Looking for validate methods for {0}", type);

            var methodPeers = new List<ValidateObjectFacet.NakedObjectValidationMethod>();
            var methods = FindMethods(type, MethodType.Object, PrefixesAndRecognisedMethods.ValidatePrefix, typeof (string));

            if (methods.Any()) {
                foreach (var method in methods) {
                    ParameterInfo[] parameters = method.GetParameters();
                    if (parameters.Length >= 2) {
                        bool parametersMatch = parameters.Select(parameter => parameter.Name).Select(name => name[0].ToString().ToUpper() + name.Substring(1)).All(p => ContainsField(p, type));
                        if (parametersMatch) {
                            methodPeers.Add(new ValidateObjectFacet.NakedObjectValidationMethod(method));
                            methodRemover.RemoveMethod(method);
                        }
                    }
                }
            }

            var validateFacet = methodPeers.Any() ? (IValidateObjectFacet)  new ValidateObjectFacet(specification, methodPeers) : new ValidateObjectFacetNull(specification);
            return FacetUtils.AddFacet(validateFacet);
        }
    }
}
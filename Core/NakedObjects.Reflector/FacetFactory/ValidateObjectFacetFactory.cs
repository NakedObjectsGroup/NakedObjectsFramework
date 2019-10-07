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
using System.Threading;
using Common.Logging;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Core.Util;
using NakedObjects.Meta.Facet;
using NakedObjects.Meta.Utils;

namespace NakedObjects.Reflect.FacetFactory {
    public sealed class ValidateObjectFacetFactory : MethodPrefixBasedFacetFactoryAbstract {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ValidateObjectFacetFactory));

        private static readonly string[] FixedPrefixes = {
            RecognisedMethodsAndPrefixes.ValidatePrefix
        };

        public ValidateObjectFacetFactory(int numericOrder)
            : base(numericOrder, FeatureType.ObjectsAndInterfaces) { }

        public override string[] Prefixes {
            get { return FixedPrefixes; }
        }

        private bool ContainsField(string name, Type type) {
            PropertyInfo[] properties = type.GetProperties();

            return properties.Any(p => p.Name == name &&
                                       p.GetGetMethod() != null &&
                                       p.GetCustomAttribute<NakedObjectsIgnoreAttribute>() == null &&
                                       !CollectionUtils.IsCollection(p.PropertyType) &&
                                       !CollectionUtils.IsQueryable(p.PropertyType));
        }

        public override void Process(IReflector reflector, Type type, IMethodRemover methodRemover, ISpecificationBuilder specification) {
            var methodPeers = new List<ValidateObjectFacet.NakedObjectValidationMethod>();
            MethodInfo[] methods = FindMethods(reflector, type, MethodType.Object, RecognisedMethodsAndPrefixes.ValidatePrefix, typeof(string));

            if (methods.Any()) {
                foreach (MethodInfo method in methods) {
                    ParameterInfo[] parameters = method.GetParameters();
                    if (parameters.Length >= 2) {
                        bool parametersMatch = parameters.Select(parameter => parameter.Name).Select(name => name[0].ToString(Thread.CurrentThread.CurrentCulture).ToUpper() + name.Substring(1)).All(p => ContainsField(p, type));
                        if (parametersMatch) {
                            methodPeers.Add(new ValidateObjectFacet.NakedObjectValidationMethod(method));
                            methodRemover.RemoveMethod(method);
                        }
                    }
                }
            }

            IValidateObjectFacet validateFacet = methodPeers.Any() ? (IValidateObjectFacet) new ValidateObjectFacet(specification, methodPeers) : new ValidateObjectFacetNull(specification);
            FacetUtils.AddFacet(validateFacet);
        }
    }
}
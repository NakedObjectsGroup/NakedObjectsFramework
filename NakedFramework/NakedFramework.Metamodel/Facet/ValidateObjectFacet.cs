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
using System.Runtime.Serialization;
using Microsoft.Extensions.Logging;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Spec;
using NakedFramework.Core.Exception;
using NakedFramework.Core.Util;

namespace NakedFramework.Metamodel.Facet {
    [Serializable]
    public sealed class ValidateObjectFacet : FacetAbstract, IValidateObjectFacet {
        private readonly ILogger<ValidateObjectFacet> logger;

        public ValidateObjectFacet(ISpecification holder, IList<NakedObjectValidationMethod> validateMethods, ILogger<ValidateObjectFacet> logger)
            : base(Type, holder) {
            this.logger = logger;
            ValidateMethods = validateMethods;
        }

        public static Type Type => typeof(IValidateObjectFacet);

        private IEnumerable<NakedObjectValidationMethod> ValidateMethods { get; set; }

        #region IValidateObjectFacet Members

        public string Validate(INakedObjectAdapter nakedObjectAdapter) {
            foreach (var validator in ValidateMethods) {
                var objectSpec = nakedObjectAdapter.Spec as IObjectSpec ?? throw new NakedObjectSystemException("nakedObjectAdapter.Spec must be IObjectSpec");

                var matches = validator.ParameterNames.Select(name => objectSpec.Properties.SingleOrDefault(p => p.Id.ToLower() == name)).Where(s => s != null).ToArray();

                if (matches.Length == validator.ParameterNames.Length) {
                    var parameters = matches.Select(s => s.GetNakedObject(nakedObjectAdapter)).ToArray();
                    var result = validator.Execute(nakedObjectAdapter, parameters);
                    if (result != null) {
                        return result;
                    }
                }
                else {
                    var actual = objectSpec.Properties.Select(s => s.Id).Aggregate((s, t) => $"{s} {t}");
                    LogNoMatch(validator, actual);
                }
            }

            return null;
        }

        public string ValidateParms(INakedObjectAdapter nakedObjectAdapter, (string name, INakedObjectAdapter value)[] parms) {
            foreach (var validator in ValidateMethods) {
                var matches = validator.ParameterNames.Select(name => parms.SingleOrDefault(p => p.name.ToLower() == name)).Where(p => p != default).ToArray();

                if (matches.Length == validator.ParameterNames.Length) {
                    var parameters = matches.Select(p => p.value).ToArray();
                    var result = validator.Execute(nakedObjectAdapter, parameters);
                    if (result != null) {
                        return result;
                    }
                }
                else {
                    var actual = parms.Select(s => s.name).Aggregate((s, t) => $"{s} {t}");
                    LogNoMatch(validator, actual);
                }
            }

            return null;
        }

        #endregion

        private void LogNoMatch(NakedObjectValidationMethod validator, string actual) {
            var expects = validator.ParameterNames.Aggregate((s, t) => $"{s} {t}");
            logger.LogWarning($"No Matching parms Validator: {validator.Name} Expects {expects} Actual {actual} ");
        }

        #region Nested type: NakedObjectValidationMethod

        [Serializable]
        public class NakedObjectValidationMethod {
            private readonly ILogger<NakedObjectValidationMethod> logger;
            private readonly MethodInfo method;

            [field: NonSerialized] private Func<object, object[], object> methodDelegate;

            public NakedObjectValidationMethod(MethodInfo method, ILogger<NakedObjectValidationMethod> logger) {
                this.method = method;
                this.logger = logger;
                methodDelegate = LogNull(DelegateUtils.CreateDelegate(method), logger);
            }

            public string Name => method.Name;

            public string[] ParameterNames => method.GetParameters().Select(p => p.Name.ToLower()).ToArray();

            public string Execute(INakedObjectAdapter obj, INakedObjectAdapter[] parameters) => methodDelegate(obj.GetDomainObject(), parameters.Select(no => no.GetDomainObject()).ToArray()) as string;

            [OnDeserialized]
            private void OnDeserialized(StreamingContext context) => methodDelegate = LogNull(DelegateUtils.CreateDelegate(method), logger);
        }

        #endregion
    }
}
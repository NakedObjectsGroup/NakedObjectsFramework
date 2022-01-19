// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Reflection;
using Microsoft.Extensions.Logging;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Spec;
using NakedFramework.Metamodel.Facet;

namespace NakedLegacy.Reflector.Facet;

[Serializable]
public sealed class ValidateObjectViaAboutFacet : FacetAbstract, IValidateObjectFacet, IImperativeFacet {
    private readonly ILogger<ValidateObjectViaAboutFacet> logger;
    private readonly MethodInfo validateMethod;

    public ValidateObjectViaAboutFacet(ISpecification holder, MethodInfo validateMethod, ILogger<ValidateObjectViaAboutFacet> logger)
        : base(Type, holder) {
        this.validateMethod = validateMethod;
        this.logger = logger;
    }

    public static Type Type => typeof(IValidateObjectFacet);
    public MethodInfo GetMethod() => validateMethod;

    public Func<object, object[], object> GetMethodDelegate() => throw new NotImplementedException();

    public string Validate(INakedObjectAdapter nakedObjectAdapter) => null;

    public string ValidateParms(INakedObjectAdapter nakedObjectAdapter, (string name, INakedObjectAdapter value)[] parms) => null;
}
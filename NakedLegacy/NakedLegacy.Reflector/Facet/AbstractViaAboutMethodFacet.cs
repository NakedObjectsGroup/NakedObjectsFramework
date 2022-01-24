// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Reflection;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Framework;
using NakedFramework.Architecture.Spec;
using NakedFramework.Metamodel.Facet;

namespace NakedLegacy.Reflector.Facet;

public class AbstractViaAboutMethodFacet : FacetAbstract, IImperativeFacet {
    protected AbstractViaAboutMethodFacet(Type facetType, ISpecification holder, MethodInfo method, AboutHelpers.AboutType aboutType) : base(facetType, holder) {
        Method = method;
        AboutType = aboutType;
    }

    protected MethodInfo Method { get; }
    protected AboutHelpers.AboutType AboutType { get; }
    public MethodInfo GetMethod() => Method;

    protected IAbout InvokeAboutMethod(INakedFramework framework, object target, AboutTypeCodes typeCode, bool substitute, bool flagNull, params object[] proposedValues) {
        if (target is null && !Method.IsStatic) {
            if (flagNull) {
                throw new InvalidOperationException("Unexpected null object on instance about method");
            }

            return null;
        }

        var about = AboutType.AboutFactory(typeCode);
        Method.Invoke(target, Method.GetParameters(framework, about, substitute, proposedValues));
        return about;
    }

    public Func<object, object[], object> GetMethodDelegate() => throw new NotImplementedException();

    protected override string ToStringValues() => $"method={Method}";
}
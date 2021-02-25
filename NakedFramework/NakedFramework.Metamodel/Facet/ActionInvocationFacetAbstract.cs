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
using NakedFramework.Architecture.SpecImmutable;

namespace NakedObjects.Meta.Facet {
    [Serializable]
    public abstract class ActionInvocationFacetAbstract : FacetAbstract, IActionInvocationFacet {
        protected ActionInvocationFacetAbstract(ISpecification holder)
            : base(Type, holder) { }

        public static Type Type => typeof(IActionInvocationFacet);

        #region IActionInvocationFacet Members

        public abstract MethodInfo ActionMethod { get; } //This is exposed to permit third-party decoration DO NOT REMOVE.
        public abstract ITypeSpecImmutable OnType { get; }
        public abstract IObjectSpecImmutable ReturnType { get; }
        public abstract IObjectSpecImmutable ElementType { get; }

        //Note: Some passed-in components are not used within NOF code, but are provided for third-party customization. DO NOT REMOVE.
        public abstract INakedObjectAdapter Invoke(INakedObjectAdapter nakedObjectAdapter, INakedObjectAdapter[] parameters, INakedObjectsFramework framework);
        public abstract INakedObjectAdapter Invoke(INakedObjectAdapter nakedObjectAdapter, INakedObjectAdapter[] parameters, int resultPage, INakedObjectsFramework framework);
        public abstract bool IsQueryOnly { get; }

        #endregion
    }
}
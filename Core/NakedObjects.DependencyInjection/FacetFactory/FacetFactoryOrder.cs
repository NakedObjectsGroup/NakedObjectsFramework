// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedObjects.Architecture.Component;

namespace NakedObjects.DependencyInjection.FacetFactory {


    public class FacetFactoryOrder<T> : IFacetFactoryOrder<T> {
        private readonly Type[] facetFactories;

        public FacetFactoryOrder(FacetFactoryTypesProvider facetFactories) => this.facetFactories = facetFactories.FacetFactoryTypes;

        public int Order => Array.IndexOf(facetFactories, typeof(T));
    }

    public class TestFacetFactoryOrder<T> : IFacetFactoryOrder<T>
    {
        private readonly Type[] facetFactories;

        public TestFacetFactoryOrder(Type[] facetFactories) => this.facetFactories = facetFactories;

        public int Order => Array.IndexOf(facetFactories, typeof(T));
    }


    public class AppendFacetFactoryOrder<T> : IFacetFactoryOrder<T> {
        public AppendFacetFactoryOrder(Type[] facetFactories) => LastIndex = facetFactories.Length - 1;

        private int LastIndex { get; set; }

        public int Order => ++LastIndex;
    }
}
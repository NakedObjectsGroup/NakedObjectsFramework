// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Reflection;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;

namespace NakedObjects.Architecture.Component {
    public interface IFacetFactorySet {
        void FindCollectionProperties(IList<PropertyInfo> candidates, IList<PropertyInfo> methodListToAppendTo);
        void FindProperties(IList<PropertyInfo> candidates, IList<PropertyInfo> methodListToAppendTo);


        /// <summary>
        ///     Whether this <see cref="MethodInfo" /> is recognized by any of the <see cref="IFacetFactory" />s.
        /// </summary>
        /// <para>
        ///     Typically this is when the method has a specific prefix, such as <c>Validate</c> or <c>Hide</c>.
        /// </para>
        bool Recognizes(MethodInfo method);

        /// <summary>
        ///     Whether this <see cref="MethodInfo" /> is filtered by any of the <see cref="IFacetFactory" />s.
        /// </summary>
        bool Filters(MethodInfo method);

        /// <summary>
        ///     Delegates to <see cref="IFacetFactory.Process(Type,IMethodRemover,ISpecification)" /> for each appropriate factory.
        /// </summary>
        /// <param name="type">type to process</param>
        /// <param name="methodRemover">allow any methods of the class to be removed</param>
        /// <param name="specificationholder to attach facets to</param>
        /// <returns>
        ///     <c>true</c> if any facets were added, <c>false</c> otherwise.
        /// </returns>
        bool Process(Type type, IMethodRemover methodRemover, ISpecification specification);

        /// <summary>
        ///     Delegates to <see cref="IFacetFactory.Process(MethodInfo,IMethodRemover,ISpecification)" />for each appropriate factory.
        /// </summary>
        /// <param name="method">method to process</param>
        /// <param name="methodRemover">allow any methods of the class to be removed</param>
        /// <param name="specificationholder to attach facets to</param>
        /// <param name="featureType">what type of feature the method represents (property, action, collection etc)</param>
        /// <returns>
        ///     <c>true</c> if any facets were added, <c>false</c> otherwise.
        /// </returns>
        bool Process(MethodInfo method, IMethodRemover methodRemover, ISpecification specification, FeatureType featureType);

        /// <summary>
        ///     Delegates to <see cref="IFacetFactory.Process(PropertyInfo,IMethodRemover,ISpecification)" />for each appropriate factory.
        /// </summary>
        /// <param name="property">property to process</param>
        /// <param name="methodRemover">allow any methods of the class to be removed</param>
        /// <param name="specificationholder to attach facets to</param>
        /// <param name="featureType">what type of feature the method represents (property, action, collection etc)</param>
        /// <returns>
        ///     <c>true</c> if any facets were added, <c>false</c> otherwise.
        /// </returns>
        bool Process(PropertyInfo property, IMethodRemover methodRemover, ISpecification specification, FeatureType featureType);

        /// <summary>
        ///     Delegates to <see cref="IFacetFactory.ProcessParams" /> for each appropriate factory.
        /// </summary>
        /// <param name="method">action method to process</param>
        /// <param name="paramNum">zero-based</param>
        /// <param name="specificationholder to attach facets to</param>
        /// <returns>
        ///     <c>true</c> if any facets were added, <c>false</c> otherwise.
        /// </returns>
        bool ProcessParams(MethodInfo method, int paramNum, ISpecification specification);

        void Init(INakedObjectReflector reflector);
    }
}
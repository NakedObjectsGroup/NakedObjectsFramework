// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Reflection;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;

namespace NakedObjects.Architecture.FacetFactory {
    public interface IFacetFactory {
        /// <summary>
        ///     The <see cref="FeatureType" />s that this facet factory can create <see cref="IFacet" />s for.
        /// </summary>
        /// <para>
        ///     Used by the <see cref="IFacetFactorySet" /> to reduce the number of <see cref="IFacetFactory" />s that are
        ///     queried when building up the meta-model.
        /// </para>
        FeatureType FeatureTypes { get; }

        /// <summary>
        ///     Process the class, and return the correctly setup annotation if present.
        /// </summary>
        /// <param name="type">class being processed</param>
        /// <param name="methodRemover">allow any methods of the class to be removed</param>
        /// <param name="specification"> attach the facets to</param>
        /// <returns>
        ///     <c>true</c> if any facets were added, <c>false</c> otherwise.
        /// </returns>
        bool Process(Type type, IMethodRemover methodRemover, ISpecificationBuilder specification);

        /// <summary>
        ///     Process the method, and return the correctly setup annotation if present.
        /// </summary>
        /// <param name="method">MethodInfo representing the feature being processed</param>
        /// <param name="methodRemover">allow any methods of the class to be removed</param>
        /// <param name="specification"> attach the facets to</param>
        /// <returns>
        ///     <c>true</c> if any facets were added and therefore should be removed, <c>false</c> otherwise.
        ///     Returning true will cause the method to be removed
        /// </returns>
        bool Process(MethodInfo method, IMethodRemover methodRemover, ISpecificationBuilder specification);

        /// <summary>
        ///     Process the property, and return the correctly setup annotation if present.
        /// </summary>
        /// <param name="property">PropertyInfo representing the feature being processed</param>
        /// <param name="methodRemover">allow any methods of the class to be removed</param>
        /// <param name="specification"> attach the facets to</param>
        /// <returns>
        ///     <c>true</c> if any facets were added  <c>false</c> otherwise.
        /// </returns>
        bool Process(PropertyInfo property, IMethodRemover methodRemover, ISpecificationBuilder specification);

        /// <summary>
        ///     Process the parameters of the method, and return the correctly setup annotation if present.
        /// </summary>
        /// <param name="method">MethodInfo representing the feature being processed</param>
        /// <param name="paramNum">zero-based index to the parameter to be processed</param>
        /// <param name="holder">to attach the facets to</param>
        /// <returns>
        ///     <c>true</c> if any facets were added, <c>false</c> otherwise.
        /// </returns>
        bool ProcessParams(MethodInfo method, int paramNum, ISpecificationBuilder holder);

        IList<PropertyInfo> FindCollectionProperties(IList<PropertyInfo> candidates);

        IList<PropertyInfo> FindProperties(IList<PropertyInfo> candidates);
    }
}
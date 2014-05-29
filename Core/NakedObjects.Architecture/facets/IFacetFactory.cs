// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections.Generic;
using System.Reflection;
using NakedObjects.Architecture.Reflect;

namespace NakedObjects.Architecture.Facets {
    public interface IFacetFactory {
        /// <summary>
        ///     The <see cref="NakedObjectFeatureType" />s that this facet factory can create <see cref="IFacet" />s for.
        /// </summary>
        /// <para>
        ///     Used by the <see cref="IFacetFactorySet" /> to reduce the number of <see cref="IFacetFactory" />s that are
        ///     queried when building up the meta-model.
        /// </para>
        NakedObjectFeatureType[] FeatureTypes { get; }

        /// <summary>
        ///     Process the class, and return the correctly setup annotation if present.
        /// </summary>
        /// <param name="type">class being processed</param>
        /// <param name="methodRemover">allow any methods of the class to be removed</param>
        /// <param name="holder">to attach the facets to</param>
        /// <returns>
        ///     <c>true</c> if any facets were added, <c>false</c> otherwise.
        /// </returns>
        bool Process(Type type, IMethodRemover methodRemover, IFacetHolder holder);

        /// <summary>
        ///     Process the method, and return the correctly setup annotation if present.
        /// </summary>
        /// <param name="method">MethodInfo representing the feature being processed</param>
        /// <param name="methodRemover">allow any methods of the class to be removed</param>
        /// <param name="holder">to attach the facets to</param>
        /// <returns>
        ///     <c>true</c> if any facets were added and therefore should be removed, <c>false</c> otherwise.
        ///     Returning true will cause the method to be removed
        /// </returns>
        bool Process(MethodInfo method, IMethodRemover methodRemover, IFacetHolder holder);

        /// <summary>
        ///     Process the property, and return the correctly setup annotation if present.
        /// </summary>
        /// <param name="property">PropertyInfo representing the feature being processed</param>
        /// <param name="methodRemover">allow any methods of the class to be removed</param>
        /// <param name="holder">to attach the facets to</param>
        /// <returns>
        ///     <c>true</c> if any facets were added  <c>false</c> otherwise.
        /// </returns>
        bool Process(PropertyInfo property, IMethodRemover methodRemover, IFacetHolder holder);

        /// <summary>
        ///     Process the parameters of the method, and return the correctly setup annotation if present.
        /// </summary>
        /// <param name="method">MethodInfo representing the feature being processed</param>
        /// <param name="paramNum">zero-based index to the parameter to be processed</param>
        /// <param name="holder">to attach the facets to</param>
        /// <returns>
        ///     <c>true</c> if any facets were added, <c>false</c> otherwise.
        /// </returns>
        bool ProcessParams(MethodInfo method, int paramNum, IFacetHolder holder);

        void FindCollectionProperties(IList<PropertyInfo> candidates, IList<PropertyInfo> methodListToAppendTo);

        void FindProperties(IList<PropertyInfo> candidates, IList<PropertyInfo> methodListToAppendTo);
    }
}
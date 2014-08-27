// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections.Generic;
using System.Reflection;
using NakedObjects.Architecture.Reflect;

namespace NakedObjects.Architecture.Facets {
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
        ///     Delegates to <see cref="IFacetFactory.Process(Type,IMethodRemover,IFacetHolder)" /> for each appropriate factory.
        /// </summary>
        /// <param name="type">type to process</param>
        /// <param name="methodRemover">allow any methods of the class to be removed</param>
        /// <param name="facetHolder">holder to attach facets to</param>
        /// <returns>
        ///     <c>true</c> if any facets were added, <c>false</c> otherwise.
        /// </returns>
        bool Process(Type type, IMethodRemover methodRemover, IFacetHolder facetHolder);

        /// <summary>
        ///     Delegates to <see cref="IFacetFactory.Process(MethodInfo,IMethodRemover,IFacetHolder)" />for each appropriate factory.
        /// </summary>
        /// <param name="method">method to process</param>
        /// <param name="methodRemover">allow any methods of the class to be removed</param>
        /// <param name="facetHolder">holder to attach facets to</param>
        /// <param name="featureType">what type of feature the method represents (property, action, collection etc)</param>
        /// <returns>
        ///     <c>true</c> if any facets were added, <c>false</c> otherwise.
        /// </returns>
        bool Process(MethodInfo method, IMethodRemover methodRemover, IFacetHolder facetHolder, NakedObjectFeatureType featureType);

        /// <summary>
        ///     Delegates to <see cref="IFacetFactory.Process(PropertyInfo,IMethodRemover,IFacetHolder)" />for each appropriate factory.
        /// </summary>
        /// <param name="property">property to process</param>
        /// <param name="methodRemover">allow any methods of the class to be removed</param>
        /// <param name="facetHolder">holder to attach facets to</param>
        /// <param name="featureType">what type of feature the method represents (property, action, collection etc)</param>
        /// <returns>
        ///     <c>true</c> if any facets were added, <c>false</c> otherwise.
        /// </returns>
        bool Process(PropertyInfo property, IMethodRemover methodRemover, IFacetHolder facetHolder, NakedObjectFeatureType featureType);

        /// <summary>
        ///     Delegates to <see cref="IFacetFactory.ProcessParams" /> for each appropriate factory.
        /// </summary>
        /// <param name="method">action method to process</param>
        /// <param name="paramNum">zero-based</param>
        /// <param name="facetHolder">holder to attach facets to</param>
        /// <returns>
        ///     <c>true</c> if any facets were added, <c>false</c> otherwise.
        /// </returns>
        bool ProcessParams(MethodInfo method, int paramNum, IFacetHolder facetHolder);

        void Init(INakedObjectReflector reflector);
    }
}
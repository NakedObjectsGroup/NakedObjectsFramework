// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;

namespace NakedObjects {
    /// <summary>
    ///     Indicates that the class has additional facets, and specifies the
    ///     how to obtain the <c>IFacetFactory</c> to manufacture them
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         At least one factory name  or one factory type should be specified
    ///     </para>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class FacetsAttribute : Attribute {
        public FacetsAttribute() {
            FacetFactoryNames = new string[] {};
            FacetFactoryClasses = new Type[] {};
        }

        /// <summary>
        ///     Array of strings each indicating the fully qualified name of a class implementing
        ///     <c>NakedObjects.Architecture.Facets.IFacetFactory</c>
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Either the array provided by this method or by <see cref="FacetFactoryClasses" /> should be non-empty
        ///     </para>
        /// </remarks>
        public string[] FacetFactoryNames { get; set; }

        /// <summary>
        ///     Array of Types each indicating the class implementing <c>NakedObjects.Architecture.Facets.IFacetFactory</c>
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Either the array provided by this method or by <see cref="FacetFactoryNames" /> should be non-empty
        ///     </para>
        /// </remarks>
        public Type[] FacetFactoryClasses { get; set; }
    }
}
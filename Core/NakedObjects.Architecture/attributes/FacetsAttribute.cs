// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

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
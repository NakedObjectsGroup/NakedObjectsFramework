// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

namespace NakedObjects.Architecture.Facets.Actcoll.Typeof {
    /// <summary>
    ///     The type of the collection or the action
    /// </summary>
    /// <para>
    ///     In the standard Naked Objects Programming Model, corresponds to
    ///     annotating the collection's accessor or the action's invoker method
    ///     with the <see cref="TypeOfAttribute" /> annotation
    /// </para>
    public interface ITypeOfFacet : ISingleClassValueFacet {
        /// <summary>
        ///     Does <b>not</b> correspond to a member in the <see cref="TypeOfAttribute" />
        ///     annotation (or equiv), but indicates that the information provided
        ///     has been inferred rather than explicitly specified.
        /// </summary>
        bool IsInferred { get; }
    }
}
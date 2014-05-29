// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Adapter;

namespace NakedObjects.Architecture.Facets.AutoComplete {
    /// <summary>
    ///     Provides a set of autocompletions for a property or parameter
    /// </summary>
    /// <para>
    ///     Viewers would typically represent this as a drop-down list box for the property or parameter.
    /// </para>
    /// <para>
    ///     In the standard Naked Objects Programming Model, corresponds to
    ///     the <c>AutoCompleteXxx</c> supporting method for the property/parm <c>Xxx</c>.
    /// </para>
    public interface IAutoCompleteFacet : IFacet {
        int MinLength { get; }

        /// <summary>
        ///     Gets the available autocompletions for this property or parm
        /// </summary>
        object[] GetCompletions(INakedObject inObject, string autoCompleteParm);
    }
}
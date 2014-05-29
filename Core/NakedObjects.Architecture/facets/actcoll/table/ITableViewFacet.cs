// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

namespace NakedObjects.Architecture.Facets.Actcoll.Table {
    /// <summary>
    /// </summary>
    /// <para>
    /// </para>
    public interface ITableViewFacet : IMultipleValueFacet {
        bool Title { get; set; }
        string[] Columns { get; }
    }
}
// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Adapter;

namespace NakedObjects.Architecture.Facets.Objects.ViewModel {
    /// <summary>
    ///     Indicates that class is a view model
    /// </summary>
    public interface IViewModelFacet : IFacet {
        string[] Derive(INakedObject nakedObject);
        void Populate(string[] keys, INakedObject nakedObject);
        bool IsEditView(INakedObject nakedObject);
    }
}
// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

namespace NakedObjects {
    /// <summary>
    ///     Implementing this interface will ensure that the object will be presented to the user in either view or editable form
    ///     controlled by the IsEditView flag
    ///     (Individual properties may still be disabled.)
    /// </summary>
    public interface IViewModelSwitchable : IViewModel {
        bool IsEditView();
    }
}
// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

namespace NakedObjects {

    /// <summary>
    /// Role interface to be implemented by an object that is not itself a persistent object, though it may contain
    /// references to one or more persistent objects.
    /// </summary>
    public interface IViewModel {
        /// <summary>
        /// Return an array of string keys that may be used to re-create an identical object instance.
        /// Each key might be an individual value property, or the key to a referenced persistent object.
        /// </summary>
        /// <returns></returns>
        string[] DeriveKeys();

        /// <summary>
        /// Method to re-create a view model in a given state, by being passed the same array of key strings
        /// that would be returned by the DeriveKeys method.
        /// </summary>
        /// <param name="keys"></param>
        void PopulateUsingKeys(string[] keys);
    }
}
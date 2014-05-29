// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

namespace NakedObjects.Architecture.Adapter {
    public interface IEntryTextParser {
        /// <summary>
        ///     Parses a text entry made by a user and sets the domain object's value
        /// </summary>
        /// <exception cref="InvalidEntryException" />
        INakedObject ParseTextEntry(INakedObject original, string text);

        /// <summary>
        ///     Returns the title to display this object with, which is usually got from the
        ///     wrapped <see cref="INakedObject.Object" /> domain object
        /// </summary>
        string TitleString(INakedObject nakedObject);
    }

    // Copyright (c) Naked Objects Group Ltd.
}
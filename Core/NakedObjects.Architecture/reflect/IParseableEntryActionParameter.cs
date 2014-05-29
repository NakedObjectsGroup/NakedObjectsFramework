// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

namespace NakedObjects.Architecture.Reflect {
    /// <summary>
    ///     Equivalent to <see cref="INakedObjectAssociation" />, but for parameter rather than properties
    /// </summary>
    public interface IParseableEntryActionParameter : IOneToOneFeature,
                                                      INakedObjectActionParameter {}
}
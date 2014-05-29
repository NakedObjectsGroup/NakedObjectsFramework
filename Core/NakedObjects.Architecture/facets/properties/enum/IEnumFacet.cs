// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Adapter;

namespace NakedObjects.Architecture.Facets.Properties.Enums {
    public interface IEnumFacet : IMarkerFacet {
        object[] GetChoices(INakedObject inObject);
        object[] GetChoices(INakedObject inObject, object[] choiceValues);
        string GetTitle(INakedObject inObject);
    }
}
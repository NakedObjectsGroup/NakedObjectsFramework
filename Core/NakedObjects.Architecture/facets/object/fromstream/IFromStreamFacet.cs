// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.IO;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Persist;

namespace NakedObjects.Architecture.Facets.Objects.FromStream {
    public interface IFromStreamFacet : IMultipleValueFacet {
        INakedObject ParseFromStream(Stream stream, string mimeType, string name, INakedObjectManager manager);
    }
}
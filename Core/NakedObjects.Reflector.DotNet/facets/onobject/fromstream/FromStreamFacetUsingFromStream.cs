// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.IO;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Objects.FromStream;
using NakedObjects.Capabilities;
using NakedObjects.Core.Context;
using NakedObjects.Core.Persist;

namespace NakedObjects.Reflector.DotNet.Facets.Objects.FromStream {
    public class FromStreamFacetUsingFromStream : FacetAbstract, IFromStreamFacet {
        private readonly IFromStream fromStream;


        public FromStreamFacetUsingFromStream(IFromStream fromStream, IFacetHolder holder)
            : base(typeof (IFromStreamFacet), holder) {
            this.fromStream = fromStream;
        }

        #region IFromStreamFacet Members

        public INakedObject ParseFromStream(Stream stream, string mimeType, string name) {
            object obj = fromStream.ParseFromStream(stream, mimeType, name);
            return NakedObjectsContext.ObjectPersistor.CreateAdapter(obj, null, null);
        }

        #endregion

        protected override string ToStringValues() {
            return fromStream.ToString();
        }
    }
}
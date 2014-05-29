// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Linq;
using System.Net.Http.Headers;
using RestfulObjects.Snapshot.Utility;

namespace RestfulObjects.Snapshot.Representations {
    public class MapRepresentation : Representation {
        private MediaTypeHeaderValue mediaType;
        public MapRepresentation() : base(RestControlFlags.DefaultFlags()) {}

        public static MapRepresentation Create(params OptionalProperty[] properties) {
            return properties.Any() ? CreateWithOptionals<MapRepresentation>(new object[] {}, properties) : new MapRepresentation();
        }

        public void SetContentType(MediaTypeHeaderValue mt) {
            mediaType = mt;
        }

        public override MediaTypeHeaderValue GetContentType() {
            return mediaType ?? base.GetContentType();
        }
    }
}
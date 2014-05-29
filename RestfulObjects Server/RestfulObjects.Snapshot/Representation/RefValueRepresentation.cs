// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Runtime.Serialization;
using RestfulObjects.Snapshot.Constants;
using RestfulObjects.Snapshot.Utility;

namespace RestfulObjects.Snapshot.Representations {
    [DataContract]
    public class RefValueRepresentation : Representation {
        protected RefValueRepresentation(RelType relType, RestControlFlags flags)
            : base(flags) {
            Href = relType.GetUri().AbsoluteUri;
        }

        [DataMember(Name = JsonPropertyNames.Href)]
        public string Href { get; set; }

        public static RefValueRepresentation Create(RelType relType, RestControlFlags flags) {
            return new RefValueRepresentation(relType, flags);
        }
    }
}
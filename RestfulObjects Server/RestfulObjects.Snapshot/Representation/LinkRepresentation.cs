// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Linq;
using System.Runtime.Serialization;
using RestfulObjects.Snapshot.Constants;
using RestfulObjects.Snapshot.Utility;

namespace RestfulObjects.Snapshot.Representations {
    [DataContract]
    public class LinkRepresentation : RefValueRepresentation {
        protected LinkRepresentation(RelType relType, RestControlFlags flags) : base(relType, flags) {
            SetScalars(relType);
        }

        [DataMember(Name = JsonPropertyNames.Rel)]
        public string Rel { get; set; }

        [DataMember(Name = JsonPropertyNames.Method)]
        public string Method { get; set; }

        [DataMember(Name = JsonPropertyNames.Type)]
        public string Type { get; set; }

        private void SetScalars(RelType relType) {
            Rel = relType.Name;
            Method = relType.Method.ToString().ToUpper();
            Type = relType.GetMediaType(Flags).ToString();
        }

        public static LinkRepresentation Create(RelType relType, RestControlFlags flags, params OptionalProperty[] properties) {
            return properties.Any() ? CreateWithOptionals<LinkRepresentation>(new object[] {relType, flags}, properties) : new LinkRepresentation(relType, flags);
        }
    }
}
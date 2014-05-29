using System;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Runtime.Serialization;
using RestfulObjects.Snapshot.Utility;

namespace RestfulObjects.Snapshot.Representations {
    [DataContract]
    public class NullRepresentation : Representation {
        public NullRepresentation() : base(RestControlFlags.DefaultFlags()) {}

        public override HttpResponseMessage GetAsMessage(MediaTypeFormatter formatter, Tuple<int, int, int> cacheSettings) {
            var msg = new HttpResponseMessage {Content = new StringContent("")};
            msg.Content.Headers.ContentType = GetContentType();
            SetCaching(msg, cacheSettings);
            return msg;
        }

        public static Representation Create() {
            return new NullRepresentation();
        }
    }
}
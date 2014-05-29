// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.IO;
using System.Net;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestfulObjects.Snapshot.Representations;
using RestfulObjects.Snapshot.Utility;

namespace RestfulObjects.Mvc.Media {
    public class JsonNetFormatter : MediaTypeFormatter {
        private readonly JsonSerializerSettings jsonSerializerSettings;

        public JsonNetFormatter(JsonSerializerSettings jsonSerializerSettings) {
            this.jsonSerializerSettings = jsonSerializerSettings ?? new JsonSerializerSettings();
            this.jsonSerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/json"));
        }

        public override bool CanReadType(Type type) {
            return true;
        }

        public override bool CanWriteType(Type type) {
            return true;
        }

        public override Task<object> ReadFromStreamAsync(Type type, Stream readStream, System.Net.Http.HttpContent content, IFormatterLogger formatterLogger) {
          
            JsonSerializer serializer = JsonSerializer.Create(jsonSerializerSettings);

            return Task.Factory.StartNew(() => {
                using (var streamReader = new StreamReader(readStream, new UTF8Encoding(false, true))) {
                                                 using (var jsonTextReader = new JsonTextReader(streamReader)) {
                                                     return serializer.Deserialize(jsonTextReader);
                                                 }
                                             }
                                         });
        }

        public override Task WriteToStreamAsync(Type type, object value, Stream writeStream, System.Net.Http.HttpContent content, TransportContext transportContext) {
           
            JsonSerializer serializer = JsonSerializer.Create(jsonSerializerSettings);

            return Task.Factory.StartNew(() => {
                                             var rep = value as Representation;
                                             if (rep != null) {
                                                 // don't want to dispose the stream so use a local one and copy
                                                 var localStream = new MemoryStream();

                                                 using (var streamWriter = new StreamWriter(localStream, new UTF8Encoding(false, true))) {
                                                     using (var jsonTextWriter = new JsonTextWriter(streamWriter)) {
                                                         serializer.Serialize(jsonTextWriter, rep);
                                                         jsonTextWriter.Flush();
                                                         localStream.Position = 0;
                                                         localStream.CopyTo(writeStream);
                                                     }
                                                 }
                                             }
                                         });
        }
    }
}
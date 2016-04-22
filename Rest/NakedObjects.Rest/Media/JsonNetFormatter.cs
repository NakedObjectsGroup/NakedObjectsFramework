// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NakedObjects.Rest.Snapshot.Representations;

namespace NakedObjects.Rest.Media {
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

        public override Task<object> ReadFromStreamAsync(Type type, Stream readStream, HttpContent content, IFormatterLogger formatterLogger) {
            JsonSerializer serializer = JsonSerializer.Create(jsonSerializerSettings);

            return Task.Factory.StartNew(() => {
                using (var streamReader = new StreamReader(readStream, new UTF8Encoding(false, true))) {
                    using (var jsonTextReader = new JsonTextReader(streamReader)) {
                        return serializer.Deserialize(jsonTextReader);
                    }
                }
            });
        }

        public override Task WriteToStreamAsync(Type type, object value, Stream writeStream, HttpContent content, TransportContext transportContext) {
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
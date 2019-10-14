// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using Common.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NakedObjects.Rest.Snapshot.Constants;
using NakedObjects.Rest.Snapshot.Utility;

namespace NakedObjects.Rest.Model {
    public static class ModelBinderUtils {
        private static readonly ILog Logger = LogManager.GetLogger(typeof (ModelBinderUtils));

        private static bool IsReservedName(string name) {
            return name.StartsWith(RestControlFlags.ReservedPrefix);
        }

        public static JObject DeserializeJsonStream(Stream stream) {
            if (stream.Length > 0) {
                JsonSerializer serializer = JsonSerializer.Create(new JsonSerializerSettings());
                using (var streamReader = new StreamReader(stream, new UTF8Encoding(false, true))) {
                    using (var jsonTextReader = new JsonTextReader(streamReader)) {
                        return (JObject) serializer.Deserialize(jsonTextReader);
                    }
                }
            }
            return new JObject();
        }

        public static byte[] DeserializeBinaryStream(Stream stream) {
            if (stream.Length > 0) {
                using (var br = new BinaryReader(stream)) {
                    return br.ReadBytes((int) stream.Length);
                }
            }
            return new byte[] {};
        }

        public static Stream QueryStringToStream(string qs) {
            string decodedQs = HttpUtility.UrlDecode(qs)?.Remove(0, 1); // trim '?'
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);

            writer.Write(decodedQs);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        // name parm is just to improve logging 
        private static IValue GetValue(JObject jObject, string name) {
            if (GetNonReservedProperties(jObject).Count() > 1) {
                Logger.ErrorFormat("Malformed json name: {0)  arguments: {1}", name, jObject.ToString());
                throw new ArgumentException("malformed arguments");
            }

            JToken value = jObject[JsonPropertyNames.Value];
            var valueAsArray = value as JArray;

            if (valueAsArray != null) {
                IValue[] arr = valueAsArray.Children().Select(v => ToIValue("", v)).ToArray();
                return new ListValue(arr);
            }
            return value != null ? ToIValue(name, value) : null;
        }

        private static IValue ToIValue(string name, JToken value) {
            JValue href = value.HasValues ? value[JsonPropertyNames.Href] as JValue : null;
            JValue type = value.HasValues ? value[JsonPropertyNames.Type] as JValue : null;
            if (href == null) {
                return new ScalarValue(((JValue) value).Value);
            }
            if (type == null) {
                 return new ReferenceValue(href.Value, name);
            }
            string fileName = value.HasValues ? (value[JsonPropertyNames.Title] as JValue)?.Value as string : "";
            return new FileValue((string) href.Value, (string) type.Value, fileName);
        }

        private static string GetSearchTerm(JObject jObject) {
            var searchTerm = jObject[RestControlFlags.SearchTermReserved] as JObject;
            JValue value = searchTerm == null ? null : searchTerm[JsonPropertyNames.Value] as JValue;

            return value == null ? null : (string) value.Value;
        }

        private static bool GetValidateOnlyFlag(JObject jObject) {
            var voFlag = jObject[RestControlFlags.ValidateOnlyReserved] as JValue;
            return voFlag != null && (bool) voFlag.Value;
        }

        private static bool? GetInlinePropertyDetailsFlag(JObject jObject) {
            var flag = jObject[RestControlFlags.InlinePropertyDetailsReserved] as JValue;
            return flag == null ? (bool?) null : (bool) flag.Value;
        }

        private static bool? GetInlineCollectionItemsFlag(JObject jObject) {
            var flag = jObject[RestControlFlags.InlineCollectionItemsReserved] as JValue;
            return flag == null ? (bool?)null : (bool)flag.Value;
        }

        private static int GetPageValue(JObject jObject) {
            var pageValue = jObject[RestControlFlags.PageReserved] as JValue;
            return pageValue == null ? 0 : Convert.ToInt32(pageValue.Value);
        }

        private static int GetPageSizeValue(JObject jObject) {
            var pageSizeValue = jObject[RestControlFlags.PageSizeReserved] as JValue;
            return pageSizeValue == null ? 0 : Convert.ToInt32(pageSizeValue.Value);
        }

        private static string GetDomainModelValue(JObject m) {
            var domainModel = m[RestControlFlags.DomainModelReserved] as JValue;
            return domainModel == null ? null : (string) domainModel.Value;
        }

        private static IEnumerable<JProperty> FilterProperties(JToken jToken, Func<JProperty, bool> filter) {
            return jToken.Children().Cast<JProperty>().Where(filter);
        }

        private static IEnumerable<JProperty> GetNonReservedProperties(JToken jToken) {
            return FilterProperties(jToken, c => !IsReservedName(c.Name));
        }

        public static SingleValueArgument CreateSingleValueArgument(object obj) {
            var arg = new SingleValueArgument();

            var jObject = obj as JObject;
            var bObject = obj as byte[];

            if (jObject != null) {
                try {
                    arg.Value = GetValue(jObject, "Single");
                    arg.ValidateOnly = GetValidateOnlyFlag(jObject);
                    arg.IsMalformed = (!arg.HasValue && GetNonReservedProperties(jObject).Any()) ||
                                      (arg.HasValue && GetNonReservedProperties(jObject).Count() > 1);
                }
                catch (Exception e) {
                    Logger.ErrorFormat("Malformed single value argument: {0}", e.Message);
                    arg.IsMalformed = true;
                }
            }

            if (bObject != null) {
                arg.Value = new AttachmentValue(bObject);
                arg.ValidateOnly = false; // not supported on blob/clob
            }

            return arg;
        }

        public static SingleValueArgument CreateSingleValueArgumentForMalformedArgs() {
            return new SingleValueArgument {IsMalformed = true};
        }

        private static ArgumentMap CreatePromptArgumentMap(JObject jObject, Action<JObject, ArgumentMap> populate) {
            var arg = new PromptArgumentMap {MemberMap = new Dictionary<string, IValue>()};

            InitArgumentMap(jObject, populate, arg);

            return arg;
        }

        private static ArgumentMap CreateArgumentMap(JObject jObject, Action<JObject, ArgumentMap> populate) {
            var arg = new ArgumentMap();

            InitArgumentMap(jObject, populate, arg);

            return arg;
        }

        private static void InitArgumentMap(JObject jObject, Action<JObject, ArgumentMap> populate, ArgumentMap arg) {
            if (jObject != null) {
                try {
                    populate(jObject, arg);

                    arg.ValidateOnly = GetValidateOnlyFlag(jObject);
                    arg.DomainModel = GetDomainModelValue(jObject);
                    arg.SearchTerm = GetSearchTerm(jObject);
                    arg.Page = GetPageValue(jObject);
                    arg.PageSize = GetPageSizeValue(jObject);
                    arg.InlinePropertyDetails = GetInlinePropertyDetailsFlag(jObject);
                    arg.InlineCollectionItems = GetInlineCollectionItemsFlag(jObject);
                }
                catch (Exception e) {
                    Logger.ErrorFormat("Malformed argument map: {0}", e.Message);
                    arg.IsMalformed = true;
                }
            }
            else {
                arg.Map = new Dictionary<string, IValue>();
            }
        }

        private static void PopulateArgumentMap(JToken jObject, ArgumentMap arg) {
            arg.Map = GetNonReservedProperties(jObject).ToDictionary(jt => jt.Name, jt => GetValue((JObject) jt.Value, jt.Name));
        }

        private static void PopulatePersistArgumentMap(JToken jObject, ArgumentMap arg) {
            JToken members = jObject[JsonPropertyNames.Members];

            if (members != null) {
                PopulateArgumentMap(members, arg);
            }
            else {
                arg.Map = new Dictionary<string, IValue>();
            }
        }

        private static void PopulatePromptArgumentMap(JToken jObject, ArgumentMap arg) {
            JToken members = jObject[JsonPropertyNames.PromptMembers];
            var promptArgumentMap = arg as PromptArgumentMap;

            if (promptArgumentMap != null) {
                if (members != null) {
                    promptArgumentMap.MemberMap = GetNonReservedProperties(members).ToDictionary(jt => jt.Name, jt => GetValue((JObject) jt.Value, jt.Name));
                }
                else {
                    promptArgumentMap.MemberMap = new Dictionary<string, IValue>();
                }
            }
            arg.Map = GetNonReservedProperties(jObject).ToDictionary(jt => jt.Name, jt => GetValue((JObject) jt.Value, jt.Name));
        }

        public static ArgumentMap CreateArgumentMap(JObject jObject) {
            return CreateArgumentMap(jObject, PopulateArgumentMap);
        }

        public static ArgumentMap CreatePersistArgMap(JObject jObject) {
            return CreateArgumentMap(jObject, PopulatePersistArgumentMap);
        }

        public static ArgumentMap CreatePromptArgMap(JObject jObject) {
            return CreatePromptArgumentMap(jObject, PopulatePromptArgumentMap);
        }

        public static ArgumentMap CreateArgumentMapForMalformedArgs() {
            return new ArgumentMap {IsMalformed = true};
        }

        private static void PopulateReservedArgs(NameValueCollection collection, ReservedArguments args) {
            try {
                string voFlag = collection[RestControlFlags.ValidateOnlyReserved];
                string domainModel = collection[RestControlFlags.DomainModelReserved];
                string page = collection[RestControlFlags.PageReserved];
                string pageSize = collection[RestControlFlags.PageSizeReserved];
                string inlineFlag = collection[RestControlFlags.InlinePropertyDetailsReserved];
                string inlineItemsFlag = collection[RestControlFlags.InlineCollectionItemsReserved];

                args.ValidateOnly = voFlag != null && bool.Parse(voFlag);
                args.DomainModel = domainModel;
                args.Page = page != null ? int.Parse(page) : 0;
                args.PageSize = pageSize != null ? int.Parse(pageSize) : 0;
                args.InlinePropertyDetails = inlineFlag != null ? bool.Parse(inlineFlag) : (bool?)null;
                args.InlineCollectionItems = inlineItemsFlag != null ? bool.Parse(inlineItemsFlag) : (bool?)null;
            }
            catch (Exception e) {
                Logger.ErrorFormat("Malformed reserved arguments: {0}", e.Message);
                args.IsMalformed = true;
            }
        }

        public static ReservedArguments CreateReservedArguments(string query) {
            NameValueCollection collection = HttpUtility.ParseQueryString(query);
            var args = new ReservedArguments();
            PopulateReservedArgs(collection, args);
            return args;
        }

        public static ReservedArguments CreateReservedArgumentsForMalformedArgs() {
            return new ReservedArguments {IsMalformed = true};
        }

        private static void PopulateSimpleArgumentMap(NameValueCollection collection, ArgumentMap args) {
            args.Map = collection.AllKeys.Where(k => !IsReservedName(k)).ToDictionary(s => s, s => (IValue) new ScalarValue(collection[s]));
        }

        public static ArgumentMap CreateSimpleArgumentMap(string query) {
            NameValueCollection collection = HttpUtility.ParseQueryString(query);

            if (collection.AllKeys.Any() && collection.AllKeys.First() == null) {
                return null;
            }

            var args = new ArgumentMap();
            PopulateReservedArgs(collection, args);
            PopulateSimpleArgumentMap(collection, args);
            return args;
        }
    }
}
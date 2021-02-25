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
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using NakedFramework.Rest.API;
using NakedFramework.Rest.Snapshot.Constants;
using NakedFramework.Rest.Snapshot.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NakedFramework.Rest.Model {
    public static class ModelBinderUtils {
        private static string ExceptionWarning(Exception e) => ControllerHelpers.DebugFilter(() => $"{e.Message} {e.StackTrace?.Replace("\r", " ").Replace("\n", " ")}");

        private static bool IsReservedName(string name) => name.StartsWith(RestControlFlags.ReservedPrefix);

        public static async Task<JObject> DeserializeJsonStreamAsync(Stream stream) {
            using var streamReader = new StreamReader(stream, new UTF8Encoding(false, true));
            using var jsonTextReader = new JsonTextReader(streamReader);
            return await JObject.LoadAsync(jsonTextReader);
        }

        public static byte[] DeserializeBinaryStream(Stream stream) {
            if (stream.CanRead) {
                using var br = new BinaryReader(stream);
                return br.ReadBytes((int) stream.Length);
            }

            return new byte[] { };
        }

        public static Stream QueryStringToStream(string qs) {
            var decodedQs = HttpUtility.UrlDecode(qs)?.Remove(0, 1); // trim '?'
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);

            writer.Write(decodedQs);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        // name parameter is just to improve logging 
        private static IValue GetValue(JObject jObject, string name) {
            if (GetNonReservedProperties(jObject).Count() > 1) {
                throw new ArgumentException($"malformed arguments: Malformed json name: {name}  arguments: {jObject}");
            }

            var value = jObject[JsonPropertyNames.Value];

            if (value is JArray valueAsArray) {
                var arr = valueAsArray.Children().Select(v => ToIValue("", v)).ToArray();
                return new ListValue(arr);
            }

            return value != null ? ToIValue(name, value) : null;
        }

        private static IValue ToIValue(string name, JToken value) {
            var href = value.HasValues ? value[JsonPropertyNames.Href] as JValue : null;
            var type = value.HasValues ? value[JsonPropertyNames.Type] as JValue : null;
            if (href == null) {
                return new ScalarValue(((JValue) value).Value);
            }

            if (type == null) {
                return new ReferenceValue(href.Value, name);
            }

            var fileName = value.HasValues ? (value[JsonPropertyNames.Title] as JValue)?.Value as string : "";
            return new FileValue((string) href.Value, (string) type.Value, fileName);
        }

        private static string GetSearchTerm(JObject jObject) {
            var searchTerm = jObject[RestControlFlags.SearchTermReserved] as JObject;
            var value = searchTerm?[JsonPropertyNames.Value] as JValue;

            return (string) value?.Value;
        }

        private static bool GetValidateOnlyFlag(JObject jObject) => jObject[RestControlFlags.ValidateOnlyReserved] is JValue voFlag && (bool) voFlag.Value;

        private static bool? GetInlinePropertyDetailsFlag(JObject jObject) {
            var flag = jObject[RestControlFlags.InlinePropertyDetailsReserved] as JValue;
            return (bool?) flag?.Value;
        }

        private static bool? GetInlineCollectionItemsFlag(JObject jObject) {
            var flag = jObject[RestControlFlags.InlineCollectionItemsReserved] as JValue;
            return (bool?) flag?.Value;
        }

        private static int GetIntValue(JObject jObject, string name) => jObject[name] is JValue v ? Convert.ToInt32(v.Value) : 0;

        private static int GetPageValue(JObject jObject) => GetIntValue(jObject, RestControlFlags.PageReserved);

        private static int GetPageSizeValue(JObject jObject) => GetIntValue(jObject, RestControlFlags.PageSizeReserved);

        private static string GetDomainModelValue(JObject m) {
            var domainModel = m[RestControlFlags.DomainModelReserved] as JValue;
            return (string) domainModel?.Value;
        }

        private static IEnumerable<JProperty> FilterProperties(JToken jToken, Func<JProperty, bool> filter) => jToken.Children().Cast<JProperty>().Where(filter);

        private static IEnumerable<JProperty> GetNonReservedProperties(JToken jToken) => FilterProperties(jToken, c => !IsReservedName(c.Name));

        public static SingleValueArgument CreateSingleValueArgument(object obj, bool includeReservedArgs) {
            var arg = new SingleValueArgument();

            var jObject = obj as JObject;
            var bObject = obj as byte[];

            if (jObject != null) {
                try {
                    arg.Value = GetValue(jObject, "Single");
                    arg.IsMalformed = !arg.HasValue && GetNonReservedProperties(jObject).Any() ||
                                      arg.HasValue && GetNonReservedProperties(jObject).Count() > 1;

                    if (includeReservedArgs) {
                        arg.ReservedArguments = new ReservedArguments {
                            ValidateOnly = GetValidateOnlyFlag(jObject)
                        };
                    }
                }
                catch (Exception e) {
                    arg.IsMalformed = true;
                    arg.MalformedReason = $"Malformed single value argument: {ExceptionWarning(e)}";
                }
            }

            if (bObject != null) {
                arg.Value = new AttachmentValue(bObject);
                arg.ReservedArguments = new ReservedArguments {
                    ValidateOnly = false
                }; // not supported on blob/clob
            }

            return arg;
        }

        private static T InitArgumentMap<T>(JObject jObject, Action<JObject, T> populate, bool includeReservedArgs) where T : ArgumentMap, new() {
            var arg = new T();

            if (jObject != null) {
                try {
                    populate(jObject, arg);

                    if (includeReservedArgs) {
                        arg.ReservedArguments = new ReservedArguments {
                            ValidateOnly = GetValidateOnlyFlag(jObject),
                            DomainModel = GetDomainModelValue(jObject),
                            SearchTerm = GetSearchTerm(jObject),
                            Page = GetPageValue(jObject),
                            PageSize = GetPageSizeValue(jObject),
                            InlinePropertyDetails = GetInlinePropertyDetailsFlag(jObject),
                            InlineCollectionItems = GetInlineCollectionItemsFlag(jObject)
                        };
                    }
                }
                catch (Exception e) {
                    arg.IsMalformed = true;
                    arg.MalformedReason = $"Malformed argument map: {ExceptionWarning(e)}";
                }
            }
            else {
                arg.Map = new Dictionary<string, IValue>();
            }

            return arg;
        }

        private static IDictionary<string, IValue> ExtractProperties(JToken jObject) => GetNonReservedProperties(jObject).ToDictionary(jt => jt.Name, jt => GetValue((JObject) jt.Value, jt.Name));

        private static void PopulateArgumentMap(JToken jObject, ArgumentMap arg) => arg.Map = ExtractProperties(jObject);

        private static void PopulatePersistArgumentMap(JToken jObject, PersistArgumentMap arg) {
            var members = jObject[JsonPropertyNames.Members];
            arg.Map = members != null ? ExtractProperties(members) : new Dictionary<string, IValue>();
        }

        private static void PopulatePromptArgumentMap(JToken jObject, PromptArgumentMap arg) {
            var members = jObject[JsonPropertyNames.PromptMembers];
            arg.MemberMap = members != null ? ExtractProperties(members) : new Dictionary<string, IValue>();
            arg.Map = ExtractProperties(jObject);
        }

        public static ArgumentMap CreateArgumentMap(JObject jObject, bool includeReservedArgs) => InitArgumentMap<ArgumentMap>(jObject, PopulateArgumentMap, includeReservedArgs);

        public static PersistArgumentMap CreatePersistArgMap(JObject jObject, bool includeReservedArgs) => InitArgumentMap<PersistArgumentMap>(jObject, PopulatePersistArgumentMap, includeReservedArgs);

        public static PromptArgumentMap CreatePromptArgMap(JObject jObject, bool includeReservedArgs) => InitArgumentMap<PromptArgumentMap>(jObject, PopulatePromptArgumentMap, includeReservedArgs);

        public static T CreateMalformedArguments<T>(string msg) where T : Arguments, new() =>
            new() {IsMalformed = true, MalformedReason = ControllerHelpers.DebugFilter(() => msg)};

        private static void PopulateSimpleArgumentMap(NameValueCollection collection, ArgumentMap args) => args.Map = collection.AllKeys.Where(k => !IsReservedName(k)).ToDictionary(s => s, s => (IValue) new ScalarValue(collection[s]));

        public static ArgumentMap CreateSimpleArgumentMap(string query) {
            var collection = HttpUtility.ParseQueryString(query);

            if (collection.AllKeys.Any() && collection.AllKeys.First() == null) {
                return null;
            }

            var args = new ArgumentMap {ReservedArguments = new ReservedArguments()};
            PopulateSimpleArgumentMap(collection, args);
            PopulateReservedArgs(collection, args);
            return args;
        }

        public static async Task<JObject> DeserializeJsonContent(ModelBindingContext bindingContext) {
            if (bindingContext.HttpContext.Request.ContentLength > 0) {
                return await DeserializeJsonStreamAsync(bindingContext.HttpContext.Request.Body);
            }

            return new JObject();
        }

        public static byte[] DeserializeBinaryContent(ModelBindingContext bindingContext) => DeserializeBinaryStream(bindingContext.HttpContext.Request.Body);

        public static async Task<JObject> DeserializeQueryString(ModelBindingContext bindingContext) {
            using var stream = QueryStringToStream(bindingContext.HttpContext.Request.QueryString.ToString());
            return await DeserializeJsonStreamAsync(stream);
        }

        public static async Task BindModelOnSuccessOrFail<T>(ModelBindingContext bindingContext, Func<Task<T>> parseFunc, Func<string, T> failFunc) {
            try {
                try {
                    bindingContext.Result = ModelBindingResult.Success(await parseFunc());
                }
                catch (Exception e) {
                    var msg = $"Parsing of request arguments failed: {ExceptionWarning(e)}";
                    bindingContext.Result = ModelBindingResult.Success(failFunc(msg));
                }
            }
            catch (Exception) {
                bindingContext.Result = ModelBindingResult.Failed();
            }
        }

        public static async Task<object> DeserializeContent(ModelBindingContext bindingContext) {
            var requestHeaders = bindingContext.HttpContext.Request.GetTypedHeaders();

            if (RestUtils.IsJsonMediaType(requestHeaders.ContentType.ToString())) {
                return await DeserializeJsonContent(bindingContext);
            }

            return DeserializeBinaryContent(bindingContext);
        }

        private static void PopulateReservedArgs(NameValueCollection collection, Arguments args) {
            try {
                var reservedArgs = args.ReservedArguments;

                var voFlag = collection[RestControlFlags.ValidateOnlyReserved];
                var domainModel = collection[RestControlFlags.DomainModelReserved];
                var page = collection[RestControlFlags.PageReserved];
                var pageSize = collection[RestControlFlags.PageSizeReserved];
                var inlineFlag = collection[RestControlFlags.InlinePropertyDetailsReserved];
                var inlineItemsFlag = collection[RestControlFlags.InlineCollectionItemsReserved];

                reservedArgs.ValidateOnly = voFlag != null && bool.Parse(voFlag);
                reservedArgs.DomainModel = domainModel;
                reservedArgs.Page = page != null ? int.Parse(page) : 0;
                reservedArgs.PageSize = pageSize != null ? int.Parse(pageSize) : 0;
                reservedArgs.InlinePropertyDetails = inlineFlag != null ? bool.Parse(inlineFlag) : (bool?) null;
                reservedArgs.InlineCollectionItems = inlineItemsFlag != null ? bool.Parse(inlineItemsFlag) : (bool?) null;
            }
            catch (Exception e) {
                args.IsMalformed = true;
                args.MalformedReason = $"Malformed reserved arguments: {ExceptionWarning(e)}";
            }
        }

        public static bool IsGet(this HttpRequest request) => new HttpMethod(request.Method) == HttpMethod.Get;
    }
}
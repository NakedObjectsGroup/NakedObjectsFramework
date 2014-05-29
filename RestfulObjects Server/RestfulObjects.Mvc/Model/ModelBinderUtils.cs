// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

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
using RestfulObjects.Snapshot.Constants;
using RestfulObjects.Snapshot.Utility;

namespace RestfulObjects.Mvc.Model {
    public static class ModelBinderUtils {

        private static readonly ILog Logger = LogManager.GetCurrentClassLogger();

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
            string decodedQs = HttpUtility.UrlDecode(qs).Remove(0, 1); // trim '?'
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
            if (value != null) {
                return ToIValue(name, value);
            }

            return null;
        }

        private static IValue ToIValue(string name, JToken value) {
            JValue href = value.HasValues ? value[JsonPropertyNames.Href] as JValue : null;
            return href == null ? (IValue) new ScalarValue(((JValue) value).Value) : new ReferenceValue(href.Value, name);
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

        private static string GetDomainModelValue(JObject m) {
            var domainModel = m[RestControlFlags.DomainModelReserved] as JValue;
            return domainModel == null ? null : (string) domainModel.Value;
        }

        private static IEnumerable<JProperty> GetNonReservedProperties(JToken jToken) {
            return jToken.Children().Cast<JProperty>().Where(c => !IsReservedName(c.Name));
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

        private static ArgumentMap CreateArgumentMap(JObject jObject, Action<JObject, ArgumentMap> populate) {
            var arg = new ArgumentMap();

            if (jObject != null) {
                try {
                    populate(jObject, arg);

                    arg.ValidateOnly = GetValidateOnlyFlag(jObject);
                    arg.DomainModel = GetDomainModelValue(jObject);
                    arg.SearchTerm = GetSearchTerm(jObject);
                }
                catch (Exception e) {
                    Logger.ErrorFormat("Malformed argument map: {0}", e.Message);
                    arg.IsMalformed = true;
                }
            }
            else {
                arg.Map = new Dictionary<string, IValue>();
            }

            return arg;
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

        public static ArgumentMap CreateArgumentMap(JObject jObject) {
            return CreateArgumentMap(jObject, PopulateArgumentMap);
        }

        public static ArgumentMap CreatePersistArgMap(JObject jObject) {
            return CreateArgumentMap(jObject, PopulatePersistArgumentMap);
        }

        public static ArgumentMap CreateArgumentMapForMalformedArgs() {
            return new ArgumentMap {IsMalformed = true};
        }


        private static void PopulateReservedArgs(NameValueCollection collection, ReservedArguments args) {
            try {
                string voFlag = collection[RestControlFlags.ValidateOnlyReserved];
                string domainModel = collection[RestControlFlags.DomainModelReserved];

                args.ValidateOnly = voFlag != null && bool.Parse(voFlag);
                args.DomainModel = domainModel;
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
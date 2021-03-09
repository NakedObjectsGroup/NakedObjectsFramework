// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using Microsoft.Extensions.Logging;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Component;
using NakedFramework.Core.Error;
using NakedFramework.Core.Util;

namespace NakedFramework.Core.Adapter {
    /// <summary>
    ///     Provide consistent string decoding strategy for <see cref="IEncodedToStrings" />
    /// </summary>
    /// <seealso cref="StringEncoderHelper" />
    public class StringDecoderHelper {
        private readonly ILogger<StringDecoderHelper> logger;
        private readonly ILoggerFactory loggerFactory;
        private readonly IMetamodelManager metamodel;
        private readonly string[] strings;
        private int index;

        public StringDecoderHelper(IMetamodelManager metamodel, ILoggerFactory loggerFactory, ILogger<StringDecoderHelper> logger, string[] strings, bool decode = false) {
            this.metamodel = metamodel ?? throw new InitialisationException($"{nameof(metamodel)} is null");
            this.loggerFactory = loggerFactory ?? throw new InitialisationException($"{nameof(loggerFactory)} is null");
            this.logger = logger ?? throw new InitialisationException($"{nameof(logger)} is null");
            this.strings = decode ? strings.Select(HttpUtility.UrlDecode).ToArray() : strings;
        }

        public bool HasNext => index + 1 < strings.Length;

        public short GetNextShort() {
            CheckCurrentIndex();
            return short.Parse(strings[index++]);
        }

        public int GetNextInt() {
            CheckCurrentIndex();
            return int.Parse(strings[index++]);
        }

        public long GetNextLong() {
            CheckCurrentIndex();
            return long.Parse(strings[index++]);
        }

        public string GetNextString() {
            CheckCurrentIndex();
            return strings[index++];
        }

        public bool GetNextBool() {
            CheckCurrentIndex();
            return bool.Parse(strings[index++]);
        }

        public T GetNextEnum<T>() {
            CheckCurrentIndex();
            return (T) Enum.Parse(typeof(T), strings[index++]);
        }

        public string[] GetNextArray() {
            var list = new List<string>();
            var count = GetNextInt();

            for (var i = 0; i < count; i++) {
                list.Add(GetNextString());
            }

            return list.ToArray();
        }

        public IList<object> GetNextValueCollection(out Type instanceType) {
            var list = new List<object>();
            var count = GetNextInt();
            var type = GetNextString();
            instanceType = TypeUtils.GetType(type);

            for (var i = 0; i < count; i++) {
                list.Add(GetNextObject());
            }

            return list;
        }

        public IList<IEncodedToStrings> GetNextObjectCollection(out Type instanceType) {
            var list = new List<IEncodedToStrings>();
            var count = GetNextInt();
            var type = GetNextString();
            instanceType = TypeUtils.GetType(type);

            for (var i = 0; i < count; i++) {
                list.Add(GetNextEncodedToStrings());
            }

            return list;
        }

        public object GetNextObject() {
            var type = GetNextString();
            var value = GetNextString();

            if (type.Length == 0) {
                // null object indicated by empty type string 
                return null;
            }

            var objectType = TypeUtils.GetType(type);
            if (objectType == null) {
                throw new Exception(logger.LogAndReturn($"Cannot find type for name: {type}"));
            }

            if (objectType == typeof(string)) {
                return value;
            }

            if (objectType.IsEnum) {
                return Enum.Parse(objectType, value);
            }

            var parseMethod = objectType.GetMethod("Parse", new[] {typeof(string)});
            if (parseMethod == null) {
                throw new Exception(logger.LogAndReturn($"Cannot find Parse method on type: {objectType}"));
            }

            var result = parseMethod.Invoke(null, new object[] {value});
            if (result == null) {
                throw new Exception(logger.LogAndReturn($"Failed to Parse value: {value} on type: {objectType}"));
            }

            return result;
        }

        public object[] GetNextObjectArray() {
            var list = new List<object>();
            var count = GetNextInt();

            for (var i = 0; i < count; i++) {
                list.Add(GetNextObject());
            }

            return list.ToArray();
        }

        public object GetNextSerializable() {
            var type = GetNextString();
            var value = GetNextString();
            if (type.Length == 0) {
                // null object indicated by empty type string 
                return null;
            }

            var objectType = TypeUtils.GetType(type);
            if (objectType == null) {
                throw new Exception(logger.LogAndReturn($"Cannot find type for name: {type}"));
            }

            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(value);
            writer.Flush();
            stream.Position = 0;
            var serializer = new DataContractSerializer(objectType);
            return serializer.ReadObject(stream);
        }

        public IEncodedToStrings GetNextEncodedToStrings() {
            var type = GetNextString();
            var encodedData = GetNextArray();

            if (type.Length == 0) {
                // null object indicated by empty type string 
                return null;
            }

            var objectType = TypeUtils.GetType(type);
            if (objectType == null) {
                throw new Exception(logger.LogAndReturn($"Cannot find type for name: {type}"));
            }

            if (!typeof(IEncodedToStrings).IsAssignableFrom(objectType)) {
                throw new Exception(logger.LogAndReturn($"Type: {objectType} needs to be: {typeof(IEncodedToStrings)}"));
            }

            return (IEncodedToStrings) Activator.CreateInstance(objectType, metamodel, loggerFactory, encodedData);
        }

        private void CheckCurrentIndex() {
            if (index >= strings.Length) {
                throw new IndexOutOfRangeException(logger.LogAndReturn($"String decode fail index: {index} length: {strings.Length}"));
            }
        }
    }
}
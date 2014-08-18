// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Web;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Core.Context;
using NakedObjects.Core.Util;
using NakedObjects.Util;

namespace NakedObjects.Core.Adapter {
    /// <summary>
    ///     Provide consistent string decoding strategy for <see cref="IEncodedToStrings" />
    /// </summary>
    /// <seealso cref="StringEncoderHelper" />
    public class StringDecoderHelper {
        private readonly INakedObjectReflector reflector;
        private readonly string[] strings;
        private int index;

        public StringDecoderHelper(INakedObjectReflector reflector,  string[] strings, bool decode = false) {
            Assert.AssertNotNull(reflector);
            this.reflector = reflector;
            this.strings = decode ? strings.Select(HttpUtility.UrlDecode).ToArray() : strings;
        }

        public bool HasNext {
            get { return index + 1 < strings.Length; }
        }

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
            return (T) Enum.Parse(typeof (T), strings[index++]);
        }

        public string[] GetNextArray() {
            var list = new List<string>();
            int count = GetNextInt();

            for (int i = 0; i < count; i++) {
                list.Add(GetNextString());
            }
            return list.ToArray();
        }

        public IList<object> GetNextValueCollection(out Type instanceType) {
            var list = new List<object>();
            int count = GetNextInt();
            string type = GetNextString();
            instanceType = TypeUtils.GetType(type);

            for (int i = 0; i < count; i++) {
                list.Add(GetNextObject());
            }
            return list;
        }

        public IList<IEncodedToStrings> GetNextObjectCollection(out Type instanceType) {
            var list = new List<IEncodedToStrings>();
            int count = GetNextInt();
            string type = GetNextString();
            instanceType = TypeUtils.GetType(type);

            for (int i = 0; i < count; i++) {
                list.Add(GetNextEncodedToStrings());
            }
            return list;
        }


        public object GetNextObject() {
            string type = GetNextString();
            string value = GetNextString();

            if (type.Length == 0) {
                // null object indicated by empty type string 
                return null;
            }

            Type objectType = TypeUtils.GetType(type);
            if (objectType == null) {
                throw new Exception(string.Format("Cannot find type for name: {0}", type));
            }
            if (objectType == typeof (string)) {
                return value;
            }

            if (objectType.IsEnum) {
                return Enum.Parse(objectType, value);
            }

            MethodInfo parseMethod = objectType.GetMethod("Parse", new[] {typeof (string)});
            if (parseMethod == null) {
                throw new Exception(string.Format("Cannot find Parse method on type: {0}", objectType));
            }
            object result = parseMethod.Invoke(null, new object[] {value});
            if (result == null) {
                throw new Exception(string.Format("Failed to Parse value: {0} on type: {1}", value, objectType));
            }
            return result;
        }

        public object[] GetNextObjectArray() {
            var list = new List<object>();
            int count = GetNextInt();

            for (int i = 0; i < count; i++) {
                list.Add(GetNextObject());
            }

            return list.ToArray();
        }

        public object GetNextSerializable() {
            string type = GetNextString();
            string value = GetNextString();
            if (type.Length == 0) {
                // null object indicated by empty type string 
                return null;
            }

            Type objectType = TypeUtils.GetType(type);
            if (objectType == null) {
                throw new Exception(string.Format("Cannot find type for name: {0}", type));
            }
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(value);
            writer.Flush();
            stream.Position = 0;
            return new NetDataContractSerializer().Deserialize(stream);
        }

        public IEncodedToStrings GetNextEncodedToStrings() {
            string type = GetNextString();
            string[] encodedData = GetNextArray();

            if (type.Length == 0) {
                // null object indicated by empty type string 
                return null;
            }

            Type objectType = TypeUtils.GetType(type);
            if (objectType == null) {
                throw new Exception(string.Format("Cannot find type for name: {0}", type));
            }
            if (!typeof (IEncodedToStrings).IsAssignableFrom(objectType)) {
                throw new Exception(string.Format("Type: {0} needs to be: {1} ", objectType, typeof (IEncodedToStrings)));
            }
            return (IEncodedToStrings)Activator.CreateInstance(objectType, new object[] { reflector, encodedData });
        }

        private void CheckCurrentIndex() {
            if (index >= strings.Length) {
                throw new IndexOutOfRangeException(string.Format("String decode fail index: {0} length: {1}", index, strings.Length));
            }
        }
    }
}
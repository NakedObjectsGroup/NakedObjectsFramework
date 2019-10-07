// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Core.Util;

namespace NakedObjects.Core.Adapter {
    /// <summary>
    ///     Provide consistent string encoding strategy for <see cref="IEncodedToStrings" />
    /// </summary>
    /// <seealso cref="StringDecoderHelper" />
    public class StringEncoderHelper {
        private readonly List<string> strings = new List<string>();
        public bool Encode { get; set; }

        /// <summary>
        ///     Use where type is known at compile time
        /// </summary>
        public void Add<T>(T item) where T : struct {
            strings.Add(item.ToString());
        }

        public void Add(string item) {
            strings.Add(item);
        }

        private void AddNullItem() {
            strings.Add("");
            strings.Add("0");
        }

        /// <summary>
        ///     Use where type is not known at compile time - the underlying type should still be a value
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         item should support <c>Parse</c> if <see cref="StringDecoderHelper" /> is used to extract value
        ///     </para>
        /// </remarks>
        public void Add(object item) {
            if (item == null) {
                AddNullItem();
            }
            else {
                strings.Add(item.GetType().FullName);
                strings.Add(item.ToString());
            }
        }

        public void Add(IEnumerable items, Type instanceType) {
            if (items == null) {
                Add(0);
                Add(instanceType.FullName);
            }
            else {
                object[] itemsAsArray = items.Cast<object>().ToArray();
                Add(itemsAsArray.Length);
                Add(instanceType.FullName);
                itemsAsArray.ForEach(Add);
            }
        }

        public void Add(IEnumerable<IEncodedToStrings> items, Type instanceType) {
            if (items == null) {
                Add(0);
                Add(instanceType.FullName);
            }
            else {
                IEncodedToStrings[] itemsAsArray = items.ToArray();
                Add(itemsAsArray.Length);
                Add(instanceType.FullName);
                itemsAsArray.ForEach(Add);
            }
        }

        public void Add(object[] items) {
            if (items == null) {
                Add(0);
            }
            else {
                Add(items.Length);
                items.ForEach(Add);
            }
        }

        public void Add(string[] items) {
            if (items == null) {
                Add(0);
            }
            else {
                Add(items.Length);
                items.ForEach(Add);
            }
        }

        public void Add(IEncodedToStrings item) {
            if (item == null) {
                AddNullItem();
            }
            else {
                strings.Add(item.GetType().FullName);
                Add(item.ToEncodedStrings());
            }
        }

        //public void AddSerializable(object serializable) {
        //    if (serializable == null) {
        //        AddNullItem();
        //    }
        //    else {
        //        strings.Add(serializable.GetType().FullName);
        //        var stream = new MemoryStream();
        //        new NetDataContractSerializer().Serialize(stream, serializable);
        //        stream.Position = 0;
        //        strings.Add(new StreamReader(stream).ReadToEnd());
        //    }
        //}

        public string[] ToArray() {
            if (Encode) {
                return strings.Select(HttpUtility.UrlEncode).ToArray();
            }
            return strings.ToArray();
        }
    }
}
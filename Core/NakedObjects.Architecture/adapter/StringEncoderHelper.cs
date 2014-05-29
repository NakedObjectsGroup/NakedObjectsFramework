// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace NakedObjects.Architecture.Adapter {
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
                Add(items.Cast<object>().Count());
                Add(instanceType.FullName);
                foreach (object item in items) {
                    Add(item);
                }
            }
        }

        public void Add(IEnumerable<IEncodedToStrings> items, Type instanceType) {
            if (items == null) {
                Add(0);
                Add(instanceType.FullName);
            }
            else {
                Add(items.Count());
                Add(instanceType.FullName);
                foreach (IEncodedToStrings item in items) {
                    Add(item);
                }
            }
        }

        public void Add(object[] items) {
            if (items == null) {
                Add(0);
            }
            else {
                Add(items.Length);
                foreach (object item in items) {
                    Add(item);
                }
            }
        }

        public void Add(string[] items) {
            if (items == null) {
                Add(0);
            }
            else {
                Add(items.Length);
                foreach (string item in items) {
                    Add(item);
                }
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

        public void AddSerializable(object serializable) {
            if (serializable == null) {
                AddNullItem();
            }
            else {
                strings.Add(serializable.GetType().FullName);
                var stream = new MemoryStream();
                new NetDataContractSerializer().Serialize(stream, serializable);
                stream.Position = 0;
                strings.Add(new StreamReader(stream).ReadToEnd());
            }
        }

        public string[] ToArray() {
            if (Encode) {
                return strings.Select(HttpUtility.UrlEncode).ToArray();
            }
            return strings.ToArray();
        }
    }
}
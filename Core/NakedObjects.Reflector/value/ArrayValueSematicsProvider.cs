// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using NakedObjects.Architecture;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Capabilities;

namespace NakedObjects.Reflector.DotNet.Value {
    public class ArrayValueSemanticsProvider<T> : ValueSemanticsProviderAbstract<T[]>, IPropertyDefaultFacet, IArrayValueFacet<T>, IFromStream {
        private const T[] DefaultValueConst = null;
        private const bool EqualByContent = true;
        private const bool Immutable = true;
        private const int TypicalLengthConst = 20;

        /// <summary>
        ///     Required because implementation of <see cref="IParser{T}" /> and <see cref="IEncoderDecoder{T}" />.
        /// </summary>
        public ArrayValueSemanticsProvider(IObjectSpecImmutable spec)
            : this(spec, null) {}

        public ArrayValueSemanticsProvider(IObjectSpecImmutable spec, ISpecification holder)
            : base(Type, holder, AdaptedType, TypicalLengthConst, Immutable, EqualByContent, DefaultValueConst, spec) {}

        public static Type Type {
            get { return typeof (IArrayValueFacet<T>); }
        }

        public static Type AdaptedType {
            get { return typeof (T[]); }
        }

        public override IFromStream FromStream {
            get { return typeof (T) == typeof (byte) ? this : null; }
        }

        #region IArrayValueFacet<T> Members

        public T[] ArrayValue(INakedObject nakedObject) {
            return nakedObject.GetDomainObject<T[]>();
        }

        #endregion

        #region IFromStream Members

        public object ParseFromStream(Stream stream, string mimeType = null, string name = null) {
            if (typeof (T) == typeof (byte)) {
                var ba = new byte[stream.Length];
                stream.Position = 0;
                stream.Read(ba, 0, (int) stream.Length);
                return ba;
            }
            throw new NotImplementedException(string.Format("Cannot parse an array of {0} from stream", typeof (T)));
        }

        #endregion

        #region IPropertyDefaultFacet Members

        public object GetDefault(INakedObject inObject) {
            return DefaultValueConst;
        }

        #endregion

        public static bool IsAdaptedType(Type type) {
            return type == typeof (T[]);
        }

        protected override T[] DoParse(string entry) {
            try {
                return (from s in entry.Split(' ')
                    where s.Trim().Length > 0
                    select (T) Convert.ChangeType(s, typeof (T))).ToArray();
            }
            catch (FormatException) {
                throw new InvalidEntryException(FormatMessage(entry));
            }
            catch (InvalidCastException) {
                throw new InvalidEntryException(string.Format(Resources.NakedObjects.ArrayConvertError, entry, typeof (T)));
            }
            catch (ArgumentNullException) {
                throw new InvalidEntryException(FormatMessage(entry));
            }
            catch (OverflowException) {
                // no simple way of getting min and maxvalue of 'T' = complexity isn't worth the risk just for an error message
                throw new InvalidEntryException(OutOfRangeMessage(entry, new T[] {}, new T[] {}));
            }
        }

        protected override T[] DoParseInvariant(string entry) {
            return (from s in entry.Split(' ')
                where s.Trim().Length > 0
                select (T) Convert.ChangeType(s, typeof (T), CultureInfo.InvariantCulture)).ToArray();
        }

        protected override string GetInvariantString(T[] obj) {
            return obj.Aggregate("", (s, t) => (string.IsNullOrEmpty(s) ? "" : s + " ") + t.ToString());
        }

        protected override string TitleStringWithMask(string mask, T[] value) {
            return TitleString(value);
        }

        protected override string TitleString(T[] obj) {
            return obj == null ? "" : obj.Aggregate("", (s, t) => (string.IsNullOrEmpty(s) ? "" : s + " ") + t.ToString());
        }

        protected override string DoEncode(T[] obj) {
            var stream = new MemoryStream();
            (new NetDataContractSerializer()).Serialize(stream, obj);
            stream.Position = 0;
            return new StreamReader(stream).ReadToEnd();
        }

        protected override T[] DoRestore(string data) {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(data);
            writer.Flush();
            stream.Position = 0;
            return (T[]) new NetDataContractSerializer().Deserialize(stream);
        }

        public override string ToString() {
            return string.Format("ArrayAdapter<{0}>", typeof (T));
        }
    }
}
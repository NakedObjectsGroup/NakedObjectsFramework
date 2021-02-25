// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.IO;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Spec;
using NakedFramework.Architecture.SpecImmutable;
using NakedFramework.Metamodel.Facet;

namespace NakedFramework.Metamodel.SemanticsProvider {
    [Serializable]
    public abstract class ValueSemanticsProviderAbstract<T> : FacetAbstract, IValueSemanticsProvider<T> {
        private readonly Type adaptedType;

        /// <summary>
        ///     Lazily looked up per <see cref="SpecImmutable" />
        /// </summary>
        protected ValueSemanticsProviderAbstract(Type adapterFacetType,
                                                 ISpecification holder,
                                                 Type adaptedType,
                                                 int typicalLength,
                                                 bool immutable,
                                                 bool equalByContent,
                                                 T defaultValue,
                                                 IObjectSpecImmutable specImmutable)
            : base(adapterFacetType, holder) {
            this.adaptedType = adaptedType;
            TypicalLength = typicalLength;
            IsImmutable = immutable;
            IsEqualByContent = equalByContent;
            DefaultValue = defaultValue;
            SpecImmutable = specImmutable;
        }

        public IObjectSpecImmutable SpecImmutable { get; }

        /// <summary>
        ///     We don't replace any (non- no-op) facets.
        /// </summary>
        /// <para>
        ///     For example, if there is already a <see cref="IPropertyDefaultFacet" /> then we shouldn't replace it.
        /// </para>
        public override bool CanAlwaysReplace => false;

        #region IValueSemanticsProvider<T> Members

        public T DefaultValue { get; }

        public int TypicalLength { get; }

        public bool IsEqualByContent { get; }

        public bool IsImmutable { get; }

        public string ToEncodedString(T obj) => DoEncode(obj);

        public T FromEncodedString(string data) => DoRestore(data);

        public virtual object ParseTextEntry(string entry) {
            if (entry == null) {
                throw new ArgumentException();
            }

            if (entry.Trim().Equals("")) {
                return null;
            }

            return DoParse(entry);
        }

        public object ParseInvariant(string entry) => DoParseInvariant(entry);

        public string InvariantString(T obj) => GetInvariantString(obj);

        public string EditableTitleOf(T existing) => DisplayTitleOf(existing);

        public string DisplayTitleOf(T obj) => TitleString(obj);

        public string TitleWithMaskOf(string mask, T obj) => TitleStringWithMask(mask, obj);

        #endregion

        public Type GetAdaptedClass() => adaptedType;

        protected abstract T DoParse(string entry);
        protected abstract T DoParseInvariant(string entry);
        protected abstract string GetInvariantString(T obj);

        protected virtual string TitleString(T obj) => obj.ToString();

        protected virtual string TitleStringWithMask(string mask, T obj) => obj.ToString();

        protected abstract string DoEncode(T obj);
        protected abstract T DoRestore(string data);

        protected string OutOfRangeMessage(string entry, T minValue, T maxValue) => string.Format(NakedObjects.Resources.NakedObjects.OutOfRange, entry, minValue, maxValue);

        protected static string FormatMessage(string entry) => string.Format(NakedObjects.Resources.NakedObjects.CannotFormat, entry, typeof(T).Name);

        /// <summary>
        ///     http://jonskeet.uk/csharp/readbinary.html
        ///     Reads data into a complete array, throwing an EndOfStreamException
        ///     if the stream runs out of data first, or if an IOException
        ///     naturally occurs.
        /// </summary>
        /// <param name="stream">The stream to read data from</param>
        /// <param name="data">
        ///     The array to read bytes into. The array
        ///     will be completely filled from the stream, so an appropriate
        ///     size must be given.
        /// </param>
        protected static void ReadWholeArray(Stream stream, byte[] data) {
            var offset = 0;
            var remaining = data.Length;
            while (remaining > 0) {
                var read = stream.Read(data, offset, remaining);
                if (read <= 0) {
                    throw new EndOfStreamException($"End of stream reached with {remaining} bytes left to read");
                }

                remaining -= read;
                offset += read;
            }
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}
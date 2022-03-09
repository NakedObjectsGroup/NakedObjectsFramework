// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.IO;
using System.Linq;
using NakedFramework.Architecture.Facet;
using NakedFramework.Core.Error;

namespace NakedFramework.Metamodel.SemanticsProvider;

[Serializable]
public sealed class ArrayValueSemanticsProvider<T> : ValueSemanticsProviderAbstract<T[]>, IArrayValueFacet<T>, IFromStream {
    private const T[] DefaultValueConst = null;
    private const bool Immutable = true;

    public ArrayValueSemanticsProvider()
        : base(Immutable, DefaultValueConst) { }

    public static Type AdaptedType => typeof(T[]);

    public override Type FacetType => typeof(IArrayValueFacet<T>);

    #region IFromStream Members

    public object ParseFromStream(Stream stream, string mimeType = null, string name = null) {
        if (typeof(T) != typeof(byte)) {
            throw new NakedObjectSystemException($"Cannot parse an array of {typeof(T)} from stream");
        }

        var ba = new byte[stream.Length];
        stream.Position = 0;
        stream.Read(ba, 0, (int)stream.Length);
        return ba;
    }

    #endregion

    public static bool IsAdaptedType(Type type) => type == AdaptedType;

    protected override T[] DoParse(string entry) {
        try {
            return (from s in entry.Split(' ')
                    where s.Trim().Length > 0
                    select (T)Convert.ChangeType(s, typeof(T))).ToArray();
        }
        catch (FormatException) {
            throw new InvalidEntryException(FormatMessage(entry));
        }
        catch (InvalidCastException) {
            throw new InvalidEntryException(string.Format(NakedObjects.Resources.NakedObjects.ArrayConvertError, entry, typeof(T)));
        }
        catch (ArgumentNullException) {
            throw new InvalidEntryException(FormatMessage(entry));
        }
        catch (OverflowException) {
            // no simple way of getting min and maxvalue of 'T' = complexity isn't worth the risk just for an error message
            throw new InvalidEntryException(OutOfRangeMessage(entry, Array.Empty<T>(), Array.Empty<T>()));
        }
    }

    protected override string TitleStringWithMask(string mask, T[] value) => TitleString(value);

    protected override string TitleString(T[] obj) {
        return obj is null ? "" : obj.Aggregate("", (s, t) => (string.IsNullOrEmpty(s) ? "" : $"{s} ") + t);
    }
}
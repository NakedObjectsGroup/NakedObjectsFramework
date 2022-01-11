// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Spec;
using NakedFramework.Architecture.SpecImmutable;
using NakedFramework.Core.Error;

namespace NakedFramework.Metamodel.SemanticsProvider;

[Serializable]
public sealed class ColorValueSemanticsProvider : ValueSemanticsProviderAbstract<Color>, IColorValueFacet {
    private const bool Immutable = true;
    private static readonly Color DefaultValueConst = Color.Black;

    public ColorValueSemanticsProvider(IObjectSpecImmutable spec, ISpecification holder)
        : base(Type, holder, AdaptedType, Immutable, DefaultValueConst, spec) { }

    public static Type Type => typeof(IColorValueFacet);

    public static Type AdaptedType => typeof(Color);

    public static KeyValuePair<Type, Func<IObjectSpecImmutable, ISpecification, IValueSemanticsProvider>> Factory => new(AdaptedType, (o, s) => new ColorValueSemanticsProvider(o, s));

    protected override Color DoParse(string entry) {
        try {
            int argb;
            if (entry.StartsWith("0x")) {
                argb = int.Parse(entry[2..], NumberStyles.AllowHexSpecifier);
            }
            else {
                argb = entry.StartsWith("#") ? int.Parse(entry[1..], NumberStyles.AllowHexSpecifier) : int.Parse(entry);
            }

            return Color.FromArgb(argb);
        }
        catch (FormatException) {
            throw new InvalidEntryException(FormatMessage(entry));
        }
        catch (OverflowException) {
            throw new InvalidEntryException(string.Format(NakedObjects.Resources.NakedObjects.OutOfRange, entry, int.MinValue, int.MaxValue));
        }
    }

    protected override string TitleStringWithMask(string mask, Color value) => value.ToString();
}
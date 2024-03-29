// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Interactions;
using NakedFramework.Metamodel.Error;

namespace NakedFramework.Metamodel.Facet;

[Serializable]
public sealed class RangeFacet : FacetAbstract, IRangeFacet {
    private readonly object max;

    // storing properties as IConvertible causes BinaryDeserialization to fail (eg with int). 
    // so store as object and cast. 
    private readonly object min;

    public RangeFacet(IConvertible min, IConvertible max, bool isDateRange) {
        this.min = min;
        this.max = max;
        IsDateRange = isDateRange;
    }

    public bool IsDateRange { get; set; }

    public int OutOfRange(INakedObjectAdapter nakedObjectAdapter) {
        var origVal = (IConvertible)nakedObjectAdapter?.Object;
        return origVal switch {
            null => 0,
            _ when IsSIntegral(origVal) => Compare(origVal.ToInt64(null), Min.ToInt64(null), Max.ToInt64(null)),
            _ when IsUIntegral(origVal) => Compare(origVal.ToUInt64(null), Min.ToUInt64(null), Max.ToUInt64(null)),
            _ when IsFloat(origVal) => Compare(origVal.ToDouble(null), Min.ToDouble(null), Max.ToDouble(null)),
            _ when IsDecimal(origVal) => Compare(origVal.ToDecimal(null), Min.ToDecimal(null), Max.ToDecimal(null)),
            _ when IsDateTime(origVal) => DateCompare(origVal.ToDateTime(null), Min.ToDouble(null), Max.ToDouble(null)),
            _ => 0
        };
    }

    public string Invalidates(IInteractionContext ic) {
        var proposedArgument = ic.ProposedArgument;
        if (OutOfRange(proposedArgument) == 0) {
            return null;
        }

        if (IsDateTime(proposedArgument.Object)) {
            var minDate = DateTime.Today.AddDays(Min.ToDouble(null)).ToShortDateString();
            var maxDate = DateTime.Today.AddDays(Max.ToDouble(null)).ToShortDateString();
            return string.Format(NakedObjects.Resources.NakedObjects.RangeMismatch, minDate, maxDate);
        }

        return string.Format(NakedObjects.Resources.NakedObjects.RangeMismatch, Min, Max);
    }

    public Exception CreateExceptionFor(IInteractionContext ic) => new InvalidRangeException(ic, Min, Max, Invalidates(ic));

    public IConvertible Min => (IConvertible)min;

    public IConvertible Max => (IConvertible)max;

    public override Type FacetType => typeof(IRangeFacet);

    private static int Compare<T>(T val, T min, T max) where T : struct, IComparable =>
        val.CompareTo(min) < 0
            ? -1
            : val.CompareTo(max) > 0
                ? 1
                : 0;

    private static int DateCompare(DateTime date, double min, double max) {
        var earliest = DateTime.Today.AddDays(min);
        var latest = DateTime.Today.AddDays(max);
        if (date < earliest) {
            return -1;
        }

        if (date > latest) {
            return +1;
        }

        return 0;
    }

    private static bool IsSIntegral(object o) => o is sbyte or short or int or long;

    private static bool IsUIntegral(object o) => o is byte or ushort or uint or ulong;

    private static bool IsFloat(object o) => o is float or double;

    private static bool IsDecimal(object o) => o is decimal;

    private static bool IsDateTime(object o) => o is DateTime;
}

// Copyright (c) Naked Objects Group Ltd.
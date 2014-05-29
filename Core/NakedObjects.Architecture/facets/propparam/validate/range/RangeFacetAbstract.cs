// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Interactions;

namespace NakedObjects.Architecture.Facets.Propparam.Validate.Range {
    public abstract class RangeFacetAbstract : FacetAbstract, IRangeFacet {
        protected RangeFacetAbstract(object min, object max, IFacetHolder holder)
            : base(Type, holder) {
            Min = (IConvertible) min;
            Max = (IConvertible) max;
        }

        public static Type Type {
            get { return typeof (IRangeFacet); }
        }

        #region IRangeFacet Members

        public virtual int OutOfRange(INakedObject nakedObject) {
            if (nakedObject == null) {
                return 0; //Date fields can contain nulls
            }
            var origVal = ((IConvertible) nakedObject.Object);
            if (IsSIntegral(origVal)) {
                return Compare(origVal.ToInt64(null), Min.ToInt64(null), Max.ToInt64(null));
            }
            if (IsUIntegral(origVal)) {
                return Compare(origVal.ToUInt64(null), Min.ToUInt64(null), Max.ToUInt64(null));
            }
            if (IsFloat(origVal)) {
                return Compare(origVal.ToDouble(null), Min.ToDouble(null), Max.ToDouble(null));
            }
            if (IsDecimal(origVal)) {
                return Compare(origVal.ToDecimal(null), Min.ToDecimal(null), Max.ToDecimal(null));
            }
            if (IsDateTime(origVal)) {
                return DateCompare(origVal.ToDateTime(null), Min.ToDouble(null), Max.ToDouble(null));
            }
            return 0;
        }

        public virtual string Invalidates(InteractionContext ic) {
            INakedObject proposedArgument = ic.ProposedArgument;
            if (OutOfRange(proposedArgument) == 0) {
                return null;
            }

            if (IsDateTime(proposedArgument.Object)) {
                string minDate = DateTime.Today.AddDays(Min.ToDouble(null)).ToShortDateString();
                string maxDate = DateTime.Today.AddDays(Max.ToDouble(null)).ToShortDateString();
                return string.Format(Resources.NakedObjects.RangeMismatch, minDate, maxDate);
            }
            return string.Format(Resources.NakedObjects.RangeMismatch, Min, Max);
        }

        public virtual InvalidException CreateExceptionFor(InteractionContext ic) {
            return new InvalidRangeException(ic, Min, Max, Invalidates(ic));
        }

        protected int Compare<T>(T val, T min, T max) where T : struct, IComparable {
            if (val.CompareTo(min) < 0) {
                return -1;
            }

            if (val.CompareTo(max) > 0) {
                return 1;
            }

            return 0;
        }


        protected int DateCompare(DateTime date, double min, double max) {
            DateTime earliest = (DateTime.Today).AddDays(min);
            DateTime latest = (DateTime.Today).AddDays(max);
            if (date < earliest) return -1;
            if (date > latest) return +1;
            return 0;
        }

        private static bool IsSIntegral(object o) {
            return o is sbyte || o is short || o is int || o is long;
        }

        private static bool IsUIntegral(object o) {
            return o is byte || o is ushort || o is uint || o is ulong;
        }

        private static bool IsFloat(object o) {
            return o is float || o is double;
        }

        private static bool IsDecimal(object o) {
            return o is decimal;
        }

        private static bool IsDateTime(object o) {
            return o is DateTime;
        }

        #endregion

        public IConvertible Min { get; private set; }
        public IConvertible Max { get; private set; }
        public bool IsDateRange { get; set; }
    }
}
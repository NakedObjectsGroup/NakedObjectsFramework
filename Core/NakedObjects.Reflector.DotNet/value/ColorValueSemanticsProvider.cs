// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Drawing;
using System.Globalization;
using NakedObjects.Architecture;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Adapter.Value;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Properties.Defaults;
using NakedObjects.Capabilities;
using NakedObjects.Core.Context;
using NakedObjects.Core.Persist;

namespace NakedObjects.Reflector.DotNet.Value {
    public class ColorValueSemanticsProvider : ValueSemanticsProviderAbstract<Color>, IPropertyDefaultFacet {
        private const bool equalByContent = true;
        private const bool immutable = true;
        private const int typicalLength = 4;
        private static readonly Color defaultValue = Color.Black;

        /// <summary>
        ///     Required because implementation of <see cref="IParser{T}" /> and <see cref="IEncoderDecoder{T}" />.
        /// </summary>
        public ColorValueSemanticsProvider()
            : this(null) {}

        public ColorValueSemanticsProvider(IFacetHolder holder)
            : base(Type, holder, AdaptedType, typicalLength, immutable, equalByContent, defaultValue) {}

        public static Type Type {
            get { return typeof (IColorValueFacet); }
        }

        public static Type AdaptedType {
            get { return typeof (Color); }
        }

        #region IPropertyDefaultFacet Members

        public object GetDefault(INakedObject inObject) {
            return defaultValue;
        }

        #endregion

        public static bool IsAdaptedType(Type type) {
            return type == typeof (Color);
        }


        protected override Color DoParse(string entry) {
            try {
                int argb;
                if (entry.StartsWith("0x")) {
                    argb = int.Parse(entry.Substring(2), NumberStyles.AllowHexSpecifier);
                }
                else {
                    argb = entry.StartsWith("#") ? int.Parse(entry.Substring(1), NumberStyles.AllowHexSpecifier) : int.Parse(entry);
                }

                return Color.FromArgb(argb);
            }
            catch (FormatException) {
                throw new InvalidEntryException(FormatMessage(entry));
            }
            catch (OverflowException) {
                throw new InvalidEntryException(string.Format(Resources.NakedObjects.OutOfRange, entry, int.MinValue, int.MaxValue));
            }
        }

        protected override Color DoParseInvariant(string entry) {
            return Color.FromArgb(int.Parse(entry, CultureInfo.InvariantCulture));
        }

        protected override string GetInvariantString(Color obj) {
            return obj.ToArgb().ToString(CultureInfo.InvariantCulture);
        }

        protected override string TitleStringWithMask(string mask, Color value) {
            return value.ToString();
        }

        protected override string DoEncode(Color obj) {
            return (obj).ToArgb().ToString();
        }

        protected override Color DoRestore(string data) {
            return Color.FromArgb(int.Parse(data));
        }

        public int ColorValue(INakedObject nakedObject) {
            if (nakedObject == null) {
                return 0;
            }
            var color = (Color) nakedObject.Object;
            return color.ToArgb();
        }

        public INakedObject CreateValue(INakedObject nakedObject, int color) {
            return NakedObjectsContext.ObjectPersistor.CreateAdapter(Color.FromArgb(color), null, null);
        }


        public override string ToString() {
            return "ColorAdapter: ";
        }
    }
}
// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections.Generic;
using System.Linq;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Properties.Enums;
using NakedObjects.Util;

namespace NakedObjects.Reflector.DotNet.Facets.Properties.Enums {
    public class EnumFacet : EnumFacetAbstract {
        private readonly EnumNameComparer comparer;
        private readonly Type typeOfEnum;

        public EnumFacet(ISpecification holder, Type typeOfEnum)
            : base(holder) {
            this.typeOfEnum = typeOfEnum;
            comparer = new EnumNameComparer(this);
        }

        private string ToDisplayName(string enumName) {
            return NameUtils.NaturalName(Enum.Parse(typeOfEnum, enumName).ToString());
        }

        public override object[] GetChoices(INakedObject inObject) {
            return Enum.GetNames(typeOfEnum).OrderBy(s => s, comparer).Select(s => Enum.Parse(typeOfEnum, s)).ToArray();
        }

        public override object[] GetChoices(INakedObject inObject, object[] choiceValues) {
            return choiceValues.Select(o => Enum.Parse(typeOfEnum, o.ToString())).ToArray();
        }

        public override string GetTitle(INakedObject inObject) {
            return ToDisplayName(inObject.Object.ToString());
        }

        #region Nested type: EnumNameComparer

        private class EnumNameComparer : IComparer<string> {
            private readonly EnumFacet facet;

            public EnumNameComparer(EnumFacet facet) {
                this.facet = facet;
            }

            #region IComparer<string> Members

            public int Compare(string x, string y) {
                return string.Compare(facet.ToDisplayName(x), facet.ToDisplayName(y));
            }

            #endregion
        }

        #endregion
    }

    // Copyright (c) Naked Objects Group Ltd.
}
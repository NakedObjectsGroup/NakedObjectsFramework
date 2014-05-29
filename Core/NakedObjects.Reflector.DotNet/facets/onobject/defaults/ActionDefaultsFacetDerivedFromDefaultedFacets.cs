// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Actions.Defaults;
using NakedObjects.Architecture.Facets.Objects.Defaults;
using NakedObjects.Architecture.Reflect;

namespace NakedObjects.Reflector.DotNet.Facets.Objects.Defaults {
    public class ActionDefaultsFacetDerivedFromDefaultedFacets : ActionDefaultsFacetAbstract {
        private readonly IDefaultedFacet defaultedFacet;

        public ActionDefaultsFacetDerivedFromDefaultedFacets(IDefaultedFacet defaultedFacet, IFacetHolder holder)
            : base(holder) {
            this.defaultedFacet = defaultedFacet;
        }

        /// <summary>
        ///     Return the defaults.
        /// </summary>
        /// <para>
        ///     We get the defaults fresh each time in case the defaults might
        ///     conceivably change.
        /// </para>
        public override Tuple<object, TypeOfDefaultValue> GetDefault(INakedObject nakedObject) {
            return new Tuple<object, TypeOfDefaultValue>(defaultedFacet == null ? null : defaultedFacet.Default, TypeOfDefaultValue.Implicit);
        }
    }
}
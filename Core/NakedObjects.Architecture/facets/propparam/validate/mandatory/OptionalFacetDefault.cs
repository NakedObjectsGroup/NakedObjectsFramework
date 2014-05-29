// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

namespace NakedObjects.Architecture.Facets.Propparam.Validate.Mandatory {
    /// <summary>
    ///     Whether a property or a parameter is optional (not mandatory).
    /// </summary>
    /// <para>
    ///     For a mandatory property, the object cannot be saved/updated without
    ///     the value being provided.  For a mandatory parameter, the action cannot
    ///     be invoked without the value being provided.
    /// </para>
    /// <para>
    ///     This is used in the alternative Naked Objects Programming Model, where
    ///     all properties and parameters are optional by default and need to be annotated
    ///     as manadatory.
    /// </para>
    public class OptionalFacetDefault : MandatoryFacetAbstract {
        public OptionalFacetDefault(IFacetHolder holder)
            : base(holder) {}

        public override bool IsMandatory {
            get { return false; }
        }

        public override bool CanAlwaysReplace {
            get { return false; }
        }
    }
}
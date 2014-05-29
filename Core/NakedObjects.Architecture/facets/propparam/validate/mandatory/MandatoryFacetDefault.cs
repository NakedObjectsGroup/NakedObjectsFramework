// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

namespace NakedObjects.Architecture.Facets.Propparam.Validate.Mandatory {
    /// <summary>
    ///     Whether a property or a parameter is mandatory (not optional).
    /// </summary>
    /// <para>
    ///     For a mandatory property, the object cannot be saved/updated without
    ///     the value being provided.  For a mandatory parameter, the action cannot
    ///     be invoked without the value being provided.
    /// </para>
    /// <para>
    ///     In the standard Naked Objects Programming Model, specify mandatory by
    ///     <i>omitting</i> the <see cref="OptionallyAttribute" /> annotation.
    /// </para>
    public class MandatoryFacetDefault : MandatoryFacetAbstract {
        public MandatoryFacetDefault(IFacetHolder holder)
            : base(holder) {}

        public override bool IsMandatory {
            get { return true; }
        }

        public override bool CanAlwaysReplace {
            get { return false; }
        }
    }
}
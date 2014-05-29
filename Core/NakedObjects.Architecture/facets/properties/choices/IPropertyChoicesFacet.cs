// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections.Generic;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Spec;

namespace NakedObjects.Architecture.Facets.Properties.Choices {
    /// <summary>
    ///     Provides a set of choices for a property.
    /// </summary>
    /// <para>
    ///     Viewers would typically represent this as a drop-down list box for the property.
    /// </para>
    /// <para>
    ///     In the standard Naked Objects Programming Model, corresponds to
    ///     the <c>ChoicesXxx</c> supporting method for the property <c>Xxx</c>.
    /// </para>
    /// <para>
    ///     An alternative mechanism may be to use the <see cref="BoundedAttribute" /> annotation
    ///     against the referenced class.
    /// </para>
    public interface IPropertyChoicesFacet : IFacet {
        Tuple<string, INakedObjectSpecification>[] ParameterNamesAndTypes { get; }
        /// <summary>
        ///     Gets the available choices for this property
        /// </summary>
        object[] GetChoices(INakedObject inObject, IDictionary<string, INakedObject> parameterNameValues);
    }
}
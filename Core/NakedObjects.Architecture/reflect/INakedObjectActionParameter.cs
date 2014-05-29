// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections.Generic;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Spec;

namespace NakedObjects.Architecture.Reflect {
    public interface INakedObjectActionParameter : INakedObjectFeature {
        /// <summary>
        /// </summary>
        /// <para>
        ///     Either this or <see cref="IsCollection" /> will be true.
        /// </para>
        /// <para>
        ///     Design comment: modelled after  <see cref="INakedObjectAssociation.IsObject" />
        /// </para>
        bool IsObject { get; }

        /// <summary>
        ///     Only for symmetry with <see cref="INakedObjectAssociation" />, however since the NOF does not support collections as
        ///     actions all implementations should return <c>false</c>.
        /// </summary>
        bool IsCollection { get; }

        /// <summary>
        ///     The Owning <see cref="INakedObjectAction" />
        /// </summary>
        INakedObjectAction Action { get; }

        /// <summary>
        ///     Returns a flag indicating if it can be left unset when the action can be invoked
        /// </summary>
        bool IsMandatory { get; }

        /// <summary>
        ///     Returns the zero-based index to this parameter
        /// </summary>
        int Number { get; }

        /// <summary>
        ///     Returns the identifier of the member, which must not change. This should be all Pascal-case with no
        ///     spaces: so if the member is called 'Return Date' then the a suitable id would be 'ReturnDate'.
        /// </summary>
        string Id { get; }

        /// <summary>
        ///     Whether the parameter has a bounded set associated or a set of options coded.
        /// </summary>
        bool IsChoicesEnabled { get; }

        /// <summary>
        ///     Whether the parameter has a bounded set associated or a set of options coded.
        /// </summary>
        bool IsMultipleChoicesEnabled { get; }

        /// <summary>
        ///     Whether the parameter has a autocomplete method associated.
        /// </summary>
        bool IsAutoCompleteEnabled { get; }

        /// <summary>
        ///     Whether proposed value for this parameter is valid
        /// </summary>
        IConsent IsValid(INakedObject nakedObject, INakedObject proposedValue);

        /// <summary>
        ///     Get set of options for the parameter - either coded choices or bounded set
        /// </summary>
        INakedObject[] GetChoices(INakedObject nakedObject, IDictionary<string, INakedObject> parameterNameValues);

        /// <summary>
        ///     Get set of options for the parameter - either coded choices or bounded set
        /// </summary>
        Tuple<string, INakedObjectSpecification>[] GetChoicesParameters();

        /// <summary>
        ///     Get set of autocompletions for the parameter
        /// </summary>
        INakedObject[] GetCompletions(INakedObject nakedObject, string autoCompleteParm);


        /// <summary>
        ///     GetDefault value for parameter
        /// </summary>
        INakedObject GetDefault(INakedObject nakedObject);

        /// <summary>
        ///     GetDefault type value for parameter
        /// </summary>
        TypeOfDefaultValue GetDefaultType(INakedObject nakedObject);
    }

    public enum TypeOfDefaultValue {
        Explicit,
        Implicit
    }
}
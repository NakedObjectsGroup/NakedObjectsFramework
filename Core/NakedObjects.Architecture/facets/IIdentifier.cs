// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture.Spec;

namespace NakedObjects.Architecture.Facets {
    public enum IdentifierDepth {
        Class,
        ClassName,
        ClassNameParams,
        Name,
        Parms
    }

    public enum CheckType {
        Action,
        ViewField,
        EditField
    }

    public interface IIdentifier : IComparable {
        string ClassName { get; }

        string MemberName { get; }

        string[] MemberParameterTypeNames { get; }

        string[] MemberParameterNames { get; }

        INakedObjectSpecification[] MemberParameterSpecifications { get; }

        /// <summary>
        ///     Returns <c>true</c> if the member is for a property or collection; <c>false</c> if for an action
        /// </summary>
        bool IsField { get; }

        string ToIdentityString(IdentifierDepth depth);

        string ToIdentityStringWithCheckType(IdentifierDepth depth, CheckType checkType);
    }

    // Copyright (c) Naked Objects Group Ltd.
}
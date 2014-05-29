// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

namespace NakedObjects.Architecture.Facets.Objects.Defaults {
    /// <summary>
    ///     Indicates that this class has a default.
    /// </summary>
    /// <para>
    ///     The mechanism for providing a default will vary by the applib.
    /// </para>
    /// <para>
    ///     The rest of the framework does not used this directly, but instead we infer from
    ///     method's return type / parameter types, and copy over.
    /// </para>
    public interface IDefaultedFacet : ISingleValueFacet {
        object Default { get; }
        bool IsValid { get; }
    }
}
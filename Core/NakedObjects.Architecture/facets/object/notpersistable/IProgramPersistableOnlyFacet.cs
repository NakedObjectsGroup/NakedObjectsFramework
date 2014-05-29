// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

namespace NakedObjects.Architecture.Facets.Objects.NotPersistable {
    /// <summary>
    ///     Indicates that the instances of this class are not persistable either
    ///     by the user
    ///     In the standard Naked Objects Programming Model, typically corresponds to applying the
    ///     <see cref="ProgramPersistableOnlyAttribute" /> annotation at the class level
    /// </summary>
    public interface IProgramPersistableOnlyFacet : IMarkerFacet {}
}
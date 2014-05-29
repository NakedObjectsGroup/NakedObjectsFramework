// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Reflect;

namespace NakedObjects.Architecture.Facets.Actions.Executed {
    /// <summary>
    ///     Whether the action should be invoked locally, remotely,
    ///     or on the default location depending on its persistence state
    /// </summary>
    /// <para>
    ///     In the standard Naked Objects Programming Model, corresponds to
    ///     annotating the action method using <see cref="ExecutedAttribute" />
    /// </para>
    public interface IExecutedFacet : IFacet {
        Target Target { get; }

        Where ExecutedWhere();
    }
}
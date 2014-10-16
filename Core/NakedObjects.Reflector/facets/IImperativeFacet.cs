// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Reflection;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Reflector.DotNet.Reflect;

namespace NakedObjects.Reflector.DotNet.Facets {
    /// <summary>
    ///     A <see cref="IFacet" /> implementation that ultimately wraps a
    ///     <see cref="MethodInfo" />, for a implementation of a <see cref="INakedObjectMember" />
    /// </summary>
    /// <para>
    ///     Used by <see cref="NakedObjectSpecification.GetMember" /> in order to
    ///     reverse lookup <see cref="INakedObjectMember" />s from underlying <see cref="MethodInfo" />s.
    ///     So, for example, the facets that represent an action Xxx, or a ValidateXxx method, or
    ///     an AddToXxx collection, can all be used to lookup the member.
    /// </para>
    /// <para>
    ///     <see cref="IFacet" />s relating to the class itself (ie for <see cref="IObjectSpec" />
    ///     should not implement this interface.
    /// </para>
    public interface IImperativeFacet {
        /// <summary>
        ///     The <see cref="MethodInfo" /> invoked by this Facet.
        /// </summary>
        MethodInfo GetMethod();
    }
}
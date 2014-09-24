// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Facets;

namespace NakedObjects.Reflector.I18n {
    /// <summary>
    ///     Authorizes the user in the current session view and use members of an object
    /// </summary>
    public interface II18nManager  {
        /// <summary>
        ///     Get the localized description for the specified identified action/property. Returns null if no
        ///     description available.
        /// </summary>
        string GetDescription(IIdentifier identifier, string original);

        /// <summary>
        ///     Get the localized name for the specified identified action/property. Returns null if no name available
        /// </summary>
        string GetName(IIdentifier identifier, string original);

        /// <summary>
        ///     Get the localized help text for the specified identified action/property. Returns null if no help text
        ///     available.
        /// </summary>
        string GetHelp(IIdentifier identifier);

        /// <summary>
        ///     Get the localized parameter name for the specified identified action/property. Returns null if no
        ///     parameter is available. Otherwise returns a string for the localised name for the corresponding parameter, or is null if no
        ///     parameter name is available.
        /// </summary>
        string GetParameterName(IIdentifier identifier, int index, string original);

        string GetParameterDescription(IIdentifier identifier, int index, string original);
    }

    // Copyright (c) Naked Objects Group Ltd.
}
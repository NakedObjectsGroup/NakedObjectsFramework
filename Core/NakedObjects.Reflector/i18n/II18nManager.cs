// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using NakedObjects.Architecture.Adapter;

namespace NakedObjects.Reflect.I18n {
    /// <summary>
    ///     Authorizes the user in the current session view and use members of an object
    /// </summary>
    public interface II18nManager {
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
// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;

namespace NakedFramework {
    public interface IMenuFactory {
        /// <summary>
        ///     Method is included for backwards compatibility with previous versions.
        ///     However, it is recommended that you use newer methods which specify the Id as the second parameter
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="addAllActions"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        IMenu NewMenu<T>(bool addAllActions = false, string name = null);

        /// <summary>
        ///     Method is included for backwards compatibility with previous versions.
        ///     However, it is recommended that you use newer methods which specify the Id as the second parameter
        /// </summary>
        /// <param name="type"></param>
        /// <param name="addAllActions"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        IMenu NewMenu(Type type, bool addAllActions = false, string name = null);

        /// <summary>
        ///     Creates a new menu without any default type (so when adding actions to the IMenu, must use methods that specify the
        ///     type)
        /// </summary>
        /// <param name="name">Cannot be null or empty</param>
        /// <param name="id">Cannot be null or empty</param>
        /// <returns>New menu</returns>
        IMenu NewMenu(string name, string id);

        /// <summary>
        ///     Creates a new menu with the Type specified.
        ///     If the optional name parameter is not specified, then the menu takes its name from the type.
        ///     Menu Id is based on the type
        /// </summary>
        /// <param name="name">Cannot be null or empty</param>
        /// <param name="id">Cannot be null or empty</param>
        /// <param name="defaultType">The default type from which actions may be added without specifying the type.</param>
        /// <param name="addAllActions">Add all the actions from the specified defaultType</param>
        /// <returns>New menu</returns>
        IMenu NewMenu(string name, string id, Type defaultType, bool addAllActions = false);
    }
}
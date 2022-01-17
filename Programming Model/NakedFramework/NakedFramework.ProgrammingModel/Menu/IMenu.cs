// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;

namespace NakedFramework.Menu {
    /// <summary>
    ///     Provides methods for constructing a menu on a domain object. Implementation of this
    ///     interface will be provided by the framework.
    /// </summary>
    public interface IMenu {
        /// <summary>
        ///     Allows the default name for the menu to be over-ridden
        ///     The type on which the actions to be added are defined. Will normally be the
        ///     object on which the Menu is defined, but may be changed to allow actions
        ///     to be added from another service.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>This menu (for fluent programming)</returns>
        IMenu WithMenuName(string name);

        /// <summary>
        ///     Allows the id for the menu to be specified or over-ridden.
        /// </summary>
        /// <returns>This menu (for fluent programming)</returns>
        IMenu WithId(string id);

        /// <summary>
        ///     Add an action from the Type.
        /// </summary>
        /// <param name="actionName"></param>
        /// <param name="ignoreCase"></param>
        /// <returns>This menu (for fluent programming)</returns>
        IMenu AddAction(string actionName, bool ignoreCase = false);

        /// <summary>
        /// </summary>
        /// <param name="subMenuName"></param>
        /// <returns>
        ///     The new menu, which will already have been added to the hosting menu. It will
        ///     have the same Type as the super-menu.
        /// </returns>
        IMenu CreateSubMenu(string subMenuName);

        /// <summary>
        ///     Finds a previously-created sub menu.  Throws exception if sub-menu of that name does not exist.
        /// </summary>
        /// <param name="menuName"></param>
        /// <returns></returns>
        IMenu GetSubMenu(string menuName);

        /// <summary>
        ///     Adds all native actions from the Default Type not previously added directly or to a sub-menu.
        /// </summary>
        /// <returns>This menu (for fluent programming)</returns>
        IMenu AddRemainingNativeActions();

        /// <summary>
        ///     Adds all actions contributed to the Default Type.
        ///     Where a sub-menu exists of the correct name, actions will be added to that.
        ///     Will throw an exception if SetDefaultType has not been called.
        /// </summary>
        /// <returns>This menu (for fluent programming)</returns>
        IMenu AddContributedActions();

        /// <summary>
        ///     Add an action from the specified Type.
        /// </summary>
        /// <param name="fromType"></param>
        /// <param name="actionName"></param>
        /// <param name="ignoreCase"></param>
        /// <returns>This menu (for fluent programming)</returns>
        IMenu AddAction(Type fromType, string actionName, bool ignoreCase = false);

        IMenu AddRemainingActions(Type fromType);
    }
}
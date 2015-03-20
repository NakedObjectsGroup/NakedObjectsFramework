using System;
namespace NakedObjects.Menu {

    /// <summary>
    /// Provides methods for constructing a menu on a domain object. Implementation of this
    /// interface will be provided by the framework.
    /// </summary>
    public interface IMenu  {

        /// <summary>
        /// Allows the default name for the menu to be over-ridden
        /// The type on which the actions to be added are defined. Will normally be the
        /// object on which the Menu is defined, but may be changed to allow actions
        /// to be added from another service.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>This menu (for fluent programming)</returns>
        IMenu WithMenuName(string name);

        /// <summary>
        /// The type on which the actions to be added are defined. Will normally be the
        /// object on which the Menu is defined, but may be changed to allow actions
        /// to be added from another service.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>This menu (for fluent programming)</returns>
        Type Type { get; set; }

        /// <summary>
        /// Allows the id for the menu to be specified or over-ridden.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>This menu (for fluent programming)</returns>
        IMenu WithId(string id);

        /// <summary>
        /// Add an action from the Type.
        /// </summary>
        /// <param name="actionName"></param>
        /// <param name="renamedTo"></param>
        /// <returns>This menu (for fluent programming)</returns>
        IMenu AddAction(string actionName);

         /// <summary>
        /// </summary>
        /// <param name="subMenuName"></param>
        /// <returns>The new menu, which will already have been added to the hosting menu. It will
        /// have the same Type as the super-menu.</returns>
        IMenu CreateSubMenu(string subMenuName);

        /// <summary>
        /// Finds a previously-created sub menu.  Throws exception if sub-menu of that name does not exist.
        /// </summary>
        /// <param name="menuName"></param>
        /// <returns></returns>
        IMenu GetSubMenu(string menuName);

        /// <summary>
        /// Adds all native actions from the Default Type not previously added directly or to a sub-menu.
        /// </summary>
        /// <returns>This menu (for fluent programming)</returns>
        IMenu AddRemainingNativeActions();

        /// <summary>
        /// Adds all actions contributed to the Default Type.
        /// Where a sub-menu exists of the correct name, actions will be added to that.
        /// Will throw an exception if SetDefaultType has not been called.
        /// </summary>
        /// <returns></returns>
        IMenu AddContributedActions();
    }
}

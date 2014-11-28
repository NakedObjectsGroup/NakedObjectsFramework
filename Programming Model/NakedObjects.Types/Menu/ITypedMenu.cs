
namespace NakedObjects.Menu {
    /// <summary>
    /// An easy way to build a menu based on the actions of a single object (or service).
    /// You can still add actions, or sub-menus, from other types to it.
    /// </summary>
    /// <typeparam name="TObject"></typeparam>
    public interface ITypedMenu<TObject> : IMenu {
           
        /// <summary>
        /// 
        /// </summary>
        /// <param name="actionName"></param>
        /// <param name="renamedTo"></param>
        /// <returns>This menu (for fluent programming)</returns>
        ITypedMenu<TObject> AddAction(string actionName, string renamedTo = null);

        /// <summary>
        /// Adds all native actions from the object/service not previously added directly or to a sub-menu)
        /// </summary>
        /// <returns>This menu (for fluent programming)</returns>
        ITypedMenu<TObject> AddRemainingNativeActions();

        /// <summary>
        /// Adds all actions contributed to the object type.
        /// Where a sub-menu exists of the correct name, actions will be added to that.
        /// </summary>
        /// <returns></returns>
        ITypedMenu<TObject> AddContributedActions();

        /// <summary>
        /// Create a new TypedMenu based on same object type
        /// </summary>
        /// <param name="subMenuName"></param>
        /// <returns>The new menu, which will already have been added to the hosting menu</returns>
        ITypedMenu<TObject> CreateSubMenuOfSameType(string subMenuName);
    }
}

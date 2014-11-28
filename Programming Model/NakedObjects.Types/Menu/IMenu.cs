using System.Collections.Generic;
namespace NakedObjects.Menu {

    /// <summary>
    /// Extension of IMenuImmutable that provides methods for building the menu
    /// during reflection time.
    /// </summary>
    public interface IMenu  {

        /// <summary>
        /// Adds specified action as the next menu item
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="actionName"></param>
        /// <param name="renamedTo"></param>
        /// <returns>This menu (for fluent programming)</returns>
        IMenu AddActionFrom<TObject>(string actionName, string renamedTo = null);

        /// <summary>
        /// Adds all actions from the service not previously added individually,
        /// in the order they are specified in the service.
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <returns>This menu (for fluent programming)</returns>
        IMenu AddAllRemainingActionsFrom<TService>();

        /// <summary>
        /// </summary>
        /// <param name="subMenuName"></param>
        /// <returns>The new menu, which will already have been added to the hosting menu</returns>
        IMenu CreateSubMenu(string subMenuName);
    }
}

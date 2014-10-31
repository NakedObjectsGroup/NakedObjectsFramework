// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System.Collections;
using System.Linq;
using System.Web.Mvc;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Menu;
using System.Collections.Generic;

namespace NakedObjects.Web.Mvc.Html {
    public static class MenuExtensions {

        #region ServiceMenus

        /// <summary>
        /// Create menus for all services in ViewData
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static MvcHtmlString MainMenus(this HtmlHelper html) {
            var tag = new TagBuilder("div");
            tag.AddCssClass(IdHelper.ServicesContainerName);
            var menus = (IEnumerable) html.ViewData[IdHelper.NofMainMenus];

            if (menus != null && menus.Cast<IMenu>().Any()) {
                foreach (IMenu menu in menus) {
                    tag.InnerHtml += html.Menu(menu);
                }
                return MvcHtmlString.Create(tag.ToString());
            }
            return MvcHtmlString.Create("");
        }

        public static MvcHtmlString Menu(this HtmlHelper html, IMenu menu) {
            //Build the appropriate surrounding Html

            //Iterate through the items on the menu
            //Where item is an action, get the IActionSpecImmutable
            //Using INakedObjectManager ? get the corresponding IActionSpec
            //Get a service instance (as INakedObject) for the IActionSpec

            IList<ElementDescriptor> elements = html.MenuActions(menu);
            string menuName = menu.Name;
            string menuId = menu.Name; //This is redundant, but should also be unique.  Could replace with a Guid Id on the Menu?
            return CommonHtmlHelper.BuildMenuContainer(elements,
                                                       IdHelper.MenuContainerName,
                                                       menuId,
                                                       menuName);
        }





        #endregion
    }
}
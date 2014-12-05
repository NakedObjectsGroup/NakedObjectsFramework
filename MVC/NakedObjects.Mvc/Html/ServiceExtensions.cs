// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System.Collections;
using System.Linq;
using System.Web.Mvc;
using NakedObjects.Architecture.Adapter;
using System;

namespace NakedObjects.Web.Mvc.Html {

    public static class ServiceExtensions {

        #region ServiceMenus

        //TODO: Mark obsolete when Menus refactoring complete
        //[Obsolete("Use MenuExtensions#MainMenus")]
        public static MvcHtmlString Services(this HtmlHelper html) {
            return MenuExtensions.MainMenus(html);
        }

        //TODO: Mark obsolete when Menus refactoring complete
        //[Obsolete("Add CustomMenuItems into an IMenu directly when constructing menus")]
        public static MvcHtmlString Service(this HtmlHelper html, object service, params CustomMenuItem[] menuItems) {
            INakedObject nakedObject = html.Framework().GetNakedObject(service);
            return CommonHtmlHelper.BuildMenuContainer(html.ObjectActions(nakedObject, false, menuItems),
                                                       IdHelper.MenuContainerName,
                                                       IdHelper.GetServiceContainerId(nakedObject),
                                                       nakedObject.TitleString());
        }

        //TODO: Mark obsolete when Menus refactoring complete
        //[Obsolete("Use MenuExtensions#MainMenu")]
        public static MvcHtmlString ServiceMenu(this HtmlHelper html, object service) {
            return html.Service(service);
        }

        #endregion
    }
}
// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System.Collections;
using System.Linq;
using System.Web.Mvc;
using NakedObjects.Architecture.Adapter;

namespace NakedObjects.Web.Mvc.Html {
    public static class ServiceExtensions {
        #region ServiceMenus

        /// <summary>
        /// Create menus for all services in ViewData
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static MvcHtmlString Services(this HtmlHelper html) {
            var tag = new TagBuilder("div");
            tag.AddCssClass(IdHelper.ServicesContainerName);
            var services = (IEnumerable) html.ViewData[IdHelper.NofServices];

            if (services != null && services.Cast<object>().Any()) {
                foreach (object service in services) {
                    tag.InnerHtml += html.Service(service);
                }
                return MvcHtmlString.Create(tag.ToString());
            }
            return MvcHtmlString.Create("");
        }

        /// <summary>
        /// Create menu from actions of service - inserting additional items from menuItems parameter. To emulate generic services 
        /// list wrap in a div with class=IdHelper.ServicesContainerName with any other required services. 
        /// </summary> 
        public static MvcHtmlString Service(this HtmlHelper html, object service, params CustomMenuItem[] menuItems) {
            INakedObject nakedObject = FrameworkHelper.GetNakedObject(service);
            return CommonHtmlHelper.BuildMenuContainer(html.ObjectActions(nakedObject, false, menuItems),
                                                       IdHelper.MenuContainerName,
                                                       IdHelper.GetServiceContainerId(nakedObject),
                                                       nakedObject.TitleString());
        }

        /// <summary>
        /// create menu for service
        /// </summary>
        /// <param name="html"></param>
        /// <param name="service"></param>
        /// <returns></returns>
        public static MvcHtmlString ServiceMenu(this HtmlHelper html, object service) {
            return html.Service(service);
        }

        #endregion
    }
}
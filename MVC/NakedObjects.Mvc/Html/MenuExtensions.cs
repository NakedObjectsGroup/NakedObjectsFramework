// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System.Collections;
using System.Linq;
using System.Web.Mvc;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Menu;
using System.Collections.Generic;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;
using System;
using System.Web.Routing;
using NakedObjects.Architecture.Reflect;

namespace NakedObjects.Web.Mvc.Html {
    public static class MenuExtensions {

        /// <summary>
        ///     Create menu from actions of domainObject
        /// </summary>
        public static MvcHtmlString ObjectMenu(this HtmlHelper html, object domainObject) {
            INakedObject nakedObject = html.Framework().GetNakedObject(domainObject);
            IMenu objectMenu = nakedObject.Spec.ObjectMenu;
            return html.Menu(objectMenu);
        }

        /// <summary>
        /// Create main menus for all menus in ViewData
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static MvcHtmlString MainMenus(this HtmlHelper html) {
            var tag = new TagBuilder("div");
            tag.AddCssClass(IdHelper.ServicesContainerName);
            var menus = (IEnumerable)html.ViewData[IdHelper.NofMainMenus];

            if (menus != null && menus.Cast<IMenu>().Any()) {
                foreach (IMenu menu in menus) {
                    tag.InnerHtml += html.Menu(menu);
                }
                return MvcHtmlString.Create(tag.ToString());
            }
            return MvcHtmlString.Create("");
        }

        public static MvcHtmlString Menu(this HtmlHelper html, IMenu menu) {
            var descriptors = new List<ElementDescriptor>();
            foreach (IMenuItem item in menu.MenuItems) {
                ElementDescriptor descriptor = MenuItemAsElementDescriptor(html, item);
                descriptors.Add(descriptor);
            }
            string menuName = menu.Name;
            return CommonHtmlHelper.BuildMenuContainer(descriptors,
                                                       IdHelper.MenuContainerName + " " + menuName,
                                                       null,
                                                       menuName);
        }


        private static ElementDescriptor MenuItemAsElementDescriptor(this HtmlHelper html, IMenuItem item) {
            ElementDescriptor descriptor = null;
            if (item is IMenuAction) {
                descriptor = MenuActionAsElementDescriptor(html, item as IMenuAction);
            } else if (item is IMenu) {
                descriptor = SubMenuAsElementDescriptor(html, item as IMenu);
            } else if (item is CustomMenuItem) {
                descriptor = CustomMenuItemAsDescriptor(html, item as CustomMenuItem);
            }
            return descriptor;
        }

        private static ElementDescriptor MenuActionAsElementDescriptor(this HtmlHelper html, IMenuAction menuAction) {

            IActionSpecImmutable actionIm = menuAction.Action;
            IActionSpec actionSpec = html.Framework().Metamodel.GetActionSpec(actionIm);
            IObjectSpecImmutable objectIm = actionIm.Specification; //This is the spec for the service
            IObjectSpec serviceSpec = html.Framework().Metamodel.GetSpecification(objectIm);
            //TODO: Add method to IServicesManager to get a service by its IObjectSpec (or IObjectSpecImmutable)
            INakedObject service = html.Framework().Services.GetServices().Single(s => s.Spec == serviceSpec);

            var actionContext = new ActionContext(false, service, actionSpec);

            RouteValueDictionary attributes;
            string tagType;
            string value;

            IConsent consent = actionContext.Action.IsUsable(actionContext.Target);
            if (consent.IsVetoed) {
                tagType = html.GetVetoedAction(actionContext, consent, out value, out attributes);
            } else {
                tagType = html.GetActionAsForm(actionContext, html.Framework().GetObjectTypeName(actionContext.Target.Object), new { id = html.Framework().GetObjectId(actionContext.Target) }, out value, out attributes);
            }

            return new ElementDescriptor {
                TagType = tagType,
                Value = value,
                Attributes = attributes
            };
        }

        private static ElementDescriptor SubMenuAsElementDescriptor(this HtmlHelper html, IMenu subMenu) {
            string tagType = "div";
            string value = CommonHtmlHelper.WrapInDiv(subMenu.Name, IdHelper.MenuNameLabel).ToString();
            RouteValueDictionary attributes = new RouteValueDictionary(new {
                @class = IdHelper.SubMenuName + " " + subMenu.Name
            });

            return new ElementDescriptor {
                TagType = tagType,
                Value = value,
                Attributes = attributes,
                Children = subMenu.MenuItems.
                    Select(item => html.MenuItemAsElementDescriptor(item)).
                    WrapInCollection("div", new { @class = IdHelper.SubMenuItemsName })
            };
        }

        private static ElementDescriptor CustomMenuItemAsDescriptor(HtmlHelper html, CustomMenuItem customItem) {
            return html.ObjectActionAsElementDescriptor(customItem, false);
        }
    }
}
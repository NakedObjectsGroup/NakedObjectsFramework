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
using NakedObjects.Menu;
using NakedObjects.Resources;

namespace NakedObjects.Web.Mvc.Html {
    public static class MenuExtensions {

        /// <summary>
        ///     Create menu from actions of domainObject
        /// </summary>
        public static MvcHtmlString ObjectMenu(this HtmlHelper html, object domainObject) {
            return html.ObjectMenu(domainObject, false);
        }

        /// <summary>
        ///     Create menu from actions of domainObject
        /// </summary>
        public static MvcHtmlString ObjectMenuOnTransient(this HtmlHelper html, object domainObject) {
            return html.ObjectMenu(domainObject, true);
        }

        private static MvcHtmlString ObjectMenu(this HtmlHelper html, object domainObject, bool isEdit) {
            INakedObject nakedObject = html.Framework().GetNakedObject(domainObject);
            IMenuImmutable objectMenu = nakedObject.Spec.ObjectMenu;

            return html.MenuAsHtml(objectMenu, nakedObject, isEdit);
        }

        /// <summary>
        /// Create main menus for all menus in ViewData
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static MvcHtmlString MainMenus(this HtmlHelper html) {
            var mainMenusFromViewData = (IEnumerable)html.ViewData[IdHelper.NofMainMenus];
            if (mainMenusFromViewData != null && mainMenusFromViewData.Cast<IMenuImmutable>().Any()) {
                return RenderMainMenus(html, mainMenusFromViewData.Cast<IMenuImmutable>());
            } else { //Use the MenuServices to derive the menus
                var services = (IEnumerable)html.ViewData[IdHelper.NofServices];
                var mainMenus = new List<IMenuImmutable>();
                foreach (object service in services.Cast<object>()) {
                    INakedObject nakedObject = html.Framework().GetNakedObject(service);
                    mainMenus.Add(nakedObject.Spec.ObjectMenu);
                }
                return RenderMainMenus(html, mainMenus);
            }
        }

        private static MvcHtmlString RenderMainMenus(HtmlHelper html, IEnumerable<IMenuImmutable> menus) {
            var tag = new TagBuilder("div");
            tag.AddCssClass(IdHelper.ServicesContainerName);
            foreach (IMenuImmutable menu in menus) {
                tag.InnerHtml += MenuAsHtml(html, menu, null, false);
            }
            return MvcHtmlString.Create(tag.ToString());
        }

        private static MvcHtmlString MenuAsHtml(this HtmlHelper html, IMenuImmutable menu, INakedObject nakedObject, bool isEdit) {
            var descriptors = new List<ElementDescriptor>();
            foreach (IMenuItemImmutable item in menu.MenuItems) {
                var descriptor = MenuItemAsElementDescriptor(html, item, nakedObject, isEdit);
               if (IsDuplicateAndIsVisibleActions(html, item, menu.MenuItems, nakedObject)) {
                   //Test that both items are in fact visible
                    //The Id is set just to preseve backwards compatiblity
                    string id = menu.Id;
                    if (id.EndsWith("Actions")) {
                        id = id.Split('-').First()+"-DuplicateAction";
                    }
                    descriptor = new ElementDescriptor {
                        TagType = "div",
                        Value = item.Name,
                        Attributes = new RouteValueDictionary(new {
                            @id = id,
                            @class = IdHelper.ActionName,
                            title = MvcUi.DuplicateAction
                        })
                    };
                } 
                if (descriptor != null) { //Would be null for an invisible action
                    descriptors.Add(descriptor);
                }
            }
            return CommonHtmlHelper.BuildMenuContainer(descriptors,
                                                       IdHelper.MenuContainerName,
                                                       menu.Id,
                                                       menu.Name);
        }

        private static bool IsDuplicateAndIsVisibleActions(
                HtmlHelper html, IMenuItemImmutable item, 
                IList<IMenuItemImmutable> items, INakedObject nakedObject) {
            var itemsOfSameName = items.Where(i => i.Name == item.Name);
            if (itemsOfSameName.Count() == 1) return false;
           return itemsOfSameName.Count(i => MenuActionAsElementDescriptor(html, i as IMenuActionImmutable, nakedObject, false) != null) > 1;
        }



        private static ElementDescriptor MenuItemAsElementDescriptor(this HtmlHelper html, IMenuItemImmutable item, INakedObject nakedObject, bool isEdit) {
            ElementDescriptor descriptor = null;
            if (item is IMenuActionImmutable) {
                descriptor = MenuActionAsElementDescriptor(html, item as IMenuActionImmutable, nakedObject, isEdit);
            } else if (item is IMenu) {
                descriptor = SubMenuAsElementDescriptor(html, item as IMenuImmutable, nakedObject, isEdit);
            } else if (item is CustomMenuItem) {
                descriptor = CustomMenuItemAsDescriptor(html, item as CustomMenuItem);
            }
            return descriptor;
        }

        private static ElementDescriptor MenuActionAsElementDescriptor(this HtmlHelper html, IMenuActionImmutable menuAction, INakedObject nakedObject, bool isEdit) {
            IActionSpecImmutable actionIm = menuAction.Action;
            IActionSpec actionSpec = html.Framework().MetamodelManager.GetActionSpec(actionIm);
            if (nakedObject == null) {

                IObjectSpecImmutable objectIm = actionIm.OwnerSpec; //This is the spec for the service

                if (!objectIm.Service) {
                    throw new Exception("Action is not on a known object or service");
                }
                //TODO: Add method to IServicesManager to get a service by its IObjectSpec (or IObjectSpecImmutable)
                IObjectSpec objectSpec = html.Framework().MetamodelManager.GetSpecification(objectIm);
                nakedObject = html.Framework().ServicesManager.GetServices().Single(s => s.Spec == objectSpec);

            }

            var actionContext = new ActionContext(false, nakedObject, actionSpec);

            RouteValueDictionary attributes;
            string tagType;
            string value;
            if (!actionContext.Action.IsVisible(actionContext.Target)) {
                return null;
            }
            IConsent consent = actionContext.Action.IsUsable(actionContext.Target);
            if (consent.IsVetoed) {
                tagType = html.GetVetoedAction(actionContext, consent, out value, out attributes);
            } else if (isEdit) {
                tagType = html.GetActionAsButton(actionContext, out value, out attributes);
            } else {
                tagType = html.GetActionAsForm(actionContext, html.Framework().GetObjectTypeName(actionContext.Target.Object), new { id = html.Framework().GetObjectId(actionContext.Target) }, out value, out attributes);
            }

            return new ElementDescriptor {
                TagType = tagType,
                Value = value,
                Attributes = attributes
            };
        }

        private static ElementDescriptor SubMenuAsElementDescriptor(
                this HtmlHelper html, IMenuImmutable subMenu, INakedObject nakedObject, bool isEdit) {
            string tagType = "div";
            string value = CommonHtmlHelper.WrapInDiv(subMenu.Name, IdHelper.MenuNameLabel).ToString();
            RouteValueDictionary attributes = new RouteValueDictionary(new {
                @class = IdHelper.SubMenuName,
                @id = subMenu.Id
            });
            var visibleSubMenuItems = subMenu.MenuItems.Select(item => html.MenuItemAsElementDescriptor(item, nakedObject, isEdit));
            if (visibleSubMenuItems.Any(x => x != null)) {
                return new ElementDescriptor {
                    TagType = tagType,
                    Value = value,
                    Attributes = attributes,
                    Children = visibleSubMenuItems.WrapInCollection("div", new { @class = IdHelper.SubMenuItemsName })
                };
            } else {
                return null;
            }
        }

        private static ElementDescriptor CustomMenuItemAsDescriptor(HtmlHelper html, CustomMenuItem customItem) {
            return html.ObjectActionAsElementDescriptor(customItem, false);
        }
    }
}
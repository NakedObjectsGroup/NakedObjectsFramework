// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using Common.Logging.Configuration;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Menu;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;
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
            INakedObjectAdapter nakedObject = html.Framework().GetNakedObject(domainObject);
            IMenuImmutable objectMenu = nakedObject.Spec.Menu;
            return html.MenuAsHtml(objectMenu, nakedObject, isEdit, true);
        }

        /// <summary>
        /// Create main menus for all menus in ViewData
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static MvcHtmlString MainMenus(this HtmlHelper html) {
            MvcHtmlString menus;

            const string key = "NofCachedMenus";
            var session = html.ViewContext.HttpContext.Session;

            var cachedmenus = session == null ? null : session[key] as string;

            if (!string.IsNullOrEmpty(cachedmenus)) {
                return new MvcHtmlString(cachedmenus);
            }

            var mainMenusFromViewData = (IEnumerable) html.ViewData[IdHelper.NofMainMenus];
            var immutableMenus = mainMenusFromViewData != null ? mainMenusFromViewData.Cast<IMenuImmutable>().ToArray() :  new IMenuImmutable[]{};
            if (immutableMenus.Any()) {
                menus = RenderMainMenus(html, immutableMenus);
            }
            else {
                //Use the MenuServices to derive the menus
                var services = (IEnumerable) html.ViewData[IdHelper.NofServices];
                var mainMenus = new List<IMenuImmutable>();
                foreach (object service in services.Cast<object>()) {
                    var menu = GetMenu(html, service);
                    mainMenus.Add(menu);
                }
                menus = RenderMainMenus(html, mainMenus);
            }

            if (session != null) {
                session.Add(key, menus.ToString());
            }

            return menus;
        }

        private static IMenuImmutable GetMenu(HtmlHelper html, object service) {
            return html.Framework().GetNakedObject(service).Spec.Menu;
        }

        public static MvcHtmlString MainMenu(this HtmlHelper html, object service) {
            var menu = GetMenu(html, service);
            return html.MenuAsHtml(menu, null, false, false);
        }

        private static MvcHtmlString RenderMainMenus(this HtmlHelper html, IEnumerable<IMenuImmutable> menus) {
            var tag = new TagBuilder("div");
            tag.AddCssClass(IdHelper.ServicesContainerName);
            AddMainMenusIntoTag(html, menus, tag);
            return MvcHtmlString.Create(tag.ToString());
        }

        public static void AddMainMenusIntoTag(this HtmlHelper html, IEnumerable<IMenuImmutable> menus, TagBuilder tag) {
            foreach (IMenuImmutable menu in menus) {
                tag.InnerHtml += html.MenuAsHtml(menu, null, false, false);
            }
        }

        private static MvcHtmlString MenuAsHtml(this HtmlHelper html, IMenuImmutable menu, INakedObjectAdapter nakedObject, bool isEdit, bool defaultToEmptyMenu) {
            var descriptors = new List<ElementDescriptor>();
            foreach (IMenuItemImmutable item in menu.MenuItems) {
                ElementDescriptor descriptor;

                if (IsDuplicateAndIsVisibleActions(html, item, menu.MenuItems, nakedObject)) {
                    //Test that both items are in fact visible
                    //The Id is set just to preseve backwards compatiblity
                    string id = menu.Id;
                    if (id.EndsWith("Actions")) {
                        id = id.Split('-').First() + "-DuplicateAction";
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
                else {
                    descriptor = MenuItemAsElementDescriptor(html, item, nakedObject, isEdit);
                }

                if (descriptor != null) {
                    //Would be null for an invisible action
                    descriptors.Add(descriptor);
                }
            }
            if (descriptors.Count == 0 && !defaultToEmptyMenu) {
                return null;
            }
            return CommonHtmlHelper.BuildMenuContainer(descriptors,
                IdHelper.MenuContainerName,
                menu.Id,
                menu.Name);
        }

        private static bool IsDuplicateAndIsVisibleActions(
            HtmlHelper html, IMenuItemImmutable item,
            IList<IMenuItemImmutable> items, INakedObjectAdapter nakedObject) {
            var itemsOfSameName = items.Where(i => i.Name == item.Name);
            if (itemsOfSameName.Count() == 1) return false;
            return itemsOfSameName.Count(i => MenuActionAsElementDescriptor(html, i as IMenuActionImmutable, nakedObject, false) != null) > 1;
        }

        private static ElementDescriptor MenuItemAsElementDescriptor(this HtmlHelper html, IMenuItemImmutable item, INakedObjectAdapter nakedObject, bool isEdit) {
            ElementDescriptor descriptor = null;
            if (item is IMenuActionImmutable) {
                descriptor = MenuActionAsElementDescriptor(html, item as IMenuActionImmutable, nakedObject, isEdit);
            }
            else if (item is IMenu) {
                descriptor = SubMenuAsElementDescriptor(html, item as IMenuImmutable, nakedObject, isEdit);
            }
            #pragma warning disable 612,618
             else if (item is CustomMenuItem) {
                            descriptor = CustomMenuItemAsDescriptor(html, item as CustomMenuItem);
                        }
            #pragma warning restore 612,618
            return descriptor;
        }

        private static ElementDescriptor MenuActionAsElementDescriptor(this HtmlHelper html, IMenuActionImmutable menuAction, INakedObjectAdapter nakedObject, bool isEdit) {
            IActionSpecImmutable actionIm = menuAction.Action;
            IActionSpec actionSpec = html.Framework().MetamodelManager.GetActionSpec(actionIm);
            if (nakedObject == null) {
                var serviceIm = actionIm.OwnerSpec as IServiceSpecImmutable;

                if (serviceIm == null) {
                    throw new Exception("Action is not on a known service");
                }
                ITypeSpec objectSpec = html.Framework().MetamodelManager.GetSpecification(serviceIm);
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
            }
            else if (isEdit) {
                tagType = html.GetActionAsButton(actionContext, out value, out attributes);
            }
            else {
                tagType = html.GetActionAsForm(actionContext, html.Framework().GetObjectTypeName(actionContext.Target.Object), new {id = html.Framework().GetObjectId(actionContext.Target)}, out value, out attributes);
            }

            return new ElementDescriptor {
                TagType = tagType,
                Value = value,
                Attributes = attributes
            };
        }

        private static ElementDescriptor SubMenuAsElementDescriptor(
            this HtmlHelper html, IMenuImmutable subMenu, INakedObjectAdapter nakedObject, bool isEdit) {
            string tagType = "div";
            string value = CommonHtmlHelper.WrapInDiv(subMenu.Name, IdHelper.MenuNameLabel).ToString();
            RouteValueDictionary attributes = new RouteValueDictionary(new {
                @class = IdHelper.SubMenuName,
                @id = subMenu.Id
            });
            var visibleSubMenuItems = subMenu.MenuItems.Select(item => html.MenuItemAsElementDescriptor(item, nakedObject, isEdit)).Where(x => x != null);
            if (visibleSubMenuItems.Any()) {
                return new ElementDescriptor {
                    TagType = tagType,
                    Value = value,
                    Attributes = attributes,
                    Children = visibleSubMenuItems.WrapInCollection("div", new {@class = IdHelper.SubMenuItemsName})
                };
            }
            else {
                return null;
            }
        }

        [Obsolete]
        private static ElementDescriptor CustomMenuItemAsDescriptor(HtmlHelper html, CustomMenuItem customItem) {
            return html.ObjectActionAsElementDescriptor(customItem, false);
        }

        internal static MvcHtmlString MenuOnQueryable(this HtmlHelper html, object domainObject) {
            INakedObjectAdapter nakedObject = html.Framework().GetNakedObject(domainObject);
            if (!nakedObject.Spec.IsQueryable) {
                throw new ArgumentException(String.Format("{0} is not a Queryable", nakedObject.Spec));
            }
            return CommonHtmlHelper.BuildMenuContainer(html.CollectionContributedActions(nakedObject, true),
                IdHelper.MenuContainerName,
                IdHelper.GetActionContainerId(nakedObject),
                IdHelper.GetActionLabel(nakedObject));
        }
    }
}
// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using NakedObjects.Resources;
using NakedObjects.Surface;
using NakedObjects.Surface.Context;
using NakedObjects.Surface.Utility;
using NakedObjects.Surface.Utility.Restricted;
using NakedObjects.Util;
using NakedObjects.Web.Mvc.Models;

namespace NakedObjects.Web.Mvc.Html {
    public class CustomMenuItem : IMenuItem {
        public string Controller { get; set; }
        public string Action { get; set; }
        public object RouteValues { get; set; }
        public int MemberOrder { get; set; }

        #region IMenuItem Members

        public string Name { get; set; }
        public string Id { get; set; }
        public object Wrapped { get; private set; }

        #endregion
    }

    internal static class CommonHtmlHelper {
        public static INakedObjectsSurface Surface(this HtmlHelper html) {
            return (INakedObjectsSurface) html.ViewData[IdConstants.NoSurface];
        }

        public static IIdHelper IdHelper(this HtmlHelper html) {
            return (IIdHelper) html.ViewData[IdConstants.IdHelper];
        }

        public static string GetObjectType(Type type) {
            return type.GetProxiedTypeFullName().Replace('.', '-');
        }

        public static string GetObjectType(object model) {
            return model == null ? String.Empty : GetObjectType(model.GetType());
        }

        #region internal api

        internal static MvcHtmlString PropertyListWithFilter(this HtmlHelper html, object domainObject, Func<INakedObjectAssociationSurface, bool> filter, Func<INakedObjectAssociationSurface, int> order) {
            var nakedObject = html.Surface().GetObject(domainObject);
            bool anyEditableFields;
            IEnumerable<ElementDescriptor> viewObjectFields = html.ViewObjectFields(nakedObject, null, filter, order, out anyEditableFields);
            return html.BuildViewContainer(nakedObject,
                viewObjectFields,
                IdConstants.FieldContainerName,
                html.IdHelper().GetFieldContainerId(nakedObject),
                anyEditableFields);
        }

        internal static MvcHtmlString PropertyListEditWithFilter(this HtmlHelper html, object domainObject, Func<INakedObjectAssociationSurface, bool> filter, Func<INakedObjectAssociationSurface, int> order) {
            var nakedObject = html.Surface().GetObject(domainObject);
            return html.BuildEditContainer(nakedObject,
                html.EditObjectFields(nakedObject, null, filter, order),
                IdConstants.FieldContainerName,
                html.IdHelper().GetFieldContainerId(nakedObject));
        }

        internal static MvcHtmlString Scalar(this HtmlHelper html, object scalar) {
            var nakedObject = html.Surface().GetObject(scalar);
            return new MvcHtmlString(nakedObject.TitleString);
        }

        internal static string UserMessages(string[] items, string cls) {
            if (items.Any()) {
                var divTag = new TagBuilder("div");
                var ulTag = new TagBuilder("ul");
                divTag.AddCssClass(cls);
                foreach (string item in items) {
                    var liTag = new TagBuilder("li");
                    liTag.SetInnerText(item);
                    ulTag.InnerHtml += liTag + Environment.NewLine;
                }
                divTag.InnerHtml += ulTag;
                return divTag + Environment.NewLine;
            }
            return String.Empty;
        }

        internal static string ObjectIconAndLink(this HtmlHelper html, string linkText, string actionName, object model, bool withTitleAttr = false) {
            var nakedObject = html.Surface().GetObject(model);
            return html.ObjectIcon(nakedObject) + html.ObjectLink(linkText, actionName, model, withTitleAttr);
        }

        internal static string ObjectIconAndDetailsLink(this HtmlHelper html, string linkText, string actionName, object model) {
            var nakedObject = html.Surface().GetObject(model);
            return html.ObjectIcon(nakedObject) + html.ObjectTitle(model) + html.ObjectLink(MvcUi.Details, actionName, model);
        }

        internal static string ActionResultLink(this HtmlHelper html, string linkText, string actionName, ActionResultModel arm, object titleAttr) {
            var no = html.Surface().GetObject(arm.Result);
            string id = html.Surface().OidStrategy.GetObjectId(no);
            int pageSize = arm.PageSize;
            int page = arm.Page;
            string format = arm.Format;

            string url = html.GenerateUrl(actionName, html.Surface().GetObjectTypeShortName(arm.Result), new RouteValueDictionary(new {id, pageSize, page, format}));

            var linkTag = new TagBuilder("a");
            linkTag.MergeAttribute("href", url);
            linkTag.SetInnerText(linkText);

            if (titleAttr != null) {
                linkTag.MergeAttributes(new RouteValueDictionary(titleAttr));
            }

            return linkTag.ToString();
        }

        internal static string ObjectLink(this HtmlHelper html, string linkText, string actionName, object domainObject, bool withTitleAttr = false) {
            var titleAttr = withTitleAttr && (domainObject != null) ? new {title = html.ObjectTitle(domainObject)} : null;

            var actionResultModel = domainObject as ActionResultModel;
            if (actionResultModel != null) {
                return html.ActionResultLink(linkText, actionName, actionResultModel, titleAttr);
            }

            string controllerName = html.Surface().GetObjectTypeShortName(domainObject);
            return html.ActionLink(linkText, actionName, controllerName, new {id = html.GetObjectId(domainObject)}, titleAttr).ToString();
        }

        internal static string CollectionLink(this HtmlHelper html, string linkText, string actionName, object domainObject) {
            var no = html.Surface().GetObject(domainObject);
            var data = new RouteValueDictionary(new {id = html.Surface().OidStrategy.GetObjectId(no)});
            UpdatePagingValues(html, data);
            return GetSubmitButton(null, linkText, actionName, data);
        }

        internal static MvcHtmlString ObjectButton(this HtmlHelper html, string linkText, string actionName, string classAttribute, object domainObject) {
            string controllerName = html.Surface().GetObjectTypeShortName(domainObject);
            return html.ObjectActionAsString(linkText, actionName, controllerName, classAttribute, "", new RouteValueDictionary(new {id = html.Surface().OidStrategy.GetObjectId(html.Surface().GetObject(domainObject))}));
        }

        internal static MvcHtmlString EditObjectButton(this HtmlHelper html, string linkText, string actionName, object domainObject) {
            string controllerName = html.Surface().GetObjectTypeShortName(domainObject);
            return html.TransientObjectActionAsString(linkText, actionName, controllerName, new RouteValueDictionary(new {id = html.Surface().OidStrategy.GetObjectId(html.Surface().GetObject(domainObject))}));
        }

        internal static string ObjectIcon(this HtmlHelper html, INakedObjectSurface nakedObject) {
            if (nakedObject == null || nakedObject.Specification.IsService) {
                // no icons for services 
                return String.Empty;
            }
            html.ViewContext.HttpContext.Session.AddToCache(html.Surface(), nakedObject);

            var url = new UrlHelper(html.ViewContext.RequestContext);
            var tag = new TagBuilder("img");
            tag.MergeAttribute("src", url.Content("~/Images/" + SurfaceHelper.IconName(nakedObject)));
            tag.MergeAttribute("alt", nakedObject.Specification.SingularName);
            return tag.ToString(TagRenderMode.SelfClosing);
        }

        internal static MvcHtmlString BuildEditContainer(this HtmlHelper html, INakedObjectSurface nakedObject, IEnumerable<ElementDescriptor> elements, string cls, string id) {
            TagBuilder fieldSet = AddClassAndIdToElementSet(elements, cls, id);

            AddAjaxDataUrlsToElementSet(html, nakedObject, fieldSet);

            fieldSet.InnerHtml += html.Hidden(html.IdHelper().GetDisplayFormatId(id), ToNameValuePairs(html.GetDisplayStatuses()));
            fieldSet.InnerHtml += nakedObject.IsViewModelEditView ? "" : GetSubmitButton(IdConstants.SaveButtonClass, MvcUi.Save, String.Empty, new RouteValueDictionary());
            fieldSet.InnerHtml += nakedObject.IsTransient ? GetSubmitButton(IdConstants.SaveCloseButtonClass, MvcUi.SaveAndClose, IdConstants.SaveAndCloseAction, new RouteValueDictionary(new {close = true})) : "";

            return MvcHtmlString.Create(fieldSet.ToString());
        }

        private static void AddAjaxDataUrlsToElementSet(this HtmlHelper html, INakedObjectSurface nakedObject, TagBuilder fieldSet, PropertyContext parent = null) {
            var parameters = new HashSet<string>(nakedObject.Specification.Properties.SelectMany(p => p.GetChoicesParameters()).Select(t => t.Item1));

            // check the names match 

            var properties = nakedObject.Specification.Properties;
            IEnumerable<string> matches = from p in parameters
                from pp in properties
                where p.ToLower() == pp.Id.ToLower()
                select p;

            if (matches.Count() != parameters.Count) {
                string error = String.Format("On choices method in: {0} one or more properties in: '{1}' does not match a property on that class", nakedObject.Specification.FullName, parameters.Aggregate("", (s, t) => s + " " + t));
                throw new ArgumentException(error);
            }

            string parameterNames = parameters.Aggregate("", (s, t) => (s == "" ? "" : s + ",") + new PropertyContext(html.IdHelper(), nakedObject, nakedObject.Specification.Properties.Single(p => p.Id.ToLower() == t.ToLower()), false, parent).GetFieldInputId());

            string url = html.GenerateUrl("GetPropertyChoices", "Ajax", new RouteValueDictionary(new {id = html.Surface().OidStrategy.GetObjectId(nakedObject)}));
            fieldSet.MergeAttribute("data-choices", url);
            fieldSet.MergeAttribute("data-choices-parameters", parameterNames);
        }

        private static void AddAjaxDataUrlsToElementSet(this HtmlHelper html, INakedObjectSurface nakedObject, INakedObjectActionSurface action, TagBuilder fieldSet) {
            var parameters = new HashSet<string>(action.Parameters.SelectMany(p => p.GetChoicesParameters()).Select(t => t.Item1));
            // check the names match 

            IEnumerable<string> matches = from p in parameters
                from pp in action.Parameters
                where p.ToLower() == pp.Id.ToLower()
                select p;

            if (matches.Count() != parameters.Count) {
                string error = String.Format("On choices method Choices{0} one or more parameters in: '{1}' does not match a parameter on : {0}", action.Id, parameters.Aggregate("", (s, t) => s + " " + t));
                throw new ArgumentException(error);
            }

            string parameterNames = parameters.Aggregate("", (s, t) => (s == "" ? "" : s + ",") + html.IdHelper().GetParameterInputId(action, action.Parameters.Single(p => p.Id.ToLower() == t.ToLower())));

            var url = html.GenerateUrl("GetActionChoices", "Ajax", new RouteValueDictionary(new {id = html.Surface().OidStrategy.GetObjectId(nakedObject), actionName = action.Id}));
            fieldSet.MergeAttribute("data-choices", url);
            fieldSet.MergeAttribute("data-choices-parameters", parameterNames);
        }

        internal static MvcHtmlString BuildParamContainer(this HtmlHelper html, ActionContext actionContext, IEnumerable<ElementDescriptor> elements, string cls, string id) {
            if (actionContext.Action.IsQueryOnly) {
                cls += (" " + IdConstants.QueryOnlyClass);
            }
            else if (actionContext.Action.IsIdempotent) {
                cls += (" " + IdConstants.IdempotentClass);
            }

            TagBuilder fieldSet = AddClassAndIdToElementSet(elements, cls, id);

            AddAjaxDataUrlsToElementSet(html, actionContext.Target, actionContext.Action, fieldSet);

            var data = new RouteValueDictionary();
            UpdatePagingValues(html, data);

            fieldSet.InnerHtml += GetSubmitButton(IdConstants.OkButtonClass, MvcUi.OK, "None", data);
            return MvcHtmlString.Create(fieldSet.ToString());
        }

        private static void UpdatePagingValues(HtmlHelper html, RouteValueDictionary data) {
            int pageSize, maxPage, currentPage, total;
            if (html.GetPagingValues(out pageSize, out maxPage, out currentPage, out total)) {
                string displayType = html.ViewData.ContainsKey(IdConstants.CollectionFormat) ? (string) html.ViewData[IdConstants.CollectionFormat] : IdConstants.ListDisplayFormat;
                data.Add(IdConstants.PageKey, currentPage);
                data.Add(IdConstants.PageSizeKey, pageSize);
                data.Add(IdConstants.CollectionFormat, displayType);
            }
        }

        internal static MvcHtmlString BuildViewContainer(this HtmlHelper html, INakedObjectSurface nakedObject, IEnumerable<ElementDescriptor> elements, string cls, string id, bool anyEditableFields) {
            TagBuilder fieldSet = AddClassAndIdToElementSet(elements, cls, id);

            if (nakedObject.IsNotPersistent) {
                fieldSet.InnerHtml += ElementDescriptor.BuildElementSet(html.EditObjectFields(nakedObject, null, x => false, null));

                if (anyEditableFields && !nakedObject.Specification.IsAlwaysImmutable) {
                    fieldSet.InnerHtml += GetSubmitButton(IdConstants.EditButtonClass, MvcUi.Edit, String.Empty, new RouteValueDictionary());
                }
            }
            else {
                fieldSet.InnerHtml += html.GetEditButtonIfRequired(anyEditableFields, nakedObject);
            }

            return MvcHtmlString.Create(fieldSet.ToString());
        }

        internal static MvcHtmlString BuildMenuContainer(IList<ElementDescriptor> elements, string cls, string id, string label) {
            var menuSet = new TagBuilder("div");
            menuSet.AddCssClass(cls);
            menuSet.GenerateId(id);

            if (!elements.Any()) {
                menuSet.MergeAttribute("title", MvcUi.NoActionsAvailable);
            }

            menuSet.InnerHtml += WrapInDiv(label, IdConstants.MenuNameLabel);

            TagBuilder fieldSet = ElementDescriptor.BuildElementSet(elements);
            fieldSet.AddCssClass(IdConstants.ActionListName);

            menuSet.InnerHtml += fieldSet;

            return MvcHtmlString.Create(menuSet.ToString());
        }

        internal static MvcHtmlString GetStandaloneCollection(this HtmlHelper html,
                                                              INakedObjectSurface collectionNakedObject,
                                                              Func<INakedObjectAssociationSurface, bool> filter,
                                                              Func<INakedObjectAssociationSurface, int> order,
                                                              bool withTitle) {
            var tag = new TagBuilder("div");
            tag.AddCssClass(IdConstants.CollectionTableName);
            return GetStandalone(html, collectionNakedObject, filter, order, tag, withTitle);
        }

        private static MvcHtmlString GetStandalone(HtmlHelper html, INakedObjectSurface collectionNakedObject, Func<INakedObjectAssociationSurface, bool> filter, Func<INakedObjectAssociationSurface, int> order, TagBuilder tag, bool withTitle) {
            Func<INakedObjectSurface, string> linkFunc = item => html.Object(html.ObjectTitle(item).ToString(), IdConstants.ViewAction, item.Object).ToString();

            string menu = collectionNakedObject.Specification.IsQueryable ? html.MenuOnTransient(collectionNakedObject.Object).ToString() : "";
            string id = collectionNakedObject.Oid == null ? "" : html.Surface().OidStrategy.GetObjectId(collectionNakedObject);

            // can only be standalone and hence page if we have an id 
            tag.InnerHtml += html.CollectionTable(collectionNakedObject, linkFunc, filter, order, !String.IsNullOrEmpty(id), collectionNakedObject.Specification.IsQueryable, withTitle);

            return html.WrapInForm(IdConstants.EditObjectAction,
                html.Surface().GetObjectTypeShortName(collectionNakedObject.Object),
                menu + tag,
                IdConstants.ActionName,
                new RouteValueDictionary(new {id}));
        }

        internal static MvcHtmlString GetStandaloneList(this HtmlHelper html,
                                                        INakedObjectSurface collectionNakedObject,
                                                        Func<INakedObjectAssociationSurface, int> order) {
            var tag = new TagBuilder("div");
            tag.AddCssClass(IdConstants.CollectionListName);
            return GetStandalone(html, collectionNakedObject, x => false, order, tag, true);
        }

        internal static IEnumerable<ElementDescriptor> ActionParameterFields(this HtmlHelper html,
                                                                             ActionContext actionContext,
                                                                             IList<ElementDescriptor> childElements = null,
                                                                             string propertyName = null) {
            IEnumerable<ElementDescriptor> concurrencyElements = html.GetConcurrencyElements(actionContext.Target, actionContext.GetConcurrencyActionInputId);
            IEnumerable<ElementDescriptor> collectionFilterElements = html.GetCollectionSelectedElements(actionContext.Target);

            return (from parameterContext in actionContext.GetParameterContexts(html.Surface())
                let parmValue = html.GetParameter(parameterContext, childElements, propertyName)
                select new ElementDescriptor {
                    TagType = "div",
                    Label = html.GetLabel(parameterContext),
                    Value = parmValue,
                    Attributes = new RouteValueDictionary(new {
                        id = parameterContext.GetParameterId(),
                        @class = parameterContext.GetParameterClass()
                    })
                }).Union(concurrencyElements).Union(collectionFilterElements);
        }

        internal static ElementDescriptor EditObjectField(this HtmlHelper html,
                                                          PropertyContext propertyContext,
                                                          bool noFinder = false,
                                                          IList<ElementDescriptor> childElements = null,
                                                          string idToAddTo = null) {
            string editValue = html.GetEditValue(propertyContext, childElements, propertyContext.Property.Id == idToAddTo, noFinder);

            return new ElementDescriptor {
                TagType = "div",
                Label = html.GetLabel(propertyContext),
                Value = editValue,
                Attributes = new RouteValueDictionary(new {
                    id = propertyContext.GetFieldId(),
                    @class = propertyContext.GetFieldClass()
                })
            };
        }

        private static IEnumerable<Tuple<INakedObjectAssociationSurface, INakedObjectSurface>> Items(this INakedObjectAssociationSurface assoc, HtmlHelper html, INakedObjectSurface target) {
            return assoc.GetNakedObject(target).ToEnumerable().Select(no => new Tuple<INakedObjectAssociationSurface, INakedObjectSurface>(assoc, no));
        }

        internal static IEnumerable<ElementDescriptor> EditObjectFields(this HtmlHelper html,
                                                                        INakedObjectSurface nakedObject,
                                                                        PropertyContext parentContext,
                                                                        Func<INakedObjectAssociationSurface, bool> filter,
                                                                        Func<INakedObjectAssociationSurface, int> order,
                                                                        bool noFinder = false,
                                                                        IList<ElementDescriptor> childElements = null,
                                                                        string idToAddTo = null) {
            var query = nakedObject.Specification.Properties.Where(p => p.IsVisible(nakedObject)).Where(filter);

            if (order != null) {
                query = query.OrderBy(order);
            }

            var visibleFields = query.ToList();

            IEnumerable<ElementDescriptor> visibleElements = visibleFields.Select(property => html.EditObjectField(new PropertyContext(html.IdHelper(), nakedObject, property, true, parentContext), noFinder, childElements, idToAddTo));

            if (nakedObject.IsTransient) {
                IEnumerable<ElementDescriptor> hiddenElements = nakedObject.Specification.Properties.Where(p => !p.IsVisible(nakedObject)).
                    Select(property => new ElementDescriptor {
                        TagType = "div",
                        Value = html.GetEditValue(new PropertyContext(html.IdHelper(), nakedObject, property, true, parentContext), childElements, property.Id == idToAddTo, noFinder),
                    });

                visibleElements = visibleElements.Union(hiddenElements);

                IEnumerable<ElementDescriptor> collectionElements = nakedObject.Specification.Properties.Where(p => p.IsCollection).
                    SelectMany(p => p.Items(html, nakedObject)).
                    Select(t => new ElementDescriptor {
                        TagType = "div",
                        Value = html.GetCollectionItem(t.Item2, html.IdHelper().GetCollectionItemId((nakedObject), (t.Item1)))
                    });

                visibleElements = visibleElements.Union(collectionElements);
            }

            // add filtered fields as hidden to preserve their values 

            var filteredFields = nakedObject.Specification.Properties.Where(p => !p.IsCollection && p.IsVisible(nakedObject)).Except(visibleFields);
            IEnumerable<ElementDescriptor> filteredElements = filteredFields.Select(property => new PropertyContext(html.IdHelper(), nakedObject, property, false, parentContext)).Select(pc => new ElementDescriptor {
                TagType = "div",
                Value = html.GetHiddenValue(pc, pc.GetFieldInputId(), false)
            });
            IEnumerable<ElementDescriptor> elements = visibleElements.Union(filteredElements);

            if (!nakedObject.IsTransient) {
                // if change existing object add concurrency check fields as hidden  

                IEnumerable<ElementDescriptor> concurrencyElements = html.GetConcurrencyElements(nakedObject, x => new PropertyContext(html.IdHelper(), nakedObject, x, false, parentContext).GetConcurrencyFieldInputId());
                elements = elements.Union(concurrencyElements);
            }

            return elements;
        }

        private static IEnumerable<ElementDescriptor> GetConcurrencyElements(this HtmlHelper html, INakedObjectSurface nakedObject, Func<INakedObjectAssociationSurface, string> idFunc) {
            var objectSpec = nakedObject.Specification;

            var concurrencyFields = objectSpec == null ? new INakedObjectAssociationSurface[] {} : objectSpec.Properties.Where(p => p.IsConcurrency);
            return concurrencyFields.Select(property => new ElementDescriptor {
                TagType = "div",
                Value = html.GetHiddenValue(new PropertyContext(html.IdHelper(), nakedObject, property, false), idFunc(property), true)
            });
        }

        private static IEnumerable<ElementDescriptor> GetCollectionSelectedElements(this HtmlHelper html, INakedObjectSurface nakedObject) {
            if (nakedObject.IsCollectionMemento) {
                var selectedObjects = nakedObject.GetSelected();
                var selectedObjectIds = selectedObjects.Select(o => html.Surface().GetObject(o)).Select(no => html.Surface().OidStrategy.GetObjectId(no)).ToArray();
                int index = 0;
                return selectedObjectIds.Select(id => new ElementDescriptor {
                    TagType = "input",
                    Attributes = new RouteValueDictionary(new {
                        type = "hidden",
                        name = id,
                        value = "true",
                        @class = IdConstants.CheckboxClass,
                        id = IdConstants.Checkbox + index++
                    }),
                });
            }

            return new ElementDescriptor[] {};
        }

        internal static IEnumerable<ElementDescriptor> EditObjectFields(this HtmlHelper html, object contextObject, ActionContext targetActionContext, string propertyName, IEnumerable actionResult, bool all) {
            var contextNakedObject = html.Surface().GetObject(contextObject);
            var actionContext = new ActionContext(html.IdHelper(), false, contextNakedObject, null);
            List<ElementDescriptor> childElements = html.GetChildElements(actionResult, targetActionContext, actionContext, propertyName, x => html.Surface().GetObject(x).IsTransient);
            return html.EditObjectFields(contextNakedObject, null, x => all || x.Id == propertyName, null, false, childElements, propertyName);
        }

        internal static IEnumerable<ElementDescriptor> ActionParameterFields(this HtmlHelper html, ActionContext actionContext, ActionContext targetActionContext, string propertyName, IEnumerable actionResult) {
            List<ElementDescriptor> childElements = html.GetChildElements(actionResult, targetActionContext, actionContext, propertyName, x => (html.Surface().GetObject(x).IsTransient && !html.Surface().GetObject(x).Specification.IsCollection));
            return html.ActionParameterFields(actionContext, childElements, propertyName);
        }

        private static List<ElementDescriptor> GetChildElements(this HtmlHelper html, IEnumerable actionResult, ActionContext targetActionContext, ActionContext actionContext, string propertyName, Func<object, bool> actionResultFilter) {
            List<ElementDescriptor> childElements;

            if (actionResult == null) {
                List<ElementDescriptor> paramElements = html.ActionParameterFields(targetActionContext).ToList();
                childElements = html.GetActionDialog(targetActionContext, actionContext, paramElements, propertyName).InList();
            }
            else {
                List<object> result = actionResult.Cast<object>().ToList();
                if (result.Count() == 1 && actionResultFilter(result.First())) {
                    childElements = html.GetSubEditObject(targetActionContext, actionContext, result.First(), propertyName).InList();
                }
                else {
                    childElements = html.SelectionView(actionContext.Target.Object, propertyName, actionResult).InList();
                }
            }
            return childElements;
        }

        internal static MvcHtmlString ObjectActionAsString(this HtmlHelper html, string linkText, string actionName, string controllerName, string classAttribute) {
            return html.ObjectActionAsString(linkText, actionName, controllerName, classAttribute, "", new RouteValueDictionary());
        }

        internal static MvcHtmlString ObjectActionAsString(this HtmlHelper html, string linkText, string actionName, string controllerName, string classAttribute, string name, RouteValueDictionary routeValues) {
            string innerHtml = html.Hidden(html.IdHelper().GetDisplayFormatId(actionName), ToNameValuePairs(GetDisplayStatuses(html))) +
                               GetSubmitButton(classAttribute, linkText, name, routeValues);

            return html.WrapInForm(actionName, controllerName, innerHtml, IdConstants.ActionName, routeValues);
        }

        internal static MvcHtmlString TransientObjectActionAsString(this HtmlHelper html, string linkText, string actionName, string controllerName) {
            return html.TransientObjectActionAsString(linkText, actionName, controllerName, new RouteValueDictionary());
        }

        internal static MvcHtmlString TransientObjectActionAsString(this HtmlHelper html, string linkText, string actionName, string controllerName, RouteValueDictionary routeValues) {
            return MvcHtmlString.Create(GetSubmitButton(null, linkText, String.Empty, routeValues));
        }

        internal static ElementDescriptor ActionFormAsElementDescriptor(this HtmlHelper html, string linkText, string actionName, string id, string controllerName) {
            return html.ActionFormAsElementDescriptor(linkText, actionName, controllerName, id, new RouteValueDictionary());
        }

        internal static ElementDescriptor ActionFormAsElementDescriptor(this HtmlHelper html, string linkText, string actionName, string controllerName, string id, RouteValueDictionary routeValues) {
            string innerHtml = GetSubmitButton(null, linkText, String.Empty, routeValues);
            return html.WrapInFormElementDescriptor(actionName, controllerName, innerHtml, id, routeValues);
        }

        internal static ElementDescriptor ActionMenuAsElementDescriptor(this HtmlHelper html, string name, string id, IEnumerable<ElementDescriptor> children) {
            return new ElementDescriptor {
                TagType = "div",
                Value = WrapInDiv(name, IdConstants.MenuNameLabel).ToString(),
                Attributes = new RouteValueDictionary(new {
                    id,
                    @class = IdConstants.SubMenuName
                }),
                Children = children.WrapInCollection("div", new {@class = IdConstants.SubMenuItemsName})
            };
        }

        internal static MvcHtmlString WrapInForm(this HtmlHelper html, string actionName, string controllerName, string innerHtml, string cls, RouteValueDictionary routeValues) {
            var formTag = new TagBuilder("form");
            formTag.AddCssClass(cls);
            formTag.MergeAttribute("method", "post");
            formTag.MergeAttribute("action", html.GenerateUrl(actionName, controllerName, routeValues));
            formTag.InnerHtml += innerHtml.WrapInDivTag();
            return MvcHtmlString.Create(formTag.ToString());
        }

        internal static string WrapInDivTag(this string innerHtml) {
            var tag = new TagBuilder("div");
            tag.InnerHtml += innerHtml;
            return tag.ToString();
        }

        internal static MvcHtmlString WrapInDiv(string innerText, string cls) {
            var tag = new TagBuilder("div");
            tag.AddCssClass(cls);
            tag.InnerHtml += innerText;
            return MvcHtmlString.Create(tag.ToString());
        }

        internal static ElementDescriptor WrapInFormElementDescriptor(this HtmlHelper html, string actionName, string controllerName, string innerHtml, string id, RouteValueDictionary routeValues) {
            return new ElementDescriptor {
                TagType = "form",
                Attributes = new RouteValueDictionary(new {
                    id,
                    @class = IdConstants.ActionName,
                    method = "post",
                    action = html.GenerateUrl(actionName, controllerName, routeValues),
                }),
                Value = innerHtml.WrapInDivTag()
            };
        }

        internal static ElementDescriptor ObjectActionAsElementDescriptor(this HtmlHelper html,
                                                                          CustomMenuItem menuItem,
                                                                          bool isEdit) {
            string controllerName = menuItem.Controller;
            string actionName = menuItem.Action;
            string actionLabel = menuItem.Name ?? menuItem.Action;
            object routeValues = menuItem.RouteValues;

            return new ElementDescriptor {
                TagType = "form",
                Value = GetSubmitButton(null, actionLabel, IdConstants.ActionInvokeAction, new RouteValueDictionary(new {action = "action"})).WrapInDivTag(),
                Attributes = new RouteValueDictionary(new {
                    action = html.GenerateUrl(Action(actionName), controllerName, new RouteValueDictionary(routeValues)),
                    method = "post",
                    id = html.IdHelper().MakeId(controllerName, actionName),
                    @class = IdConstants.ActionName
                })
            };
        }

        internal static string GetVetoedAction(this HtmlHelper html, ActionContext actionContext, IConsentSurface consent, out string value, out RouteValueDictionary attributes) {
            value = actionContext.Action.Name;
            attributes = new RouteValueDictionary(new {
                id = actionContext.GetActionId(),
                @class = actionContext.GetActionClass(),
                title = consent.Reason
            });
            return "div";
        }

        internal static string GetDuplicateAction(this HtmlHelper html, ActionContext actionContext, string reason, out string value, out RouteValueDictionary attributes) {
            value = actionContext.Action.Name;
            attributes = new RouteValueDictionary(new {
                id = actionContext.GetActionId(),
                @class = actionContext.GetActionClass(),
                title = reason
            });
            return "div";
        }

        internal static ElementDescriptor ObjectActionAsElementDescriptor(this HtmlHelper html,
                                                                          ActionContext actionContext,
                                                                          object routeValues,
                                                                          bool isEdit,
                                                                          Tuple<bool, string> disabled = null) {
            RouteValueDictionary attributes;
            string tagType;
            string value;

            var consent = actionContext.Action.IsUsable(actionContext.Target);
            if (consent.IsVetoed) {
                tagType = html.GetVetoedAction(actionContext, consent, out value, out attributes);
            }
            else if (disabled != null && disabled.Item1) {
                tagType = html.GetDuplicateAction(actionContext, disabled.Item2, out value, out attributes);
            }
            else if (isEdit) {
                tagType = html.GetActionAsButton(actionContext, out value, out attributes);
            }
            else {
                tagType = html.GetActionAsForm(actionContext, html.Surface().GetObjectTypeShortName(actionContext.Target.Object), routeValues, out value, out attributes);
            }

            return new ElementDescriptor {
                TagType = tagType,
                Value = value,
                Attributes = attributes
            };
        }

        internal static string GetActionAsForm(this HtmlHelper html, ActionContext actionContext, string controllerName, object routeValues, out string value, out RouteValueDictionary attributes) {
            const string tagType = "form";

            IEnumerable<ElementDescriptor> concurrencyElements = html.GetConcurrencyElements(actionContext.Target, actionContext.GetConcurrencyActionInputId);

            string elements = concurrencyElements.Aggregate(String.Empty, (s, t) => s + t.BuildElement());

            RouteValueDictionary routeValueDictionary;
            if (actionContext.ParameterValues != null && actionContext.ParameterValues.Any()) {
                // custom values have been set so push the values into view data 
                // fields will be hidden and action will be 'none' so that action goes straight through and doesn't prompt for 
                // parameters 

                foreach (var pc in actionContext.GetParameterContexts(html.Surface())) {
                    if (pc.CustomValue != null) {
                        html.ViewData[html.IdHelper().GetParameterInputId((actionContext.Action), (pc.Parameter))] = pc.CustomValue.Specification.IsParseable ? pc.CustomValue.Object : pc.CustomValue;
                    }
                }

                List<ElementDescriptor> paramElements = html.ActionParameterFields(actionContext).ToList();
                elements = paramElements.Aggregate(String.Empty, (s, t) => s + t.BuildElement());
                routeValueDictionary = new RouteValueDictionary();
            }
            else {
                routeValueDictionary = new RouteValueDictionary(new {action = "action"});
            }

            value = (elements + GetSubmitButton(null, actionContext.Action.Name, IdConstants.ActionInvokeAction, routeValueDictionary)).WrapInDivTag();

            attributes = new RouteValueDictionary(new {
                action = html.GenerateUrl(Action(actionContext.Action.Id), controllerName, new RouteValueDictionary(routeValues)),
                method = "post",
                id = actionContext.GetActionId(),
                @class = actionContext.GetActionClass(),
            });
            return tagType;
        }

        internal static string GetActionAsButton(this HtmlHelper html, ActionContext actionContext, out string value, out RouteValueDictionary attributes) {
            const string tagType = "button";

            value = actionContext.Action.Name;
            attributes = html.GetActionAttributes(IdConstants.ActionInvokeAction, actionContext, new ActionContext(html.IdHelper(), actionContext.Target, null), String.Empty);

            return tagType;
        }

        internal static ElementDescriptor ViewObjectField(this HtmlHelper html, PropertyContext propertyContext) {
            return new ElementDescriptor {
                TagType = "div",
                Label = html.GetLabel(propertyContext),
                Value = html.GetViewValue(propertyContext),
                Attributes = new RouteValueDictionary(new {
                    id = propertyContext.GetFieldId(),
                    @class = propertyContext.GetFieldClass()
                })
            };
        }

        internal static IEnumerable<ElementDescriptor> ViewObjectFields(this HtmlHelper html, INakedObjectSurface nakedObject, PropertyContext parentContext, Func<INakedObjectAssociationSurface, bool> filter, Func<INakedObjectAssociationSurface, int> order, out bool anyEditableFields) {
            var query = nakedObject.Specification.Properties.Where(p => p.IsVisible(nakedObject)).Where(filter);

            if (order != null) {
                query = query.OrderBy(order);
            }

            var visibleFields = query.ToList();
            anyEditableFields = visibleFields.Any(p => p.IsUsable(nakedObject).IsAllowed);
            return visibleFields.Select(property => html.ViewObjectField(new PropertyContext(html.IdHelper(), nakedObject, property, false, parentContext)));
        }

        internal static Tuple<bool, string> IsDuplicate(this HtmlHelper html, IEnumerable<INakedObjectActionSurface> allActions, INakedObjectActionSurface action) {
            return new Tuple<bool, string>(allActions.Count(a => a.Name == action.Name) > 1, MvcUi.DuplicateAction);
        }

        internal static IList<ElementDescriptor> ObjectActions(this HtmlHelper html, INakedObjectSurface nakedObject, bool isEdit) {
            var allActions = html.Surface().GetTopLevelActions(nakedObject).ToList();

            return allActions.Select(action => html.ObjectActionAsElementDescriptor(new ActionContext(html.IdHelper(), false, nakedObject, action),
                new {id = html.Surface().OidStrategy.GetObjectId(nakedObject)},
                isEdit,
                html.IsDuplicate(allActions, action))).ToList();
        }

        internal static IList<ElementDescriptor> ObjectActions(this HtmlHelper html, INakedObjectSurface nakedObject, bool isEdit, params CustomMenuItem[] menuItems) {
            List<ElementDescriptor> actions = html.ObjectActions(nakedObject, isEdit).ToList();

            IEnumerable<Tuple<ElementDescriptor, int>> otherActions = menuItems.Select(item => new Tuple<ElementDescriptor, int>(html.ObjectActionAsElementDescriptor(item, isEdit), item.MemberOrder));

            foreach (var otherAction in otherActions) {
                if (otherAction.Item2 >= 0 && otherAction.Item2 <= actions.Count) {
                    actions.Insert(otherAction.Item2, otherAction.Item1);
                }
                else {
                    actions.Insert(actions.Count, otherAction.Item1);
                }
            }

            return actions;
        }

        internal static IList<ElementDescriptor> ObjectActions(this HtmlHelper html, bool isEdit, params CustomMenuItem[] menuItems) {
            return menuItems.OrderBy(x => x.MemberOrder).Select(item => html.ObjectActionAsElementDescriptor(item, false)).ToList();
        }

        #endregion

        #region private

        private static ElementDescriptor GetActionDialog(this HtmlHelper html,
                                                         ActionContext targetActionContext,
                                                         ActionContext actionContext,
                                                         IList<ElementDescriptor> paramElements,
                                                         string propertyName) {
            if (!paramElements.Any()) {
                return null;
            }

            var nameTag = new TagBuilder("div");
            nameTag.AddCssClass(IdConstants.ActionNameLabel);
            nameTag.SetInnerText(targetActionContext.Action.Name);

            TagBuilder parms = ElementDescriptor.BuildElementSet(paramElements);
            parms.AddCssClass(IdConstants.ParamContainerName);

            html.AddAjaxDataUrlsToElementSet(targetActionContext.Target, targetActionContext.Action, parms);

            return new ElementDescriptor {
                TagType = "div",
                Value = nameTag.ToString() + parms + GetSubmitButton(IdConstants.OkButtonClass,
                    MvcUi.OK,
                    IdConstants.InvokeFindAction,
                    html.GetButtonNameValues(targetActionContext, actionContext, null, propertyName)),
                Attributes = new RouteValueDictionary(new {
                    @class = IdConstants.ActionDialogName,
                    id = targetActionContext.GetActionDialogId()
                })
            };
        }

        private static ElementDescriptor GetSubEditObject(this HtmlHelper html,
                                                          ActionContext targetActionContext,
                                                          ActionContext actionContext,
                                                          object subEditObject,
                                                          string propertyName) {
            var nakedObject = html.Surface().GetObject(subEditObject);

            Func<INakedObjectAssociationSurface, bool> filterCollections = x => !x.IsCollection;

            TagBuilder elementSet = AddClassAndIdToElementSet(html.EditObjectFields(nakedObject, null, filterCollections, null, true),
                IdConstants.FieldContainerName,
                html.IdHelper().GetFieldContainerId((nakedObject)));
            html.AddAjaxDataUrlsToElementSet(nakedObject, elementSet);

            return new ElementDescriptor {
                TagType = "div",
                Value = html.Object(html.ObjectTitle(nakedObject.Object).ToString(), IdConstants.ViewAction, nakedObject.Object).ToString()
                        + elementSet
                        + GetSubmitButton(IdConstants.SaveButtonClass,
                            MvcUi.Save,
                            IdConstants.InvokeSaveAction,
                            html.GetButtonNameValues(targetActionContext, actionContext, nakedObject, propertyName)),
                Attributes = new RouteValueDictionary(new {
                    @class = html.ObjectEditClass(nakedObject.Object),
                    id = GetObjectType(nakedObject.Object)
                })
            };
        }

        private static TagBuilder AddClassAndIdToElementSet(IEnumerable<ElementDescriptor> elements, string cls, string id) {
            TagBuilder elementSet = ElementDescriptor.BuildElementSet(elements);
            elementSet.AddCssClass(cls);
            elementSet.GenerateId(id);
            return elementSet;
        }

        private static ElementDescriptor SelectionView(this HtmlHelper html, object target, string propertyName, IEnumerable collection) {
            var collectionNakedObject = html.Surface().GetObject(collection);
            var targetNakedObject = html.Surface().GetObject(target);
            return html.GetSelectionCollection(collectionNakedObject, targetNakedObject, propertyName);
        }

        private static string FinderActions(this HtmlHelper html, INakedObjectSpecificationSurface spec, ActionContext actionContext, string propertyName) {
            if (spec.IsCollection) {
                return String.Empty; // We don't want Finder menu rendered on any collection field
            }
            var allActions = new List<ElementDescriptor> {
                RemoveAction(propertyName),
                html.RecentlyViewedAction(spec, actionContext, propertyName)
            };
            allActions.AddRange(html.FinderActionsForField(actionContext, spec, propertyName));

            if (allActions.Any()) {
                return BuildMenuContainer(allActions,
                    IdConstants.MenuContainerName,
                    actionContext.GetFindMenuId(propertyName),
                    MvcUi.Find).ToString();
            }

            return String.Empty;
        }

        private static string CollectionTable(this HtmlHelper html,
                                              INakedObjectSurface collectionNakedObject,
                                              Func<INakedObjectSurface, string> linkFunc,
                                              Func<INakedObjectAssociationSurface, bool> filter,
                                              Func<INakedObjectAssociationSurface, int> order,
                                              bool isStandalone,
                                              bool withSelection,
                                              bool withTitle,
                                              bool defaultChecked = false) {
            var table = new TagBuilder("table");
            table.AddCssClass(html.CollectionItemTypeName(collectionNakedObject));
            table.InnerHtml += Environment.NewLine;

            string innerHtml = "";

            var collection = collectionNakedObject.ToEnumerable().ToArray();

            var collectionSpec = collectionNakedObject.ElementSpecification;

            var collectionAssocs = html.CollectionAssociations(collection, collectionSpec, filter, order);

            int index = 0;
            foreach (var item in collection) {
                var row = new TagBuilder("tr");

                if (withSelection) {
                    var cbTag = new TagBuilder("td");
                    int i = index++;
                    string id = "checkbox" + i;
                    string label = GetLabelTag(true, (i + 1).ToString(CultureInfo.InvariantCulture), () => id);
                    cbTag.InnerHtml += (label + html.CheckBox(html.Surface().OidStrategy.GetObjectId(item), defaultChecked, new {id, @class = IdConstants.CheckboxClass}));
                    row.InnerHtml += cbTag.ToString();
                }

                if (withTitle) {
                    var itemTag = new TagBuilder("td");
                    itemTag.InnerHtml += linkFunc(item);
                    row.InnerHtml += itemTag.ToString();
                }

                string[] collectionValues = collectionAssocs.Select(a => html.GetViewField(new PropertyContext(html.IdHelper(), item, a, false), a.Description, true, true)).ToArray();

                foreach (string s in collectionValues) {
                    row.InnerHtml += new TagBuilder("td") {InnerHtml = s};
                }
                innerHtml += (row + Environment.NewLine);
            }

            var headers = collectionAssocs.Select(a => a.Name).ToArray();
            html.AddHeader(headers, table, isStandalone, withSelection, withTitle, defaultChecked);
            table.InnerHtml += innerHtml;

            return table + html.AddFooter(collectionNakedObject);
        }

        private static void AddHeader(this HtmlHelper html,
                                      IList<string> headers,
                                      TagBuilder table,
                                      bool isStandalone,
                                      bool isSelection,
                                      bool withTitle,
                                      bool defaultChecked) {
            if (isStandalone) {
                int pageSize, maxPage, currentPage, total;
                html.GetPagingValues(out pageSize, out maxPage, out currentPage, out total);

                var tagFormat = new TagBuilder("div");
                tagFormat.AddCssClass(IdConstants.CollectionFormatClass);

                tagFormat.InnerHtml += GetSubmitButton(IdConstants.ListButtonClass, MvcUi.List, IdConstants.PageAction, new RouteValueDictionary(new {page = currentPage, pageSize, NofCollectionFormat = IdConstants.ListDisplayFormat}));
                tagFormat.InnerHtml += GetSubmitButton(IdConstants.TableButtonClass, MvcUi.Table, IdConstants.PageAction, new RouteValueDictionary(new {page = currentPage, pageSize, NofCollectionFormat = IdConstants.TableDisplayFormat}));

                table.InnerHtml += tagFormat;
            }

            if (headers.Any() || isSelection) {
                var row1 = new TagBuilder("tr");
                if (isSelection) {
                    var cbTag = new TagBuilder("th");
                    cbTag.InnerHtml += html.CheckBox(IdConstants.CheckboxAll, defaultChecked);
                    cbTag.InnerHtml += GetLabelTag(true, MvcUi.All, () => IdConstants.CheckboxAll);
                    row1.InnerHtml += cbTag.ToString();
                }

                if (withTitle) {
                    var emptyCell = new TagBuilder("th");
                    row1.InnerHtml += emptyCell;
                }

                foreach (string s in headers) {
                    var cell = new TagBuilder("th");
                    cell.SetInnerText(s);
                    row1.InnerHtml += cell;
                }

                table.InnerHtml += row1 + Environment.NewLine;
            }
        }

        private static string AddFooter(this HtmlHelper html, INakedObjectSurface pagedCollectionNakedObject) {
            int pageSize, maxPage, currentPage, total;
            html.GetPagingValues(out pageSize, out maxPage, out currentPage, out total);

            if (maxPage > 1) {
                var tagPaging = new TagBuilder("div");
                tagPaging.AddCssClass(IdConstants.PagingClass);

                var tagPageNumber = new TagBuilder("div");
                tagPageNumber.AddCssClass(IdConstants.PageNumberClass);
                tagPageNumber.InnerHtml += String.Format(MvcUi.PageOf, currentPage, maxPage);
                tagPaging.InnerHtml += tagPageNumber;

                var tagTotalCount = new TagBuilder("div");
                tagTotalCount.AddCssClass(IdConstants.TotalCountClass);

                var typeSpec = pagedCollectionNakedObject.ElementSpecification;

                tagTotalCount.InnerHtml += String.Format(MvcUi.TotalOfXType, total, total == 1 ? typeSpec.SingularName : typeSpec.PluralName);
                tagPaging.InnerHtml += tagTotalCount;
                string displayType = html.ViewData.ContainsKey(IdConstants.CollectionFormat) ? (string) html.ViewData[IdConstants.CollectionFormat] : IdConstants.ListDisplayFormat;

                if (currentPage > 1) {
                    tagPaging.InnerHtml += GetSubmitButton(null, MvcUi.First, IdConstants.PageAction, new RouteValueDictionary(new {page = 1, pageSize, NofCollectionFormat = displayType}));
                    tagPaging.InnerHtml += GetSubmitButton(null, MvcUi.Previous, IdConstants.PageAction, new RouteValueDictionary(new {page = currentPage - 1, pageSize, NofCollectionFormat = displayType}));
                }
                else {
                    tagPaging.InnerHtml += GetDisabledButton(null, MvcUi.First);
                    tagPaging.InnerHtml += GetDisabledButton(null, MvcUi.Previous);
                }

                if (currentPage < maxPage) {
                    tagPaging.InnerHtml += GetSubmitButton(null, MvcUi.Next, IdConstants.PageAction, new RouteValueDictionary(new {page = currentPage + 1, pageSize, NofCollectionFormat = displayType}));
                    tagPaging.InnerHtml += GetSubmitButton(null, MvcUi.Last, IdConstants.PageAction, new RouteValueDictionary(new {page = maxPage, pageSize, NofCollectionFormat = displayType}));
                }
                else {
                    tagPaging.InnerHtml += GetDisabledButton(null, MvcUi.Next);
                    tagPaging.InnerHtml += GetDisabledButton(null, MvcUi.Last);
                }

                return tagPaging.ToString();
            }

            return "";
        }

        internal static bool GetPagingValues(this HtmlHelper html, out int pageSize, out int maxPage, out int currentPage, out int total) {
            currentPage = 1;
            total = 1;
            pageSize = 20;
            bool found = false;

            if (html.ViewData.ContainsKey(IdConstants.PagingData)) {
                var data = (Dictionary<string, int>) html.ViewData[IdConstants.PagingData];
                currentPage = data[IdConstants.PagingCurrentPage];
                total = data[IdConstants.PagingTotal];
                pageSize = data[IdConstants.PagingPageSize];
                found = true;
            }

            maxPage = (int) Math.Ceiling(total/(decimal) pageSize);
            return found;
        }

        private static string GetFieldValue(this HtmlHelper html, ParameterContext context, INakedObjectSurface valueNakedObject) {
            string value = "";

            if (context.Parameter.IsAutoCompleteEnabled) {
                var htmlAttributes = new RouteValueDictionary(new {title = context.Parameter.Description});

                html.AddClientValidationAttributes(context, htmlAttributes);
                value += html.GetAutoCompleteTextBox(context, htmlAttributes, valueNakedObject);
            }
            else if (valueNakedObject != null) {
                string link = "{0}";

                if (context.Parameter.Specification.IsCollection) {
                    link = html.CollectionLink(link, IdConstants.ViewAction, valueNakedObject.Object);
                }
                else if (!context.Parameter.Specification.IsParseable && !context.Parameter.Specification.IsCollection) {
                    link = html.ObjectLink(link, IdConstants.ViewAction, valueNakedObject.Object);
                }

                string title = html.GetDisplayTitle(context.Parameter, valueNakedObject);
                value += String.Format(link, title);
            }

            return value;
        }

        private static string GetFieldValue(this HtmlHelper html, PropertyContext context, INakedObjectSurface valueNakedObject) {
            string value = "";

            if (context.Property.IsAutoCompleteEnabled) {
                var htmlAttributes = new RouteValueDictionary(new {title = context.Property.Description});

                html.AddClientValidationAttributes(context, htmlAttributes);
                value += html.GetAutoCompleteTextBox(context, htmlAttributes, valueNakedObject);
            }
            else if (valueNakedObject != null) {
                string link = "{0}";

                if (!context.Property.Specification.IsParseable) {
                    link = html.ObjectLink(link, IdConstants.ViewAction, valueNakedObject.Object);
                }
                value += String.Format(link, html.GetDisplayTitle(context.Property, valueNakedObject));
            }

            return value;
        }

        private static string GetAutoCompleteTextBox(this HtmlHelper html, ParameterContext context, RouteValueDictionary htmlAttributes, INakedObjectSurface valueNakedObject) {
            string completionAjaxUrl = html.GenerateUrl("GetActionCompletions", "Ajax", new RouteValueDictionary(new {id = html.Surface().OidStrategy.GetObjectId(context.Target), actionName = context.Action.Id, parameterIndex = context.Parameter.Number}));
            RouteValueDictionary attrs = CreateAutoCompleteAttributes(context.Parameter, completionAjaxUrl);
            SurfaceHelper.ForEach(attrs, kvp => htmlAttributes.Add(kvp.Key, kvp.Value));
            string title = valueNakedObject == null ? "" : html.GetDisplayTitle(context.Parameter, valueNakedObject);
            return html.TextBox(context.GetParameterAutoCompleteId(), title, htmlAttributes).ToHtmlString();
        }

        private static string GetAutoCompleteTextBox(this HtmlHelper html, PropertyContext context, RouteValueDictionary htmlAttributes, INakedObjectSurface valueNakedObject) {
            string completionAjaxUrl = html.GenerateUrl("GetPropertyCompletions", "Ajax", new RouteValueDictionary(new {id = html.Surface().OidStrategy.GetObjectId(context.Target), propertyId = context.Property.Id}));
            RouteValueDictionary attrs = CreateAutoCompleteAttributes(context.Property, completionAjaxUrl);
            SurfaceHelper.ForEach(attrs, kvp => htmlAttributes.Add(kvp.Key, kvp.Value));
            string title = valueNakedObject == null ? "" : html.GetDisplayTitle(context.Property, valueNakedObject);
            return html.TextBox(context.GetAutoCompleteFieldId(), title, htmlAttributes).ToHtmlString();
        }

        private static RouteValueDictionary CreateAutoCompleteAttributes(INakedObjectAssociationSurface holder, string completionAjaxUrl) {
            int minLength = holder.AutoCompleteMinLength;
            var attrs = new RouteValueDictionary {{"data-completions", completionAjaxUrl}, {"data-completions-minlength", minLength}};
            return attrs;
        }

        private static RouteValueDictionary CreateAutoCompleteAttributes(INakedObjectActionParameterSurface holder, string completionAjaxUrl) {
            int minLength = holder.AutoCompleteMinLength;
            var attrs = new RouteValueDictionary {{"data-completions", completionAjaxUrl}, {"data-completions-minlength", minLength}};
            return attrs;
        }

        private static string GetFieldValue(this HtmlHelper html, PropertyContext propertyContext, bool inTable = false) {
            var valueNakedObject = propertyContext.GetValue(html.Surface());

            if (valueNakedObject == null) {
                return String.Empty;
            }

            if (propertyContext.Property.IsFile) {
                return html.GetFileFieldValue(propertyContext);
            }

            if (propertyContext.Property.Specification.IsBoolean) {
                return html.GetBooleanFieldValue(valueNakedObject);
            }

            if (propertyContext.Property.IsEnum) {
                return GetEnumFieldValue(propertyContext.Property, valueNakedObject);
            }

            return html.GetTextOrRefFieldValue(propertyContext, valueNakedObject, inTable);
        }

        private static string GetEnumFieldValue(INakedObjectAssociationSurface property, INakedObjectSurface valueNakedObject) {
            return property.GetTitle(valueNakedObject);
        }

        private static string GetTextOrRefFieldValue(this HtmlHelper html, PropertyContext propertyContext, INakedObjectSurface valueNakedObject, bool inTable = false) {
            if (valueNakedObject.Specification.IsCollection) {
                valueNakedObject.Resolve();
            }

            string link = "{0}";

            if (!propertyContext.Property.Specification.IsParseable && !propertyContext.Property.IsCollection) {
                string displayType = html.ViewData.ContainsKey(propertyContext.GetFieldId()) ? (string) html.ViewData[propertyContext.GetFieldId()] : String.Empty;
                bool renderEagerly = RenderEagerly(propertyContext.Property);

                link = html.ObjectLink(link, IdConstants.ViewAction, valueNakedObject.Object) + (inTable ? "" : html.GetObjectDisplayLinks(propertyContext));

                if (displayType == IdConstants.MaxDisplayFormat || renderEagerly) {
                    var inlineNakedObject = propertyContext.GetValue(html.Surface());
                    bool anyEditableFields;
                    TagBuilder elementSet = ElementDescriptor.BuildElementSet(html.ViewObjectFields(inlineNakedObject, propertyContext, x => true, null, out anyEditableFields));

                    html.AddAjaxDataUrlsToElementSet(inlineNakedObject, elementSet, propertyContext);
                    elementSet.AddCssClass(IdConstants.FieldContainerName);
                    elementSet.GenerateId(html.IdHelper().GetFieldContainerId(inlineNakedObject));

                    link = link + html.GetEditButtonIfRequired(anyEditableFields, inlineNakedObject) + elementSet;
                }
            }

            string title = html.GetDisplayTitle(propertyContext.Property, valueNakedObject);

            if (propertyContext.Property.NumberOfLines > 1) {
                int typicalLength = propertyContext.Property.TypicalLength;
                int width = propertyContext.Property.Width;

                typicalLength = typicalLength == 0 ? 20 : typicalLength;
                width = width == 0 ? typicalLength : width;

                if (inTable) {
                    // truncate to width 
                    if (title.Length > width) {
                        const string elipsis = "...";
                        int length = width - elipsis.Length;
                        title = title.Substring(0, length > 0 ? length : 1) + elipsis;
                    }
                }
            }

            return String.Format(link, title);
        }

        private static MvcHtmlString GetEditButtonIfRequired(this HtmlHelper html, bool anyEditableFields, INakedObjectSurface inlineNakedObject) {
            if (anyEditableFields &&
                !inlineNakedObject.Specification.IsAlwaysImmutable &&
                !inlineNakedObject.Specification.IsImmutableOncePersisted &&
                !inlineNakedObject.Specification.IsComplexType) {
                return html.ControllerAction(MvcUi.Edit, IdConstants.EditObjectAction, IdConstants.EditButtonClass, inlineNakedObject.Object);
            }
            return new MvcHtmlString("");
        }

        private static string GetBooleanFieldValue(this HtmlHelper html, INakedObjectSurface valueNakedObject) {
            var state = valueNakedObject.GetDomainObject<bool?>();

            string img = "unset.png";
            string alt = MvcUi.TriState_NotSet;

            if (state.HasValue) {
                if (state.Value) {
                    img = "checked.png";
                    alt = MvcUi.TriState_True;
                }
                else {
                    img = "unchecked.png";
                    alt = MvcUi.TriState_False;
                }
            }

            var url = new UrlHelper(html.ViewContext.RequestContext);
            var tag = new TagBuilder("img");
            tag.MergeAttribute("src", url.Content("~/Images/" + img));
            tag.MergeAttribute("alt", alt);

            return tag.ToString();
        }

        private static string GetFileFieldValue(this HtmlHelper html, PropertyContext propertyContext) {
            // todo is this right ?
            string title = propertyContext.Property.GetNakedObject(propertyContext.Target).TitleString;
            string utitle = propertyContext.Property.GetNakedObject(propertyContext.Target).Specification.UntitledName;

            title = String.IsNullOrEmpty(title) || title == utitle ? (propertyContext.Property.Specification.IsImage ? propertyContext.Property.Name : MvcUi.ShowFile) : title;

            string imageUrl = html.GenerateUrl(IdConstants.GetFileAction + "/" + title.Replace(' ', '_'),
                html.Surface().GetObjectTypeShortName(propertyContext.Target.Object),
                new RouteValueDictionary(new {
                    Id = html.Surface().OidStrategy.GetObjectId(propertyContext.Target),
                    PropertyId = propertyContext.Property.Id
                }));

            var linktag = new TagBuilder("a");
            linktag.MergeAttribute("href", imageUrl);

            if (propertyContext.Property.Specification.IsImage) {
                var imageTag = new TagBuilder("img");
                imageTag.MergeAttribute("src", imageUrl);
                imageTag.MergeAttribute("alt", title);
                linktag.InnerHtml += imageTag.ToString(TagRenderMode.SelfClosing);
            }
            else {
                linktag.InnerHtml += title;
            }

            return linktag.ToString();
        }

        private static string GetInvariantValue(this HtmlHelper html, PropertyContext propertyContext) {
            var valueNakedObject = propertyContext.GetValue(html.Surface());

            if (valueNakedObject == null) {
                return String.Empty;
            }
            return valueNakedObject.InvariantString;
        }

        private static string GetRawValue(this HtmlHelper html, PropertyContext propertyContext) {
            var valueNakedObject = propertyContext.GetValue(html.Surface());

            if (valueNakedObject == null) {
                return String.Empty;
            }

            if (valueNakedObject.Specification.IsEnum) {
                return valueNakedObject.Object.ToString();
            }

            return valueNakedObject.TitleString;
        }

        private static IDictionary<string, object> GetDisplayStatuses(this HtmlHelper html) {
            IEnumerable<KeyValuePair<string, object>> status = html.ViewData.Where(kvp => kvp.Value != null &&
                                                                                          (kvp.Value.Equals(IdConstants.MinDisplayFormat) ||
                                                                                           kvp.Value.Equals(IdConstants.MaxDisplayFormat) ||
                                                                                           kvp.Value.Equals(IdConstants.ListDisplayFormat) ||
                                                                                           kvp.Value.Equals(IdConstants.TableDisplayFormat) ||
                                                                                           kvp.Value.Equals(IdConstants.SummaryDisplayFormat)));
            return status.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        private static string GenerateUrl(this HtmlHelper html, string actionName, object domainObject) {
            string controllerName = html.Surface().GetObjectTypeShortName(domainObject);
            return html.GenerateUrl(actionName, controllerName, new RouteValueDictionary(new {id = html.GetObjectId(domainObject)}));
        }

        public static string GenerateUrl(this HtmlHelper html, string actionName, string controllerName, RouteValueDictionary routeValues) {
            return UrlHelper.GenerateUrl(null, actionName, controllerName, null, null, null, routeValues, html.RouteCollection, html.ViewContext.RequestContext, false);
        }

        private static string GetObjectDisplayLinks(this HtmlHelper html, PropertyContext propertyContext) {
            IDictionary<string, object> objectDisplayStatuses = GetDisplayStatuses(html);

            string actionName = propertyContext.IsEdit ? IdConstants.EditObjectAction : IdConstants.ViewAction;

            if (propertyContext.IsEdit || propertyContext.Target.IsTransient) {
                // for the moment no expand and delete on editable views 
                return "";
            }
            var formTag = new TagBuilder("form");
            formTag.MergeAttribute("method", "post");

            object objectToView;
            string objectId;
            //for ajax use the property 
            if (html.ViewContext.RequestContext.HttpContext.Request.IsAjaxRequest()) {
                objectId = "";
                objectToView = propertyContext.GetValue(html.Surface()).Object;
            }
            else {
                objectToView = propertyContext.OriginalTarget.Object;
                objectId = propertyContext.GetFieldId();
            }

            formTag.MergeAttribute("action", html.GenerateUrl(actionName, objectToView));

            formTag.InnerHtml += html.Hidden(html.IdHelper().GetDisplayFormatId(objectId), ToNameValuePairs(objectDisplayStatuses));

            formTag.InnerHtml += GetSubmitButton(IdConstants.MinButtonClass, MvcUi.Collapse, IdConstants.RedisplayAction, new RouteValueDictionary {{objectId, IdConstants.MinDisplayFormat}, {"editMode", propertyContext.IsEdit}});
            formTag.InnerHtml += GetSubmitButton(IdConstants.MaxButtonClass, MvcUi.Expand, IdConstants.RedisplayAction, new RouteValueDictionary {{objectId, IdConstants.MaxDisplayFormat}, {"editMode", propertyContext.IsEdit}});

            formTag.InnerHtml = formTag.InnerHtml.WrapInDivTag();
            return formTag.ToString();
        }

        private static string GetCollectionDisplayLinks(this HtmlHelper html, PropertyContext propertyContext) {
            IDictionary<string, object> collectionStatuses = GetDisplayStatuses(html);
            string collectionId = propertyContext.GetFieldId();

            string actionName = propertyContext.IsEdit ? IdConstants.EditObjectAction : IdConstants.ViewAction;

            if (propertyContext.IsEdit || propertyContext.Target.IsTransient) {
                return GetSubmitButton(IdConstants.SummaryButtonClass, MvcUi.Summary, IdConstants.RedisplayAction, new RouteValueDictionary {{collectionId, IdConstants.SummaryDisplayFormat}, {"editMode", propertyContext.IsEdit}}) +
                       GetSubmitButton(IdConstants.ListButtonClass, MvcUi.List, IdConstants.RedisplayAction, new RouteValueDictionary {{collectionId, IdConstants.ListDisplayFormat}, {"editMode", propertyContext.IsEdit}}) +
                       GetSubmitButton(IdConstants.TableButtonClass, MvcUi.Table, IdConstants.RedisplayAction, new RouteValueDictionary {{collectionId, IdConstants.TableDisplayFormat}, {"editMode", propertyContext.IsEdit}});
            }
            var formTag = new TagBuilder("form");
            formTag.MergeAttribute("method", "post");
            formTag.MergeAttribute("action", html.GenerateUrl(actionName, propertyContext.OriginalTarget.Object));

            formTag.InnerHtml += html.Hidden(html.IdHelper().GetDisplayFormatId(collectionId), ToNameValuePairs(collectionStatuses));

            formTag.InnerHtml += GetSubmitButton(IdConstants.SummaryButtonClass, MvcUi.Summary, IdConstants.RedisplayAction, new RouteValueDictionary {{collectionId, IdConstants.SummaryDisplayFormat}, {"editMode", propertyContext.IsEdit}});
            formTag.InnerHtml += GetSubmitButton(IdConstants.ListButtonClass, MvcUi.List, IdConstants.RedisplayAction, new RouteValueDictionary {{collectionId, IdConstants.ListDisplayFormat}, {"editMode", propertyContext.IsEdit}});
            formTag.InnerHtml += GetSubmitButton(IdConstants.TableButtonClass, MvcUi.Table, IdConstants.RedisplayAction, new RouteValueDictionary {{collectionId, IdConstants.TableDisplayFormat}, {"editMode", propertyContext.IsEdit}});

            formTag.InnerHtml = formTag.InnerHtml.WrapInDivTag();
            return formTag.ToString();
        }

        private static INakedObjectSurface GetExistingValue(this HtmlHelper html, string id, PropertyContext propertyContext) {
            ModelState modelState;
            string rawExistingValue = html.ViewData.ModelState.TryGetValue(id, out modelState) ? (string) modelState.Value.RawValue : null;

            INakedObjectSurface existingValue;
            if (String.IsNullOrEmpty(rawExistingValue)) {
                existingValue = propertyContext.GetValue(html.Surface());
            }
            else {
                existingValue = propertyContext.Property.Specification.IsParseable ? html.Surface().GetObject(propertyContext.Property.Specification, rawExistingValue) :
                    html.Surface().GetObject(html.Surface().OidFactory.GetLinkOid((string) modelState.Value.RawValue)).Target;
            }
            return existingValue;
        }

        private static IEnumerable<SelectListItem> GetItems(this HtmlHelper html, string id, ParameterContext context) {
            var existingValue = html.GetParmExistingValue(id, context, true); /*remove from viewdata as it confuses dropdown helper*/

            var choices = context.Parameter.GetChoicesParameters();
            var values = new Dictionary<string, INakedObjectSurface>();
            if (choices.Any()) {
                values = context.Action.Parameters.
                    Where(p => choices.Select(pnt => pnt.Item1).Contains(p.Id.ToLower())).
                    ToDictionary(p => p.Id.ToLower(),
                        p => html.GetParmExistingValue(html.IdHelper().GetParameterInputId(context.Action, p), new ParameterContext(html.IdHelper(), context) {Parameter = p}, false));
            }

            return html.GetChoicesSet(context, existingValue, values);
        }

        private static INakedObjectSurface GetParmExistingValue(this HtmlHelper html, string id, ParameterContext context, bool clear) {
            return html.GetParameterExistingValue(id, context, clear) ?? html.GetParameterDefaultValue(id, context, clear);
        }

        private static IEnumerable<SelectListItem> GetItems(this HtmlHelper html, string id, PropertyContext propertyContext) {
            INakedObjectSurface existingValue;

            if (propertyContext.Target.IsTransient && !propertyContext.Property.DefaultTypeIsExplicit(propertyContext.Target)) {
                // ignore implicit defaults on transients
                existingValue = null;
            }
            else {
                existingValue = html.GetExistingValue(id, propertyContext);
            }

            var values = new Dictionary<string, INakedObjectSurface>();
            var choicesParameters = propertyContext.Property.GetChoicesParameters();

            if (choicesParameters.Any()) {
                values = propertyContext.Target.Specification.Properties.
                    Where(p => choicesParameters.Select(pnt => pnt.Item1).Contains(p.Id.ToLower())).
                    ToDictionary(p => p.Id.ToLower(),
                        p => html.GetExistingValue(html.IdHelper().GetFieldInputId(propertyContext.Target, p),
                            new PropertyContext(html.IdHelper(), propertyContext) {
                                Property = p
                            }));
            }

            return html.GetChoicesSet(propertyContext, existingValue, values);
        }

        private static IEnumerable<SelectListItem> GetItems(this HtmlHelper html, string id, FeatureContext context) {
            if (context is PropertyContext) {
                return html.GetItems(id, context as PropertyContext);
            }
            if (context is ParameterContext) {
                return html.GetItems(id, context as ParameterContext);
            }
            // todo is this correct exception
            throw new BadRequestNOSException(String.Format("Unexpected context type {0}", context.GetType()));
            //throw new UnexpectedCallException(string.Format("Unexpected context type {0}", context.GetType()));
        }

        private static INakedObjectSurface GetAndParseValueAsNakedObject(this HtmlHelper html, INakedObjectSpecificationSurface spec, object value) {
            return html.Surface().GetObject(spec, value);
        }

        private static INakedObjectSurface GetAndParseValueAsNakedObject(this HtmlHelper html, ParameterContext context, object value) {
            return html.GetAndParseValueAsNakedObject(context.Parameter.Specification, value);
        }

        private static INakedObjectSurface GetParameterExistingValue(this HtmlHelper html, string id, ParameterContext context, bool clear = false) {
            ModelState modelState = html.ViewData.ModelState.TryGetValue(id, out modelState) ? modelState : null;

            if (modelState != null && modelState.Value != null) {
                object rawvalue = modelState.Value.RawValue;
                if (clear) {
                    // only clear the value and keep any error 
                    ModelErrorCollection errors = modelState.Errors;
                    html.ViewData.ModelState.Remove(id);

                    if (errors.Any()) {
                        SurfaceHelper.ForEach(errors, e => html.ViewData.ModelState.AddModelError(id, e.ErrorMessage));
                    }
                }
                if (context.Parameter.Specification.IsParseable) {
                    return html.GetAndParseValueAsNakedObject(context, rawvalue);
                }

                if (!context.Parameter.Specification.IsCollection) {
                    return (INakedObjectSurface) rawvalue;
                }

                if (context.Parameter.Specification.IsCollection) {
                    var itemSpec = context.Parameter.ElementType;

                    if (itemSpec.IsParseable) {
                        // todo this may not work
                        return html.GetAndParseValueAsNakedObject(context, rawvalue);
                    }

                    return (INakedObjectSurface) rawvalue;
                }

                return context.Parameter.Specification.IsParseable ? html.GetAndParseValueAsNakedObject(context, rawvalue) : (INakedObjectSurface) rawvalue;
            }
            return null;
        }

        private static INakedObjectSurface GetParameterDefaultValue(this HtmlHelper html, string id, ParameterContext context, bool clear = false) {
            object value = html.ViewData.TryGetValue(id, out value) ? value : null;

            if (value != null) {
                if (clear) {
                    html.ViewData.Remove(id);
                }
                return context.Parameter.Specification.IsParseable ? html.GetAndParseValueAsNakedObject(context, value) : (INakedObjectSurface) value;
            }
            return null;
        }

        private static INakedObjectSurface GetSuggestedItem(this HtmlHelper html, string refId, INakedObjectSurface existingNakedObject) {
            if (html.ViewData.ContainsKey(refId)) {
                return (INakedObjectSurface) html.ViewData[refId];
            }
            return existingNakedObject;
        }

        private static string GetValueForChoice(this HtmlHelper html, INakedObjectSurface choice) {
            if (choice.Specification.IsEnum) {
                return choice.EnumIntegralValue;
            }
            if (choice.Specification.IsParseable) {
                return choice.TitleString;
            }
            return html.Surface().OidStrategy.GetObjectId(choice);
        }

        private static string GetTextForChoice(INakedObjectSurface choice) {
            return choice.TitleString;
        }

        private static bool GetSelectedForChoice(this HtmlHelper html, INakedObjectSurface choice, INakedObjectSurface existingNakedObject) {
            IEnumerable<INakedObjectSurface> existingNakedObjects;

            if (existingNakedObject == null) {
                existingNakedObjects = new INakedObjectSurface[] {};
            }
            else if (existingNakedObject.Specification.IsParseable || !existingNakedObject.Specification.IsCollection) {
                // isParseable to catch strings 
                existingNakedObjects = new[] {existingNakedObject};
            }
            else {
                existingNakedObjects = existingNakedObject.ToEnumerable();
            }

            if (choice.Specification.IsEnum) {
                return existingNakedObjects.Any(no => no != null && choice.EnumIntegralValue == no.EnumIntegralValue);
            }

            if (choice.Specification.IsParseable) {
                return existingNakedObjects.Any(no => choice.TitleString.Trim() == no.TitleString.Trim());
            }
            return existingNakedObjects.Any(choice.Equals);
        }

        private static SelectListItem WrapChoice(this HtmlHelper html, INakedObjectSurface choice, INakedObjectSurface existingNakedObject) {
            if (choice == null) {
                return new SelectListItem();
            }
            return new SelectListItem {
                Text = GetTextForChoice(choice),
                Value = html.GetValueForChoice(choice),
                Selected = html.GetSelectedForChoice(choice, existingNakedObject)
            };
        }

        private static IEnumerable<SelectListItem> GetTriStateSet(bool? isChecked) {
            return new List<SelectListItem> {
                new SelectListItem {Text = MvcUi.TriState_NotSet, Value = "", Selected = !isChecked.HasValue},
                new SelectListItem {Text = MvcUi.TriState_True, Value = "true", Selected = isChecked.HasValue && isChecked.Value},
                new SelectListItem {Text = MvcUi.TriState_False, Value = "false", Selected = isChecked.HasValue && !isChecked.Value},
            };
        }

        private static IEnumerable<SelectListItem> GetChoicesSet(this HtmlHelper html,
                                                                 PropertyContext propertyContext,
                                                                 INakedObjectSurface existingNakedObject,
                                                                 IDictionary<string, INakedObjectSurface> values) {
            var nakedObjects = propertyContext.Property.GetChoices(propertyContext.Target, values.ToDictionary(kvp => kvp.Key, kvp => kvp.Value == null ? null : kvp.Value.Object)).ToList();
            return html.GetChoicesSet(nakedObjects, existingNakedObject);
        }

        private static IEnumerable<SelectListItem> GetChoicesSet(this HtmlHelper html, List<INakedObjectSurface> nakedObjects, INakedObjectSurface existingNakedObject) {
            nakedObjects.Insert(0, null); // empty entry at start of list 
            return nakedObjects.Select(no => html.WrapChoice(no, existingNakedObject));
        }

        private static IEnumerable<SelectListItem> GetChoicesSet(this HtmlHelper html,
                                                                 ParameterContext parameterContext,
                                                                 INakedObjectSurface existingNakedObject,
                                                                 IDictionary<string, INakedObjectSurface> values) {
            var nakedObjects = parameterContext.Parameter.GetChoices(parameterContext.Target, values.ToDictionary(kvp => kvp.Key, kvp => kvp.Value == null ? null : kvp.Value.Object)).ToList();
            return html.GetChoicesSet(nakedObjects, existingNakedObject);
        }

        private static string GetLabelTag(bool isEdit, string name, Func<string> getInputId) {
            var divTag = new TagBuilder("div");
            divTag.AddCssClass(IdConstants.Label);

            if (isEdit) {
                var labelTag = new TagBuilder("label");
                labelTag.MergeAttribute("for", getInputId());
                labelTag.SetInnerText(name + ":");
                divTag.InnerHtml = labelTag.ToString();
            }
            else {
                divTag.SetInnerText(name + ":");
            }

            return divTag.ToString();
        }

        private static string GetLabel(this HtmlHelper html, PropertyContext propertyContext) {
            bool isAutoComplete = propertyContext.IsEdit && propertyContext.Property.IsAutoCompleteEnabled;
            Func<string> propId = isAutoComplete ? (Func<string>) propertyContext.GetAutoCompleteFieldId : propertyContext.GetFieldInputId;
            return GetLabelTag(propertyContext.IsPropertyEdit || isAutoComplete, propertyContext.Property.Name, propId);
        }

        private static string GetLabel(this HtmlHelper html, ParameterContext parameterContext) {
            if (parameterContext.IsHidden) {
                return "";
            }
            bool isAutoComplete = parameterContext.Parameter.IsAutoCompleteEnabled;
            Func<string> parmId = isAutoComplete ? (Func<string>) parameterContext.GetParameterAutoCompleteId : parameterContext.GetParameterInputId;
            return GetLabelTag(parameterContext.IsParameterEdit || isAutoComplete, parameterContext.Parameter.Name, parmId);
        }

        internal static string GetSubmitButton(string classAttribute, string label, string name, RouteValueDictionary data) {
            return GetSubmitButton(classAttribute, label, name, ToNameValuePairs(data));
        }

        private static string GetSubmitButton(string classAttribute, string label, string name, string value) {
            var tag = new TagBuilder("button");
            tag.SetInnerText(label);
            tag.MergeAttribute("name", name);
            tag.MergeAttribute("value", value);
            tag.MergeAttribute("type", "submit");
            tag.MergeAttribute("title", label);
            if (classAttribute != null) {
                tag.MergeAttribute("class", classAttribute);
            }
            return tag.ToString();
        }

        private static string GetDisabledButton(string classAttribute, string label) {
            var tag = new TagBuilder("button");
            tag.SetInnerText(label);
            tag.MergeAttribute("title", label);
            if (classAttribute != null) {
                tag.MergeAttribute("class", classAttribute);
            }
            tag.MergeAttribute("disabled", "disabled");
            return tag.ToString();
        }

        private static ElementDescriptor RecentlyViewedAction(this HtmlHelper html, INakedObjectSpecificationSurface spec, ActionContext actionContext, string propertyName) {
            return new ElementDescriptor {
                TagType = "button",
                Value = MvcUi.RecentlyViewed,
                Attributes = new RouteValueDictionary(new {
                    title = MvcUi.RecentlyViewed,
                    name = IdConstants.FindAction,
                    type = "submit",
                    @class = actionContext.GetActionClass(),
                    value = ToNameValuePairs(new {
                        spec = spec.FullName,
                        contextObjectId = html.Surface().OidStrategy.GetObjectId(actionContext.Target),
                        contextActionId = actionContext.Action == null ? "" : actionContext.Action.Id,
                        propertyName
                    })
                })
            };
        }

        private static ElementDescriptor RemoveAction(string propertyName) {
            return new ElementDescriptor {
                TagType = "button",
                Value = MvcUi.Remove,
                Attributes = new RouteValueDictionary(new {
                    title = MvcUi.Remove,
                    name = IdConstants.SelectAction,
                    type = "submit",
                    @class = IdConstants.RemoveButtonClass,
                    value = ToNameValuePairs(new Dictionary<string, object> {{propertyName, String.Empty}})
                })
            };
        }

        private static string GetParameter(this HtmlHelper html, ParameterContext context, IList<ElementDescriptor> childElements, string propertyName) {
            string id = context.GetParameterInputId();
            string tooltip = context.Parameter.Description;

            if (context.Parameter.Specification.IsFile) {
                return html.GetFileParameter(context, id, tooltip);
            }

            if (context.Parameter.Specification.IsParseable) {
                return html.GetTextParameter(context, id, tooltip);
            }

            return html.GetReferenceParameter(context, id, tooltip, childElements, context.Parameter.Id == propertyName);
        }

        private static INakedObjectAssociationSurface[] CollectionAssociations(this HtmlHelper html,
                                                                               INakedObjectSurface[] collection,
                                                                               INakedObjectSpecificationSurface collectionSpec,
                                                                               Func<INakedObjectAssociationSurface, bool> filter,
                                                                               Func<INakedObjectAssociationSurface, int> order) {
            var assocs = collectionSpec.Properties.Where(filter).Where(a => collection.Any(a.IsVisible));

            if (order != null) {
                assocs = assocs.OrderBy(order);
            }

            return assocs.ToArray();
        }

        private static string CollectionItemTypeName(this HtmlHelper html, INakedObjectSurface collectionNakedObject) {
            return collectionNakedObject.ElementSpecification.ShortName;
        }

        private static string GetReferenceParameter(this HtmlHelper html, ParameterContext context, string id, string tooltip, IList<ElementDescriptor> childElements, bool addToThis) {
            var tag = new TagBuilder("div");
            tag.AddCssClass(IdConstants.ObjectName);

            if (context.IsHidden) {
                var suggestedItem = html.GetSuggestedItem(id, (INakedObjectSurface) null);
                string valueId = suggestedItem == null ? String.Empty : html.Surface().OidStrategy.GetObjectId(suggestedItem);
                tag.InnerHtml += html.CustomEncrypted(id, valueId);
            }
            else if (context.Parameter.IsChoicesEnabled == Choices.Single) {
                var htmlAttributes = new RouteValueDictionary(new {title = tooltip});
                html.AddDropDownControl(tag, htmlAttributes, context, id);
            }
            else if (context.Parameter.IsChoicesEnabled == Choices.Multiple) {
                var htmlAttributes = new RouteValueDictionary(new {title = tooltip});
                html.AddListBoxControl(tag, htmlAttributes, context, id);
            }
            else {
                var existingValue = html.GetParameterExistingValue(id, context);
                var suggestedItem = html.GetSuggestedItem(id, existingValue);
                string valueId = suggestedItem == null ? String.Empty : html.Surface().OidStrategy.GetObjectId(suggestedItem);

                string url = html.GenerateUrl("ValidateParameter", "Ajax", new RouteValueDictionary(new {
                    id = html.Surface().OidStrategy.GetObjectId(context.Target),
                    actionName = context.Action.Id,
                    parameterName = context.Parameter.Id,
                }));

                tag.MergeAttribute("data-validate", url);

                bool noFinder = context.EmbeddedInObject || !context.IsFindMenuEnabled();

                tag.InnerHtml += html.ObjectIcon(suggestedItem) +
                                 html.GetFieldValue(context, suggestedItem) +
                                 (noFinder ? String.Empty : html.FinderActions(context.Parameter.Specification, context, context.Parameter.Id)) +
                                 html.GetMandatoryIndicator(context) +
                                 html.ValidationMessage(context.Parameter.IsAutoCompleteEnabled ? context.GetParameterAutoCompleteId() : id) +
                                 html.CustomEncrypted(id, valueId);
                context.IsParameterEdit = false;
            }
            AddInsertedElements(childElements, addToThis, tag);
            return tag.ToString();
        }

        private static string GetReferenceField(this HtmlHelper html,
                                                PropertyContext propertyContext,
                                                string id,
                                                string tooltip,
                                                IList<ElementDescriptor> childElements,
                                                bool addToThis,
                                                bool readOnly,
                                                bool noFinder) {
            var tag = new TagBuilder("div");
            tag.AddCssClass(IdConstants.ObjectName);

            if (!propertyContext.Property.IsVisible(propertyContext.Target)) {
                var existingValue = propertyContext.GetValue(html.Surface());
                string value = existingValue == null ? String.Empty : html.Surface().OidStrategy.GetObjectId(existingValue);
                tag.InnerHtml += html.Encrypted(id, value).ToString();
                propertyContext.IsPropertyEdit = false;
            }
            else {
                if (readOnly) {
                    var valueNakedObject = propertyContext.GetValue(html.Surface());
                    string valueId = valueNakedObject == null ? String.Empty : html.Surface().OidStrategy.GetObjectId(valueNakedObject);

                    tag.InnerHtml += html.ObjectIcon(propertyContext.Property.GetNakedObject(propertyContext.Target)) +
                                     html.GetFieldValue(propertyContext) +
                                     html.CustomEncrypted(id, valueId);
                    propertyContext.IsPropertyEdit = false;
                }
                else if (propertyContext.Property.IsChoicesEnabled != Choices.NotEnabled) {
                    IEnumerable<SelectListItem> items = html.GetItems(id, propertyContext);

                    tag.InnerHtml += html.ObjectIcon(propertyContext.Property.GetNakedObject(propertyContext.Target)) +
                                     html.DropDownList(id, items, new {title = tooltip}) +
                                     html.GetMandatoryIndicator(propertyContext) +
                                     html.ValidationMessage(id);
                }
                else {
                    var valueNakedObject = html.GetExistingValue(id, propertyContext);
                    var suggestedItem = html.GetSuggestedItem(id, valueNakedObject);
                    string valueId = suggestedItem == null ? String.Empty : html.Surface().OidStrategy.GetObjectId(suggestedItem);

                    if (!propertyContext.Target.IsTransient) {
                        // do not only allow drag and drop onto transients - otherwise  attempt to validate 
                        // may depend on missing fields/data. cf check at top of AjaxControllerImpl:ValidateProperty

                        string url = html.GenerateUrl("ValidateProperty", "Ajax", new RouteValueDictionary(new {
                            id = html.Surface().OidStrategy.GetObjectId(propertyContext.Target),
                            propertyName = propertyContext.Property.Id
                        }));
                        tag.MergeAttribute("data-validate", url);
                    }
                    //Translates to: Only render finder if the Context is FindMenu enabled AND 
                    //calling code has not overridden it by setting noFinder to true.
                    noFinder = noFinder || !propertyContext.IsFindMenuEnabled();

                    tag.InnerHtml += html.ObjectIcon(suggestedItem) +
                                     html.GetFieldValue(propertyContext, suggestedItem) +
                                     (noFinder ? String.Empty : html.FinderActions(propertyContext.Property.Specification, new ActionContext(html.IdHelper(), propertyContext.Target, null), propertyContext.Property.Id)) +
                                     html.GetMandatoryIndicator(propertyContext) +
                                     html.ValidationMessage(propertyContext.Property.IsAutoCompleteEnabled ? propertyContext.GetAutoCompleteFieldId() : id) +
                                     html.CustomEncrypted(id, valueId);
                    propertyContext.IsPropertyEdit = false;
                }
            }
            AddInsertedElements(childElements, addToThis, tag);
            return tag.ToString();
        }

        private static string GetCollectionItem(this HtmlHelper html,
                                                INakedObjectSurface item,
                                                string id) {
            var tag = new TagBuilder("div");
            tag.AddCssClass(IdConstants.ObjectName);
            string value = html.Surface().OidStrategy.GetObjectId(item);
            tag.InnerHtml += html.Hidden(id, value, new {id = String.Empty}).ToString();
            return tag.ToString();
        }

        private static void AddInsertedElements(IList<ElementDescriptor> childElements, bool addToThis, TagBuilder parent) {
            if (addToThis && childElements.Any()) {
                foreach (ElementDescriptor field in childElements) {
                    field.BuildElement(parent);
                }
            }
        }

        private static string GetCollectionAsTable(this HtmlHelper html, PropertyContext propertyContext) {
            var collectionNakedObject = propertyContext.GetValue(html.Surface());
            bool any = collectionNakedObject.ToEnumerable().Any();
            Func<INakedObjectSurface, string> linkFunc = item => html.Object(html.ObjectTitle(item).ToString(), IdConstants.ViewAction, item.Object).ToString();

            Func<INakedObjectAssociationSurface, bool> filterFunc;
            Func<INakedObjectAssociationSurface, int> orderFunc;
            bool withTitle;

            GetTableColumnInfo(propertyContext.Property, out filterFunc, out orderFunc, out withTitle);

            return (any ? html.GetCollectionDisplayLinks(propertyContext) : GetCollectionTitle(propertyContext, 0)) +
                   html.CollectionTable(collectionNakedObject, linkFunc, filterFunc, orderFunc, false, false, withTitle);
        }

        internal static void GetTableColumnInfo(INakedObjectActionSurface holder, out Func<INakedObjectAssociationSurface, bool> filterFunc, out Func<INakedObjectAssociationSurface, int> orderFunc, out bool withTitle) {
            var tableViewData = holder == null ? null : holder.TableViewData;

            GetTableColumnInfo(out filterFunc, out orderFunc, out withTitle, tableViewData);
        }

        internal static void GetTableColumnInfo(INakedObjectAssociationSurface holder, out Func<INakedObjectAssociationSurface, bool> filterFunc, out Func<INakedObjectAssociationSurface, int> orderFunc, out bool withTitle) {
            var tableViewData = holder == null ? null : holder.TableViewData;

            GetTableColumnInfo(out filterFunc, out orderFunc, out withTitle, tableViewData);
        }

        private static void GetTableColumnInfo(out Func<INakedObjectAssociationSurface, bool> filterFunc, out Func<INakedObjectAssociationSurface, int> orderFunc, out bool withTitle, Tuple<bool, string[]> tableViewData) {
            if (tableViewData == null) {
                filterFunc = x => true;
                orderFunc = null;
                withTitle = true;
            }
            else {
                string[] columns = tableViewData.Item2;
                filterFunc = x => columns.Contains(x.Id);
                orderFunc = x => Array.IndexOf(columns, x.Id);
                withTitle = tableViewData.Item1;
            }
        }

        internal static bool RenderEagerly(INakedObjectAssociationSurface holder) {
            return holder.RenderEagerly;
        }

        internal static bool RenderEagerly(INakedObjectActionSurface holder) {
            return holder != null && holder.RenderEagerly;
        }

        internal static bool DoNotCount(INakedObjectAssociationSurface holder) {
            return holder.DoNotCount;
        }

        private static string GetCollectionAsSummary(this HtmlHelper html, PropertyContext propertyContext) {
            if (DoNotCount(propertyContext.Property)) {
                return html.GetCollectionDisplayLinks(propertyContext);
            }
            int count = propertyContext.Property.Count(propertyContext.Target);
            return (count > 0 ? html.GetCollectionDisplayLinks(propertyContext) : String.Empty) + GetCollectionTitle(propertyContext, count);
        }

        private static string GetCollectionAsList(this HtmlHelper html, PropertyContext propertyContext) {
            var collectionNakedObject = propertyContext.GetValue(html.Surface());
            bool any = collectionNakedObject.ToEnumerable().Any();
            Func<INakedObjectSurface, string> linkFunc = item => html.Object(html.ObjectTitle(item).ToString(), IdConstants.ViewAction, item.Object).ToString();
            return (any ? html.GetCollectionDisplayLinks(propertyContext) : GetCollectionTitle(propertyContext, 0)) +
                   html.CollectionTable(collectionNakedObject, linkFunc, x => false, null, false, false, true);
        }

        private static string GetChildCollection(this HtmlHelper html, PropertyContext propertyContext) {
            string displayType = html.ViewData.ContainsKey(propertyContext.GetFieldId()) ? (string) html.ViewData[propertyContext.GetFieldId()] : String.Empty;
            bool renderEagerly = RenderEagerly(propertyContext.Property);

            var tag = new TagBuilder("div");
            if (displayType == IdConstants.TableDisplayFormat || (String.IsNullOrWhiteSpace(displayType) && renderEagerly)) {
                tag.AddCssClass(IdConstants.CollectionTableName);
                tag.InnerHtml += html.GetCollectionAsTable(propertyContext);
            }
            else if (displayType == IdConstants.ListDisplayFormat) {
                tag.AddCssClass(IdConstants.CollectionListName);
                tag.InnerHtml += html.GetCollectionAsList(propertyContext);
            }
            else {
                tag.AddCssClass(IdConstants.CollectionSummaryName);
                tag.InnerHtml += html.GetCollectionAsSummary(propertyContext);
            }

            return tag.ToString();
        }

        private static ElementDescriptor GetSelectionCollection(this HtmlHelper html, INakedObjectSurface collectionNakedObject, INakedObjectSurface targetNakedObject, string propertyName) {
            Func<INakedObjectSurface, string> linkFunc = item => WrapInDiv(html.ObjectIconAndDetailsLink(item.TitleString, IdConstants.ViewAction, item.Object) + " " +
                                                                           GetSubmitButton(IdConstants.SelectButtonClass, MvcUi.Select, IdConstants.SelectAction, new RouteValueDictionary(new {id = html.Surface().OidStrategy.GetObjectId(targetNakedObject)}) {{propertyName, html.Surface().OidStrategy.GetObjectId(item)}}), IdConstants.ObjectName).ToString();

            return new ElementDescriptor {
                TagType = "div",
                Value = html.CollectionTable(collectionNakedObject, linkFunc, x => false, null, false, false, true),
                Attributes = new RouteValueDictionary(new {
                    @class = IdConstants.CollectionListName
                })
            };
        }

        private static string GetCollectionTitle(PropertyContext propertyContext, int count) {
            var tag = new TagBuilder("div");
            tag.AddCssClass(IdConstants.ObjectName);
            tag.MergeAttribute("title", "");
            var property = propertyContext.Property;
            var coll = propertyContext.Property.GetNakedObject(propertyContext.Target);
            tag.InnerHtml += property.GetTitle(coll);
            return tag.ToString();
        }

        private static string GetViewField(this HtmlHelper html, PropertyContext propertyContext, string tooltip, bool addIcon = true, bool inTable = false) {
            var tag = new TagBuilder("div");

            if (propertyContext.Property.IsVisible(propertyContext.Target)) {
                string value = html.GetFieldValue(propertyContext, inTable);
                string cls = propertyContext.Property.Specification.IsParseable ? IdConstants.ValueName : IdConstants.ObjectName;

                if (propertyContext.Property.NumberOfLines > 1) {
                    cls += (" " + IdConstants.MultilineDisplayFormat);
                }

                tag.AddCssClass(cls);
                tag.MergeAttribute("title", tooltip);
                if (!propertyContext.Property.Specification.IsParseable && addIcon) {
                    tag.InnerHtml += html.ObjectIcon(propertyContext.Property.GetNakedObject(propertyContext.Target));
                }
                tag.InnerHtml += value;
            }
            return tag.ToString();
        }

        private static string GetFileParameter(this HtmlHelper html, ParameterContext context, string id, string tooltip) {
            var tag = new TagBuilder("div");
            tag.AddCssClass(IdConstants.ValueName);

            var fileInput = new TagBuilder("input");

            fileInput.MergeAttribute("type", "file");
            fileInput.MergeAttribute("id", id);
            fileInput.MergeAttribute("name", id);
            fileInput.MergeAttribute("title", tooltip);

            string input = fileInput.ToString();

            tag.InnerHtml += input +
                             html.GetMandatoryIndicator(context) +
                             html.ValidationMessage(id);

            return tag.ToString();
        }

        private static string GetTextParameter(this HtmlHelper html, ParameterContext context, string id, string tooltip) {
            var tag = new TagBuilder("div");
            tag.AddCssClass(IdConstants.ValueName);
            var htmlAttributes = new RouteValueDictionary(new {title = tooltip});

            html.AddClientValidationAttributes(context, htmlAttributes);

            if (context.IsHidden) {
                object obj = html.ViewData[id];
                tag.InnerHtml += html.Encrypted(id, obj.ToString());
            }
            else if (context.Parameter.Specification.IsBoolean) {
                if (context.Parameter.IsNullable) {
                    html.AddTriState(tag, htmlAttributes, id, null);
                }
                else {
                    html.AddCheckBox(tag, htmlAttributes, id, null);
                }
            }
            else if (context.Parameter.Specification.IsDateTime) {
                html.AddDateTimeControl(tag, htmlAttributes, context, id, html.GetFieldValue(context, id));
            }
            else if (context.Parameter.IsPassword) {
                html.AddPasswordControl(tag, htmlAttributes, context, id, html.GetFieldValue(context, id));
            }
            else if (context.Parameter.IsChoicesEnabled == Choices.Single) {
                html.AddDropDownControl(tag, htmlAttributes, context, id);
            }
            else if (context.Parameter.IsChoicesEnabled == Choices.Multiple) {
                html.AddListBoxControl(tag, htmlAttributes, context, id);
            }
            else if (context.Parameter.IsAutoCompleteEnabled) {
                html.AddAutoCompleteControl(tag, htmlAttributes, context, html.GetParameterDefaultValue(id, context));
            }
            else {
                html.AddTextControl(tag, htmlAttributes, context, id, null);
            }
            return tag.ToString();
        }

        private static string GetFieldValue(this HtmlHelper html, ParameterContext context, string id) {
            var valueNakedObject = html.GetParameterDefaultValue(id, context);
            return context.Parameter.GetMaskedValue(valueNakedObject);
        }

        private static string GetTextControl(this HtmlHelper html, string id, int numberOfLines, int width, int maxLength, string value, RouteValueDictionary htmlAttributes) {
            MvcHtmlString textBox;
            if (numberOfLines > 1) {
                textBox = html.TextArea(id, value, numberOfLines, width, htmlAttributes);
            }
            else {
                htmlAttributes["size"] = width;
                if (maxLength > 0) {
                    htmlAttributes["maxlength"] = maxLength;
                }
                textBox = html.TextBox(id, value, htmlAttributes);
            }

            return textBox.ToString();
        }

        private static string MandatoryIndicator(this HtmlHelper html) {
            var tag = new TagBuilder("span");
            tag.AddCssClass(html.IdHelper().GetMandatoryIndicatorClass());
            tag.SetInnerText(html.IdHelper().GetMandatoryIndicator());
            return tag.ToString();
        }

        private static bool IsMandatory(this HtmlHelper html, ParameterContext parameterContext) {
            return (parameterContext.Parameter.IsMandatory/*&& parameterContext.Parameter.IsUsable(parameterContext.Target).IsAllowed*/);
        }

        private static bool IsMandatory(this HtmlHelper html, PropertyContext propertyContext) {
            return (propertyContext.Property.IsMandatory && propertyContext.Property.IsUsable(propertyContext.Target).IsAllowed);
        }

        private static bool IsMandatory(this HtmlHelper html, FeatureContext context) {
            if (context is PropertyContext) {
                return html.IsMandatory(context as PropertyContext);
            }
            if (context is ParameterContext) {
                return html.IsMandatory(context as ParameterContext);
            }
            // todo is this correct exception
            throw new BadRequestNOSException(String.Format("Unexpected context type {0}", context.GetType()));

            //throw new UnexpectedCallException(string.Format("Unexpected context type {0}", context.GetType()));
        }

        private static bool IsAutoComplete(FeatureContext context) {
            if (context is PropertyContext) {
                return (context as PropertyContext).Property.IsAutoCompleteEnabled;
            }
            if (context is ParameterContext) {
                return (context as ParameterContext).Parameter.IsAutoCompleteEnabled;
            }
            // todo is this correct exception
            throw new BadRequestNOSException(String.Format("Unexpected context type {0}", context.GetType()));

            // throw new UnexpectedCallException(string.Format("Unexpected context type {0}", context.GetType()));
        }

        private static bool IsAjax(FeatureContext context) {
            if (context is PropertyContext) {
                return (context as PropertyContext).Property.IsAjax;
            }
            if (context is ParameterContext) {
                return (context as ParameterContext).Parameter.IsAjax;
            }
            // todo is this correct exception
            throw new BadRequestNOSException(String.Format("Unexpected context type {0}", context.GetType()));

            //throw new UnexpectedCallException(string.Format("Unexpected context type {0}", context.GetType()));
        }

        private static string GetMandatoryIndicator(this HtmlHelper html, FeatureContext context) {
            return html.IsMandatory(context) ? html.MandatoryIndicator() : String.Empty;
        }

        private static bool ShouldClearValue(object value) {
            TypeCode t = Convert.GetTypeCode(value);

            switch (t) {
                case (TypeCode.DateTime):
                    if (((DateTime) value).Ticks == 0) {
                        return true;
                    }
                    return false;
                case (TypeCode.Byte):
                case (TypeCode.Char):
                case (TypeCode.Decimal):
                case (TypeCode.Double):
                case (TypeCode.Int16):
                case (TypeCode.Int32):
                case (TypeCode.Int64):
                case (TypeCode.SByte):
                case (TypeCode.Single):
                case (TypeCode.UInt16):
                case (TypeCode.UInt32):
                case (TypeCode.UInt64):
                    if (Convert.ToInt64(value) == 0) {
                        return true;
                    }
                    return false;
                default:
                    return false;
            }
        }

        private static string GetTextField(this HtmlHelper html, PropertyContext propertyContext, string id, string tooltip, bool readOnly) {
            var tag = new TagBuilder("div");
            tag.AddCssClass(IdConstants.ValueName);

            var htmlAttributes = new RouteValueDictionary(new {title = tooltip});

            html.AddClientValidationAttributes(propertyContext, htmlAttributes);

            if (!propertyContext.Property.IsVisible(propertyContext.Target)) {
                tag.InnerHtml += html.Encrypted(id, html.GetRawValue(propertyContext)).ToString();
                propertyContext.IsPropertyEdit = false;
            }

            else if (propertyContext.Property.Specification.IsBoolean && !readOnly) {
                var state = propertyContext.Property.GetNakedObject(propertyContext.Target).GetDomainObject<bool?>();

                if (propertyContext.Property.IsNullable) {
                    html.AddTriState(tag, htmlAttributes, id, state);
                }
                else {
                    html.AddCheckBox(tag, htmlAttributes, id, state);
                }
            }
            else if (propertyContext.Property.Specification.IsDateTime && !readOnly) {
                html.AddDateTimeControl(tag, htmlAttributes, propertyContext, id, html.GetPropertyValue(propertyContext));
            }
            else if (propertyContext.Property.IsPassword && !readOnly) {
                html.AddPasswordControl(tag, htmlAttributes, propertyContext, id, html.GetPropertyValue(propertyContext));
            }
            else if (propertyContext.Property.IsChoicesEnabled != Choices.NotEnabled && !readOnly) {
                html.AddDropDownControl(tag, htmlAttributes, propertyContext, id);
            }
            else if (propertyContext.Property.IsAutoCompleteEnabled && !readOnly) {
                html.AddAutoCompleteControl(tag, htmlAttributes, propertyContext, propertyContext.Property.GetNakedObject(propertyContext.Target));
            }
            else {
                string rawValue = html.GetRawValue(propertyContext);
                if (readOnly) {
                    tag.InnerHtml += html.GetFieldValue(propertyContext) + html.CustomEncrypted(id, rawValue);
                    propertyContext.IsPropertyEdit = false;
                }
                else {
                    html.AddTextControl(tag, htmlAttributes, propertyContext, id, html.ZeroValueIfTransientAndNotSet(propertyContext, rawValue));
                }
            }

            return tag.ToString();
        }

        private static void RangeValidation(RangeData rangeData, RouteValueDictionary htmlAttributes) {
            //Because JQuery client-side validation will not work for Date fields

            if (rangeData.IsValid && !rangeData.IsDateRange) {
                htmlAttributes["data-val"] = "true";
                htmlAttributes["data-val-number"] = MvcUi.InvalidEntry;
                htmlAttributes["data-val-range-min"] = rangeData.Min;
                htmlAttributes["data-val-range-max"] = rangeData.Max;
                htmlAttributes["data-val-range"] = String.Format(Resources.NakedObjects.RangeMismatch, rangeData.Min, rangeData.Max);
            }
        }

        private static void RegExValidation(RegexData regexData, RouteValueDictionary htmlAttributes) {
            if (regexData.IsValid) {
                htmlAttributes["data-val"] = "true";
                htmlAttributes["data-val-regex-pattern"] = regexData.Pattern;
                htmlAttributes["data-val-regex"] = regexData.FailureMessage ?? MvcUi.InvalidEntry;
            }
        }

        private static void MaxlengthValidation(int maxlength, RouteValueDictionary htmlAttributes) {
            if (maxlength > 0) {
                htmlAttributes["data-val"] = "true";
                htmlAttributes["data-val-length-max"] = maxlength;
                htmlAttributes["data-val-length"] = String.Format(Resources.NakedObjects.MaximumLengthMismatch, maxlength);
            }
        }

        private static void AddRemoteValidation(this HtmlHelper html, FeatureContext context, RouteValueDictionary htmlAttributes) {
            htmlAttributes["data-val"] = "true";
            htmlAttributes["data-val-remote"] = MvcUi.InvalidEntry;

            string action;
            RouteValueDictionary routeValueDictionary;

            if (context is PropertyContext) {
                var propertyContext = context as PropertyContext;
                action = "ValidateProperty";
                routeValueDictionary = new RouteValueDictionary(new {
                    id = html.Surface().OidStrategy.GetObjectId(propertyContext.Target),
                    propertyName = propertyContext.Property.Id
                });
            }
            else {
                // (context is ParameterContext)
                var parameterContext = context as ParameterContext;
                action = "ValidateParameter";
                routeValueDictionary = new RouteValueDictionary(new {
                    id = html.Surface().OidStrategy.GetObjectId(parameterContext.Target),
                    actionName = parameterContext.Action.Id,
                    parameterName = parameterContext.Parameter.Id
                });
            }

            htmlAttributes["data-val-remote-url"] = html.GenerateUrl(action, "Ajax", routeValueDictionary);
        }

        private static void AddClientValidationAttributes(this HtmlHelper html, FeatureContext context, RouteValueDictionary htmlAttributes) {
            if (html.IsMandatory(context)) {
                htmlAttributes["data-val"] = "true";
                htmlAttributes["data-val-required"] = MvcUi.Mandatory;
            }

            // remote validation and autocomplete on reference fields do not play nicely together as the actual value is in the hidden field
            if (IsAjax(context) && !IsAutoComplete(context)) {
                html.AddRemoteValidation(context, htmlAttributes);
            }

            var range = GetRange(context);
            var regex = GetRegex(context);
            var maxLength = GetMaxlength(context).GetValueOrDefault(0);

            RangeValidation(range, htmlAttributes);

            RegExValidation(regex, htmlAttributes);

            MaxlengthValidation(maxLength, htmlAttributes);
        }

        private static int? GetMaxlength(FeatureContext context) {
            if (context is PropertyContext) {
                return (context as PropertyContext).Property.MaxLength;
            }
            if (context is ParameterContext) {
                return (context as ParameterContext).Parameter.MaxLength;
            }
            // todo is this correct exception
            throw new BadRequestNOSException(String.Format("Unexpected context type {0}", context.GetType()));

            //throw new UnexpectedCallException(string.Format("Unexpected context type {0}", context.GetType()));
        }

        private class RegexData {
            public RegexData(Tuple<Regex, string> tuple) {
                if (tuple == null) {
                    IsValid = false;
                    return;
                }

                Pattern = tuple.Item1.ToString();
                FailureMessage = tuple.Item2;
                IsValid = true;
            }

            public bool IsValid { get; set; }
            public string FailureMessage { get; set; }
            public object Pattern { get; set; }
        }

        private static RegexData GetRegex(FeatureContext context) {
            if (context is PropertyContext) {
                return new RegexData((context as PropertyContext).Property.RegEx);
            }
            if (context is ParameterContext) {
                return new RegexData((context as ParameterContext).Parameter.RegEx);
            }
            // todo is this correct exception
            throw new BadRequestNOSException(String.Format("Unexpected context type {0}", context.GetType()));

            //throw new UnexpectedCallException(string.Format("Unexpected context type {0}", context.GetType()));
        }

        private class RangeData {
            public RangeData(Tuple<IConvertible, IConvertible, bool> tuple) {
                if (tuple == null) {
                    IsValid = false;
                    return;
                }

                Min = tuple.Item1.ToString(CultureInfo.InvariantCulture);
                Max = tuple.Item2.ToString(CultureInfo.InvariantCulture);
                IsDateRange = tuple.Item3;
                IsValid = true;
            }

            public string Min { get; set; }
            public string Max { get; set; }
            public bool IsDateRange { get; set; }
            public bool IsValid { get; set; }
        }

        private static RangeData GetRange(FeatureContext context) {
            if (context is PropertyContext) {
                return new RangeData((context as PropertyContext).Property.Range);
            }
            if (context is ParameterContext) {
                return new RangeData((context as ParameterContext).Parameter.Range);
            }
            // todo is this correct exception
            throw new BadRequestNOSException(String.Format("Unexpected context type {0}", context.GetType()));

            //throw new UnexpectedCallException(string.Format("Unexpected context type {0}", context.GetType()));
        }

        private static void AddTextControl(this HtmlHelper html, TagBuilder tag, RouteValueDictionary htmlAttributes, PropertyContext context, string id, string value) {
            var typicalLength = context.Property.TypicalLength;
            var maxLength = context.Property.MaxLength.GetValueOrDefault(0);
            int numberOfLines = context.Property.NumberOfLines;
            int width = context.Property.Width;

            width = width == 0 ? (typicalLength == 0 ? 20 : typicalLength)/numberOfLines : width;

            string textBox = html.GetTextControl(id, numberOfLines, width, maxLength, value, htmlAttributes);
            tag.InnerHtml += textBox + html.GetMandatoryIndicator(context) + html.ValidationMessage(id);
        }

        private static void AddTextControl(this HtmlHelper html, TagBuilder tag, RouteValueDictionary htmlAttributes, ParameterContext context, string id, string value) {
            var typicalLength = context.Parameter.TypicalLength;
            var maxLength = context.Parameter.MaxLength.GetValueOrDefault(0);
            int numberOfLines = context.Parameter.NumberOfLines;
            int width = context.Parameter.Width;

            width = width == 0 ? (typicalLength == 0 ? 20 : typicalLength)/numberOfLines : width;

            string textBox = html.GetTextControl(id, numberOfLines, width, maxLength, value, htmlAttributes);
            tag.InnerHtml += textBox + html.GetMandatoryIndicator(context) + html.ValidationMessage(id);
        }

        private static void AddDropDownControl(this HtmlHelper html, TagBuilder tag, RouteValueDictionary htmlAttributes, FeatureContext context, string id) {
            IEnumerable<SelectListItem> items = html.GetItems(id, context);
            tag.InnerHtml += html.DropDownList(id, items, htmlAttributes) + html.GetMandatoryIndicator(context) + html.ValidationMessage(id);
        }

        private static void AddListBoxControl(this HtmlHelper html, TagBuilder tag, RouteValueDictionary htmlAttributes, FeatureContext context, string id) {
            IEnumerable<SelectListItem> items = html.GetItems(id, context).ToList();
            int lines = items.Count() < 10 ? items.Count() : 10;
            htmlAttributes.Add("size", lines);
            tag.InnerHtml += html.ListBox(id, items, htmlAttributes) + html.GetMandatoryIndicator(context) + html.ValidationMessage(id);
        }

        private static void AddAutoCompleteControl(this HtmlHelper html, TagBuilder tag, RouteValueDictionary htmlAttributes, ParameterContext context, INakedObjectSurface valueNakedObject) {
            tag.InnerHtml += html.GetAutoCompleteTextBox(context, htmlAttributes, valueNakedObject) + html.GetMandatoryIndicator(context) + html.ValidationMessage(context.GetParameterAutoCompleteId());
        }

        private static void AddAutoCompleteControl(this HtmlHelper html, TagBuilder tag, RouteValueDictionary htmlAttributes, PropertyContext context, INakedObjectSurface valueNakedObject) {
            tag.InnerHtml += html.GetAutoCompleteTextBox(context, htmlAttributes, valueNakedObject) + html.GetMandatoryIndicator(context) + html.ValidationMessage(context.GetAutoCompleteFieldId());
        }

        private static string GetPropertyValue(this HtmlHelper html, PropertyContext propertyContext) {
            var valueNakedObject = propertyContext.GetValue(html.Surface());
            string value = propertyContext.Property.GetMaskedValue(valueNakedObject);
            value = html.ZeroValueIfTransientAndNotSet(propertyContext, value);
            return value;
        }

        private static void AddDateTimeControl(this HtmlHelper html, TagBuilder tag, RouteValueDictionary htmlAttributes, PropertyContext context, string id, string value) {
            var typicalLengthFacet = context.Property.TypicalLength;
            htmlAttributes["size"] = typicalLengthFacet == 0 ? 20 : typicalLengthFacet;
            htmlAttributes["class"] = "datetime";
            tag.InnerHtml += html.TextBox(id, value, htmlAttributes) + html.GetMandatoryIndicator(context) + html.ValidationMessage(id);
        }

        private static void AddDateTimeControl(this HtmlHelper html, TagBuilder tag, RouteValueDictionary htmlAttributes, ParameterContext context, string id, string value) {
            var typicalLengthFacet = context.Parameter.TypicalLength;
            htmlAttributes["size"] = typicalLengthFacet == 0 ? 20 : typicalLengthFacet;
            htmlAttributes["class"] = "datetime";
            tag.InnerHtml += html.TextBox(id, value, htmlAttributes) + html.GetMandatoryIndicator(context) + html.ValidationMessage(id);
        }

        private static void AddPasswordControl(this HtmlHelper html, TagBuilder tag, RouteValueDictionary htmlAttributes, ParameterContext context, string id, string value) {
            var length = context.Parameter.TypicalLength;
            htmlAttributes["size"] = length == 0 ? 20 : length;
            tag.InnerHtml += html.Password(id, value, htmlAttributes) + html.GetMandatoryIndicator(context) + html.ValidationMessage(id);
        }

        private static void AddPasswordControl(this HtmlHelper html, TagBuilder tag, RouteValueDictionary htmlAttributes, PropertyContext context, string id, string value) {
            var length = context.Property.TypicalLength;
            htmlAttributes["size"] = length == 0 ? 20 : length;
            tag.InnerHtml += html.Password(id, value, htmlAttributes) + html.GetMandatoryIndicator(context) + html.ValidationMessage(id);
        }

        private static void AddTriState(this HtmlHelper html, TagBuilder tag, RouteValueDictionary htmlAttributes, string id, bool? isChecked) {
            tag.InnerHtml += html.DropDownList(id, GetTriStateSet(isChecked), htmlAttributes).ToString();
            tag.InnerHtml += html.ValidationMessage(id);
        }

        private static void AddCheckBox(this HtmlHelper html, TagBuilder tag, RouteValueDictionary htmlAttributes, string id, bool? isChecked) {
            if (isChecked == null) {
                tag.InnerHtml += html.CheckBox(id, htmlAttributes).ToString();
            }
            else {
                tag.InnerHtml += html.CheckBox(id, isChecked.Value, htmlAttributes).ToString();
            }
            tag.InnerHtml += html.ValidationMessage(id);
        }

        private static string ZeroValueIfTransientAndNotSet(this HtmlHelper html, PropertyContext propertyContext, string value) {
            if (propertyContext.Target.IsTransient && !String.IsNullOrEmpty(value)) {
                var valueNakedObject = propertyContext.GetValue(html.Surface());

                if (!propertyContext.Property.DefaultTypeIsExplicit(propertyContext.Target) && ShouldClearValue(valueNakedObject.Object)) {
                    value = null;
                }
            }
            return value;
        }

        private static string GetEditValue(this HtmlHelper html,
                                           PropertyContext propertyContext,
                                           IList<ElementDescriptor> childElements,
                                           bool addToThis,
                                           bool noFinder) {
            string tooltip = propertyContext.Property.Description;
            string id = propertyContext.GetFieldInputId();
            if (propertyContext.Property.IsCollection) {
                propertyContext.IsPropertyEdit = false;
                return html.GetChildCollection(propertyContext);
            }
            var consent = propertyContext.Property.IsUsable(propertyContext.Target);
            if (consent.IsVetoed && !propertyContext.Target.IsTransient) {
                propertyContext.IsPropertyEdit = false;
                return html.GetViewField(propertyContext, consent.Reason);
            }

            bool readOnly = consent.IsVetoed && propertyContext.Target.IsTransient;

            // for the moment do not allow file properties to be edited 
            if (propertyContext.Property.Specification.IsFileAttachment) {
                // return html.GetFileProperty(propertyContext, id, tooltip);
                readOnly = true;
            }

            if (propertyContext.Property.Specification.IsParseable) {
                return html.GetTextField(propertyContext, id, tooltip, readOnly);
            }

            if (propertyContext.Property.IsInline) {
                var inlineNakedObject = propertyContext.GetValue(html.Surface());
                TagBuilder elementSet = ElementDescriptor.BuildElementSet(html.EditObjectFields(inlineNakedObject, propertyContext, x => true, null, true));
                html.AddAjaxDataUrlsToElementSet(inlineNakedObject, elementSet, propertyContext);

                return elementSet.ToString();
            }

            return html.GetReferenceField(propertyContext, id, tooltip, childElements, addToThis, readOnly, noFinder);
        }

        private static string GetHiddenValue(this HtmlHelper html, PropertyContext propertyContext, string id, bool invariant) {
            var tag = new TagBuilder("div");
            string value;

            if (propertyContext.Property.Specification.IsParseable) {
                tag.AddCssClass(IdConstants.ValueName);
                value = invariant ? html.GetInvariantValue(propertyContext) : html.GetRawValue(propertyContext);
            }
            else {
                tag.AddCssClass(IdConstants.ObjectName);
                var existingValue = propertyContext.GetValue(html.Surface());
                value = existingValue == null ? String.Empty : html.Surface().OidStrategy.GetObjectId(existingValue);
            }
            tag.InnerHtml += html.Encrypted(id, value).ToString();
            return tag.ToString();
        }

        private static string GetViewValue(this HtmlHelper html, PropertyContext propertyContext) {
            string tooltip = propertyContext.Property.Description;
            if (propertyContext.Property.IsCollection && !propertyContext.Property.Specification.IsFileAttachment) {
                return html.GetChildCollection(propertyContext);
            }

            return html.GetViewField(propertyContext, tooltip);
        }

        private static string ToNameValuePairs(object data) {
            return ToNameValuePairs(new RouteValueDictionary(data));
        }

        private static string ToNameValuePairs(IEnumerable<KeyValuePair<string, object>> data) {
            var value = new StringBuilder();
            foreach (var item in data) {
                value.Append(item.Key).Append("=").Append(item.Value).Append("&");
            }

            if (value.Length > 1) {
                value.Remove(value.Length - 1, 1);
            }

            return value.ToString();
        }

        internal static IList<ElementDescriptor> WrapInCollection(this IEnumerable<ElementDescriptor> collection, string tagType, object routeValues) {
            List<ElementDescriptor> children = collection.ToList();
            if (children.Any()) {
                return new ElementDescriptor {
                    TagType = tagType,
                    Value = String.Empty,
                    Children = children,
                    Attributes = new RouteValueDictionary(routeValues)
                }.InList();
            }
            return new List<ElementDescriptor>();
        }

        private static string GetButtonNameValues(this HtmlHelper html,
                                                  ActionContext targetActionContext,
                                                  ActionContext actionContext,
                                                  INakedObjectSurface subEditNakedObject,
                                                  string propertyName) {
            var data = new RouteValueDictionary(new {
                targetActionId = targetActionContext.Action.Id, //e.g.  FindEmployeeByName
                targetObjectId = html.Surface().OidStrategy.GetObjectId(targetActionContext.Target), //e.g. Expenses.ExpenseEmployees.EmployeeRepository;1;System.Int32;0;False;;0
                contextObjectId = html.Surface().OidStrategy.GetObjectId(actionContext.Target), //e.g. Expenses.ExpenseClaims.Claim;1;System.Int32;1;False;;0
                contextActionId = actionContext.Action == null ? "" : actionContext.Action.Id, //e.g. Approver
                propertyName
            });

            if (subEditNakedObject != null) {
                data.Add("subEditObjectId", html.Surface().OidStrategy.GetObjectId(subEditNakedObject));
            }

            UpdatePagingValues(html, data);

            return ToNameValuePairs(data);
        }

        private static RouteValueDictionary GetActionAttributes(this HtmlHelper html,
                                                                string name,
                                                                ActionContext targetActionContext,
                                                                ActionContext actionContext,
                                                                string propertyName) {
            var actionContextAction = actionContext.Action;
            var actionContextTarget = actionContext.Target;
            var targetActionContextTarget = targetActionContext.Target;
            var targetActionContextAction = targetActionContext.Action;

            return new RouteValueDictionary(new {
                @class = targetActionContext.GetActionClass(),
                id = html.IdHelper().GetActionId(propertyName, actionContextAction, actionContextTarget, targetActionContextTarget, targetActionContextAction),
                name,
                type = "submit",
                value = html.GetButtonNameValues(targetActionContext, actionContext, null, propertyName)
            });
        }

        private static ElementDescriptor GetActionInstanceElementDescriptor(this HtmlHelper html,
                                                                            ActionContext targetActionContext,
                                                                            ActionContext actionContext,
                                                                            string propertyName,
                                                                            Tuple<bool, string> disabled = null) {
            if (disabled != null && disabled.Item1) {
                string value;
                RouteValueDictionary attributes;
                string tagType = html.GetDuplicateAction(actionContext, disabled.Item2, out value, out attributes);

                return new ElementDescriptor {
                    TagType = tagType,
                    Value = value,
                    Attributes = attributes
                };
            }

            return new ElementDescriptor {
                TagType = "button",
                Value = targetActionContext.Action.Name,
                Attributes = html.GetActionAttributes(IdConstants.ActionFindAction, targetActionContext, actionContext, propertyName)
            };
        }

        private static ElementDescriptor GetActionElementDescriptor(this HtmlHelper html,
                                                                    ActionContext targetActionContext,
                                                                    ActionContext actionContext,
                                                                    INakedObjectSpecificationSurface spec,
                                                                    string propertyName,
                                                                    Tuple<bool, string> disabled = null) {
            return html.GetActionInstanceElementDescriptor(targetActionContext, actionContext, propertyName, disabled);
        }

        private static IList<ElementDescriptor> FinderActionsForField(this HtmlHelper html,
                                                                      ActionContext actionContext,
                                                                      INakedObjectSpecificationSurface fieldSpec,
                                                                      string propertyName) {
            var finderActions = fieldSpec.GetFinderActions();
            var descriptors = new List<ElementDescriptor>();
            foreach (var finderAction in finderActions) {
                var serviceSpec = finderAction.OnType;
                var service = html.Surface().GetServices().List.Single(s => s.Specification.Equals(serviceSpec));
                var targetActionContext = new ActionContext(html.IdHelper(), service, finderAction);
                var ed = html.GetActionElementDescriptor(targetActionContext, actionContext, fieldSpec, propertyName, html.IsDuplicate(finderActions, finderAction));
                descriptors.Add(ed);
            }
            return descriptors;
        }

        #endregion

        #region private

        private static string GetDisplayTitle(this HtmlHelper html, INakedObjectAssociationSurface holder, INakedObjectSurface nakedObject) {
            return holder.GetMaskedValue(nakedObject);
        }

        private static string GetDisplayTitle(this HtmlHelper html, INakedObjectActionParameterSurface holder, INakedObjectSurface nakedObject) {
            return holder.GetMaskedValue(nakedObject);
        }

        private static string Action(string actionName) {
            return IdConstants.ActionAction + "/" + actionName;
        }

        internal static MethodInfo GetAction(this HtmlHelper html, LambdaExpression expression) {
            if (expression == null) {
                throw new ArgumentNullException("expression");
            }

            if (expression.Body.NodeType != ExpressionType.Convert) {
                throw new ArgumentException("must be method");
            }

            if (html.ViewData.Model == null) {
                throw new ArgumentException("html");
            }

            Expression actionExpr = ((MethodCallExpression) (((UnaryExpression) expression.Body).Operand)).Object;
            return (MethodInfo) ((ConstantExpression) actionExpr).Value;
        }

        internal static MemberInfo GetProperty(this HtmlHelper html, LambdaExpression expression) {
            if (expression == null) {
                throw new ArgumentNullException("expression");
            }

            if (html.ViewData.Model == null) {
                throw new ArgumentException("html");
            }

            if (expression.Body.NodeType == ExpressionType.MemberAccess) {
                return ((MemberExpression) expression.Body).Member;
            }

            if (expression.Body.NodeType == ExpressionType.Convert) {
                Expression op = ((UnaryExpression) expression.Body).Operand;

                if (op.NodeType == ExpressionType.MemberAccess) {
                    return ((MemberExpression) op).Member;
                }
            }

            throw new ArgumentException("must be member access");
        }

        private static void ValidateParamValues(MethodInfo methodInfo, object paramValues) {
            if (paramValues != null && methodInfo.GetParameters().Select(p => p.ParameterType).Any(p => typeof (IEnumerable).IsAssignableFrom(p) && !(p == typeof (string)))) {
                throw new NotSupportedException("Cannot pass collection as parameter value to custom ObjectAction");
            }
        }

        internal static MvcHtmlString ObjectAction(this HtmlHelper html, object target, MethodInfo methodInfo, object paramValues = null) {
            ValidateParamValues(methodInfo, paramValues);
            var nakedObject = html.Surface().GetObject(target);
            var action = html.GetActionByMethodInfo(nakedObject, methodInfo);
            return action == null ? MvcHtmlString.Create("") : html.ObjectAction(new ActionContext(html.IdHelper(), nakedObject, action) {ParameterValues = new RouteValueDictionary(paramValues)});
        }

        private static INakedObjectActionSurface GetActionByMethodInfo(this HtmlHelper html, INakedObjectSurface nakedObject, MethodInfo methodInfo) {
            return nakedObject.Specification.GetActionLeafNodes().
                Where(a => a.Id == methodInfo.Name).SingleOrDefault(a => a.IsVisible(nakedObject));
        }

        internal static MvcHtmlString ObjectActionOnTransient(this HtmlHelper html, object target, MethodInfo methodInfo) {
            var nakedObject = html.Surface().GetObject(target);
            var action = nakedObject.Specification.GetActionLeafNodes().
                Where(a => a.Id == methodInfo.Name).SingleOrDefault(a => a.IsVisible(nakedObject));

            return action == null ? MvcHtmlString.Create("") : html.ObjectActionOnTransient(new ActionContext(html.IdHelper(), nakedObject, action));
        }

        internal static MvcHtmlString ObjectPropertyView(this HtmlHelper html, object target, MemberInfo propertyInfo) {
            var nakedObject = html.Surface().GetObject(target);
            var property = nakedObject.Specification.Properties.Where(a => a.Id == propertyInfo.Name).SingleOrDefault(a => a.IsVisible(nakedObject));

            return property == null ? MvcHtmlString.Create("") : html.ObjectPropertyView(new PropertyContext(html.IdHelper(), nakedObject, property, false));
        }

        internal static MvcHtmlString ObjectPropertyEdit(this HtmlHelper html, object target, MemberInfo propertyInfo) {
            var nakedObject = html.Surface().GetObject(target);
            var property = nakedObject.Specification.Properties.Where(a => a.Id == propertyInfo.Name).SingleOrDefault(a => a.IsVisible(nakedObject));

            return property == null ? MvcHtmlString.Create("") : html.ObjectPropertyEdit(new PropertyContext(html.IdHelper(), nakedObject, property, true));
        }

        private static MvcHtmlString ObjectAction(this HtmlHelper html, ActionContext actionContext, bool isEdit) {
            return MvcHtmlString.Create(html.ObjectActionAsElementDescriptor(actionContext, new {id = html.Surface().OidStrategy.GetObjectId(actionContext.Target)}, isEdit).BuildElement());
        }

        internal static MvcHtmlString ObjectAction(this HtmlHelper html, ActionContext actionContext) {
            return html.ObjectAction(actionContext, false);
        }

        internal static MvcHtmlString ObjectActionOnTransient(this HtmlHelper html, ActionContext actionContext) {
            return html.ObjectAction(actionContext, true);
        }

        internal static MvcHtmlString ObjectPropertyView(this HtmlHelper html, PropertyContext propertyContext) {
            return MvcHtmlString.Create(html.ViewObjectField(propertyContext).BuildElement());
        }

        internal static MvcHtmlString ObjectPropertyEdit(this HtmlHelper html, PropertyContext propertyContext) {
            return MvcHtmlString.Create(html.EditObjectField(propertyContext).BuildElement());
        }

        internal static MvcHtmlString ObjectActionAsDialog(this HtmlHelper html, object target, MethodInfo methodInfo) {
            var nakedObject = html.Surface().GetObject(target);
            var action = nakedObject.Specification.GetActionLeafNodes().
                Where(a => a.Id == methodInfo.Name).SingleOrDefault(a => a.IsVisible(nakedObject));

            return action == null ? MvcHtmlString.Create("") : html.ObjectActionAsDialog(new ActionContext(html.IdHelper(), nakedObject, action));
        }

        internal static MvcHtmlString ObjectActionAsDialog(this HtmlHelper html, ActionContext actionContext) {
            bool allowed = actionContext.Action.IsUsable(actionContext.Target).IsAllowed;

            if (allowed) {
                return html.WrapInForm(Action(actionContext.Action.Id),
                    html.Surface().GetObjectTypeShortName(actionContext.Target.Object),
                    html.ParameterList(actionContext).ToString(),
                    actionContext.GetActionClass(),
                    new RouteValueDictionary(new {id = html.Surface().OidStrategy.GetObjectId(actionContext.Target)}));
            }
            return html.ObjectAction(actionContext);
        }

        #endregion
    }
}
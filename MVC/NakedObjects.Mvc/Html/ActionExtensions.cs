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
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Spec;
using NakedObjects.Core;
using NakedObjects.Resources;
using NakedObjects.Surface;
using NakedObjects.Surface.Utility;
using NakedObjects.Web.Mvc.Helpers;

namespace NakedObjects.Web.Mvc.Html {
    public static class ActionExtensions {
        #region name and ids

        /// <summary>
        ///     Get the id for an action dialog
        /// </summary>
        //public static MvcHtmlString ObjectActionDialogId(this HtmlHelper html, object domainObject, IActionSpec action) {
        //    //var nakedObject = html.Surface().GetObject(domainObject);
        //    //return MvcHtmlString.Create(html.IdHelper().GetActionDialogId(ScaffoldAdapter.Wrap(nakedObject), ScaffoldAction.Wrap(action)));
        //    throw new UnexpectedCallException();
        //}

      

        public static MvcHtmlString ObjectActionDialogId(this HtmlHelper html, object domainObject, INakedObjectActionSurface action) {
            INakedObjectSurface nakedObject = html.Surface().GetObject(domainObject);
            return MvcHtmlString.Create(html.IdHelper().GetActionDialogId(nakedObject, action));
        }

        /// <summary>
        ///     Get the name for an action on an object
        /// </summary>
        public static MvcHtmlString ObjectActionName(this HtmlHelper html, string name) {
            return CommonHtmlHelper.WrapInDiv(name, IdConstants.ActionNameLabel);
        }

        private static string GetPresentationHint(IActionSpec action) {
            var facet = action.GetFacet<IPresentationHintFacet>();
            return facet == null ? "" : " " + facet.Value;
        }

        /// <summary>
        ///     Get the classes for an action on an object
        /// </summary>
        public static MvcHtmlString ObjectActionClass(this HtmlHelper html, IActionSpec action) {
            return MvcHtmlString.Create(IdConstants.ActionAction + GetPresentationHint(action));
        }

        #endregion

        #region parameter list

        /// <summary>
        ///     Get the parameters of an action for display within a form
        /// </summary>
      

        public static MvcHtmlString ParameterList(this HtmlHelper html,
           object contextObject,
           object targetObject,
           INakedObjectActionSurface contextAction,
           INakedObjectActionSurface targetAction,
           string propertyName,
           IEnumerable collection) {
            var actionContext = new ActionContext(html.IdHelper(), false, html.Surface().GetObject(contextObject), contextAction);

            return ParameterList(contextAction, targetObject, targetAction, propertyName, collection, html, actionContext);
        }

        /// <summary>
        ///     Get the parameters of an action for display within a form
        /// </summary>
      

        public static MvcHtmlString ParameterListWith(this HtmlHelper html,
            object contextObject,
            object targetObject,
            INakedObjectActionSurface contextAction,
            INakedObjectActionSurface targetAction,
            string propertyName,
            IEnumerable collection) {
            var actionContext = new ActionContext(html.IdHelper(), false, html.Surface().GetObject(contextObject), contextAction) { Filter = x => x.Id == propertyName };

            return ParameterList(contextAction, targetObject, targetAction, propertyName, collection, html, actionContext);
        }

       

        private static MvcHtmlString ParameterList(INakedObjectActionSurface contextAction, object targetObject, INakedObjectActionSurface targetAction, string propertyName, IEnumerable collection, HtmlHelper html, ActionContext actionContext) {
            if ((targetObject == null || targetAction == null || string.IsNullOrEmpty(propertyName)) && collection == null) {
                return html.ParameterList(actionContext);
            }

            var targetActionContext = new ActionContext(html.IdHelper(), false, html.Surface().GetObject(targetObject), targetAction);
            return html.BuildParamContainer(actionContext,
                html.ActionParameterFields(actionContext, targetActionContext, propertyName, collection),
                IdConstants.ParamContainerName,
                html.IdHelper().GetParameterContainerId((contextAction)));
        }

        /// <summary>
        ///     Get the parameters of an action for display within a form
        /// </summary>
       
        public static MvcHtmlString ParameterList(this HtmlHelper html, object context, INakedObjectActionSurface action) {
            var actionContext = new ActionContext(html.IdHelper(), false, html.Surface().GetObject(context), action);
            return html.ParameterList(actionContext);
        }


        internal static MvcHtmlString ParameterList(this HtmlHelper html, ActionContext actionContext) {
            return html.BuildParamContainer(actionContext,
                html.ActionParameterFields(actionContext),
                IdConstants.ParamContainerName,
                html.IdHelper().GetParameterContainerId((actionContext.Action)));
        }

        #endregion

        #region controller action

        /// <summary>
        ///     Post to identified action and controller
        /// </summary>
        public static MvcHtmlString ControllerAction(this HtmlHelper html, string linkText, string actionName, string controller) {
            return html.ObjectActionAsString(linkText, actionName, controller, null);
        }

        /// <summary>
        ///     Post to identified action on model
        /// </summary>
        public static MvcHtmlString ControllerAction(this HtmlHelper html, string linkText, string actionName, string classAttribute, object model) {
            return html.ObjectButton(linkText, actionName, classAttribute, model);
        }

        /// <summary>
        ///     Post to identified action and controller with route values
        /// </summary>
        public static MvcHtmlString ControllerAction(this HtmlHelper html, string linkText, string actionName, string controller, RouteValueDictionary routeValueDictionary) {
            return html.ObjectActionAsString(linkText, actionName, controller, null, "", routeValueDictionary);
        }

        /// <summary>
        ///     Post to identified action and controller with route values and class
        /// </summary>
        public static MvcHtmlString ControllerAction(this HtmlHelper html, string linkText, string actionName, string controller, string classAttribute, string name, RouteValueDictionary routeValueDictionary) {
            return html.ObjectActionAsString(linkText, actionName, controller, classAttribute, name, routeValueDictionary);
        }

        #endregion

        #region generic edit action

        /// <summary>
        ///     Post to identified action and controller
        /// </summary>
        public static MvcHtmlString ControllerActionOnTransient(this HtmlHelper html, string linkText, string actionName, string controller) {
            return html.TransientObjectActionAsString(linkText, actionName, controller);
        }

        /// <summary>
        ///     Post to identified action on model
        /// </summary>
        public static MvcHtmlString ControllerActionOnTransient(this HtmlHelper html, string linkText, string actionName, object model) {
            return html.EditObjectButton(linkText, actionName, model);
        }

        /// <summary>
        ///     Post to identified action and controller with route values
        /// </summary>
        public static MvcHtmlString ControllerActionOnTransient(this HtmlHelper html, string linkText, string actionName, string controller, RouteValueDictionary routeValueDictionary) {
            return html.TransientObjectActionAsString(linkText, actionName, controller, routeValueDictionary);
        }

        #endregion

        #region menu

        /// <summary>
        ///     Create menu from actions of domainObject. (Just delegates to MenuExtenions#ObjectMenu)
        /// </summary>
        public static MvcHtmlString Menu(this HtmlHelper html, object domainObject) {
            return MenuExtensions.ObjectMenu(html, domainObject);
        }

        /// <summary>
        ///     Create menu from actions of domainObject - inserting additional items from menuItems parameter
        /// </summary>
       

        public static MvcHtmlString Menu(this HtmlHelper html, object domainObject, params CustomMenuItem[] menuItems) {
            var nakedObject = html.Surface().GetObject(domainObject);
            return CommonHtmlHelper.BuildMenuContainer(html.ObjectActions(nakedObject, false, menuItems),
                IdConstants.MenuContainerName,
                html.IdHelper().GetActionContainerId(nakedObject),
                html.IdHelper().GetActionLabel(nakedObject));
        }

        /// <summary>
        ///     Create menu from menuItems parameter
        /// </summary>
        public static MvcHtmlString Menu(this HtmlHelper html, string name, params CustomMenuItem[] menuItems) {
            return CommonHtmlHelper.BuildMenuContainer(html.ObjectActions(false, menuItems),
                IdConstants.MenuContainerName,
                html.IdHelper().MakeId(name, IdConstants.ActionContainerName),
                name + " " + MvcUi.Actions);
        }

        /// <summary>
        ///     Create menu from actions of domainObject
        /// </summary>
        public static MvcHtmlString MenuOnTransient(this HtmlHelper html, object domainObject) {
            var nakedObject = html.Surface().GetObject(domainObject);
            return CommonHtmlHelper.BuildMenuContainer(html.ObjectActions(nakedObject, true),
                IdConstants.MenuContainerName,
                html.IdHelper().GetActionContainerId(nakedObject),
                html.IdHelper().GetActionLabel(nakedObject));
        }

      

        /// <summary>
        ///     Create menu from actions of domainObject - inserting additional items from menuItems parameter
        /// </summary>
      

        public static MvcHtmlString MenuOnTransient(this HtmlHelper html, object domainObject, params CustomMenuItem[] menuItems) {
            var nakedObject = html.Surface().GetObject(domainObject);
            return CommonHtmlHelper.BuildMenuContainer(html.ObjectActions(nakedObject, true, menuItems),
                IdConstants.MenuContainerName,
                html.IdHelper().GetActionContainerId(nakedObject),
                html.IdHelper().GetActionLabel(nakedObject));
        }

        /// <summary>
        ///     Create menu from menuItems parameter
        /// </summary>
        public static MvcHtmlString MenuOnTransient(this HtmlHelper html, string name, params CustomMenuItem[] menuItems) {
            return CommonHtmlHelper.BuildMenuContainer(html.ObjectActions(true, menuItems),
                IdConstants.MenuContainerName,
                html.IdHelper().MakeId(name, IdConstants.ActionContainerName),
                name + " " + MvcUi.Actions);
        }

        #endregion

        #region ObjectAction

        /// <summary>
        ///     Display a single action on ViewData.Model with no parameters and returning void, as an menu item.
        /// </summary>
        /// <example>
        ///     Html.ObjectAction(y => y.NoParameterAction)
        /// </example>
        public static MvcHtmlString ObjectAction<TModel>(this HtmlHelper<TModel> html,
            Expression<Func<TModel, Action>> expression) {
            return html.ObjectAction(html.ViewData.Model, html.GetAction(expression));
        }

        /// <summary>
        ///     Display a single action on parameter model with no parameters and returning void, as an menu item.
        /// </summary>
        /// <example>
        ///     Html.ObjectAction(model, y => y.NoParameterAction)
        /// </example>
        public static MvcHtmlString ObjectAction<TModel>(this HtmlHelper html, TModel model,
            Expression<Func<TModel, Action>> expression) {
            return html.ObjectAction(model, html.GetAction(expression));
        }

        /// <summary>
        ///     Display a single action on ViewData.Model with a single parameter and returning void, as an menu item.
        /// </summary>
        /// <example>
        ///     Html.ObjectAction&lt;ModelType,ParmType&gt;(y => y.OneParameterAction)
        /// </example>
        public static MvcHtmlString ObjectAction<TModel, TParm>(this HtmlHelper<TModel> html,
            Expression<Func<TModel, Action<TParm>>> expression,
            object paramValues = null) {
            return html.ObjectAction(html.ViewData.Model, html.GetAction(expression), paramValues);
        }

        /// <summary>
        ///     Display a single action on parameter model with a single parameter and returning void, as an menu item.
        /// </summary>
        /// <example>
        ///     Html.ObjectAction&lt;ModelType,ParmType&gt;(model, y => y.OneParameterAction)
        /// </example>
        public static MvcHtmlString ObjectAction<TModel, TParm>(this HtmlHelper html, TModel model,
            Expression<Func<TModel, Action<TParm>>> expression,
            object paramValues = null) {
            return html.ObjectAction(model, html.GetAction(expression), paramValues);
        }

        /// <summary>
        ///     Display a single action on ViewData.Model with two parameters and returning void, as an menu item.
        /// </summary>
        /// <example>
        ///     Html.ObjectAction&lt;ModelType,Parm1Type, Parm2Type&gt;(y => y.TwoParameterAction)
        /// </example>
        public static MvcHtmlString ObjectAction<TModel, TParm1, TParm2>(this HtmlHelper<TModel> html,
            Expression<Func<TModel, Action<TParm1, TParm2>>> expression,
            object paramValues = null) {
            return html.ObjectAction(html.ViewData.Model, html.GetAction(expression), paramValues);
        }

        /// <summary>
        ///     Display a single action on parameter model with two parameters and returning void, as an menu item.
        /// </summary>
        /// <example>
        ///     Html.ObjectAction&lt;ModelType,Parm1Type,Parm2Type&gt;(model, y => y.TwoParameterAction)
        /// </example>
        public static MvcHtmlString ObjectAction<TModel, TParm1, TParm2>(this HtmlHelper html, TModel model,
            Expression<Func<TModel, Action<TParm1, TParm2>>> expression,
            object paramValues = null) {
            return html.ObjectAction(model, html.GetAction(expression), paramValues);
        }

        /// <summary>
        ///     Display a single action on ViewData.Model with three parameters and returning void, as an menu item.
        /// </summary>
        /// <example>
        ///     Html.ObjectAction&lt;ModelType,Parm1Type, Parm2Type, Parm3Type&gt;(y => y.ThreeParameterAction)
        /// </example>
        public static MvcHtmlString ObjectAction<TModel, TParm1, TParm2, TParm3>(this HtmlHelper<TModel> html,
            Expression<Func<TModel, Action<TParm1, TParm2, TParm3>>> expression,
            object paramValues = null) {
            return html.ObjectAction(html.ViewData.Model, html.GetAction(expression), paramValues);
        }

        /// <summary>
        ///     Display a single action on parameter model with three parameters and returning void, as an menu item.
        /// </summary>
        /// <example>
        ///     Html.ObjectAction&lt;ModelType,Parm1Type,Parm2Type,Parm3Type&gt;(model, y => y.ThreeParameterAction)
        /// </example>
        public static MvcHtmlString ObjectAction<TModel, TParm1, TParm2, TParm3>(this HtmlHelper html, TModel model,
            Expression<Func<TModel, Action<TParm1, TParm2, TParm3>>> expression,
            object paramValues = null) {
            return html.ObjectAction(model, html.GetAction(expression), paramValues);
        }

        /// <summary>
        ///     Display a single action on ViewData.Model with four parameters and returning void, as an menu item.
        /// </summary>
        /// <example>
        ///     Html.ObjectAction&lt;ModelType,Parm1Type, Parm2Type, Parm3Type, Parm4Type&gt;(y => y.FourParameterAction)
        /// </example>
        // eg void DoSomething(bool parm1, bool parm2, bool parm3, bool parm4)
        public static MvcHtmlString ObjectAction<TModel, TParm1, TParm2, TParm3, TParm4>(this HtmlHelper<TModel> html,
            Expression<Func<TModel, Action<TParm1, TParm2, TParm3, TParm4>>> expression,
            object paramValues = null) {
            return html.ObjectAction(html.ViewData.Model, html.GetAction(expression), paramValues);
        }

        /// <summary>
        ///     Display a single action on parameter model with four parameters and returning void, as an menu item.
        /// </summary>
        /// <example>
        ///     Html.ObjectAction&lt;ModelType,Parm1Type, Parm2Type, Parm3Type, Parm4Type&gt;(model, y => y.FourParameterAction)
        /// </example>
        // eg void DoSomething(bool parm1, bool parm2, bool parm3, bool parm4)
        public static MvcHtmlString ObjectAction<TModel, TParm1, TParm2, TParm3, TParm4>(this HtmlHelper html, TModel model,
            Expression<Func<TModel, Action<TParm1, TParm2, TParm3, TParm4>>> expression,
            object paramValues = null) {
            return html.ObjectAction(model, html.GetAction(expression), paramValues);
        }

        /// <summary>
        ///     Display a single action on ViewData.Model with no parameters, as an menu item.
        /// </summary>
        /// <example>
        ///     Html.ObjectAction(y => y.NoParameterAction)
        /// </example>
        public static MvcHtmlString ObjectAction<TModel, TResult>(this HtmlHelper<TModel> html,
            Expression<Func<TModel, Func<TResult>>> expression) {
            return html.ObjectAction(html.ViewData.Model, html.GetAction(expression));
        }

        /// <summary>
        ///     Display a single action on parameter model as an menu item.
        /// </summary>
        /// <example>
        ///     Html.ObjectAction(model, y => y.NoParameterAction)
        /// </example>
        // eg bool DoSomething()
        public static MvcHtmlString ObjectAction<TModel, TResult>(this HtmlHelper html, TModel model,
            Expression<Func<TModel, Func<TResult>>> expression) {
            return html.ObjectAction(model, html.GetAction(expression));
        }

        /// <summary>
        ///     Display a single action on ViewData.Model as an menu item.
        /// </summary>
        /// <example>
        ///     Html.ObjectAction(y => y.NoParameterAction)
        /// </example>
        // eg bool DoSomething(bool parm)
        public static MvcHtmlString ObjectAction<TModel, TParm, TResult>(this HtmlHelper<TModel> html,
            Expression<Func<TModel, Func<TParm, TResult>>> expression,
            object paramValues = null) {
            return html.ObjectAction(html.ViewData.Model, html.GetAction(expression), paramValues);
        }

        /// <summary>
        ///     Display a single action on parameter model as an menu item.
        /// </summary>
        /// <example>
        ///     Html.ObjectAction(model, y => y.NoParameterAction)
        /// </example>
        // eg bool DoSomething(bool parm)
        public static MvcHtmlString ObjectAction<TModel, TParm, TResult>(this HtmlHelper html, TModel model,
            Expression<Func<TModel, Func<TParm, TResult>>> expression,
            object paramValues = null) {
            return html.ObjectAction(model, html.GetAction(expression), paramValues);
        }

        /// <summary>
        ///     Display a single action on ViewData.Model as an menu item.
        /// </summary>
        /// <example>
        ///     Html.ObjectAction(y => y.NoParameterAction)
        /// </example>
        // eg bool DoSomething(bool parm1, bool parm2)
        public static MvcHtmlString ObjectAction<TModel, TParm1, TParm2, TResult>(this HtmlHelper<TModel> html,
            Expression<Func<TModel, Func<TParm1, TParm2, TResult>>> expression,
            object paramValues = null) {
            return html.ObjectAction(html.ViewData.Model, html.GetAction(expression), paramValues);
        }

        /// <summary>
        ///     Display a single action on parameter model as an menu item.
        /// </summary>
        /// <example>
        ///     Html.ObjectAction(model, y => y.NoParameterAction)
        /// </example>
        // eg bool DoSomething(bool parm1, bool parm2)
        public static MvcHtmlString ObjectAction<TModel, TParm1, TParm2, TResult>(this HtmlHelper html, TModel model,
            Expression<Func<TModel, Func<TParm1, TParm2, TResult>>> expression,
            object paramValues = null) {
            return html.ObjectAction(model, html.GetAction(expression), paramValues);
        }

        /// <summary>
        ///     Display a single action on ViewData.Model as an menu item.
        /// </summary>
        /// <example>
        ///     Html.ObjectAction(y => y.NoParameterAction)
        /// </example>
        // eg bool DoSomething(bool parm1, bool parm2, bool parm3)
        public static MvcHtmlString ObjectAction<TModel, TParm1, TParm2, TParm3, TResult>(this HtmlHelper<TModel> html,
            Expression<Func<TModel, Func<TParm1, TParm2, TParm3, TResult>>> expression,
            object paramValues = null) {
            return html.ObjectAction(html.ViewData.Model, html.GetAction(expression), paramValues);
        }

        /// <summary>
        ///     Display a single action on parameter model as an menu item.
        /// </summary>
        /// <example>
        ///     Html.ObjectAction(model, y => y.NoParameterAction)
        /// </example>
        // eg bool DoSomething(bool parm1, bool parm2, bool parm3)
        public static MvcHtmlString ObjectAction<TModel, TParm1, TParm2, TParm3, TResult>(this HtmlHelper html, TModel model,
            Expression<Func<TModel, Func<TParm1, TParm2, TParm3, TResult>>> expression,
            object paramValues = null) {
            return html.ObjectAction(model, html.GetAction(expression), paramValues);
        }

        /// <summary>
        ///     Display a single action on ViewData.Model as an menu item.
        /// </summary>
        /// <example>
        ///     Html.ObjectAction(y => y.NoParameterAction)
        /// </example>
        // eg bool DoSomething(bool parm1, bool parm2, bool parm3, bool parm4)
        public static MvcHtmlString ObjectAction<TModel, TParm1, TParm2, TParm3, TParm4, TResult>(this HtmlHelper<TModel> html,
            Expression<Func<TModel, Func<TParm1, TParm2, TParm3, TParm4, TResult>>> expression,
            object paramValues = null) {
            return html.ObjectAction(html.ViewData.Model, html.GetAction(expression), paramValues);
        }

        /// <summary>
        ///     Display a single action on parameter model as an menu item.
        /// </summary>
        /// <example>
        ///     Html.ObjectAction(model, y => y.NoParameterAction)
        /// </example>
        // eg bool DoSomething(bool parm1, bool parm2, bool parm3, bool parm4)
        public static MvcHtmlString ObjectAction<TModel, TParm1, TParm2, TParm3, TParm4, TResult>(this HtmlHelper html, TModel model,
            Expression<Func<TModel, Func<TParm1, TParm2, TParm3, TParm4, TResult>>> expression,
            object paramValues = null) {
            return html.ObjectAction(model, html.GetAction(expression), paramValues);
        }

        /// <summary>
        ///     Display a single action on ViewData.Model as an menu item.
        /// </summary>
        /// <example>
        ///     Html.ObjectAction(y => y.NoParameterAction)
        /// </example>
        // non lambda 
        public static MvcHtmlString ObjectAction(this HtmlHelper html, string id, object paramValues = null) {
            if (html.ViewData.Model == null) {
                throw new ArgumentException("html");
            }
            return html.ObjectAction(html.ViewData.Model, id, paramValues);
        }

        /// <summary>
        ///     Display a single action on parameter model as an menu item.
        /// </summary>
        /// <example>
        ///     Html.ObjectAction(model, y => y.NoParameterAction)
        /// </example>
        // non lambda 
       

        public static MvcHtmlString ObjectAction(this HtmlHelper html, object model, string id, object paramValues = null) {
            var nakedObject = html.Surface().GetObject(model);
            var action = nakedObject.Specification.GetActionLeafNodes().Where(a => a.IsVisible(nakedObject)).SingleOrDefault(a => a.Id == id);
            ValidateParamValues(action, paramValues);
            return action == null ? MvcHtmlString.Create("") : html.ObjectAction(new ActionContext(html.IdHelper(), nakedObject, action) { ParameterValues = new RouteValueDictionary(paramValues) });
        }

        private static void ValidateParamValues(INakedObjectActionSurface action, object paramValues) {
            if (paramValues != null && action.Parameters.Select(p => p.Specification).Any(s => s.IsCollection())) {
                throw new NotSupportedException("Cannot pass collection as parameter value to custom ObjectAction");
            }
        }

        private static void ValidateParamValues(IActionSpec action, object paramValues) {
            if (paramValues != null && action.Parameters.Select(p => p.Spec).Any(s => s.IsCollection)) {
                throw new NotSupportedException("Cannot pass collection as parameter value to custom ObjectAction");
            }
        }

        private static IEnumerable<IActionSpec> GetObjectAndContributedActions(this HtmlHelper html, INakedObjectAdapter nakedObject) {
            return nakedObject.Spec.GetActions().Where(a => a.IsVisible(nakedObject));
        }

        #endregion

        #region ObjectActionOnTransient

        /// <summary>
        ///     Display a single action on ViewData.Model with no parameters and returning void, as an menu item.
        /// </summary>
        /// <example>
        ///     Html.ObjectActionOnTransient(y => y.NoParameterAction)
        /// </example>
        public static MvcHtmlString ObjectActionOnTransient<TModel>(this HtmlHelper<TModel> html,
            Expression<Func<TModel, Action>> expression) {
            return html.ObjectActionOnTransient(html.ViewData.Model, html.GetAction(expression));
        }

        /// <summary>
        ///     Display a single action on parameter model with no parameters and returning void, as an menu item.
        /// </summary>
        /// <example>
        ///     Html.ObjectActionOnTransient(model, y => y.NoParameterAction)
        /// </example>
        public static MvcHtmlString ObjectActionOnTransient<TModel>(this HtmlHelper html, TModel model,
            Expression<Func<TModel, Action>> expression) {
            return html.ObjectActionOnTransient(model, html.GetAction(expression));
        }

        /// <summary>
        ///     Display a single action on ViewData.Model with a single parameter and returning void, as an menu item.
        /// </summary>
        /// <example>
        ///     Html.ObjectActionOnTransient&lt;ModelType,ParmType&gt;(y => y.OneParameterAction)
        /// </example>
        public static MvcHtmlString ObjectActionOnTransient<TModel, TParm>(this HtmlHelper<TModel> html,
            Expression<Func<TModel, Action<TParm>>> expression) {
            return html.ObjectActionOnTransient(html.ViewData.Model, html.GetAction(expression));
        }

        /// <summary>
        ///     Display a single action on parameter model with a single parameter and returning void, as an menu item.
        /// </summary>
        /// <example>
        ///     Html.ObjectActionOnTransient&lt;ModelType,ParmType&gt;(model, y => y.OneParameterAction)
        /// </example>
        public static MvcHtmlString ObjectActionOnTransient<TModel, TParm>(this HtmlHelper html, TModel model,
            Expression<Func<TModel, Action<TParm>>> expression) {
            return html.ObjectActionOnTransient(model, html.GetAction(expression));
        }

        /// <summary>
        ///     Display a single action on ViewData.Model with two parameters and returning void, as an menu item.
        /// </summary>
        /// <example>
        ///     Html.ObjectActionOnTransient&lt;ModelType,Parm1Type, Parm2Type&gt;(y => y.TwoParameterAction)
        /// </example>
        public static MvcHtmlString ObjectActionOnTransient<TModel, TParm1, TParm2>(this HtmlHelper<TModel> html,
            Expression<Func<TModel, Action<TParm1, TParm2>>> expression) {
            return html.ObjectActionOnTransient(html.ViewData.Model, html.GetAction(expression));
        }

        /// <summary>
        ///     Display a single action on parameter model with two parameters and returning void, as an menu item.
        /// </summary>
        /// <example>
        ///     Html.ObjectActionOnTransient&lt;ModelType,Parm1Type,Parm2Type&gt;(model, y => y.TwoParameterAction)
        /// </example>
        public static MvcHtmlString ObjectActionOnTransient<TModel, TParm1, TParm2>(this HtmlHelper html, TModel model,
            Expression<Func<TModel, Action<TParm1, TParm2>>> expression) {
            return html.ObjectActionOnTransient(model, html.GetAction(expression));
        }

        /// <summary>
        ///     Display a single action on ViewData.Model with three parameters and returning void, as an menu item.
        /// </summary>
        /// <example>
        ///     Html.ObjectActionOnTransient&lt;ModelType,Parm1Type, Parm2Type, Parm3Type&gt;(y => y.ThreeParameterAction)
        /// </example>
        public static MvcHtmlString ObjectActionOnTransient<TModel, TParm1, TParm2, TParm3>(this HtmlHelper<TModel> html,
            Expression<Func<TModel, Action<TParm1, TParm2, TParm3>>> expression) {
            return html.ObjectActionOnTransient(html.ViewData.Model, html.GetAction(expression));
        }

        /// <summary>
        ///     Display a single action on parameter model with three parameters and returning void, as an menu item.
        /// </summary>
        /// <example>
        ///     Html.ObjectActionOnTransient&lt;ModelType,Parm1Type,Parm2Type,Parm3Type&gt;(model, y => y.ThreeParameterAction)
        /// </example>
        public static MvcHtmlString ObjectActionOnTransient<TModel, TParm1, TParm2, TParm3>(this HtmlHelper html, TModel model,
            Expression<Func<TModel, Action<TParm1, TParm2, TParm3>>> expression) {
            return html.ObjectActionOnTransient(model, html.GetAction(expression));
        }

        /// <summary>
        ///     Display a single action on ViewData.Model with four parameters and returning void, as an menu item.
        /// </summary>
        /// <example>
        ///     Html.ObjectActionOnTransient&lt;ModelType,Parm1Type, Parm2Type, Parm3Type, Parm4Type&gt;(y => y.FourParameterAction)
        /// </example>
        // eg void DoSomething(bool parm1, bool parm2, bool parm3, bool parm4)
        public static MvcHtmlString ObjectActionOnTransient<TModel, TParm1, TParm2, TParm3, TParm4>(this HtmlHelper<TModel> html,
            Expression<Func<TModel, Action<TParm1, TParm2, TParm3, TParm4>>> expression) {
            return html.ObjectActionOnTransient(html.ViewData.Model, html.GetAction(expression));
        }

        /// <summary>
        ///     Display a single action on parameter model with four parameters and returning void, as an menu item.
        /// </summary>
        /// <example>
        ///     Html.ObjectActionOnTransient&lt;ModelType,Parm1Type, Parm2Type, Parm3Type, Parm4Type&gt;(model, y => y.FourParameterAction)
        /// </example>
        // eg void DoSomething(bool parm1, bool parm2, bool parm3, bool parm4)
        public static MvcHtmlString ObjectActionOnTransient<TModel, TParm1, TParm2, TParm3, TParm4>(this HtmlHelper html, TModel model,
            Expression<Func<TModel, Action<TParm1, TParm2, TParm3, TParm4>>> expression) {
            return html.ObjectActionOnTransient(model, html.GetAction(expression));
        }

        /// <summary>
        ///     Display a single action on ViewData.Model with no parameters, as an menu item.
        /// </summary>
        /// <example>
        ///     Html.ObjectActionOnTransient(y => y.NoParameterAction)
        /// </example>
        public static MvcHtmlString ObjectActionOnTransient<TModel, TResult>(this HtmlHelper<TModel> html,
            Expression<Func<TModel, Func<TResult>>> expression) {
            return html.ObjectActionOnTransient(html.ViewData.Model, html.GetAction(expression));
        }

        /// <summary>
        ///     Display a single action on parameter model as an menu item.
        /// </summary>
        /// <example>
        ///     Html.ObjectActionOnTransient(model, y => y.NoParameterAction)
        /// </example>
        // eg bool DoSomething()
        public static MvcHtmlString ObjectActionOnTransient<TModel, TResult>(this HtmlHelper html, TModel model,
            Expression<Func<TModel, Func<TResult>>> expression) {
            return html.ObjectActionOnTransient(model, html.GetAction(expression));
        }

        /// <summary>
        ///     Display a single action on ViewData.Model as an menu item.
        /// </summary>
        /// <example>
        ///     Html.ObjectActionOnTransient(y => y.NoParameterAction)
        /// </example>
        // eg bool DoSomething(bool parm)
        public static MvcHtmlString ObjectActionOnTransient<TModel, TParm, TResult>(this HtmlHelper<TModel> html,
            Expression<Func<TModel, Func<TParm, TResult>>> expression) {
            return html.ObjectActionOnTransient(html.ViewData.Model, html.GetAction(expression));
        }

        /// <summary>
        ///     Display a single action on parameter model as an menu item.
        /// </summary>
        /// <example>
        ///     Html.ObjectActionOnTransient(model, y => y.NoParameterAction)
        /// </example>
        // eg bool DoSomething(bool parm)
        public static MvcHtmlString ObjectActionOnTransient<TModel, TParm, TResult>(this HtmlHelper html, TModel model,
            Expression<Func<TModel, Func<TParm, TResult>>> expression) {
            return html.ObjectActionOnTransient(model, html.GetAction(expression));
        }

        /// <summary>
        ///     Display a single action on ViewData.Model as an menu item.
        /// </summary>
        /// <example>
        ///     Html.ObjectActionOnTransient(y => y.NoParameterAction)
        /// </example>
        // eg bool DoSomething(bool parm1, bool parm2)
        public static MvcHtmlString ObjectActionOnTransient<TModel, TParm1, TParm2, TResult>(this HtmlHelper<TModel> html,
            Expression<Func<TModel, Func<TParm1, TParm2, TResult>>> expression) {
            return html.ObjectActionOnTransient(html.ViewData.Model, html.GetAction(expression));
        }

        /// <summary>
        ///     Display a single action on parameter model as an menu item.
        /// </summary>
        /// <example>
        ///     Html.ObjectActionOnTransient(model, y => y.NoParameterAction)
        /// </example>
        // eg bool DoSomething(bool parm1, bool parm2)
        public static MvcHtmlString ObjectActionOnTransient<TModel, TParm1, TParm2, TResult>(this HtmlHelper html, TModel model,
            Expression<Func<TModel, Func<TParm1, TParm2, TResult>>> expression) {
            return html.ObjectActionOnTransient(model, html.GetAction(expression));
        }

        /// <summary>
        ///     Display a single action on ViewData.Model as an menu item.
        /// </summary>
        /// <example>
        ///     Html.ObjectActionOnTransient(y => y.NoParameterAction)
        /// </example>
        // eg bool DoSomething(bool parm1, bool parm2, bool parm3)
        public static MvcHtmlString ObjectActionOnTransient<TModel, TParm1, TParm2, TParm3, TResult>(this HtmlHelper<TModel> html,
            Expression<Func<TModel, Func<TParm1, TParm2, TParm3, TResult>>> expression) {
            return html.ObjectActionOnTransient(html.ViewData.Model, html.GetAction(expression));
        }

        /// <summary>
        ///     Display a single action on parameter model as an menu item.
        /// </summary>
        /// <example>
        ///     Html.ObjectActionOnTransient(model, y => y.NoParameterAction)
        /// </example>
        // eg bool DoSomething(bool parm1, bool parm2, bool parm3)
        public static MvcHtmlString ObjectActionOnTransient<TModel, TParm1, TParm2, TParm3, TResult>(this HtmlHelper html, TModel model,
            Expression<Func<TModel, Func<TParm1, TParm2, TParm3, TResult>>> expression) {
            return html.ObjectActionOnTransient(model, html.GetAction(expression));
        }

        /// <summary>
        ///     Display a single action on ViewData.Model as an menu item.
        /// </summary>
        /// <example>
        ///     Html.ObjectActionOnTransient(y => y.NoParameterAction)
        /// </example>
        // eg bool DoSomething(bool parm1, bool parm2, bool parm3, bool parm4)
        public static MvcHtmlString ObjectActionOnTransient<TModel, TParm1, TParm2, TParm3, TParm4, TResult>(this HtmlHelper<TModel> html,
            Expression<Func<TModel, Func<TParm1, TParm2, TParm3, TParm4, TResult>>> expression) {
            return html.ObjectActionOnTransient(html.ViewData.Model, html.GetAction(expression));
        }

        /// <summary>
        ///     Display a single action on parameter model as an menu item.
        /// </summary>
        /// <example>
        ///     Html.ObjectActionOnTransient(model, y => y.NoParameterAction)
        /// </example>
        // eg bool DoSomething(bool parm1, bool parm2, bool parm3, bool parm4)
        public static MvcHtmlString ObjectActionOnTransient<TModel, TParm1, TParm2, TParm3, TParm4, TResult>(this HtmlHelper html, TModel model,
            Expression<Func<TModel, Func<TParm1, TParm2, TParm3, TParm4, TResult>>> expression) {
            return html.ObjectActionOnTransient(model, html.GetAction(expression));
        }

        /// <summary>
        ///     Display a single action on ViewData.Model as an menu item.
        /// </summary>
        /// <example>
        ///     Html.ObjectActionOnTransient(y => y.NoParameterAction)
        /// </example>
        // non lambda 
        public static MvcHtmlString ObjectActionOnTransient(this HtmlHelper html, string id) {
            if (html.ViewData.Model == null) {
                throw new ArgumentException("html");
            }
            return html.ObjectActionOnTransient(html.ViewData.Model, id);
        }

        /// <summary>
        ///     Display a single action on parameter model as an menu item.
        /// </summary>
        /// <example>
        ///     Html.ObjectActionOnTransient(model, y => y.NoParameterAction)
        /// </example>
        // non lambda 
        

        public static MvcHtmlString ObjectActionOnTransient(this HtmlHelper html, object model, string id) {
            var nakedObject = html.Surface().GetObject(model);
            var action = nakedObject.Specification.GetActionLeafNodes().Single(a => a.Id == id);
            return html.ObjectActionOnTransient(new ActionContext(html.IdHelper(), nakedObject, action));
        }

        #endregion

        #region ObjectActionAsDialog

        /// <summary>
        ///     Display a single action on ViewData.Model with a single parameter and returning void, as a dialog.
        /// </summary>
        /// <example>
        ///     Html.ObjectActionAsDialog&lt;ModelType,ParmType&gt;(y => y.OneParameterAction)
        /// </example>
        public static MvcHtmlString ObjectActionAsDialog<TModel, TParm>(this HtmlHelper<TModel> html,
            Expression<Func<TModel, Action<TParm>>> expression) {
            return html.ObjectActionAsDialog(html.ViewData.Model, html.GetAction(expression));
        }

        /// <summary>
        ///     Display a single action on parameter model with a single parameter and returning void, as a dialog.
        /// </summary>
        /// <example>
        ///     Html.ObjectActionAsDialog&lt;ModelType,ParmType&gt;(model, y => y.OneParameterAction)
        /// </example>
        public static MvcHtmlString ObjectActionAsDialog<TModel, TParm>(this HtmlHelper html, TModel model,
            Expression<Func<TModel, Action<TParm>>> expression) {
            return html.ObjectActionAsDialog(model, html.GetAction(expression));
        }

        /// <summary>
        ///     Display a single action on ViewData.Model with two parameters and returning void, as a dialog.
        /// </summary>
        // eg void DoSomething(bool parm1, bool parm2)
        public static MvcHtmlString ObjectActionAsDialog<TModel, TParm1, TParm2>(this HtmlHelper<TModel> html,
            Expression<Func<TModel, Action<TParm1, TParm2>>> expression) {
            return html.ObjectActionAsDialog(html.ViewData.Model, html.GetAction(expression));
        }

        /// <summary>
        ///     Display a single action on parameter model with two parameters and returning void, as a dialog.
        /// </summary>
        // eg void DoSomething(bool parm1, bool parm2)
        public static MvcHtmlString ObjectActionAsDialog<TModel, TParm1, TParm2>(this HtmlHelper html, TModel model,
            Expression<Func<TModel, Action<TParm1, TParm2>>> expression) {
            return html.ObjectActionAsDialog(model, html.GetAction(expression));
        }

        /// <summary>
        ///     Display a single action on ViewData.Model with three parameters and returning void, as a dialog.
        /// </summary>
        // eg void DoSomething(bool parm1, bool parm2, bool parm3)
        public static MvcHtmlString ObjectActionAsDialog<TModel, TParm1, TParm2, TParm3>(this HtmlHelper<TModel> html,
            Expression<Func<TModel, Action<TParm1, TParm2, TParm3>>> expression) {
            return html.ObjectActionAsDialog(html.ViewData.Model, html.GetAction(expression));
        }

        /// <summary>
        ///     Display a single action on parameter model with three parameters and returning void, as a dialog.
        /// </summary>
        // eg void DoSomething(bool parm1, bool parm2, bool parm3)
        public static MvcHtmlString ObjectActionAsDialog<TModel, TParm1, TParm2, TParm3>(this HtmlHelper html, TModel model,
            Expression<Func<TModel, Action<TParm1, TParm2, TParm3>>> expression) {
            return html.ObjectActionAsDialog(model, html.GetAction(expression));
        }

        /// <summary>
        ///     Display a single action on ViewData.Model with four parameters and returning void, as a dialog.
        /// </summary>
        // eg void DoSomething(bool parm1, bool parm2, bool parm3, bool parm4)
        public static MvcHtmlString ObjectActionAsDialog<TModel, TParm1, TParm2, TParm3, TParm4>(this HtmlHelper<TModel> html,
            Expression<Func<TModel, Action<TParm1, TParm2, TParm3, TParm4>>> expression) {
            return html.ObjectActionAsDialog(html.ViewData.Model, html.GetAction(expression));
        }

        /// <summary>
        ///     Display a single action on parameter model with four parameters and returning void, as a dialog.
        /// </summary>
        // eg void DoSomething(bool parm1, bool parm2, bool parm3, bool parm4)
        public static MvcHtmlString ObjectActionAsDialog<TModel, TParm1, TParm2, TParm3, TParm4>(this HtmlHelper html, TModel model,
            Expression<Func<TModel, Action<TParm1, TParm2, TParm3, TParm4>>> expression) {
            return html.ObjectActionAsDialog(model, html.GetAction(expression));
        }

        /// <summary>
        ///     Display a single action on ViewData.Model with zero parameters and returning a TResult, as a dialog.
        /// </summary>
        // eg bool DoSomething()
        public static MvcHtmlString ObjectActionAsDialog<TModel, TResult>(this HtmlHelper<TModel> html,
            Expression<Func<TModel, Func<TResult>>> expression) {
            return html.ObjectActionAsDialog(html.ViewData.Model, html.GetAction(expression));
        }

        /// <summary>
        ///     Display a single action on parameter model with zero parameters and returning a TResult, as a dialog.
        /// </summary>
        // eg bool DoSomething()
        public static MvcHtmlString ObjectActionAsDialog<TModel, TResult>(this HtmlHelper html, TModel model,
            Expression<Func<TModel, Func<TResult>>> expression) {
            return html.ObjectActionAsDialog(model, html.GetAction(expression));
        }

        /// <summary>
        ///     Display a single action on ViewData.Model with one parameter and returning a TResult, as a dialog.
        /// </summary>
        // eg bool DoSomething(bool parm)
        public static MvcHtmlString ObjectActionAsDialog<TModel, TParm, TResult>(this HtmlHelper<TModel> html,
            Expression<Func<TModel, Func<TParm, TResult>>> expression) {
            return html.ObjectActionAsDialog(html.ViewData.Model, html.GetAction(expression));
        }

        /// <summary>
        ///     Display a single action on parameter model with one parameter and returning a TResult, as a dialog.
        /// </summary>
        // eg bool DoSomething(bool parm)
        public static MvcHtmlString ObjectActionAsDialog<TModel, TParm, TResult>(this HtmlHelper html, TModel model,
            Expression<Func<TModel, Func<TParm, TResult>>> expression) {
            return html.ObjectActionAsDialog(model, html.GetAction(expression));
        }

        /// <summary>
        ///     Display a single action on ViewData.Model with two parameters and returning a TResult, as a dialog.
        /// </summary>
        // eg bool DoSomething(bool parm1, bool parm2)
        public static MvcHtmlString ObjectActionAsDialog<TModel, TParm1, TParm2, TResult>(this HtmlHelper<TModel> html,
            Expression<Func<TModel, Func<TParm1, TParm2, TResult>>> expression) {
            return html.ObjectActionAsDialog(html.ViewData.Model, html.GetAction(expression));
        }

        /// <summary>
        ///     Display a single action on parameter model with two parameters and returning a TResult, as a dialog.
        /// </summary>
        // eg bool DoSomething(bool parm1, bool parm2)
        public static MvcHtmlString ObjectActionAsDialog<TModel, TParm1, TParm2, TResult>(this HtmlHelper html, TModel model,
            Expression<Func<TModel, Func<TParm1, TParm2, TResult>>> expression) {
            return html.ObjectActionAsDialog(model, html.GetAction(expression));
        }

        /// <summary>
        ///     Display a single action on ViewData.Model with three parameters and returning a TResult, as a dialog.
        /// </summary>
        // eg bool DoSomething(bool parm1, bool parm2, bool parm3)
        public static MvcHtmlString ObjectActionAsDialog<TModel, TParm1, TParm2, TParm3, TResult>(this HtmlHelper<TModel> html,
            Expression<Func<TModel, Func<TParm1, TParm2, TParm3, TResult>>> expression) {
            return html.ObjectActionAsDialog(html.ViewData.Model, html.GetAction(expression));
        }

        /// <summary>
        ///     Display a single action on parameter model with three parameters and returning a TResult, as a dialog.
        /// </summary>
        // eg bool DoSomething(bool parm1, bool parm2, bool parm3)
        public static MvcHtmlString ObjectActionAsDialog<TModel, TParm1, TParm2, TParm3, TResult>(this HtmlHelper html, TModel model,
            Expression<Func<TModel, Func<TParm1, TParm2, TParm3, TResult>>> expression) {
            return html.ObjectActionAsDialog(model, html.GetAction(expression));
        }

        /// <summary>
        ///     Display a single action on ViewData.Model with four parameters and returning a TResult, as a dialog.
        /// </summary>
        // eg bool DoSomething(bool parm1, bool parm2, bool parm3, bool parm4)
        public static MvcHtmlString ObjectActionAsDialog<TModel, TParm1, TParm2, TParm3, TParm4, TResult>(this HtmlHelper<TModel> html,
            Expression<Func<TModel, Func<TParm1, TParm2, TParm3, TParm4, TResult>>> expression) {
            return html.ObjectActionAsDialog(html.ViewData.Model, html.GetAction(expression));
        }

        /// <summary>
        ///     Display a single action on parameter model with four parameters and returning a TResult, as a dialog.
        /// </summary>
        // eg bool DoSomething(bool parm1, bool parm2, bool parm3, bool parm4)
        public static MvcHtmlString ObjectActionAsDialog<TModel, TParm1, TParm2, TParm3, TParm4, TResult>(this HtmlHelper html, TModel model,
            Expression<Func<TModel, Func<TParm1, TParm2, TParm3, TParm4, TResult>>> expression) {
            return html.ObjectActionAsDialog(model, html.GetAction(expression));
        }

        /// <summary>
        ///     Display a single action on ViewData.Model as a dialog.
        /// </summary>
        // non lambda 
        public static MvcHtmlString ObjectActionAsDialog(this HtmlHelper html, string id) {
            if (html.ViewData.Model == null) {
                throw new ArgumentException("html");
            }
            return html.ObjectActionAsDialog(html.ViewData.Model, id);
        }

        /// <summary>
        ///     Display a single action on parameter model as a dialog.
        /// </summary>
        // non lambda 
     

        public static MvcHtmlString ObjectActionAsDialog(this HtmlHelper html, object model, string id) {
            var nakedObject = html.Surface().GetObject(model);
            var action = nakedObject.Specification.GetActionLeafNodes().SingleOrDefault(a => a.Id == id);
            return action == null ? MvcHtmlString.Create("") : html.ObjectActionAsDialog(new ActionContext(html.IdHelper(), nakedObject, action));
        }

        #endregion

        #region contents

        /// <summary>
        ///     Get the contents of the identified parameter
        /// </summary>
        public static MvcHtmlString Contents<TModel, TParm>(this HtmlHelper<TModel> html, Expression<Func<TModel, Action<TParm>>> expression, int index) {
            return html.Contents(html.GetAction(expression).Name, index);
        }

        /// <summary>
        ///     Get the contents of the identified parameter
        /// </summary>
        public static MvcHtmlString Contents<TModel, TParm>(this HtmlHelper html, TModel model, Expression<Func<TModel, Action<TParm>>> expression, int index) {
            return html.Contents(model, html.GetAction(expression).Name, index);
        }

        /// <summary>
        ///     Get the contents of the identified parameter
        /// </summary>
        public static MvcHtmlString Contents<TModel, TParm1, TParm2>(this HtmlHelper<TModel> html, Expression<Func<TModel, Action<TParm1, TParm2>>> expression, int index) {
            return html.Contents(html.GetAction(expression).Name, index);
        }

        /// <summary>
        ///     Get the contents of the identified parameter
        /// </summary>
        public static MvcHtmlString Contents<TModel, TParm1, TParm2>(this HtmlHelper html, TModel model, Expression<Func<TModel, Action<TParm1, TParm2>>> expression, int index) {
            return html.Contents(model, html.GetAction(expression).Name, index);
        }

        /// <summary>
        ///     Get the contents of the identified parameter
        /// </summary>
        public static MvcHtmlString Contents<TModel, TParm1, TParm2, TParm3>(this HtmlHelper<TModel> html, Expression<Func<TModel, Action<TParm1, TParm2, TParm3>>> expression, int index) {
            return html.Contents(html.GetAction(expression).Name, index);
        }

        /// <summary>
        ///     Get the contents of the identified parameter
        /// </summary>
        public static MvcHtmlString Contents<TModel, TParm1, TParm2, TParm3>(this HtmlHelper html, TModel model, Expression<Func<TModel, Action<TParm1, TParm2, TParm3>>> expression, int index) {
            return html.Contents(model, html.GetAction(expression).Name, index);
        }

        /// <summary>
        ///     Get the contents of the identified parameter
        /// </summary>
        public static MvcHtmlString Contents<TModel, TParm1, TParm2, TParm3, TParm4>(this HtmlHelper<TModel> html, Expression<Func<TModel, Action<TParm1, TParm2, TParm3, TParm4>>> expression, int index) {
            return html.Contents(html.GetAction(expression).Name, index);
        }

        /// <summary>
        ///     Get the contents of the identified parameter
        /// </summary>
        public static MvcHtmlString Contents<TModel, TParm1, TParm2, TParm3, TParm4>(this HtmlHelper html, TModel model, Expression<Func<TModel, Action<TParm1, TParm2, TParm3, TParm4>>> expression, int index) {
            return html.Contents(model, html.GetAction(expression).Name, index);
        }

        /// <summary>
        ///     Get the contents of the identified parameter
        /// </summary>
        public static MvcHtmlString Contents<TModel, TParm, TResult>(this HtmlHelper<TModel> html, Expression<Func<TModel, Func<TParm, TResult>>> expression, int index) {
            return html.Contents(html.GetAction(expression).Name, index);
        }

        /// <summary>
        ///     Get the contents of the identified parameter
        /// </summary>
        public static MvcHtmlString Contents<TModel, TParm, TResult>(this HtmlHelper html, TModel model, Expression<Func<TModel, Func<TParm, TResult>>> expression, int index) {
            return html.Contents(model, html.GetAction(expression).Name, index);
        }

        /// <summary>
        ///     Get the contents of the identified parameter
        /// </summary>
        public static MvcHtmlString Contents<TModel, TParm1, TParm2, TResult>(this HtmlHelper<TModel> html, Expression<Func<TModel, Func<TParm1, TParm2, TResult>>> expression, int index) {
            return html.Contents(html.GetAction(expression).Name, index);
        }

        /// <summary>
        ///     Get the contents of the identified parameter
        /// </summary>
        public static MvcHtmlString Contents<TModel, TParm1, TParm2, TResult>(this HtmlHelper html, TModel model, Expression<Func<TModel, Func<TParm1, TParm2, TResult>>> expression, int index) {
            return html.Contents(model, html.GetAction(expression).Name, index);
        }

        /// <summary>
        ///     Get the contents of the identified parameter
        /// </summary>
        public static MvcHtmlString Contents<TModel, TParm1, TParm2, TParm3, TResult>(this HtmlHelper<TModel> html, Expression<Func<TModel, Func<TParm1, TParm2, TParm3, TResult>>> expression, int index) {
            return html.Contents(html.GetAction(expression).Name, index);
        }

        /// <summary>
        ///     Get the contents of the identified parameter
        /// </summary>
        public static MvcHtmlString Contents<TModel, TParm1, TParm2, TParm3, TResult>(this HtmlHelper html, TModel model, Expression<Func<TModel, Func<TParm1, TParm2, TParm3, TResult>>> expression, int index) {
            return html.Contents(model, html.GetAction(expression).Name, index);
        }

        /// <summary>
        ///     Get the contents of the identified parameter
        /// </summary>
        public static MvcHtmlString Contents<TModel, TParm1, TParm2, TParm3, TParm4, TResult>(this HtmlHelper<TModel> html, Expression<Func<TModel, Func<TParm1, TParm2, TParm3, TParm4, TResult>>> expression, int index) {
            return html.Contents(html.GetAction(expression).Name, index);
        }

        /// <summary>
        ///     Get the contents of the identified parameter
        /// </summary>
        public static MvcHtmlString Contents<TModel, TParm1, TParm2, TParm3, TParm4, TResult>(this HtmlHelper html, TModel model, Expression<Func<TModel, Func<TParm1, TParm2, TParm3, TParm4, TResult>>> expression, int index) {
            return html.Contents(model, html.GetAction(expression).Name, index);
        }

        /// <summary>
        ///     Get the contents of the identified parameter
        /// </summary>
        public static MvcHtmlString Contents<TModel>(this HtmlHelper<TModel> html, string actionId, int index) {
            if (html.ViewData.Model == null) {
                throw new ArgumentException("html");
            }
            return html.Contents(html.ViewData.Model, actionId, index);
        }

        /// <summary>
        ///     Get the contents of the identified parameter
        /// </summary>
        public static MvcHtmlString Contents<TModel>(this HtmlHelper html, TModel model, string actionId, int index) {
            var nakedObject = html.Surface().GetObject(model);
            var dflt = nakedObject.Specification.GetActionLeafNodes().Single(p => p.Id == actionId).Parameters[index].GetDefault(nakedObject);
            return MvcHtmlString.Create(dflt == null ? "" : dflt.TitleString());
        }

        #endregion

        #region description

        /// <summary>
        ///     Get the description of the identified parameter
        /// </summary>
        public static MvcHtmlString Description<TModel, TParm>(this HtmlHelper<TModel> html, Expression<Func<TModel, Action<TParm>>> expression, int index) {
            return html.Description(html.GetAction(expression).Name, index);
        }

        /// <summary>
        ///     Get the description of the identified parameter
        /// </summary>
        public static MvcHtmlString Description<TModel, TParm>(this HtmlHelper html, TModel model, Expression<Func<TModel, Action<TParm>>> expression, int index) {
            return html.Description(model, html.GetAction(expression).Name, index);
        }

        /// <summary>
        ///     Get the description of the identified parameter
        /// </summary>
        public static MvcHtmlString Description<TModel, TParm1, TParm2>(this HtmlHelper<TModel> html, Expression<Func<TModel, Action<TParm1, TParm2>>> expression, int index) {
            return html.Description(html.GetAction(expression).Name, index);
        }

        /// <summary>
        ///     Get the description of the identified parameter
        /// </summary>
        public static MvcHtmlString Description<TModel, TParm1, TParm2>(this HtmlHelper html, TModel model, Expression<Func<TModel, Action<TParm1, TParm2>>> expression, int index) {
            return html.Description(model, html.GetAction(expression).Name, index);
        }

        /// <summary>
        ///     Get the description of the identified parameter
        /// </summary>
        public static MvcHtmlString Description<TModel, TParm1, TParm2, TParm3>(this HtmlHelper<TModel> html, Expression<Func<TModel, Action<TParm1, TParm2, TParm3>>> expression, int index) {
            return html.Description(html.GetAction(expression).Name, index);
        }

        /// <summary>
        ///     Get the description of the identified parameter
        /// </summary>
        public static MvcHtmlString Description<TModel, TParm1, TParm2, TParm3>(this HtmlHelper html, TModel model, Expression<Func<TModel, Action<TParm1, TParm2, TParm3>>> expression, int index) {
            return html.Description(model, html.GetAction(expression).Name, index);
        }

        /// <summary>
        ///     Get the description of the identified parameter
        /// </summary>
        public static MvcHtmlString Description<TModel, TParm1, TParm2, TParm3, TParm4>(this HtmlHelper<TModel> html, Expression<Func<TModel, Action<TParm1, TParm2, TParm3, TParm4>>> expression, int index) {
            return html.Description(html.GetAction(expression).Name, index);
        }

        /// <summary>
        ///     Get the description of the identified parameter
        /// </summary>
        public static MvcHtmlString Description<TModel, TParm1, TParm2, TParm3, TParm4>(this HtmlHelper html, TModel model, Expression<Func<TModel, Action<TParm1, TParm2, TParm3, TParm4>>> expression, int index) {
            return html.Description(model, html.GetAction(expression).Name, index);
        }

        /// <summary>
        ///     Get the description of the identified parameter
        /// </summary>
        public static MvcHtmlString Description<TModel, TParm, TResult>(this HtmlHelper<TModel> html, Expression<Func<TModel, Func<TParm, TResult>>> expression, int index) {
            return html.Description(html.GetAction(expression).Name, index);
        }

        /// <summary>
        ///     Get the description of the identified parameter
        /// </summary>
        public static MvcHtmlString Description<TModel, TParm, TResult>(this HtmlHelper html, TModel model, Expression<Func<TModel, Func<TParm, TResult>>> expression, int index) {
            return html.Description(model, html.GetAction(expression).Name, index);
        }

        /// <summary>
        ///     Get the description of the identified parameter
        /// </summary>
        public static MvcHtmlString Description<TModel, TParm1, TParm2, TResult>(this HtmlHelper<TModel> html, Expression<Func<TModel, Func<TParm1, TParm2, TResult>>> expression, int index) {
            return html.Description(html.GetAction(expression).Name, index);
        }

        /// <summary>
        ///     Get the description of the identified parameter
        /// </summary>
        public static MvcHtmlString Description<TModel, TParm1, TParm2, TResult>(this HtmlHelper html, TModel model, Expression<Func<TModel, Func<TParm1, TParm2, TResult>>> expression, int index) {
            return html.Description(model, html.GetAction(expression).Name, index);
        }

        /// <summary>
        ///     Get the description of the identified parameter
        /// </summary>
        public static MvcHtmlString Description<TModel, TParm1, TParm2, TParm3, TResult>(this HtmlHelper<TModel> html, Expression<Func<TModel, Func<TParm1, TParm2, TParm3, TResult>>> expression, int index) {
            return html.Description(html.GetAction(expression).Name, index);
        }

        /// <summary>
        ///     Get the description of the identified parameter
        /// </summary>
        public static MvcHtmlString Description<TModel, TParm1, TParm2, TParm3, TResult>(this HtmlHelper html, TModel model, Expression<Func<TModel, Func<TParm1, TParm2, TParm3, TResult>>> expression, int index) {
            return html.Description(model, html.GetAction(expression).Name, index);
        }

        /// <summary>
        ///     Get the description of the identified parameter
        /// </summary>
        public static MvcHtmlString Description<TModel, TParm1, TParm2, TParm3, TParm4, TResult>(this HtmlHelper<TModel> html, Expression<Func<TModel, Func<TParm1, TParm2, TParm3, TParm4, TResult>>> expression, int index) {
            return html.Description(html.GetAction(expression).Name, index);
        }

        /// <summary>
        ///     Get the description of the identified parameter
        /// </summary>
        public static MvcHtmlString Description<TModel, TParm1, TParm2, TParm3, TParm4, TResult>(this HtmlHelper html, TModel model, Expression<Func<TModel, Func<TParm1, TParm2, TParm3, TParm4, TResult>>> expression, int index) {
            return html.Description(model, html.GetAction(expression).Name, index);
        }

        /// <summary>
        ///     Get the description of the identified parameter
        /// </summary>
        public static MvcHtmlString Description<TModel>(this HtmlHelper<TModel> html, string actionId, int index) {
            if (html.ViewData.Model == null) {
                throw new ArgumentException("html");
            }
            return html.Description(html.ViewData.Model, actionId, index);
        }

        /// <summary>
        ///     Get the description of the identified parameter
        /// </summary>
        //public static MvcHtmlString Description<TModel>(this HtmlHelper html, TModel model, string actionId, int index) {
        //    INakedObjectAdapter nakedObject = html.Framework().GetNakedObject(model);

        //    return MvcHtmlString.Create(nakedObject.Spec.GetActions().Single(p => p.Id == actionId).Parameters[index].Description);
        //}

        public static MvcHtmlString Description<TModel>(this HtmlHelper html, TModel model, string actionId, int index) {
            var nakedObject = html.Surface().GetObject(model);

            return MvcHtmlString.Create(nakedObject.Specification.GetActionLeafNodes().Single(p => p.Id == actionId).Parameters[index].Description());
        }

        #endregion

        #region name

        /// <summary>
        ///     Get the name of the identified parameter
        /// </summary>
        public static MvcHtmlString Name<TModel, TParm>(this HtmlHelper<TModel> html, Expression<Func<TModel, Action<TParm>>> expression, int index) {
            return html.Name(html.GetAction(expression).Name, index);
        }

        /// <summary>
        ///     Get the name of the identified parameter
        /// </summary>
        public static MvcHtmlString Name<TModel, TParm>(this HtmlHelper html, TModel model, Expression<Func<TModel, Action<TParm>>> expression, int index) {
            return html.Name(model, html.GetAction(expression).Name, index);
        }

        /// <summary>
        ///     Get the name of the identified parameter
        /// </summary>
        public static MvcHtmlString Name<TModel, TParm1, TParm2>(this HtmlHelper<TModel> html, Expression<Func<TModel, Action<TParm1, TParm2>>> expression, int index) {
            return html.Name(html.GetAction(expression).Name, index);
        }

        /// <summary>
        ///     Get the name of the identified parameter
        /// </summary>
        public static MvcHtmlString Name<TModel, TParm1, TParm2>(this HtmlHelper html, TModel model, Expression<Func<TModel, Action<TParm1, TParm2>>> expression, int index) {
            return html.Name(model, html.GetAction(expression).Name, index);
        }

        /// <summary>
        ///     Get the name of the identified parameter
        /// </summary>
        public static MvcHtmlString Name<TModel, TParm1, TParm2, TParm3>(this HtmlHelper<TModel> html, Expression<Func<TModel, Action<TParm1, TParm2, TParm3>>> expression, int index) {
            return html.Name(html.GetAction(expression).Name, index);
        }

        /// <summary>
        ///     Get the name of the identified parameter
        /// </summary>
        public static MvcHtmlString Name<TModel, TParm1, TParm2, TParm3>(this HtmlHelper html, TModel model, Expression<Func<TModel, Action<TParm1, TParm2, TParm3>>> expression, int index) {
            return html.Name(model, html.GetAction(expression).Name, index);
        }

        /// <summary>
        ///     Get the name of the identified parameter
        /// </summary>
        public static MvcHtmlString Name<TModel, TParm1, TParm2, TParm3, TParm4>(this HtmlHelper<TModel> html, Expression<Func<TModel, Action<TParm1, TParm2, TParm3, TParm4>>> expression, int index) {
            return html.Name(html.GetAction(expression).Name, index);
        }

        /// <summary>
        ///     Get the name of the identified parameter
        /// </summary>
        public static MvcHtmlString Name<TModel, TParm1, TParm2, TParm3, TParm4>(this HtmlHelper html, TModel model, Expression<Func<TModel, Action<TParm1, TParm2, TParm3, TParm4>>> expression, int index) {
            return html.Name(model, html.GetAction(expression).Name, index);
        }

        /// <summary>
        ///     Get the name of the identified parameter
        /// </summary>
        public static MvcHtmlString Name<TModel, TParm, TResult>(this HtmlHelper<TModel> html, Expression<Func<TModel, Func<TParm, TResult>>> expression, int index) {
            return html.Name(html.GetAction(expression).Name, index);
        }

        /// <summary>
        ///     Get the name of the identified parameter
        /// </summary>
        public static MvcHtmlString Name<TModel, TParm, TResult>(this HtmlHelper html, TModel model, Expression<Func<TModel, Func<TParm, TResult>>> expression, int index) {
            return html.Name(model, html.GetAction(expression).Name, index);
        }

        /// <summary>
        ///     Get the name of the identified parameter
        /// </summary>
        public static MvcHtmlString Name<TModel, TParm1, TParm2, TResult>(this HtmlHelper<TModel> html, Expression<Func<TModel, Func<TParm1, TParm2, TResult>>> expression, int index) {
            return html.Name(html.GetAction(expression).Name, index);
        }

        /// <summary>
        ///     Get the name of the identified parameter
        /// </summary>
        public static MvcHtmlString Name<TModel, TParm1, TParm2, TResult>(this HtmlHelper html, TModel model, Expression<Func<TModel, Func<TParm1, TParm2, TResult>>> expression, int index) {
            return html.Name(model, html.GetAction(expression).Name, index);
        }

        /// <summary>
        ///     Get the name of the identified parameter
        /// </summary>
        public static MvcHtmlString Name<TModel, TParm1, TParm2, TParm3, TResult>(this HtmlHelper<TModel> html, Expression<Func<TModel, Func<TParm1, TParm2, TParm3, TResult>>> expression, int index) {
            return html.Name(html.GetAction(expression).Name, index);
        }

        /// <summary>
        ///     Get the name of the identified parameter
        /// </summary>
        public static MvcHtmlString Name<TModel, TParm1, TParm2, TParm3, TResult>(this HtmlHelper html, TModel model, Expression<Func<TModel, Func<TParm1, TParm2, TParm3, TResult>>> expression, int index) {
            return html.Name(model, html.GetAction(expression).Name, index);
        }

        /// <summary>
        ///     Get the name of the identified parameter
        /// </summary>
        public static MvcHtmlString Name<TModel, TParm1, TParm2, TParm3, TParm4, TResult>(this HtmlHelper<TModel> html, Expression<Func<TModel, Func<TParm1, TParm2, TParm3, TParm4, TResult>>> expression, int index) {
            return html.Name(html.GetAction(expression).Name, index);
        }

        /// <summary>
        ///     Get the name of the identified parameter
        /// </summary>
        public static MvcHtmlString Name<TModel, TParm1, TParm2, TParm3, TParm4, TResult>(this HtmlHelper html, TModel model, Expression<Func<TModel, Func<TParm1, TParm2, TParm3, TParm4, TResult>>> expression, int index) {
            return html.Name(model, html.GetAction(expression).Name, index);
        }

        /// <summary>
        ///     Get the name of the identified parameter
        /// </summary>
        public static MvcHtmlString Name<TModel>(this HtmlHelper<TModel> html, string actionId, int index) {
            if (html.ViewData.Model == null) {
                throw new ArgumentException("html");
            }
            return html.Name(html.ViewData.Model, actionId, index);
        }

        /// <summary>
        ///     Get the name of the identified parameter
        /// </summary>
        //public static MvcHtmlString Name<TModel>(this HtmlHelper html, TModel model, string actionId, int index) {
        //    INakedObjectAdapter nakedObject = html.Framework().GetNakedObject(model);

        //    return MvcHtmlString.Create(nakedObject.Spec.GetActions().Single(p => p.Id == actionId).Parameters[index].Name);
        //}

        public static MvcHtmlString Name<TModel>(this HtmlHelper html, TModel model, string actionId, int index) {
            var nakedObject = html.Surface().GetObject(model);

            return MvcHtmlString.Create(nakedObject.Specification.GetActionLeafNodes().Single(p => p.Id == actionId).Parameters[index].Name());
        }

        #endregion

        #region typename

        /// <summary>
        ///     Get the short type name  of the identified parameter
        /// </summary>
        public static MvcHtmlString TypeName<TModel, TParm>(this HtmlHelper<TModel> html, Expression<Func<TModel, Action<TParm>>> expression, int index) {
            return html.TypeName(html.GetAction(expression).Name, index);
        }

        /// <summary>
        ///     Get the short type name  of the identified parameter
        /// </summary>
        public static MvcHtmlString TypeName<TModel, TParm>(this HtmlHelper html, TModel model, Expression<Func<TModel, Action<TParm>>> expression, int index) {
            return html.TypeName(model, html.GetAction(expression).Name, index);
        }

        /// <summary>
        ///     Get the short type name  of the identified parameter
        /// </summary>
        public static MvcHtmlString TypeName<TModel, TParm1, TParm2>(this HtmlHelper<TModel> html, Expression<Func<TModel, Action<TParm1, TParm2>>> expression, int index) {
            return html.TypeName(html.GetAction(expression).Name, index);
        }

        /// <summary>
        ///     Get the short type name  of the identified parameter
        /// </summary>
        public static MvcHtmlString TypeName<TModel, TParm1, TParm2>(this HtmlHelper html, TModel model, Expression<Func<TModel, Action<TParm1, TParm2>>> expression, int index) {
            return html.TypeName(model, html.GetAction(expression).Name, index);
        }

        /// <summary>
        ///     Get the short type name  of the identified parameter
        /// </summary>
        public static MvcHtmlString TypeName<TModel, TParm1, TParm2, TParm3>(this HtmlHelper<TModel> html, Expression<Func<TModel, Action<TParm1, TParm2, TParm3>>> expression, int index) {
            return html.TypeName(html.GetAction(expression).Name, index);
        }

        /// <summary>
        ///     Get the short type name  of the identified parameter
        /// </summary>
        public static MvcHtmlString TypeName<TModel, TParm1, TParm2, TParm3>(this HtmlHelper html, TModel model, Expression<Func<TModel, Action<TParm1, TParm2, TParm3>>> expression, int index) {
            return html.TypeName(model, html.GetAction(expression).Name, index);
        }

        /// <summary>
        ///     Get the short type name  of the identified parameter
        /// </summary>
        public static MvcHtmlString TypeName<TModel, TParm1, TParm2, TParm3, TParm4>(this HtmlHelper<TModel> html, Expression<Func<TModel, Action<TParm1, TParm2, TParm3, TParm4>>> expression, int index) {
            return html.TypeName(html.GetAction(expression).Name, index);
        }

        /// <summary>
        ///     Get the short type name  of the identified parameter
        /// </summary>
        public static MvcHtmlString TypeName<TModel, TParm1, TParm2, TParm3, TParm4>(this HtmlHelper html, TModel model, Expression<Func<TModel, Action<TParm1, TParm2, TParm3, TParm4>>> expression, int index) {
            return html.TypeName(model, html.GetAction(expression).Name, index);
        }

        /// <summary>
        ///     Get the short type name  of the identified parameter
        /// </summary>
        public static MvcHtmlString TypeName<TModel, TParm, TResult>(this HtmlHelper<TModel> html, Expression<Func<TModel, Func<TParm, TResult>>> expression, int index) {
            return html.TypeName(html.GetAction(expression).Name, index);
        }

        /// <summary>
        ///     Get the short type name  of the identified parameter
        /// </summary>
        public static MvcHtmlString TypeName<TModel, TParm, TResult>(this HtmlHelper html, TModel model, Expression<Func<TModel, Func<TParm, TResult>>> expression, int index) {
            return html.TypeName(model, html.GetAction(expression).Name, index);
        }

        /// <summary>
        ///     Get the short type name  of the identified parameter
        /// </summary>
        public static MvcHtmlString TypeName<TModel, TParm1, TParm2, TResult>(this HtmlHelper<TModel> html, Expression<Func<TModel, Func<TParm1, TParm2, TResult>>> expression, int index) {
            return html.TypeName(html.GetAction(expression).Name, index);
        }

        /// <summary>
        ///     Get the short type name  of the identified parameter
        /// </summary>
        public static MvcHtmlString TypeName<TModel, TParm1, TParm2, TResult>(this HtmlHelper html, TModel model, Expression<Func<TModel, Func<TParm1, TParm2, TResult>>> expression, int index) {
            return html.TypeName(model, html.GetAction(expression).Name, index);
        }

        /// <summary>
        ///     Get the short type name  of the identified parameter
        /// </summary>
        public static MvcHtmlString TypeName<TModel, TParm1, TParm2, TParm3, TResult>(this HtmlHelper<TModel> html, Expression<Func<TModel, Func<TParm1, TParm2, TParm3, TResult>>> expression, int index) {
            return html.TypeName(html.GetAction(expression).Name, index);
        }

        /// <summary>
        ///     Get the short type name  of the identified parameter
        /// </summary>
        public static MvcHtmlString TypeName<TModel, TParm1, TParm2, TParm3, TResult>(this HtmlHelper html, TModel model, Expression<Func<TModel, Func<TParm1, TParm2, TParm3, TResult>>> expression, int index) {
            return html.TypeName(model, html.GetAction(expression).Name, index);
        }

        /// <summary>
        ///     Get the short type name  of the identified parameter
        /// </summary>
        public static MvcHtmlString TypeName<TModel, TParm1, TParm2, TParm3, TParm4, TResult>(this HtmlHelper<TModel> html, Expression<Func<TModel, Func<TParm1, TParm2, TParm3, TParm4, TResult>>> expression, int index) {
            return html.TypeName(html.GetAction(expression).Name, index);
        }

        /// <summary>
        ///     Get the short type name  of the identified parameter
        /// </summary>
        public static MvcHtmlString TypeName<TModel, TParm1, TParm2, TParm3, TParm4, TResult>(this HtmlHelper html, TModel model, Expression<Func<TModel, Func<TParm1, TParm2, TParm3, TParm4, TResult>>> expression, int index) {
            return html.TypeName(model, html.GetAction(expression).Name, index);
        }

        /// <summary>
        ///     Get the short type name  of the identified parameter
        /// </summary>
        public static MvcHtmlString TypeName<TModel>(this HtmlHelper<TModel> html, string actionId, int index) {
            if (html.ViewData.Model == null) {
                throw new ArgumentException("html");
            }
            return html.TypeName(html.ViewData.Model, actionId, index);
        }

        /// <summary>
        ///     Get the short type name  of the identified parameter
        /// </summary>
        //public static MvcHtmlString TypeName<TModel>(this HtmlHelper html, TModel model, string actionId, int index) {
        //    INakedObjectAdapter nakedObject = html.Framework().GetNakedObject(model);

        //    return MvcHtmlString.Create(nakedObject.Spec.GetActions().Single(p => p.Id == actionId).Parameters[index].Spec.ShortName);
        //}

        public static MvcHtmlString TypeName<TModel>(this HtmlHelper html, TModel model, string actionId, int index) {
            var nakedObject = html.Surface().GetObject(model);

            return MvcHtmlString.Create(nakedObject.Specification.GetActionLeafNodes().Single(p => p.Id == actionId).Parameters[index].Specification.FullName().Split('.').Last());
        }

        #endregion

        #region encrypted

        private static Tuple<string, string> EncryptValue(HtmlHelper html, string name, string value) {
            var encryptDecrypt = html.ViewData[IdConstants.NofEncryptDecrypt] as IEncryptDecrypt;
            if (encryptDecrypt != null) {
                return encryptDecrypt.Encrypt(html.ViewContext.HttpContext.Session, name, value);
            }
            return new Tuple<string, string>(name, value);
        }

        public static MvcHtmlString Encrypted(this HtmlHelper html, string name, string value) {
            Tuple<string, string> result = EncryptValue(html, name, value);
            return html.Hidden(result.Item1, result.Item2);
        }

        public static string CustomEncrypted(this HtmlHelper html, string name, string value) {
            // to avoid default MVC helper behaviour of picking up values from ModelState
            Tuple<string, string> result = EncryptValue(html, name, value);
            var tag = new TagBuilder("input");
            tag.GenerateId(name);
            tag.MergeAttribute("name", result.Item1);
            tag.MergeAttribute("value", result.Item2);
            tag.MergeAttribute("type", "hidden");
            return tag.ToString(TagRenderMode.SelfClosing);
        }

        #endregion
    }
}
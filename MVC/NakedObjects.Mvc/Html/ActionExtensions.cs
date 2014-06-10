// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets.Presentation;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Core.Context;
using NakedObjects.Resources;
using NakedObjects.Web.Mvc.Helpers;

namespace NakedObjects.Web.Mvc.Html {
    public static class ActionExtensions {
        #region name and ids

        /// <summary>
        ///     Get the id for an action dialog
        /// </summary>
        public static MvcHtmlString ObjectActionDialogId(this HtmlHelper html, object domainObject, INakedObjectAction action) {
            INakedObject nakedObject = FrameworkHelper.GetNakedObject(domainObject);
            return MvcHtmlString.Create(IdHelper.GetActionDialogId(nakedObject, action));
        }

        /// <summary>
        ///     Get the name for an action on an object
        /// </summary>
        public static MvcHtmlString ObjectActionName(this HtmlHelper html, string name) {
            return CommonHtmlHelper.WrapInDiv(name, IdHelper.ActionNameLabel);
        }

        private static string GetPresentationHint(INakedObjectAction action) {
            var facet = action.GetFacet<IPresentationHintFacet>();
            return facet == null ? "" : " " + facet.Value;
        }

        /// <summary>
        ///     Get the classes for an action on an object
        /// </summary>
        public static MvcHtmlString ObjectActionClass(this HtmlHelper html, INakedObjectAction action) {
            return MvcHtmlString.Create(IdHelper.ActionAction + GetPresentationHint(action));
        }

        #endregion

        #region parameter list

        /// <summary>
        ///     Get the parameters of an action for display within a form
        /// </summary>
        public static MvcHtmlString ParameterList(this HtmlHelper html,
                                                  object contextObject,
                                                  object targetObject,
                                                  INakedObjectAction contextAction,
                                                  INakedObjectAction targetAction,
                                                  string propertyName,
                                                  IEnumerable collection) {
            var actionContext = new ActionContext(false, contextObject, contextAction);

            return ParameterList(contextAction, targetObject, targetAction, propertyName, collection, html, actionContext);
        }

        /// <summary>
        ///     Get the parameters of an action for display within a form
        /// </summary>
        public static MvcHtmlString ParameterListWith(this HtmlHelper html,
                                                      object contextObject,
                                                      object targetObject,
                                                      INakedObjectAction contextAction,
                                                      INakedObjectAction targetAction,
                                                      string propertyName,
                                                      IEnumerable collection) {
            var actionContext = new ActionContext(false, contextObject, contextAction) {Filter = x => x.Id == propertyName};


            return ParameterList(contextAction, targetObject, targetAction, propertyName, collection, html, actionContext);
        }


        private static MvcHtmlString ParameterList(INakedObjectAction contextAction, object targetObject, INakedObjectAction targetAction, string propertyName, IEnumerable collection, HtmlHelper html, ActionContext actionContext) {
            if ((targetObject == null || targetAction == null || string.IsNullOrEmpty(propertyName)) && collection == null) {
                return html.ParameterList(actionContext);
            }


            var targetActionContext = new ActionContext(false, targetObject, targetAction);
            return html.BuildParamContainer(actionContext,
                                            html.ActionParameterFields(actionContext, targetActionContext, propertyName, collection),
                                            IdHelper.ParamContainerName,
                                            IdHelper.GetParameterContainerId(contextAction));
        }

        /// <summary>
        ///     Get the parameters of an action for display within a form
        /// </summary>
        public static MvcHtmlString ParameterList(this HtmlHelper html, object context, INakedObjectAction action) {
            var actionContext = new ActionContext(false, context, action);
            return html.ParameterList(actionContext);
        }

        public static MvcHtmlString ParameterListWith(this HtmlHelper html, object context, INakedObjectAction action, string parameterName) {
            var actionContext = new ActionContext(false, context, action) {Filter = x => x.Id == parameterName};
            return html.ParameterList(actionContext);
        }

        internal static MvcHtmlString ParameterList(this HtmlHelper html, ActionContext actionContext) {
            return html.BuildParamContainer(actionContext,
                                            html.ActionParameterFields(actionContext),
                                            IdHelper.ParamContainerName,
                                            IdHelper.GetParameterContainerId(actionContext.Action));
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
        ///     Create menu from actions of domainObject
        /// </summary>
        public static MvcHtmlString Menu(this HtmlHelper html, object domainObject) {
            INakedObject nakedObject = FrameworkHelper.GetNakedObject(domainObject);
            return CommonHtmlHelper.BuildMenuContainer(html.ObjectActions(nakedObject, false),
                                                       IdHelper.MenuContainerName,
                                                       IdHelper.GetActionContainerId(nakedObject),
                                                       IdHelper.GetActionLabel(nakedObject));
        }

        /// <summary>
        ///     Create menu from actions of domainObject - inserting additional items from menuItems parameter
        /// </summary>
        public static MvcHtmlString Menu(this HtmlHelper html, object domainObject, params CustomMenuItem[] menuItems) {
            INakedObject nakedObject = FrameworkHelper.GetNakedObject(domainObject);
            return CommonHtmlHelper.BuildMenuContainer(html.ObjectActions(nakedObject, false, menuItems),
                                                       IdHelper.MenuContainerName,
                                                       IdHelper.GetActionContainerId(nakedObject),
                                                       IdHelper.GetActionLabel(nakedObject));
        }

        /// <summary>
        ///     Create menu from menuItems parameter
        /// </summary>
        public static MvcHtmlString Menu(this HtmlHelper html, string name, params CustomMenuItem[] menuItems) {
            return CommonHtmlHelper.BuildMenuContainer(html.ObjectActions(false, menuItems),
                                                       IdHelper.MenuContainerName,
                                                       IdHelper.MakeId(name, IdHelper.ActionContainerName),
                                                       name + " " + MvcUi.Actions);
        }

        /// <summary>
        ///     Create menu from actions of domainObject
        /// </summary>
        public static MvcHtmlString MenuOnTransient(this HtmlHelper html, object domainObject) {
            INakedObject nakedObject = FrameworkHelper.GetNakedObject(domainObject);
            return CommonHtmlHelper.BuildMenuContainer(html.ObjectActions(nakedObject, true),
                                                       IdHelper.MenuContainerName,
                                                       IdHelper.GetActionContainerId(nakedObject),
                                                       IdHelper.GetActionLabel(nakedObject));
        }

        /// <summary>
        ///     Create menu from actions of domainObject - inserting additional items from menuItems parameter
        /// </summary>
        public static MvcHtmlString MenuOnTransient(this HtmlHelper html, object domainObject, params CustomMenuItem[] menuItems) {
            INakedObject nakedObject = FrameworkHelper.GetNakedObject(domainObject);
            return CommonHtmlHelper.BuildMenuContainer(html.ObjectActions(nakedObject, true, menuItems),
                                                       IdHelper.MenuContainerName,
                                                       IdHelper.GetActionContainerId(nakedObject),
                                                       IdHelper.GetActionLabel(nakedObject));
        }

        /// <summary>
        ///     Create menu from menuItems parameter
        /// </summary>
        public static MvcHtmlString MenuOnTransient(this HtmlHelper html, string name, params CustomMenuItem[] menuItems) {
            return CommonHtmlHelper.BuildMenuContainer(html.ObjectActions(true, menuItems),
                                                       IdHelper.MenuContainerName,
                                                       IdHelper.MakeId(name, IdHelper.ActionContainerName),
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
            INakedObject nakedObject = FrameworkHelper.GetNakedObject(model);
            INakedObjectAction action = GetObjectAndContributedActions(nakedObject).SingleOrDefault(a => a.Id == id);
            ValidateParamValues(action, paramValues);
            return action == null ? MvcHtmlString.Create("") : html.ObjectAction(new ActionContext(nakedObject, action) {ParameterValues = new RouteValueDictionary(paramValues)});
        }

        private static void ValidateParamValues(INakedObjectAction action, object paramValues) {
            if (paramValues != null && action.Parameters.Select(p => p.Specification).Any(s => s.IsCollection)) {
                throw new NotSupportedException("Cannot pass collection as parameter value to custom ObjectAction");
            }
        }

        private static IEnumerable<INakedObjectAction> GetObjectAndContributedActions(INakedObject nakedObject) {
            return nakedObject.Specification.GetObjectActions().Union(
                nakedObject.Specification.GetObjectActions().
                            Where(a => a.ActionType == NakedObjectActionType.Set).SelectMany(a => a.Actions)).
                               Where(a => a.IsVisible(NakedObjectsContext.Session, nakedObject));
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
            INakedObject nakedObject = FrameworkHelper.GetNakedObject(model);
            INakedObjectAction action = nakedObject.Specification.GetObjectActions().Single(a => a.Id == id);
            return html.ObjectActionOnTransient(new ActionContext(nakedObject, action));
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
            INakedObject nakedObject = FrameworkHelper.GetNakedObject(model);
            INakedObjectAction action = GetObjectAndContributedActions(nakedObject).Where(a => a.Id == id).SingleOrDefault();
            return action == null ? MvcHtmlString.Create("") : html.ObjectActionAsDialog(new ActionContext(nakedObject, action));
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
            INakedObject nakedObject = FrameworkHelper.GetNakedObject(model);

            return MvcHtmlString.Create(nakedObject.Specification.GetObjectActions().Single(p => p.Id == actionId).Parameters[index].GetDefault(nakedObject).TitleString());
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
        public static MvcHtmlString Description<TModel>(this HtmlHelper html, TModel model, string actionId, int index) {
            INakedObject nakedObject = FrameworkHelper.GetNakedObject(model);

            return MvcHtmlString.Create(nakedObject.Specification.GetObjectActions().Single(p => p.Id == actionId).Parameters[index].Description);
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
        public static MvcHtmlString Name<TModel>(this HtmlHelper html, TModel model, string actionId, int index) {
            INakedObject nakedObject = FrameworkHelper.GetNakedObject(model);

            return MvcHtmlString.Create(nakedObject.Specification.GetObjectActions().Single(p => p.Id == actionId).Parameters[index].Name);
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
        public static MvcHtmlString TypeName<TModel>(this HtmlHelper html, TModel model, string actionId, int index) {
            INakedObject nakedObject = FrameworkHelper.GetNakedObject(model);

            return MvcHtmlString.Create(nakedObject.Specification.GetObjectActions().Single(p => p.Id == actionId).Parameters[index].Specification.ShortName);
        }

        #endregion

        #region encrypted

        private static Tuple<string, string> EncryptValue(HtmlHelper html, string name, string value) {
            var encryptDecrypt = html.ViewData[IdHelper.NofEncryptDecrypt] as IEncryptDecrypt;
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
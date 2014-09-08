// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Core.Persist;

namespace NakedObjects.Web.Mvc.Html {
    public static class CollectionExtensions {

        private static INakedObjectsFramework Framework(this HtmlHelper html) {
            return (INakedObjectsFramework)html.ViewData["NakedObjectsFramework"];
        }

        #region all

        public static MvcHtmlString Collection(this HtmlHelper html, IEnumerable collection, INakedObjectAction action, string defaultTo = IdHelper.ListDisplayFormat) {
            bool renderEagerly = CommonHtmlHelper.RenderEagerly(action);
            string displayType = DefaultFormat(html, renderEagerly ? IdHelper.TableDisplayFormat : defaultTo);
            return displayType == IdHelper.TableDisplayFormat ? CollectionTableInternal(html, collection, action) : CollectionListInternal(html, collection, action);
        }

        public static MvcHtmlString[] Collections(this HtmlHelper html, object domainObject, string defaultTo = IdHelper.ListDisplayFormat) {
            INakedObject adapter = html.Framework().GetNakedObject(domainObject);
            IEnumerable<INakedObject> collections = adapter.Specification.Properties.Where(obj => obj.IsCollection).Select(a => a.GetNakedObject(adapter, html.Framework().ObjectPersistor));
            return collections.Select(c => html.Collection(c.GetAsEnumerable(html.Framework().ObjectPersistor), null, defaultTo)).ToArray();
        }

        public static MvcHtmlString CollectionTable(this HtmlHelper html, IEnumerable collection, INakedObjectAction action) {
            return html.Collection(collection, action, IdHelper.TableDisplayFormat);
        }

        public static MvcHtmlString CollectionList(this HtmlHelper html, IEnumerable collection, INakedObjectAction action) {
            return html.Collection(collection, action);
        }

        public static string[] CollectionTitles(this HtmlHelper html, object domainObject, string format) {
            INakedObject adapter = html.Framework().GetNakedObject(domainObject);
            var collections = adapter.Specification.Properties.Where(obj => obj.IsCollection && obj.IsVisible(html.Framework().Session, adapter, html.Framework().ObjectPersistor)).Select(a => new { assoc = a, val = a.GetNakedObject(adapter, html.Framework().ObjectPersistor) });

            return collections.Select(coll => string.Format(format, coll.assoc.GetName(html.Framework().ObjectPersistor), coll.val.TitleString())).ToArray();
        }

        #endregion

        internal static MvcHtmlString CollectionTableInternal(this HtmlHelper html, IEnumerable collection, INakedObjectAction action = null) {
            INakedObject nakedObject = html.Framework().GetNakedObject(collection);

            Func<INakedObjectAssociation, bool> filterFunc;
            Func<INakedObjectAssociation, int> orderFunc;
            bool withTitle;

            if (action == null || action.ReturnType.IsVoid) {
                var memento = nakedObject.Oid as CollectionMemento;
                if (memento != null) {
                    action = memento.Action;
                }
            }

            CommonHtmlHelper.GetTableColumnInfo(action, out filterFunc, out orderFunc, out withTitle);

            return html.GetStandaloneCollection(nakedObject, filterFunc, orderFunc, withTitle);
        }

        internal static MvcHtmlString CollectionListInternal(this HtmlHelper html, IEnumerable collection, INakedObjectAction action = null) {
            INakedObject nakedObject = html.Framework().GetNakedObject(collection);
            return html.GetStandaloneList(nakedObject, null);
        }

        #region without

        // exclusions 


        /// <summary>
        /// Display ViewData Model as a collection excluding columns identified by the excludingColumns parameters 
        /// </summary>
        /// <example>
        /// html.CollectionTableWithout(y => y.TestCollectionOne, y => y.TestInt)
        /// </example>
        public static MvcHtmlString CollectionTableWithout<TModel>(this HtmlHelper<IEnumerable<TModel>> html, params Expression<Func<TModel, object>>[] excludingColumns) {
            return html.CollectionTableWithout(excludingColumns.Select(ex => html.GetProperty(ex).Name).ToArray());
        }

        /// <summary>
        /// Display model parameter as a collection excluding columns identified by the excludingColumns parameters 
        /// </summary>
        /// <example>
        /// html.CollectionTableWithout(obj, y => y.TestCollectionOne, y => y.TestInt)
        /// </example>
        public static MvcHtmlString CollectionTableWithout<TModel>(this HtmlHelper html, IEnumerable<TModel> model, params Expression<Func<TModel, object>>[] excludingColumns) {      
            return html.CollectionTableWithout(model, excludingColumns.Select(ex => html.GetProperty(ex).Name).ToArray());
        }

        /// <summary>
        /// Display ViewData Model as a collection excluding columns identified by the excludingColumns parameters 
        /// </summary>
        /// <example>
        /// html.CollectionTableWithout("TestCollectionOne", "TestInt")
        /// </example>
        public static MvcHtmlString CollectionTableWithout<TModel>(this HtmlHelper<IEnumerable<TModel>> html, params string[] excludingColumns) {
            if (html.ViewData.Model == null) {
                throw new ArgumentException("html");
            }
            return html.CollectionTableWithout(html.ViewData.Model, excludingColumns);
        }

        /// <summary>
        /// Display domainobject parameter as a collection excluding columns identified by the excludingColumns parameters 
        /// </summary>
        /// <example>
        /// html.CollectionTableWithout(obj, "TestCollectionOne", "TestInt")
        /// </example>
        public static MvcHtmlString CollectionTableWithout(this HtmlHelper html, IEnumerable domainObject, params string[] excludingColumns) {
            INakedObject nakedObject = html.Framework().GetNakedObject(domainObject);
            string displayType = DefaultFormat(html, IdHelper.TableDisplayFormat);
            return displayType == IdHelper.TableDisplayFormat ?
                html.GetStandaloneCollection(nakedObject, x => !excludingColumns.Any(s => s == x.Id), null, true) :
                html.GetStandaloneList(nakedObject, null);
        }


        /// <summary>
        /// Display ViewData Model as a collection excluding columns identified by the excludingColumns parameters 
        /// </summary>
        /// <example>
        /// html.CollectionTableWithout(y => y.TestCollectionOne, y => y.TestInt)
        /// </example>
        public static MvcHtmlString CollectionListWithout<TModel>(this HtmlHelper<IEnumerable<TModel>> html, params Expression<Func<TModel, object>>[] excludingColumns) {
            return html.CollectionListWithout(excludingColumns.Select(ex => html.GetProperty(ex).Name).ToArray());
        }

        /// <summary>
        /// Display model parameter as a collection excluding columns identified by the excludingColumns parameters 
        /// </summary>
        /// <example>
        /// html.CollectionTableWithout(obj, y => y.TestCollectionOne, y => y.TestInt)
        /// </example>
        public static MvcHtmlString CollectionListWithout<TModel>(this HtmlHelper html, IEnumerable<TModel> model, params Expression<Func<TModel, object>>[] excludingColumns) {
            return html.CollectionListWithout(model, excludingColumns.Select(ex => html.GetProperty(ex).Name).ToArray());
        }

        /// <summary>
        /// Display ViewData Model as a collection excluding columns identified by the excludingColumns parameters 
        /// </summary>
        /// <example>
        /// html.CollectionTableWithout("TestCollectionOne", "TestInt")
        /// </example>
        public static MvcHtmlString CollectionListWithout<TModel>(this HtmlHelper<IEnumerable<TModel>> html, params string[] excludingColumns) {
            if (html.ViewData.Model == null) {
                throw new ArgumentException("html");
            }
            return html.CollectionListWithout(html.ViewData.Model, excludingColumns);
        }

        /// <summary>
        /// Display domainobject parameter as a collection excluding columns identified by the excludingColumns parameters 
        /// </summary>
        /// <example>
        /// html.CollectionTableWithout(obj, "TestCollectionOne", "TestInt")
        /// </example>
        public static MvcHtmlString CollectionListWithout(this HtmlHelper html, IEnumerable domainObject, params string[] excludingColumns) {
            INakedObject nakedObject = html.Framework().GetNakedObject(domainObject);
            string displayType = DefaultFormat(html, IdHelper.ListDisplayFormat);
            return displayType == IdHelper.TableDisplayFormat ?
                html.GetStandaloneCollection(nakedObject, x => !excludingColumns.Any(s => s == x.Id), null, true) :
                html.GetStandaloneList(nakedObject, null);
        }



        #endregion

        #region with

        // inclusions 

        /// <summary>
        /// Display ViewData Model as a collection including only columns identified by the includingColumns parameters 
        /// </summary>
        /// <example>
        /// html.CollectionTableWith(y => y.TestCollectionOne, y => y.TestInt)
        /// </example>
        public static MvcHtmlString CollectionTableWith<TModel>(this HtmlHelper<IEnumerable<TModel>> html, params Expression<Func<TModel, object>>[] includingColumns) {
           
            return html.CollectionTableWith(includingColumns.Select(ex => html.GetProperty(ex).Name).ToArray());
        }

        /// <summary>
        /// Display ViewData Model as a collection including only columns identified by the includingColumns parameters 
        /// </summary>
        /// <example>
        /// html.CollectionTableWith(obj, y => y.TestCollectionOne, y => y.TestInt)
        /// </example>
        public static MvcHtmlString CollectionTableWith<TModel>(this HtmlHelper html, IEnumerable<TModel> model, params Expression<Func<TModel, object>>[] includingColumns) {
          
            return html.CollectionTableWith(model, includingColumns.Select(ex => html.GetProperty(ex).Name).ToArray());
        }

        /// <summary>
        /// Display ViewData Model as a collection including only columns identified by the includingColumns parameters 
        /// </summary>
        /// <example>
        /// html.CollectionTableWithout("TestCollectionOne", "TestInt")
        /// </example>
        public static MvcHtmlString CollectionTableWith<TModel>(this HtmlHelper<IEnumerable<TModel>> html, params string[] includingColumns) {
            if (html.ViewData.Model == null) {
                throw new ArgumentException("html");
            }
          
            return html.CollectionTableWith(html.ViewData.Model, includingColumns);
        }

        /// <summary>
        /// Display ViewData Model as a collection including only columns identified by the includingColumns parameters 
        /// </summary>
        /// <example>
        /// html.CollectionTableWithout(obj, "TestCollectionOne", "TestInt")
        /// </example>
        public static MvcHtmlString CollectionTableWith(this HtmlHelper html, IEnumerable domainObject, params string[] includingColumns) {
            INakedObject nakedObject = html.Framework().GetNakedObject(domainObject);
            string displayType = DefaultFormat(html, IdHelper.TableDisplayFormat);
            return displayType == IdHelper.TableDisplayFormat ?
                html.GetStandaloneCollection(nakedObject, x => includingColumns.Any(s => s == x.Id), x => Array.IndexOf(includingColumns, x.Id), true) :
                html.GetStandaloneList(nakedObject, null);
        }


        private static string DefaultFormat(HtmlHelper html, string defaultTo) {
            string displayType = html.ViewData.ContainsKey(IdHelper.CollectionFormat) ? (string)html.ViewData[IdHelper.CollectionFormat] : defaultTo;
            html.ViewData[IdHelper.CollectionFormat] = displayType; // ensure default value is saved
            return displayType;
        }

        /// <summary>
        /// Display ViewData Model as a collection including only columns identified by the includingColumns parameters 
        /// </summary>
        /// <example>
        /// html.CollectionTableWith(y => y.TestCollectionOne, y => y.TestInt)
        /// </example>
        public static MvcHtmlString CollectionListWith<TModel>(this HtmlHelper<IEnumerable<TModel>> html, params Expression<Func<TModel, object>>[] includingColumns) {

            return html.CollectionListWith(includingColumns.Select(ex => html.GetProperty(ex).Name).ToArray());
        }

        /// <summary>
        /// Display ViewData Model as a collection including only columns identified by the includingColumns parameters 
        /// </summary>
        /// <example>
        /// html.CollectionTableWith(obj, y => y.TestCollectionOne, y => y.TestInt)
        /// </example>
        public static MvcHtmlString CollectionListWith<TModel>(this HtmlHelper html, IEnumerable<TModel> model, params Expression<Func<TModel, object>>[] includingColumns) {

            return html.CollectionListWith(model, includingColumns.Select(ex => html.GetProperty(ex).Name).ToArray());
        }

        /// <summary>
        /// Display ViewData Model as a collection including only columns identified by the includingColumns parameters 
        /// </summary>
        /// <example>
        /// html.CollectionTableWithout("TestCollectionOne", "TestInt")
        /// </example>
        public static MvcHtmlString CollectionListWith<TModel>(this HtmlHelper<IEnumerable<TModel>> html, params string[] includingColumns) {
            if (html.ViewData.Model == null) {
                throw new ArgumentException("html");
            }

            return html.CollectionListWith(html.ViewData.Model, includingColumns);
        }

        /// <summary>
        /// Display ViewData Model as a collection including only columns identified by the includingColumns parameters 
        /// </summary>
        /// <example>
        /// html.CollectionTableWithout(obj, "TestCollectionOne", "TestInt")
        /// </example>
        public static MvcHtmlString CollectionListWith(this HtmlHelper html, IEnumerable domainObject, params string[] includingColumns) {
            INakedObject nakedObject = html.Framework().GetNakedObject(domainObject);
            string displayType = DefaultFormat(html, IdHelper.ListDisplayFormat);
            return displayType == IdHelper.TableDisplayFormat ?
                html.GetStandaloneCollection(nakedObject, x => includingColumns.Any(s => s == x.Id), x => Array.IndexOf(includingColumns, x.Id), true) :
                html.GetStandaloneList(nakedObject, null);
        }



        #endregion
    }
}
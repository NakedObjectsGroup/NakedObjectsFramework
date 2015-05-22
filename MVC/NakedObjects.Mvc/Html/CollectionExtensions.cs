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
using NakedObjects.Surface;
using NakedObjects.Surface.Utility;

namespace NakedObjects.Web.Mvc.Html {
    public static class CollectionExtensions {
        internal static MvcHtmlString CollectionTableInternal(this HtmlHelper html, IEnumerable collection, INakedObjectActionSurface action = null) {
            var nakedObject = html.Surface().GetObject(collection);

            Func<INakedObjectAssociationSurface, bool> filterFunc;
            Func<INakedObjectAssociationSurface, int> orderFunc;
            bool withTitle;

            if (action == null || action.ReturnType.IsVoid) {
                // todo investigate other ways to do this 
                action = nakedObject.MementoAction;
            }

            CommonHtmlHelper.GetTableColumnInfo(action, out filterFunc, out orderFunc, out withTitle);

            return html.GetStandaloneCollection(nakedObject, filterFunc, orderFunc, withTitle);
        }

        internal static MvcHtmlString CollectionListInternal(this HtmlHelper html, IEnumerable collection, INakedObjectActionSurface action = null) {
            var nakedObject = html.Surface().GetObject(collection);
            return html.GetStandaloneList(nakedObject, null);
        }

        #region all

        public static MvcHtmlString Collection(this HtmlHelper html, IEnumerable collection, INakedObjectActionSurface action, string defaultTo = IdConstants.ListDisplayFormat) {
            bool renderEagerly = CommonHtmlHelper.RenderEagerly(action);
            string displayType = DefaultFormat(html, renderEagerly ? IdConstants.TableDisplayFormat : defaultTo);
            return displayType == IdConstants.TableDisplayFormat ? CollectionTableInternal(html, collection, action) : CollectionListInternal(html, collection, action);
        }

        public static MvcHtmlString[] Collections(this HtmlHelper html, object domainObject, string defaultTo = IdConstants.ListDisplayFormat) {
            var adapter = html.Surface().GetObject(domainObject);
            IEnumerable<INakedObjectSurface> collections = adapter.Specification.Properties.Where(p => p.IsCollection).Select(a => a.GetNakedObject(adapter));
            return collections.Select(c => html.Collection(c.ToEnumerable(), (INakedObjectActionSurface) null, defaultTo)).ToArray();
        }

        public static MvcHtmlString CollectionTable(this HtmlHelper html, IEnumerable collection, INakedObjectActionSurface action) {
            return html.Collection(collection, action, IdConstants.TableDisplayFormat);
        }

        public static MvcHtmlString CollectionList(this HtmlHelper html, IEnumerable collection, INakedObjectActionSurface action) {
            return html.Collection(collection, action);
        }

        public static string[] CollectionTitles(this HtmlHelper html, object domainObject, string format) {
            var adapter = html.Surface().GetObject(domainObject);
            var collections = adapter.Specification.Properties.Where(obj => obj.Specification.IsCollection && obj.IsVisible(adapter)).Select(a => new {assoc = a, val = a.GetNakedObject(adapter)});
            return collections.Select(coll => string.Format(format, coll.assoc.Name, coll.val.TitleString)).ToArray();
        }

        #endregion

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
            var nakedObject = html.Surface().GetObject(domainObject);
            string displayType = DefaultFormat(html, IdConstants.TableDisplayFormat);
            return displayType == IdConstants.TableDisplayFormat ?
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
            var nakedObject = html.Surface().GetObject(domainObject);
            string displayType = DefaultFormat(html, IdConstants.ListDisplayFormat);
            return displayType == IdConstants.TableDisplayFormat ?
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
            var nakedObject = html.Surface().GetObject(domainObject);
            string displayType = DefaultFormat(html, IdConstants.TableDisplayFormat);
            return displayType == IdConstants.TableDisplayFormat ?
                html.GetStandaloneCollection(nakedObject, x => includingColumns.Any(s => s == x.Id), x => Array.IndexOf(includingColumns, x.Id), true) :
                html.GetStandaloneList(nakedObject, null);
        }

        private static string DefaultFormat(HtmlHelper html, string defaultTo) {
            string displayType = html.ViewData.ContainsKey(IdConstants.CollectionFormat) ? (string) html.ViewData[IdConstants.CollectionFormat] : defaultTo;
            html.ViewData[IdConstants.CollectionFormat] = displayType; // ensure default value is saved
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
            var nakedObject = html.Surface().GetObject(domainObject);
            string displayType = DefaultFormat(html, IdConstants.ListDisplayFormat);
            return displayType == IdConstants.TableDisplayFormat ?
                html.GetStandaloneCollection(nakedObject, x => includingColumns.Any(s => s == x.Id), x => Array.IndexOf(includingColumns, x.Id), true) :
                html.GetStandaloneList(nakedObject, null);
        }

        #endregion
    }
}
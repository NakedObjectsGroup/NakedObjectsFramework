// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Spec;
using NakedObjects.Core.Resolve;
using NakedObjects.Core.Util;
using NakedObjects.Resources;
using NakedObjects.Surface;
using NakedObjects.Surface.Utility;
using NakedObjects.Web.Mvc.Models;

namespace NakedObjects.Web.Mvc.Html {
    public static class ObjectExtensions {
        /// <summary>
        ///     Get the object id
        /// </summary>
        //public static MvcHtmlString GetObjectId(this HtmlHelper html, object model) {
        //    Assert.AssertFalse("Cannot get Adapter for Adapter", model is INakedObjectAdapter);
        //    INakedObjectAdapter nakedObject = html.Framework().GetNakedObject(model);
        //    return MvcHtmlString.Create(html.Framework().GetObjectId(nakedObject));
        //}

        public static MvcHtmlString GetObjectId(this HtmlHelper html, object model) {
            Assert.AssertFalse("Cannot get Adapter for Adapter", model is INakedObjectSurface);
            var nakedObject = html.Surface().GetObject(model);
            return MvcHtmlString.Create(html.Surface().OidStrategy.GetObjectId(nakedObject));
        }

        /// <summary>
        ///     Get the object type in a format suitable for use as a css id
        /// </summary>
        public static MvcHtmlString ObjectTypeAsCssId(this HtmlHelper html, object model) {
            if (model.GetType().IsGenericType) {
                var gType = new StringBuilder(FrameworkHelper.GetObjectType(model.GetType().GetGenericTypeDefinition()));

                gType.Append("[[");
                foreach (Type gTypeParm in model.GetType().GetGenericArguments()) {
                    gType.Append(FrameworkHelper.GetObjectType(gTypeParm)).Append(", ");
                }
                gType.Remove(gType.Length - 2, 2);
                gType.Append("]]");

                return MvcHtmlString.Create(gType.ToString());
            }

            return MvcHtmlString.Create(FrameworkHelper.GetObjectType(model));
        }

        private static string GetPresentationHint(this HtmlHelper html, object model) {
            // todo
            //INakedObjectAdapter nakedObject = html.Framework().GetNakedObject(model);
            //var facet = nakedObject.Spec.GetFacet<IPresentationHintFacet>();
            //return facet == null ? "" : " " + facet.Value;
            return "";
        }

        /// <summary>
        ///     Get classes for an object view
        /// </summary>
        public static MvcHtmlString ObjectViewClass(this HtmlHelper html, object model) {
            return MvcHtmlString.Create(IdConstants.ObjectViewName + html.GetPresentationHint(model));
        }

        /// <summary>
        ///     Get classes for an view model edit 
        /// </summary>
        public static MvcHtmlString ViewModelClass(this HtmlHelper html, object model) {
            return MvcHtmlString.Create(IdConstants.ViewModelName + html.GetPresentationHint(model));
        }

        /// <summary>
        ///     Get classes for an object edit
        /// </summary>
        public static MvcHtmlString ObjectEditClass(this HtmlHelper html, object model) {
            return MvcHtmlString.Create(IdConstants.ObjectEditName + html.GetPresentationHint(model));
        }

        /// <summary>
        ///     Indicate if object has any visible fields
        /// </summary>
        public static bool ObjectHasVisibleFields(this HtmlHelper html, object domainObject) {
            var nakedObject = html.Surface().GetObject(domainObject);
            var objectSpec = nakedObject.Specification;
            return objectSpec != null && objectSpec.Properties.Any(p => p.IsVisible(nakedObject));
        }

        /// <summary>
        ///     Indicate if object is a not persistent  object
        /// </summary>
        public static bool ObjectIsNotPersistent(this HtmlHelper html, object domainObject) {
            var nakedObject = html.Surface().GetObject(domainObject);
            return nakedObject.IsNotPersistent();
        }

        /// <summary>
        ///     Indicate if object is a transient object
        /// </summary>
        public static bool ObjectIsTransient(this HtmlHelper html, object domainObject) {
            INakedObjectAdapter nakedObject = html.Framework().GetNakedObject(domainObject);
            return nakedObject.ResolveState.IsTransient();
        }

        /// <summary>
        ///     Indicate if object is a transient object
        /// </summary>
        public static string TransientFlag(this HtmlHelper html, object domainObject) {
            INakedObjectAdapter nakedObject = html.Framework().GetNakedObject(domainObject);
            return nakedObject.ResolveState.IsTransient() ? " " + IdConstants.TransientName : "";
        }

        /// <summary>
        ///     Display name of object
        /// </summary>
        //public static MvcHtmlString ObjectTitle(this HtmlHelper html, object model) {
        //    INakedObjectAdapter nakedObject = html.Framework().NakedObjectManager.CreateAdapter(model, null, null);
        //    return html.ObjectTitle(nakedObject);
        //}

        public static MvcHtmlString ObjectTitle(this HtmlHelper html, object model) {
            var oid = html.Surface().OidStrategy.GetOid(model);
            var nakedObject = html.Surface().GetObject(oid).Target;
            return html.ObjectTitle(nakedObject);
        }

        public static MvcHtmlString ObjectTitle(this HtmlHelper html, INakedObjectSurface nakedObject) {
            string title = nakedObject.TitleString();
            return MvcHtmlString.Create(title);
        }

        public static MvcHtmlString ObjectTitle(this HtmlHelper html, INakedObjectAdapter nakedObject) {
            string title = nakedObject.TitleString();
            return MvcHtmlString.Create(string.IsNullOrWhiteSpace(title) ? nakedObject.Spec.UntitledName : title);
        }

        /// <summary>
        ///     Display name of object with icon
        /// </summary>
        public static MvcHtmlString Object(this HtmlHelper html, object model) {
            INakedObjectAdapter nakedObject = html.Framework().NakedObjectManager.CreateAdapter(model, null, null);
            string title = nakedObject.Spec.IsCollection ? GetCollectionTitle(nakedObject, html) : nakedObject.TitleString();
            title = string.IsNullOrWhiteSpace(title) ? nakedObject.Spec.UntitledName : title;
            return CommonHtmlHelper.WrapInDiv(html.ObjectIcon(nakedObject) + title, IdConstants.ObjectName);
        }

        public static MvcHtmlString ActionResult(this HtmlHelper html, ActionResultModel model) {
            INakedObjectAdapter nakedObject = html.Framework().NakedObjectManager.CreateAdapter(model.Result, null, null);
            string title = GetCollectionTitle(nakedObject, html);
            title = model.Action.Name() + ": " + (string.IsNullOrWhiteSpace(title) ? nakedObject.Spec.UntitledName : title);
            return CommonHtmlHelper.WrapInDiv(title, IdConstants.ObjectName);
        }

        private static string GetCollectionTitle(INakedObjectAdapter nakedObject, HtmlHelper html) {
            int pageSize, maxPage, currentPage, total;
            int count = nakedObject.GetAsEnumerable(html.Framework().NakedObjectManager).Count();
            if (!html.GetPagingValues(out pageSize, out maxPage, out currentPage, out total)) {
                total = count;
            }

            string queryInd = nakedObject.Spec.IsQueryable ? MvcUi.QueryResult + ": " : "";
            int viewSize = count;

            ITypeSpec typeSpec = html.Framework().MetamodelManager.GetSpecification(nakedObject.GetTypeOfFacetFromSpec().GetValueSpec(nakedObject, html.Framework().MetamodelManager.Metamodel));
            string type = total == 1 ? typeSpec.SingularName : typeSpec.PluralName;

            return queryInd + string.Format(MvcUi.ViewingNofXType, viewSize, total, type);
        }

        /// <summary>
        ///     Display link to object with icon
        /// </summary>
        public static MvcHtmlString Object(this HtmlHelper html, string linkText, string actionName, object model) {
            return CommonHtmlHelper.WrapInDiv(html.ObjectIconAndLink(linkText, actionName, model), IdConstants.ObjectName);
        }

        public static MvcHtmlString Tab(this HtmlHelper html, string linkText, string actionName, object model) {
            return CommonHtmlHelper.WrapInDiv(html.ObjectIconAndLink(linkText, actionName, model, true), "nof-tab");
        }

        #region name 

        /// <summary>
        ///     Display name (Title) of object
        /// </summary>
        public static MvcHtmlString Name<TModel>(this HtmlHelper<TModel> html) {
            if (html.ViewData.Model == null) {
                throw new ArgumentException("html");
            }
            return html.Name(html.ViewData.Model);
        }

        /// <summary>
        ///     Display name (Title) of object
        /// </summary>
        public static MvcHtmlString Name<TModel>(this HtmlHelper html, TModel model) {
            INakedObjectAdapter nakedObject = html.Framework().GetNakedObject(model);
            return MvcHtmlString.Create(nakedObject.TitleString());
        }

        #endregion

        #region description 

        /// <summary>
        ///     Display description of object
        /// </summary>
        public static MvcHtmlString Description<TModel>(this HtmlHelper<TModel> html) {
            if (html.ViewData.Model == null) {
                throw new ArgumentException("html");
            }
            return html.Description(html.ViewData.Model);
        }

        /// <summary>
        ///     Display description of object
        /// </summary>
        public static MvcHtmlString Description<TModel>(this HtmlHelper html, TModel model) {
            INakedObjectAdapter nakedObject = html.Framework().GetNakedObject(model);
            return MvcHtmlString.Create(nakedObject.Spec.Description);
        }

        #endregion

        #region iconname 

        /// <summary>
        ///     Get icon name from object
        /// </summary>
        public static MvcHtmlString IconName<TModel>(this HtmlHelper<TModel> html) {
            if (html.ViewData.Model == null) {
                throw new ArgumentException("html");
            }
            return html.IconName(html.ViewData.Model);
        }

        /// <summary>
        ///     Get icon name from object
        /// </summary>
        public static MvcHtmlString IconName<TModel>(this HtmlHelper html, TModel model) {
            INakedObjectAdapter nakedObject = html.Framework().GetNakedObject(model);
            return MvcHtmlString.Create(nakedObject.Spec.GetIconName(nakedObject));
        }

        #endregion

        #region typename 

        /// <summary>
        ///     Get short type name from object
        /// </summary>
        public static MvcHtmlString TypeName<TModel>(this HtmlHelper<TModel> html) {
            if (html.ViewData.Model == null) {
                throw new ArgumentException("html");
            }
            return html.TypeName(html.ViewData.Model);
        }

        /// <summary>
        ///     Get short type name from object
        /// </summary>
        //public static MvcHtmlString TypeName<TModel>(this HtmlHelper html, TModel model) {
        //    INakedObjectAdapter nakedObject = html.Framework().GetNakedObject(model);
        //    return MvcHtmlString.Create(nakedObject.Spec.ShortName);
        //}

        public static MvcHtmlString TypeName<TModel>(this HtmlHelper html, TModel model) {
            var nakedObject = html.Surface().GetObject(model);
            return MvcHtmlString.Create(nakedObject.Specification.FullName().Split('.').Last());
        }

        #endregion
    }
}
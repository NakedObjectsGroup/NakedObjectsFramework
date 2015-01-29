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
using NakedObjects.Architecture.Spec;
using NakedObjects.Core.Util;

namespace NakedObjects.Web.Mvc.Html {
    public static class PropertyExtensions {

      

        #region Properties

        /// <summary>
        /// Display the identified property on the ViewData Model
        /// </summary>
        public static MvcHtmlString ObjectPropertyView<TModel, TParm>(this HtmlHelper<TModel> html,
                                                                      Expression<Func<TModel, TParm>> expression) {
            return html.ObjectPropertyView(html.ViewData.Model, html.GetProperty(expression));
        }

        /// <summary>
        /// Display the identified property on the model parameter
        /// </summary>
        public static MvcHtmlString ObjectPropertyView<TModel, TParm>(this HtmlHelper html, TModel model,
                                                                      Expression<Func<TModel, TParm>> expression) {
            return html.ObjectPropertyView(model, html.GetProperty(expression));
        }

        /// <summary>
        /// Display the identified property on the ViewData Model
        /// </summary>
        public static MvcHtmlString ObjectPropertyView(this HtmlHelper html, string propertyId) {
            if (html.ViewData.Model == null) {
                throw new ArgumentException("html");
            }
            return html.ObjectPropertyView(html.ViewData.Model, propertyId);
        }

        /// <summary>
        /// Display the identified property on the model parameter
        /// </summary>
        public static MvcHtmlString ObjectPropertyView(this HtmlHelper html, object model, string propertyId) {
            INakedObject nakedObject = html.Framework().GetNakedObject(model);
            IAssociationSpec property = nakedObject.Spec.Properties.Where(a => a.Id == propertyId).SingleOrDefault(a => a.IsVisible( nakedObject));
            return property == null ? MvcHtmlString.Create("") : html.ObjectPropertyView(new PropertyContext(nakedObject, property, false));
        }

        /// <summary>
        /// Display the identified property on the ViewData Model in an edit field 
        /// </summary>
        public static MvcHtmlString ObjectPropertyEdit<TModel, TParm>(this HtmlHelper<TModel> html,
                                                                      Expression<Func<TModel, TParm>> expression) {
            return html.ObjectPropertyEdit(html.ViewData.Model, html.GetProperty(expression));
        }

        /// <summary>
        /// Display the identified property on the model parameter in an edit field 
        /// </summary>
        public static MvcHtmlString ObjectPropertyEdit<TModel, TParm>(this HtmlHelper html, TModel model,
                                                                      Expression<Func<TModel, TParm>> expression) {
            return html.ObjectPropertyEdit(model, html.GetProperty(expression));
        }

        /// <summary>
        /// Display the identified property on the ViewData Model in an edit field 
        /// </summary>
        public static MvcHtmlString ObjectPropertyEdit<TModel>(this HtmlHelper<TModel> html, string propertyId) {
            if (html.ViewData.Model == null) {
                throw new ArgumentException("html");
            }
            return html.ObjectPropertyEdit(html.ViewData.Model, propertyId);
        }

        /// <summary>
        /// Display the identified property on the model parameter in an edit field 
        /// </summary>
        public static MvcHtmlString ObjectPropertyEdit(this HtmlHelper html, object model, string propertyId) {
            INakedObject nakedObject = html.Framework().GetNakedObject(model);
            IAssociationSpec property = nakedObject.Spec.Properties.Where(a => a.Id == propertyId).SingleOrDefault(a => a.IsVisible( nakedObject));
            return property == null ? MvcHtmlString.Create("") : html.ObjectPropertyEdit(new PropertyContext(nakedObject, property, true));
        }

        #endregion

        #region PropertyList

        /// <summary>
        /// Enum to control collection display option formats 
        /// </summary>
        public enum CollectionFormat {
            Table,
            List
        } ;

        /// <summary>
        /// List all the properties of the domain object applying the appropriate format to the identified property. 
        /// </summary>
        /// <example>
        /// html.PropertyList(obj)
        /// </example>
        public static MvcHtmlString PropertyList(this HtmlHelper html, object domainObject) {
            return html.PropertyListWithFilter(domainObject, x => true, null);
        }

        /// <summary>
        /// List all the properties of the ViewData Model applying the appropriate format to the identified property. 
        /// </summary>
        /// <example>
        /// html.PropertyList(y => y.TestCollectionOne, CustomHtmlHelper.CollectionFormat.Table)
        /// </example>
        public static MvcHtmlString PropertyList<TModel>(this HtmlHelper<TModel> html, Expression<Func<TModel, IEnumerable>> expression, CollectionFormat format) {
            return html.PropertyList(html.GetProperty(expression).Name, format);
        }

        /// <summary>
        /// List all the properties of the model parameter applying the appropriate format to the identified property. 
        /// </summary>
        /// <example>
        ///  html.PropertyList(obj, y => y.TestCollectionOne, CustomHtmlHelper.CollectionFormat.Table)
        /// </example> 
        public static MvcHtmlString PropertyList<TModel>(this HtmlHelper html, TModel model, Expression<Func<TModel, IEnumerable>> expression, CollectionFormat format) {
            return html.PropertyList(model, html.GetProperty(expression).Name, format);
        }

        /// <summary>
        /// List all the properties of the ViewData Model applying the appropriate format to the identified property. 
        /// Property and matching format are passed in as tuples.
        /// </summary>
        /// <example>
        /// html.PropertyList(new Tuple&lt;Expression&lt;Func&lt;CustomHelperTestClass, IEnumerable&gt;&gt;, CustomHtmlHelper.CollectionFormat&gt;  (y => y.TestCollectionOne, CustomHtmlHelper.CollectionFormat.Table ))
        /// </example> 
        public static MvcHtmlString PropertyList<TModel>(this HtmlHelper<TModel> html, params Tuple<Expression<Func<TModel, IEnumerable>>, CollectionFormat>[] formats) {
            return html.PropertyList(formats.Select(t => new Tuple<string, CollectionFormat>(html.GetProperty(t.Item1).Name, t.Item2)).ToArray());
        }

        /// <summary>
        /// List all the properties of the model parameter applying the appropriate format to the identified property. 
        /// Property and matching format are passed in as tuples.
        /// </summary>
        /// <example>
        /// html.PropertyList(obj, new Tuple&lt;Expression&lt;Func&lt;CustomHelperTestClass, IEnumerable&gt;&gt;, CustomHtmlHelper.CollectionFormat&gt; (y => y.TestCollectionOne, CustomHtmlHelper.CollectionFormat.Table))
        /// </example>
        public static MvcHtmlString PropertyList<TModel>(this HtmlHelper html, TModel model, params Tuple<Expression<Func<TModel, IEnumerable>>, CollectionFormat>[] formats) {
            return html.PropertyList(model, formats.Select(t => new Tuple<string, CollectionFormat>(html.GetProperty(t.Item1).Name, t.Item2)).ToArray());
        }

        /// <summary>
        /// List all the properties of the ViewData Model applying the appropriate format to the identified property. 
        /// </summary>
        /// <example>
        /// html.PropertyList("TestCollectionOne", CustomHtmlHelper.CollectionFormat.Table)
        /// </example>
        public static MvcHtmlString PropertyList<TModel>(this HtmlHelper<TModel> html, string propertyId, CollectionFormat format) {
            if (html.ViewData.Model == null) {
                throw new ArgumentException("html");
            }
            return html.PropertyList(html.ViewData.Model, propertyId, format);
        }

        /// <summary>
        /// List all the properties of the domain object applying the appropriate format to the identified property. 
        /// </summary>
        /// <example>
        /// html.PropertyList(obj, "TestCollectionOne", CustomHtmlHelper.CollectionFormat.Table)
        /// </example>
        public static MvcHtmlString PropertyList(this HtmlHelper html, object domainObject, string propertyId, CollectionFormat format) {
            return html.PropertyList(domainObject, new Tuple<string, CollectionFormat>(propertyId, format));
        }

        /// <summary>
        /// List all the properties of the Viewdata Model applying the appropriate format to each property. 
        /// Property and matching format are passed in as tuples.
        /// </summary>
        /// <example>
        /// html.PropertyList(new Tuple&lt;string, CustomHtmlHelper.CollectionFormat&gt;("TestCollectionOne", CustomHtmlHelper.CollectionFormat.Table))
        /// </example>
        public static MvcHtmlString PropertyList<TModel>(this HtmlHelper<TModel> html, params Tuple<string, CollectionFormat>[] formats) {
            if (html.ViewData.Model == null) {
                throw new ArgumentException("html");
            }
            return html.PropertyList(html.ViewData.Model, formats);
        }

        /// <summary>
        /// List all the properties of the domain object applying the appropriate format to each property. 
        /// Property and matching format are passed in as tuples.
        /// </summary>
        /// <example>
        /// html.PropertyList(tc, new Tuple&lt;string, CustomHtmlHelper.CollectionFormat&gt;("TestCollectionOne", CustomHtmlHelper.CollectionFormat.Table)).ToString(),
        /// </example>
        public static MvcHtmlString PropertyList(this HtmlHelper html, object domainObject, params Tuple<string, CollectionFormat>[] formats) {
            formats.ForEach(t => html.ViewData[t.Item1] = (t.Item2 == CollectionFormat.Table ? IdHelper.TableDisplayFormat : IdHelper.ListDisplayFormat));
            return html.PropertyList(domainObject);
        }

        // exclusions

        /// <summary>
        /// List all the properties of the domain object excluding collections 
        /// </summary>
        /// <example>
        /// html.PropertyList(obj)
        /// </example>
        public static MvcHtmlString PropertyListWithoutCollections(this HtmlHelper html, object domainObject) {
           return html.PropertyListWithFilter(domainObject, x => x is IOneToOneAssociationSpec, null);
        }

        /// <summary>
        /// List all the properties of the domain object with only collections 
        /// </summary>
        /// <example>
        /// html.PropertyList(obj)
        /// </example>
        public static MvcHtmlString PropertyListOnlyCollections(this HtmlHelper html, object domainObject) {
            return html.PropertyListWithFilter(domainObject, x => x is IOneToManyAssociationSpec, null);
        }

        /// <summary>
        /// List all the properties of the domain object with only collections 
        /// </summary>
        /// <example>
        /// html.PropertyList(obj)
        /// </example>
        public static MvcHtmlString PropertyListOnlyCollections(this HtmlHelper html, object domainObject, CollectionFormat format) {
            INakedObject nakedObject = html.Framework().GetNakedObject(domainObject);
            IEnumerable<string> collections = nakedObject.Spec.Properties.OfType<IOneToManyAssociationSpec>().Select(p => p.Id);
            collections.ForEach(t => html.ViewData[t] = format);
            return html.PropertyListWithFilter(domainObject, x => x is IOneToManyAssociationSpec, null);
        }


        /// <summary>
        /// List all the properties of the domain object with only collections 
        /// </summary>
        /// <example>
        /// html.PropertyList(obj)
        /// </example>
        public static MvcHtmlString[] PropertiesListOnlyCollections(this HtmlHelper html, object domainObject) {
            INakedObject nakedObject = html.Framework().GetNakedObject(domainObject);
            IEnumerable<string> collections = nakedObject.Spec.Properties.OfType<IOneToManyAssociationSpec>().Where(p => p.IsVisible(nakedObject)).Select(p => p.Id);

            return collections.Select(c => html.PropertyListWith(domainObject, new[] {c})).ToArray();
        }



        /// <summary>
        /// List the properties of the ViewData Model except those identified by the excludingProperties parameters
        /// </summary>
        /// <example>
        /// html.PropertyListWithout(y => y.TestCollectionOne, y => y.TestInt)
        /// </example>
        public static MvcHtmlString PropertyListWithout<TModel>(this HtmlHelper<TModel> html, params Expression<Func<TModel, object>>[] excludingProperties) {
            return html.PropertyListWithout(excludingProperties.Select(ex => html.GetProperty(ex).Name).ToArray());
        }

        /// <summary>
        /// List the properties of the model parameter except those identified by the excludingProperties parameters
        /// </summary>
        /// <example>
        /// html.PropertyListWithout(obj, y => y.TestCollectionOne, y => y.TestInt)
        /// </example>
        public static MvcHtmlString PropertyListWithout<TModel>(this HtmlHelper html, TModel model, params Expression<Func<TModel, object>>[] excludingProperties) {
            return html.PropertyListWithout(model, excludingProperties.Select(ex => html.GetProperty(ex).Name).ToArray());
        }

        /// <summary>
        /// List the properties of the ViewData Model except those identified by the excludingProperties parameters
        /// </summary>
        /// <example>
        /// html.PropertyListWithout("TestCollectionOne", "TestInt")
        /// </example>
        public static MvcHtmlString PropertyListWithout<TModel>(this HtmlHelper<TModel> html, params string[] excludingProperties) {
            if (html.ViewData.Model == null) {
                throw new ArgumentException("html");
            }
            return html.PropertyListWithout(html.ViewData.Model, excludingProperties);
        }

        /// <summary>
        /// List the properties of the domainobject parameter except those identified by the excludingProperties parameters
        /// </summary>
        /// <example>
        /// html.PropertyListWithout(obj, "TestCollectionOne", "TestInt")
        /// </example>
        public static MvcHtmlString PropertyListWithout(this HtmlHelper html, object domainObject, params string[] excludingProperties) {
            return html.PropertyListWithFilter(domainObject, x => !excludingProperties.Any(s => s == x.Id), null);
        }

        /// <summary>
        /// List the properties of the ViewData Model identified by the includingProperties parameter 
        /// </summary>
        /// <example>
        /// html.PropertyListWith(y => y.TestCollectionOne, y => y.TestInt)
        /// </example>
        public static MvcHtmlString PropertyListWith<TModel>(this HtmlHelper<TModel> html, params Expression<Func<TModel, object>>[] includingProperties) {
            return html.PropertyListWith(includingProperties.Select(ex => html.GetProperty(ex).Name).ToArray());
        }

        /// <summary>
        /// List the properties of the model parameter identified by the includingProperties parameter 
        /// </summary>
        /// <example>
        /// html.PropertyListWith(obj, y => y.TestCollectionOne, y => y.TestInt)
        /// </example>
        public static MvcHtmlString PropertyListWith<TModel>(this HtmlHelper html, TModel model, params Expression<Func<TModel, object>>[] includingProperties) {
            return html.PropertyListWith(model, includingProperties.Select(ex => html.GetProperty(ex).Name).ToArray());
        }

        /// <summary>
        /// List the properties of the ViewData Model identified by the includingProperties parameter 
        /// </summary>
        /// <example>
        /// html.PropertyListWith("TestCollectionOne", "TestInt")
        /// </example>
        public static MvcHtmlString PropertyListWith<TModel>(this HtmlHelper<TModel> html, params string[] includingProperties) {
            if (html.ViewData.Model == null) {
                throw new ArgumentException("html");
            }
            return html.PropertyListWith(html.ViewData.Model, includingProperties);
        }

        /// <summary>
        /// List the properties of the domainobject parameter identified by the includingProperties parameter 
        /// </summary>
        /// <example>
        /// html.PropertyListWith(obj, "TestCollectionOne", "TestInt")
        /// </example>
        public static MvcHtmlString PropertyListWith(this HtmlHelper html, object domainObject, params string[] includingProperties) {
            return html.PropertyListWithFilter(domainObject, x => includingProperties.Any(s => s == x.Id), x => Array.IndexOf(includingProperties, x.Id));
        }

        #endregion

        #region PropertyListEdit

        /// <summary>
        /// Display all the properties of the domain object in edit fields  
        /// </summary>
        public static MvcHtmlString PropertyListEdit(this HtmlHelper html, object domainObject) {
            INakedObject nakedObject = html.Framework().GetNakedObject(domainObject);
            return html.BuildEditContainer(nakedObject, html.EditObjectFields(nakedObject, null, x => true, null),
                                           IdHelper.FieldContainerName,
                                           IdHelper.GetFieldContainerId(nakedObject));
        }

        /// <summary>
        /// include all the properties of the domain object as hidden fields  
        /// </summary>
        public static MvcHtmlString PropertyListEditHidden(this HtmlHelper html, object domainObject) {
            INakedObject nakedObject = html.Framework().GetNakedObject(domainObject);
            var fields = html.EditObjectFields(nakedObject, null, x => false, null);
            return MvcHtmlString.Create(ElementDescriptor.BuildElementSet(fields).ToString());
        }



        /// <summary>
        ///  Display all the properties of the domain object in edit fields, with action dialog or collection view
        /// </summary>
        /// <param name="html">Html helper</param>
        /// <param name="contextObject">domain object to be displayed</param>
        /// <param name="targetObject">owning object of targetAction</param>
        /// <param name="targetAction">action to display as dialog - may be null</param>
        /// <param name="propertyName">property to be decorated with action dialog or selection view</param>
        /// <param name="actionResult">collection of objects to display in selection view - may be null</param>
        /// <returns></returns>
        public static MvcHtmlString PropertyListEdit(this HtmlHelper html, object contextObject, object targetObject, IActionSpec targetAction, string propertyName, IEnumerable actionResult) {
            INakedObject nakedObject = html.Framework().GetNakedObject(contextObject);
            INakedObject target = html.Framework().GetNakedObject(targetObject);
            return html.BuildEditContainer(nakedObject,
                                           html.EditObjectFields(contextObject, new ActionContext(true, target, targetAction), propertyName, actionResult, true),
                                           IdHelper.FieldContainerName,
                                           IdHelper.GetFieldContainerId(nakedObject));
        }

        /// <summary>
        ///  Display identified property of the domain object in edit fields, with action dialog or collection view
        /// </summary>
        /// <param name="html">Html helper</param>
        /// <param name="contextObject">domain object to be displayed</param>
        /// <param name="targetObject">owning object of targetAction</param>
        /// <param name="targetAction">action to display as dialog - may be null</param>
        /// <param name="propertyName">property to be decorated with action dialog or selection view</param>
        /// <param name="actionResult">collection of objects to display in selection view - may be null</param>
        /// <returns></returns>
        public static MvcHtmlString PropertyListEditWith(this HtmlHelper html, object contextObject, object targetObject, IActionSpec targetAction, string propertyName, IEnumerable actionResult) {
            INakedObject nakedObject = html.Framework().GetNakedObject(contextObject);
            INakedObject target = html.Framework().GetNakedObject(targetObject);
            return html.BuildEditContainer(nakedObject,
                                           html.EditObjectFields(contextObject, new ActionContext(true, target, targetAction), propertyName, actionResult, false),
                                           IdHelper.FieldContainerName,
                                           IdHelper.GetFieldContainerId(nakedObject));
        }


        // formats

        /// <summary>
        /// List in a form all the properties of the ViewData Model applying the appropriate format to the identified property. 
        /// </summary>
        /// <example>
        /// html.PropertyListEdit(y => y.TestCollectionOne, CustomHtmlHelper.CollectionFormat.Table)
        /// </example>
        public static MvcHtmlString PropertyListEdit<TModel>(this HtmlHelper<TModel> html, Expression<Func<TModel, IEnumerable>> expression, CollectionFormat format) {
            return html.PropertyListEdit(html.GetProperty(expression).Name, format);
        }

        /// <summary>
        /// List in a form all the properties of the model parameter applying the appropriate format to the identified property. 
        /// </summary>
        /// <example>
        ///  html.PropertyListEdit(obj, y => y.TestCollectionOne, CustomHtmlHelper.CollectionFormat.Table)
        /// </example> 
        public static MvcHtmlString PropertyListEdit<TModel>(this HtmlHelper html, TModel model, Expression<Func<TModel, IEnumerable>> expression, CollectionFormat format) {
            return html.PropertyListEdit(model, html.GetProperty(expression).Name, format);
        }

        /// <summary>
        /// List in a form all the properties of the ViewData Model applying the appropriate format to the identified property. 
        /// Property and matching format are passed in as tuples.
        /// </summary>
        /// <example>
        /// html.PropertyListEdit(new Tuple&lt;Expression&lt;Func&lt;CustomHelperTestClass, IEnumerable&gt;&gt;, CustomHtmlHelper.CollectionFormat&gt;  (y => y.TestCollectionOne, CustomHtmlHelper.CollectionFormat.Table ))
        /// </example> 
        public static MvcHtmlString PropertyListEdit<TModel>(this HtmlHelper<TModel> html, params Tuple<Expression<Func<TModel, IEnumerable>>, CollectionFormat>[] formats) {
            return html.PropertyListEdit(formats.Select(t => new Tuple<string, CollectionFormat>(html.GetProperty(t.Item1).Name, t.Item2)).ToArray());
        }

        /// <summary>
        /// List in a form all the properties of the model parameter applying the appropriate format to the identified property. 
        /// Property and matching format are passed in as tuples.
        /// </summary>
        /// <example>
        /// html.PropertyListEdit(obj, new Tuple&lt;Expression&lt;Func&lt;CustomHelperTestClass, IEnumerable&gt;&gt;, CustomHtmlHelper.CollectionFormat&gt; (y => y.TestCollectionOne, CustomHtmlHelper.CollectionFormat.Table))
        /// </example>
        public static MvcHtmlString PropertyListEdit<TModel>(this HtmlHelper html, TModel model, params Tuple<Expression<Func<TModel, IEnumerable>>, CollectionFormat>[] formats) {
            return html.PropertyListEdit(model, formats.Select(t => new Tuple<string, CollectionFormat>(html.GetProperty(t.Item1).Name, t.Item2)).ToArray());
        }

        /// <summary>
        /// List in a form all the properties of the ViewData Model applying the appropriate format to the identified property. 
        /// </summary>
        /// <example>
        /// html.PropertyListEdit("TestCollectionOne", CustomHtmlHelper.CollectionFormat.Table)
        /// </example>
        public static MvcHtmlString PropertyListEdit<TModel>(this HtmlHelper<TModel> html, string propertyId, CollectionFormat format) {
            if (html.ViewData.Model == null) {
                throw new ArgumentException("html");
            }
            return html.PropertyListEdit(html.ViewData.Model, propertyId, format);
        }

        /// <summary>
        /// List in a form all the properties of the domain object applying the appropriate format to the identified property. 
        /// </summary>
        /// <example>
        /// html.PropertyListEdit(obj, "TestCollectionOne", CustomHtmlHelper.CollectionFormat.Table)
        /// </example>
        public static MvcHtmlString PropertyListEdit(this HtmlHelper html, object domainObject, string propertyId, CollectionFormat format) {
            return html.PropertyListEdit(domainObject, new Tuple<string, CollectionFormat>(propertyId, format));
        }

        /// <summary>
        /// List in a form all the properties of the Viewdata Model applying the appropriate format to each property. 
        /// Property and matching format are passed in as tuples.
        /// </summary>
        /// <example>
        /// html.PropertyListEdit(new Tuple&lt;string, CustomHtmlHelper.CollectionFormat&gt;("TestCollectionOne", CustomHtmlHelper.CollectionFormat.Table))
        /// </example>
        public static MvcHtmlString PropertyListEdit<TModel>(this HtmlHelper<TModel> html, params Tuple<string, CollectionFormat>[] formats) {
            if (html.ViewData.Model == null) {
                throw new ArgumentException("html");
            }
            return html.PropertyListEdit(html.ViewData.Model, formats);
        }

        /// <summary>
        /// List in a form all the properties of the domain object applying the appropriate format to each property. 
        /// Property and matching format are passed in as tuples.
        /// </summary>
        /// <example>
        /// html.PropertyListEdit(tc, new Tuple&lt;string, CustomHtmlHelper.CollectionFormat&gt;("TestCollectionOne", CustomHtmlHelper.CollectionFormat.Table)).ToString(),
        /// </example>
        public static MvcHtmlString PropertyListEdit(this HtmlHelper html, object domainObject, params Tuple<string, CollectionFormat>[] formats) {
            formats.ForEach(t => html.ViewData[t.Item1] = (t.Item2 == CollectionFormat.Table ? IdHelper.TableDisplayFormat : IdHelper.ListDisplayFormat));
            return html.PropertyListEdit(domainObject);
        }

        // exclusions

        /// <summary>
        /// List in a form the properties of the ViewData Model except those identified by the excludingProperties parameters
        /// </summary>
        /// <example>
        /// html.PropertyListEditWithout(y => y.TestCollectionOne, y => y.TestInt)
        /// </example>
        public static MvcHtmlString PropertyListEditWithout<TModel>(this HtmlHelper<TModel> html, params Expression<Func<TModel, object>>[] excludingProperties) {
            return html.PropertyListEditWithout(excludingProperties.Select(ex => html.GetProperty(ex).Name).ToArray());
        }

        /// <summary>
        /// List in a form the properties of themodel parameter except those identified by the excludingProperties parameters
        /// </summary>
        /// <example>
        /// html.PropertyListEditWithout(obj, y => y.TestCollectionOne, y => y.TestInt)
        /// </example>
        public static MvcHtmlString PropertyListEditWithout<TModel>(this HtmlHelper html, TModel model, params Expression<Func<TModel, object>>[] excludingProperties) {
            return html.PropertyListEditWithout(model, excludingProperties.Select(ex => html.GetProperty(ex).Name).ToArray());
        }

        /// <summary>
        /// List in a form the properties of the ViewData Model except those identified by the excludingProperties parameters
        /// </summary>
        /// <example>
        /// html.PropertyListEditWithout("TestCollectionOne", "TestInt")
        /// </example>
        public static MvcHtmlString PropertyListEditWithout<TModel>(this HtmlHelper<TModel> html, params string[] excludingProperties) {
            if (html.ViewData.Model == null) {
                throw new ArgumentException("html");
            }
            return html.PropertyListEditWithout(html.ViewData.Model, excludingProperties);
        }

        /// <summary>
        /// List in a form the properties of the domainobject parameter except those identified by the excludingProperties parameters
        /// </summary>
        /// <example>
        /// html.PropertyListEditWithout(obj, "TestCollectionOne", "TestInt")
        /// </example>
        public static MvcHtmlString PropertyListEditWithout(this HtmlHelper html, object domainObject, params string[] excludingProperties) {
            return html.PropertyListEditWithFilter(domainObject, x => !excludingProperties.Any(s => s == x.Id), null);
        }

        // inclusions

        /// <summary>
        /// List in a form the properties of the ViewData Model identified by the includingProperties parameter 
        /// </summary>
        /// <example>
        /// html.PropertyListEditWith(y => y.TestCollectionOne, y => y.TestInt)
        /// </example>
        public static MvcHtmlString PropertyListEditWith<TModel>(this HtmlHelper<TModel> html, params Expression<Func<TModel, object>>[] includingProperties) {
            return html.PropertyListEditWith(includingProperties.Select(ex => html.GetProperty(ex).Name).ToArray());
        }

        /// <summary>
        /// List in a form the properties of the model parameter  identified by the includingProperties parameter 
        /// </summary>
        /// <example>
        /// html.PropertyListEditWith(y => y.TestCollectionOne, y => y.TestInt)
        /// </example>
        public static MvcHtmlString PropertyListEditWith<TModel>(this HtmlHelper html, TModel model, params Expression<Func<TModel, object>>[] includingProperties) {
            return html.PropertyListEditWith(model, includingProperties.Select(ex => html.GetProperty(ex).Name).ToArray());
        }

        /// <summary>
        /// List in a form the properties of the ViewData Model identified by the includingProperties parameter 
        /// </summary>
        /// <example>
        /// html.PropertyListEditWith(obj, "TestCollectionOne", "TestInt")
        /// </example>
        public static MvcHtmlString PropertyListEditWith<TModel>(this HtmlHelper<TModel> html, params string[] includingProperties) {
            if (html.ViewData.Model == null) {
                throw new ArgumentException("html");
            }
            return html.PropertyListEditWith(html.ViewData.Model, includingProperties);
        }

        /// <summary>
        /// List in a form the properties of the domainobject parameter identified by the includingProperties parameter 
        /// </summary>
        /// <example>
        /// html.PropertyListEditWith("TestCollectionOne", "TestInt")
        /// </example>
        public static MvcHtmlString PropertyListEditWith(this HtmlHelper html, object domainObject, params string[] includingProperties) {
            return html.PropertyListEditWithFilter(domainObject, x => includingProperties.Any(s => s == x.Id), x => Array.IndexOf(includingProperties, x.Id));
        }

        #endregion

        #region contents

        /// <summary>
        /// Display the contents of the identified property on ViewData Model 
        /// </summary>
        public static MvcHtmlString Contents<TModel, TParm>(this HtmlHelper<TModel> html, Expression<Func<TModel, TParm>> expression) {
            if (html.ViewData.Model == null) {
                throw new ArgumentException("html");
            }
            return html.Contents(html.ViewData.Model, expression);
        }

        /// <summary>
        /// Display the contents of the identified property on model parameter 
        /// </summary>
        public static MvcHtmlString Contents<TModel, TParm>(this HtmlHelper html, TModel model, Expression<Func<TModel, TParm>> expression) {
            return html.Contents(model, html.GetProperty(expression).Name);
        }

        /// <summary>
        /// Display the contents of the identified property on ViewData Model 
        /// </summary>
        public static MvcHtmlString Contents<TModel>(this HtmlHelper<TModel> html, string propertyId) {
            if (html.ViewData.Model == null) {
                throw new ArgumentException("html");
            }
            return html.Contents(html.ViewData.Model, propertyId);
        }

        /// <summary>
        /// Display the contents of the identified property on model parameter
        /// </summary>
        public static MvcHtmlString Contents<TModel>(this HtmlHelper html, TModel model, string propertyId) {
            INakedObject nakedObject = html.Framework().GetNakedObject(model);
            return MvcHtmlString.Create(nakedObject.Spec.Properties.Single(p => p.Id == propertyId).GetNakedObject(nakedObject).TitleString());
        }

        #endregion

        #region description

        /// <summary>
        /// Get the description of the identified property 
        /// </summary>
        public static MvcHtmlString Description<TModel, TParm>(this HtmlHelper<TModel> html, Expression<Func<TModel, TParm>> expression) {
            if (html.ViewData.Model == null) {
                throw new ArgumentException("html");
            }
            return html.Description(html.ViewData.Model, expression);
        }

        /// <summary>
        /// Get the description of the identified property 
        /// </summary>
        public static MvcHtmlString Description<TModel, TParm>(this HtmlHelper html, TModel model, Expression<Func<TModel, TParm>> expression) {
            return html.Description(model, html.GetProperty(expression).Name);
        }

        /// <summary>
        /// Get the description of the identified property 
        /// </summary>
        public static MvcHtmlString Description<TModel>(this HtmlHelper<TModel> html, string propertyId) {
            if (html.ViewData.Model == null) {
                throw new ArgumentException("html");
            }
            return html.Description(html.ViewData.Model, propertyId);
        }

        /// <summary>
        /// Get the description of the identified property 
        /// </summary>
        public static MvcHtmlString Description<TModel>(this HtmlHelper html, TModel model, string propertyId) {
            INakedObject nakedObject = html.Framework().GetNakedObject(model);

            return MvcHtmlString.Create(nakedObject.Spec.Properties.Where(p => p.Id == propertyId).Single().Description);
        }

        #endregion

        #region name 

        /// <summary>
        /// Get the name of the identified property 
        /// </summary>
        public static MvcHtmlString Name<TModel, TParm>(this HtmlHelper<TModel> html, Expression<Func<TModel, TParm>> expression) {
            if (html.ViewData.Model == null) {
                throw new ArgumentException("html");
            }
            return html.Name(html.ViewData.Model, expression);
        }

        /// <summary>
        /// Get the name of the identified property 
        /// </summary>
        public static MvcHtmlString Name<TModel, TParm>(this HtmlHelper html, TModel model, Expression<Func<TModel, TParm>> expression) {
            return html.Name(model, html.GetProperty(expression).Name);
        }

        /// <summary>
        /// Get the name of the identified property 
        /// </summary>
        public static MvcHtmlString Name<TModel>(this HtmlHelper<TModel> html, string propertyId) {
            if (html.ViewData.Model == null) {
                throw new ArgumentException("html");
            }
            return html.Name(html.ViewData.Model, propertyId);
        }

        /// <summary>
        /// Get the name of the identified property 
        /// </summary>
        public static MvcHtmlString Name<TModel>(this HtmlHelper html, TModel model, string propertyId) {
            INakedObject nakedObject = html.Framework().GetNakedObject(model);

            return MvcHtmlString.Create(nakedObject.Spec.Properties.Single(p => p.Id == propertyId).Name);
        }

        #endregion

        #region typename 

        /// <summary>
        /// Get the type name of the identified property 
        /// </summary>
        public static MvcHtmlString TypeName<TModel, TParm>(this HtmlHelper<TModel> html, Expression<Func<TModel, TParm>> expression) {
            if (html.ViewData.Model == null) {
                throw new ArgumentException("html");
            }
            return html.TypeName(html.ViewData.Model, expression);
        }

        /// <summary>
        /// Get the type name of the identified property 
        /// </summary>
        public static MvcHtmlString TypeName<TModel, TParm>(this HtmlHelper html, TModel model, Expression<Func<TModel, TParm>> expression) {
            return html.TypeName(model, html.GetProperty(expression).Name);
        }

        /// <summary>
        /// Get the type name of the identified property 
        /// </summary>
        public static MvcHtmlString TypeName<TModel>(this HtmlHelper<TModel> html, string propertyId) {
            if (html.ViewData.Model == null) {
                throw new ArgumentException("html");
            }
            return html.TypeName(html.ViewData.Model, propertyId);
        }

        /// <summary>
        /// Get the type name of the identified property 
        /// </summary>
        public static MvcHtmlString TypeName<TModel>(this HtmlHelper html, TModel model, string propertyId) {
            INakedObject nakedObject = html.Framework().GetNakedObject(model);

            return MvcHtmlString.Create(nakedObject.Spec.Properties.Where(p => p.Id == propertyId).Single().ReturnSpec.ShortName);
        }

        #endregion

        #region scalar

        public static MvcHtmlString ScalarView(this HtmlHelper html, object scalar) {
            return html.Scalar(scalar);
        }

        #endregion 
    }
}
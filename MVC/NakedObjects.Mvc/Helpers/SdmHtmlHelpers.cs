using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using NakedObjects;
using NakedObjects.Web.Mvc.Html;
using NakedObjects.Facade.Utility;
using NakedObjects.Facade;

namespace Sdm.App.Helpers
{
    public static class SdmHtmlHelpers
    {
        public static IFrameworkFacade Facade(this HtmlHelper html) {
            return (IFrameworkFacade)html.ViewData[IdConstants.NoFacade];
        }

        public static MvcHtmlString ModelWithoutCollection<TModel>(this HtmlHelper helper, TModel model) {
            string htmlString = string.Empty;
            var objectFacade = helper.Facade().GetObject(model);

            if (objectFacade.Specification.Properties.Any(p => !p.IsCollection)) {
                htmlString = string.Format("<div class='propertyDisplay'>{0}</div>", RemoveButtonText(helper.PropertyListWithoutCollections(model)));
            }

            return new MvcHtmlString(htmlString);
        }

        public static MvcHtmlString ModelCollections<TModel>(this HtmlHelper helper, TModel model)
        {
            var htmlString = new StringBuilder();

            var collections = helper.Collections(model, IdConstants.TableDisplayFormat);
            var titles = helper.CollectionTitles(model, "{0} for {1}");

            foreach (var ct in collections.Zip(titles, (c, t) => new {collString = c, titleString = t}))
            {
                htmlString.Append(
                    "<div class='collectionDisplay' style='float:left;padding-top:5px;'><fieldset><legend>");
                htmlString.Append(ct.titleString);
                htmlString.Append("</legend>");
                htmlString.Append(ct.collString);
                htmlString.Append("</fieldset></div>");
            }

            return new MvcHtmlString(htmlString.ToString());
        }

        public static MvcHtmlString ModelCollectionsAsProperties<TModel>(this HtmlHelper helper, TModel model)
        {
            var htmlString = new StringBuilder();

            htmlString.Append("<div class='collectionsAsProperties'>");

            MvcHtmlString[] properties = helper.PropertiesListOnlyCollections(model);

            var objectFacade = helper.Facade().GetObject(model);

            string[] titles = objectFacade.Specification.Properties.Where(p => !p.IsCollection).Select(p => p.Name).ToArray();

            //*************************************************************************
            // Remove any title from titles collection if the corresponding properties
            // do not exist due to the [Hidden] tag.
            //
            // WITEM #55476 - Collections are being displayed under the wrong headings
            //*************************************************************************
            var titlesList = titles.ToList();
            foreach (
                var title in
                    titles.Where(
                        title =>
                            !properties.Any(t => t.ToString().Contains("<div class=\"nof-label\">" + title + ":</div>")))
                )
            {
                titlesList.Remove(title);
            }
            titles = titlesList.ToArray();

            //*************************************************************************


            var zip = properties.Zip(titles, (c, t) => new {propString = c, titleString = t});

            //for each collection property create a table for the collection
            foreach (var pt in zip.ToArray())
            {
                htmlString.Append("<div class='collectionDisplay'><fieldset><legend>");
                htmlString.Append(pt.titleString);
                htmlString.Append("</legend>");

                htmlString.Append(RemoveButtonText(pt.propString));
                htmlString.Append("</fieldset></div>");
            }

            htmlString.Append("</div>");

            return new MvcHtmlString(htmlString.ToString());
        }

        /// <summary>
        ///     Removes the button text for Table, Summary and List.
        /// </summary>
        /// <param name="origional">The original string.</param>
        /// <returns>A HTML string where the button text has been removed</returns>
        private static MvcHtmlString RemoveButtonText(MvcHtmlString original)
        {
            string newstr = original.ToString();
            newstr = newstr.Replace(">List<", "><");
            newstr = newstr.Replace(">Table<", "><");
            newstr = newstr.Replace(">Summary<", "><");
            newstr = newstr.Replace(">Expand<", "><");
            newstr = newstr.Replace(">Collapse<", "><");
            return new MvcHtmlString(newstr);
        }

        public static MvcHtmlString Service(this HtmlHelper html, object service, params CustomMenuItem[] menuItems)
        {
            return ServiceExtensions.Service(html, service, menuItems);
        }

        public static MvcHtmlString MainMenusWithHome(this HtmlHelper html) {
            var tag = new TagBuilder("div");
            tag.AddCssClass(IdConstants.ServicesContainerName);

            /***
             * Add Home button to service div
            ***/
            tag.InnerHtml += html.HomeMenuItem();
            var menus = html.Facade().GetMainMenus();
            html.AddMainMenusIntoTag(menus, tag);
            return MvcHtmlString.Create(tag.ToString());
        }



        public static MvcHtmlString HomeMenuItem(this HtmlHelper html)
        {
            var tag = new TagBuilder("div");
            tag.AddCssClass(IdConstants.MenuContainerName);
            tag.GenerateId("HomeService");

            var tagName = new TagBuilder("div");
            tagName.AddCssClass(IdConstants.MenuNameLabel);
            tagName.SetInnerText("");

            tagName.InnerHtml += html.ActionLink("Home", "Index", "Home");

            tag.InnerHtml += MvcHtmlString.Create(tagName.ToString());

            return MvcHtmlString.Create(tag.ToString());
        }

        public static MvcHtmlString ServicesForIndex(this HtmlHelper html)
        {
            var servicesTag = new TagBuilder("div");
            servicesTag.AddCssClass("ServicesIndex");

            var servicesHeader = new TagBuilder("div");
            servicesHeader.AddCssClass("ServicesHeader");
            servicesHeader.GenerateId("services-header");
            servicesHeader.InnerHtml += "Services";

            servicesTag.InnerHtml += MvcHtmlString.Create(servicesHeader.ToString());

            var services = (IEnumerable)html.ViewData[IdConstants.NofServices];

            foreach (object service in services)
            {
                servicesTag.InnerHtml += html.Service(service);
            }

            return MvcHtmlString.Create(servicesTag.ToString());
        }

        public static MvcHtmlString SearchByPPSN(this HtmlHelper html)
        {
            //// Create Button
            var buttom = new TagBuilder("button");
            buttom.AddCssClass("btn btn-default btn-lg search");
            buttom.Attributes.Add("type", "button");
            //buttom.Attributes.Add("onClick", "submit");
            buttom.Attributes.Add("id", "ppsn-btn");

            //// Create Button Group
            var buttomGrp = new TagBuilder("span");
            buttomGrp.AddCssClass("input-group-btn ");

            //// Add Button to button group
            buttomGrp.InnerHtml = buttom.ToString();

            //// Create Text input box
            var input = new TagBuilder("input");
            input.AddCssClass("form-control home-search");
            input.Attributes.Add("type", "text");
            input.Attributes.Add("placeholder", "Search by PPSN");
            input.Attributes.Add("name", "Ppsn-Input");
            input.Attributes.Add("id", "Ppsn-Input");

            //// Create Wrapping DIV
            var innerDiv = new TagBuilder("div");
            innerDiv.AddCssClass("input-group");

            //// Add Button group and input to wrapper div
            innerDiv.InnerHtml = input.ToString(TagRenderMode.Normal) + buttomGrp.ToString(TagRenderMode.Normal);

            //// Create Form
            var form = new TagBuilder("form");
            form.AddCssClass("search-box nof-action");
            form.Attributes.Add("method", "post");
            form.Attributes.Add("enctype", "multipart/form-data");
            form.Attributes.Add("action",
                "/Sdm.App/Bomi2CustomerCompleteMenuServices/Action/FindByPpsn?id=Sdm.App.Menus.Bomi2CustomerCompleteMenuServices%3B1%3BSystem.Int32%3B0%3BFalse%3B%3B0");

            //// Add Wrapper div to form
            form.InnerHtml = innerDiv.ToString();

            //// Create head label
            var header = new TagBuilder("h3");
            header.InnerHtml = "Find Customer By PPSN";

            //// Create Wrapper Div
            var wrapper = new TagBuilder("div");
            wrapper.AddCssClass("call-to-action nof-objectedit");

            //// Wrap everything
            wrapper.InnerHtml = header.ToString(TagRenderMode.Normal) + form.ToString(TagRenderMode.Normal);

            //// return widget
            return MvcHtmlString.Create(wrapper.ToString());
        }


        public static MvcHtmlString GetAllMessages(this Exception ex)
        {
            if (ex == null)
                throw new ArgumentNullException("ex");

            StringBuilder sb = new StringBuilder();

            while (ex != null)
            {
                if (!string.IsNullOrEmpty(ex.Message))
                {
                    if (sb.Length > 0)
                        sb.Append(" ");

                    sb.Append("<p><h4>" + ex.Message + "</h4></p>");
                    sb.Append("<p>" + ex.StackTrace + "</p>");
                }

                ex = ex.InnerException;
            }

            return MvcHtmlString.Create(sb.ToString());
        }
    }
}
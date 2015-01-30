// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Xml.Linq;
using NakedObjects.Architecture.Resolve;
using NakedObjects.Architecture.Spec;
using NakedObjects.Core.Resolve;
using NakedObjects.Resources;
using NakedObjects.Web.Mvc.Models;

namespace NakedObjects.Web.Mvc.Html {
    public static class SystemExtensions {

        private const int DefaultHistorySize = 10;

        public static INakedObjectsFramework GetFramework(this HtmlHelper html) {
            return html.Framework();
        }

        #region system menus

        /// <summary>
        /// Display Naked Objects Framework messages and warnings from ViewData
        /// </summary>
        public static MvcHtmlString UserMessages(this HtmlHelper html) {
            string[] messages = (string[]) html.ViewData[IdHelper.NofMessages] ?? new string[0];
            string[] warnings = (string[]) html.ViewData[IdHelper.NofWarnings] ?? new string[0];
            return MvcHtmlString.Create(CommonHtmlHelper.UserMessages(messages, IdHelper.NofMessages) +
                                        CommonHtmlHelper.UserMessages(warnings, IdHelper.NofWarnings));
        }

        /// <summary>
        /// Display Naked Objects Framework messages and warnings from ViewData
        /// </summary>
        public static MvcHtmlString[] InitialisationWarnings(this HtmlHelper html) {
            // TODO display any warnings flagged during initialisation 
            return new MvcHtmlString[] {};
        }

        /// <summary>
        /// Display Naked Objects Framework messages and warnings from ViewData
        /// </summary>
        public static MvcHtmlString SystemMessages(this HtmlHelper html) {
            string[] messages = (string[])html.ViewData[IdHelper.SystemMessages] ?? new string[0];

            return MvcHtmlString.Create(CommonHtmlHelper.UserMessages(messages, IdHelper.NofMessages));
        }


        public static MvcHtmlString History(this HtmlHelper html, object domainObject = null, bool clearAll = false) {
            return html.History(DefaultHistorySize, domainObject, clearAll);
        }

        public static MvcHtmlString History(this HtmlHelper html, int count, object domainObject = null, bool clearAll = false) {
            if (domainObject != null && !(domainObject is FindViewModel)) {
                string url = html.Object(html.ObjectTitle(domainObject).ToString(), IdHelper.ViewAction, domainObject).ToString();
                html.ViewContext.HttpContext.Session.AddToCache(html.Framework(), domainObject, url, ObjectCache.ObjectFlag.BreadCrumb);
            }

            List<string> urls = html.ViewContext.HttpContext.Session.AllCachedUrls(ObjectCache.ObjectFlag.BreadCrumb).ToList();
            int sizeCache = urls.Count();
            int skip = sizeCache > count ? sizeCache - count : 0;

            urls = urls.Skip(skip).ToList();

            var tag = new TagBuilder("div");
            tag.AddCssClass(IdHelper.HistoryContainerName);

            foreach (string url in urls) {
                tag.InnerHtml += url;
            }

            if (urls.Any()) {
                tag.InnerHtml += html.ControllerAction(MvcUi.Clear, IdHelper.ClearHistoryAction, IdHelper.HomeName, IdHelper.ClearButtonClass, "", new RouteValueDictionary(new { clearAll }));
            }

            return MvcHtmlString.Create(tag.ToString());
        }

        public static MvcHtmlString TabbedHistory(this HtmlHelper html, object domainObject = null) {
            if (html.ViewData.ContainsKey("updateHistory") && !(bool)html.ViewData["updateHistory"]) {
                return new MvcHtmlString("");
            }
            return html.TabbedHistory(DefaultHistorySize, domainObject);
        }

        internal class UrlData {
            public UrlData(string url) {
                Url = url;

                if (!string.IsNullOrWhiteSpace(Url)) {
                    Document = XDocument.Parse(Url);
                    DivElement = Document.Element("div");
                    string href = DivElement.Element("a").Attribute("href").Value;
                    string id = string.IsNullOrWhiteSpace(href) ? "" : HttpUtility.UrlDecode(href.Substring(href.IndexOf('=') + 1));
            
                    if (id != null && id.Contains("&")) {
                        Id = id.Substring(0, id.IndexOf("&"));
                        var parms = id.Substring(id.IndexOf("&")).Split('&');
                        var parmDict = parms.Where(s => s.Contains("=")).Select(p => p.Split('=')).ToDictionary(arr => arr[0], arr => arr[1]);

                        Page = parmDict.ContainsKey(IdHelper.PageKey) ? parmDict[IdHelper.PageKey] : "";
                        PageSize = parmDict.ContainsKey(IdHelper.PageSizeKey) ? parmDict[IdHelper.PageSizeKey] : "";
                        Format = parmDict.ContainsKey(IdHelper.FormatKey) ? parmDict[IdHelper.FormatKey] : "";
                    }
                    else {
                        Id = id;
                    }
                    Type = string.IsNullOrWhiteSpace(Id) ? "" : Id.Substring(0, Id.IndexOf(";"));
                }
            }

            public static UrlData Empty() {
                return new UrlData("");
            }

            public XDocument Document { get; set; }
            private XElement DivElement { get; set; }
            public string Id { get; set; }
            public string Type { get; set; }
            public string Url { get; set; }
            public string Page { get; set; }
            public string PageSize { get; set; }
            public string Format { get; set; }

            public void SetActive() {
                string existingValue = DivElement.Attribute("class").Value;
                DivElement.Attribute("class").SetValue(existingValue + " " + IdHelper.ActiveClass);
            }

            public void AddElement(XElement element) {
                DivElement.Add(element);
            }

            public override string ToString() {
                return Document.ToString();
            }

            public override bool Equals(object obj) {
                var otherObj = obj as UrlData;

                if (otherObj != null) {
                    return Id == otherObj.Id;
                }

                return false;
            }

            private static RouteValueDictionary AddPageData(UrlData entry, RouteValueDictionary rvd) {
                rvd.Add(IdHelper.PageKey, entry.Page);
                rvd.Add(IdHelper.PageSizeKey, entry.PageSize);
                rvd.Add(IdHelper.FormatKey, entry.Format);

                return rvd;
            }

            public void AddCloseThis(HtmlHelper html, UrlData nextEntry) {
                var closeThis = html.ControllerAction("", IdHelper.ClearHistoryItemAction, IdHelper.HomeName, IdHelper.ClearItemButtonClass, "", AddPageData(nextEntry, new RouteValueDictionary(new { id = Id, nextId = nextEntry.Id }))).ToString();
                var closeThisElem = XDocument.Parse(closeThis).Element("form");
                AddElement(closeThisElem);
            }

            public void AddCloseOthers(HtmlHelper html) {
                var closeOthers = html.ControllerAction("", IdHelper.ClearHistoryOthersAction, IdHelper.HomeName, IdHelper.ClearOthersButtonClass, "", AddPageData(this, new RouteValueDictionary(new { id = Id }))).ToString();
                AddElement(ToElement(closeOthers));
            }

            private static XElement ToElement(string stringElement) {
                var element = XDocument.Parse(stringElement).Element("form");
                element.Element("div").Element("button").SetAttributeValue("style", "display: none;");
                return element;
            }

            public void AddCloseAll(HtmlHelper html) {
                const bool clearAll = true;
                var closeAll = html.ControllerAction("", IdHelper.ClearHistoryAction, IdHelper.HomeName, IdHelper.ClearButtonClass, "", new RouteValueDictionary(new { clearAll })).ToString();
                AddElement(ToElement(closeAll));
            }

            public override int GetHashCode() {
                return Id.GetHashCode();
            }

            public void AddCancel(HtmlHelper html, UrlData nextEntry) {
                var cancel = html.ControllerAction(MvcUi.Cancel, IdHelper.CancelAction, IdHelper.HomeName, IdHelper.CancelButtonClass, MvcUi.Cancel, AddPageData(nextEntry, new RouteValueDictionary(new { nextId = nextEntry.Id }))).ToString();
                Document = XDocument.Parse(cancel).Element("form").Document;
            }
        }

        public static MvcHtmlString TabbedHistory(this HtmlHelper html, int count, object domainObject = null) {
            List<string> existingUrls = html.ViewContext.HttpContext.Session.AllCachedUrls(ObjectCache.ObjectFlag.BreadCrumb).ToList();
            string newUrl = "";

            if (domainObject != null) {
                newUrl = html.Tab(html.ObjectTitle(domainObject).ToString(), IdHelper.ViewAction, domainObject).ToString();
                if (!(domainObject is FindViewModel) && !existingUrls.Contains(newUrl)) {
                    html.ViewContext.HttpContext.Session.AddOrUpdateInCache(html.Framework(), domainObject, newUrl, ObjectCache.ObjectFlag.BreadCrumb);
                }
            }

            List<string> urls = html.ViewContext.HttpContext.Session.AllCachedUrls(ObjectCache.ObjectFlag.BreadCrumb).ToList();

            int sizeCache = urls.Count();
            int skip = sizeCache > count ? sizeCache - count : 0;

            urls = urls.Skip(skip).ToList();

            var tag = new TagBuilder("div");
            tag.AddCssClass(IdHelper.TabbedHistoryContainerName);

            UrlData[] entries = urls.Select(u => new UrlData(u)).ToArray();
            var newEntry = new UrlData(newUrl);

            foreach (UrlData currentEntry in entries) {
                
                if (currentEntry.Equals(newEntry)) {
                    currentEntry.SetActive();
                }

                UrlData nextEntry = GetNextEntry(entries, currentEntry);
                currentEntry.AddCloseThis(html, nextEntry);

                if (urls.Count > 1) {
                    currentEntry.AddCloseOthers(html);
                }

                currentEntry.AddCloseAll(html);
                tag.InnerHtml += currentEntry.ToString();
            }
            return MvcHtmlString.Create(tag.ToString());
        }

        private static UrlData GetLastEntry(UrlData[] entries) {
            return entries.Any() ? entries.Last() : UrlData.Empty();
        }


        private static UrlData GetNextEntry(UrlData[] entries, UrlData thisEntry) {
            if (entries.Count() == 1) {
                return UrlData.Empty();
            }
            if (entries.Last().Equals(thisEntry)) {
                // last in list so return one before 
                return entries[entries.Length - 2];
            }

            var previousEntry = UrlData.Empty();

            foreach (var entry in entries.Reverse()) {
                if (entry.Equals(thisEntry)) {
                    return previousEntry;
                }
                previousEntry = entry;
            }
            // not found - should never happen 
            return entries.Last();
        }

        /// <summary>
        /// Return to view the last object in the history. If the history is empty return to the home page.  
        /// </summary>
        public static MvcHtmlString CancelButton(this HtmlHelper html, object domainObject) {
            var fvm = domainObject as FindViewModel;

            if (fvm != null) {
                // if dialog return to target - unless it's a service 
                object target = fvm.ContextObject;
                domainObject = html.Framework().NakedObjectManager.CreateAdapter(target, null, null).Spec is IObjectSpec ? target : null;
            }

            // if target is transient  cancel back to history
            if (domainObject != null && html.Framework().NakedObjectManager.CreateAdapter(domainObject, null, null).ResolveState.IsTransient()) {
                domainObject = null;
            }    

            string nextUrl = domainObject == null ? "" : html.Tab(html.ObjectTitle(domainObject).ToString(), IdHelper.ViewAction, domainObject).ToString();

            UrlData nextEntry;
            
            if (string.IsNullOrEmpty(nextUrl)) {
                List<string> existingUrls = html.ViewContext.HttpContext.Session.AllCachedUrls(ObjectCache.ObjectFlag.BreadCrumb).ToList();
                var existingEntries = existingUrls.Select(u => new UrlData(u)).ToArray();
                nextEntry = GetLastEntry(existingEntries);
            }
            else {
                nextEntry = new UrlData(nextUrl);
            }

            var cancelForm = new UrlData("");
            cancelForm.AddCancel(html, nextEntry);
     
            return MvcHtmlString.Create(cancelForm.ToString());
        }

        #endregion
    }
}
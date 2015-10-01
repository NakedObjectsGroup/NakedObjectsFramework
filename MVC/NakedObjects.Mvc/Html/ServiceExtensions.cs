// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Web.Mvc;
using NakedObjects.Architecture.Adapter;
using System;

namespace NakedObjects.Web.Mvc.Html {
    public static class ServiceExtensions {
        #region ServiceMenus

        [Obsolete("Use MainMenus (defined on MenuExtensions)")]
        public static MvcHtmlString Services(this HtmlHelper html) {
            return MenuExtensions.MainMenus(html);
        }

        [Obsolete("CustomMenuItems no longer supported")]
        public static MvcHtmlString Service(this HtmlHelper html, object service, params CustomMenuItem[] menuItems) {
            INakedObjectAdapter nakedObject = html.Framework().GetNakedObject(service);
            return CommonHtmlHelper.BuildMenuContainer(html.ObjectActions(nakedObject, false, menuItems),
                IdHelper.MenuContainerName,
                IdHelper.GetServiceContainerId(nakedObject),
                nakedObject.TitleString());
        }

         [Obsolete("Use MainMenus (defined on MenuExtensions)")]
        public static MvcHtmlString ServiceMenu(this HtmlHelper html, object service) {
            return html.Service(service);
        }

        #endregion
    }
}
// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Core.Util;
using NakedObjects.Surface;
using NakedObjects.Surface.Utility;
using NakedObjects.Web.Mvc.Html;

namespace NakedObjects.Web.Mvc.Models {
    public class ObjectAndControlData {
        private INakedObjectSurface nakedObjectSurface;
        private INakedObjectActionSurface nakedObjectAction;

        public enum SubActionType {
            Find,
            Select,
            Remove,
            Redisplay,
            ActionAsFind,
            InvokeActionAsFind,
            InvokeActionAsSave,
            Action,
            Details,
            Pager,
            Cancel,
            None,
            SaveAndClose
        };

        public SubActionType SubAction {
            get {
                var subActions = new List<string> {Finder, Selector, Redisplay, ActionAsFinder, InvokeAction, InvokeActionAsFinder, Details, Pager, Cancel, SaveAndClose};

                Assert.AssertFalse(subActions.Count(s => !string.IsNullOrEmpty(s)) > 1);

                if (!string.IsNullOrEmpty(Finder)) {
                    return SubActionType.Find;
                }
                if (!string.IsNullOrEmpty(Selector)) {
                    return SubActionType.Select;
                }
                if (!string.IsNullOrEmpty(Redisplay)) {
                    return SubActionType.Redisplay;
                }
                if (!string.IsNullOrEmpty(ActionAsFinder)) {
                    return SubActionType.ActionAsFind;
                }
                if (!string.IsNullOrEmpty(InvokeAction)) {
                    return SubActionType.Action;
                }
                if (!string.IsNullOrEmpty(InvokeActionAsFinder)) {
                    return SubActionType.InvokeActionAsFind;
                }
                if (!string.IsNullOrEmpty(InvokeActionAsSave)) {
                    return SubActionType.InvokeActionAsSave;
                }
                if (!string.IsNullOrEmpty(Details)) {
                    return SubActionType.Details;
                }
                if (!string.IsNullOrEmpty(Pager)) {
                    return SubActionType.Pager;
                }
                if (!string.IsNullOrEmpty(Cancel)) {
                    return SubActionType.Cancel;
                }
                if (!string.IsNullOrEmpty(SaveAndClose)) {
                    return SubActionType.SaveAndClose;
                }
                return SubActionType.None;
            }
        }

        public string Id { get; set; }
        public string ActionId { get; set; }

        public string Finder { get; set; }
        public string Selector { get; set; }
        public string Redisplay { get; set; }
        public string ActionAsFinder { get; set; }
        public string InvokeAction { get; set; }
        public string InvokeActionAsFinder { get; set; }
        public string InvokeActionAsSave { get; set; }
        public string Details { get; set; }
        public string Pager { get; set; }
        public string Cancel { get; set; }
        public string None { get; set; }
        public string SaveAndClose { get; set; }

        public string PageSize { get; set; }
        public string Page { get; set; }
        public string Format { get; set; }

        public IDictionary<string, HttpPostedFileBase> files = new Dictionary<string, HttpPostedFileBase>();

        public IDictionary<string, HttpPostedFileBase> Files {
            get { return files; }
        }

        public FormCollection Form { get; set; }

        private IDictionary<string, string> dataDict;

        public IDictionary<string, string> DataDict {
            get {
                if (dataDict == null) {
                    string data = Finder ?? Selector ?? ActionAsFinder ?? Redisplay ?? InvokeActionAsFinder ?? InvokeActionAsSave ?? InvokeAction ?? Details ?? Pager ?? None ?? "";
                    dataDict = data.Split('&').ToDictionary(GetName, GetValue);
                }
                return dataDict;
            }
        }

        public INakedObjectSurface GetNakedObject(INakedObjectsSurface surface) {

            if (nakedObjectSurface == null) {

                var link = surface.OidStrategy.GetOid(Id, "");

                // hack
                try {
                    nakedObjectSurface = surface.GetObject(link).Target;
                }
                catch {
                    nakedObjectSurface = surface.GetService(link).Target;
                }

                if (nakedObjectSurface == null) {
                    throw new ObjectNotFoundException();
                }
            }

            return nakedObjectSurface;
        }

        public INakedObjectActionSurface GetAction(INakedObjectsSurface surface) {
            if (nakedObjectAction == null) {

                // todo collection contributed actions ? 

                var no = GetNakedObject(surface);
                var id = surface.OidStrategy.GetOid(no);
                var action = surface.GetObjectAction(id, ActionId).Action;

                nakedObjectAction = action.IsUsable(no).IsAllowed ? action : null;
            }
            return nakedObjectAction;
        }

        //public IActionSpec GetAction(INakedObjectsFramework framework) {
        //    if (nakedObjectAction == null) {
        //        GetNakedObject(framework);
        //        IActionSpec[] actions;
        //        if (nakedObject.Spec.IsCollection) {
        //            var metamodel = framework.MetamodelManager.Metamodel;
        //            IObjectSpecImmutable elementSpecImmut = nakedObject.Spec.GetFacet<ITypeOfFacet>().GetValueSpec(nakedObject, metamodel);
        //            var elementSpec = framework.MetamodelManager.GetSpecification(elementSpecImmut) as IObjectSpec;
        //            Trace.Assert(elementSpec != null);
        //            actions = elementSpec.GetCollectionContributedActions();
        //        }
        //        else {
        //            actions = nakedObject.Spec.GetActions();
        //        }
        //        nakedObjectAction = actions.Where(a => a.IsUsable(nakedObject).IsAllowed).Where(a => a.IsVisible(nakedObject)).SingleOrDefault(a => a.Id == ActionId);
        //    }
        //    return nakedObjectAction;
        //}

        private static string GetName(string nameValue) {
            if (string.IsNullOrEmpty(nameValue)) {
                return string.Empty;
            }
            return nameValue.Remove(nameValue.IndexOf('='));
        }

        private static string GetValue(string nameValue) {
            if (string.IsNullOrEmpty(nameValue)) {
                return string.Empty;
            }
            int indexOfValue = nameValue.IndexOf('=') + 1;
            return indexOfValue == nameValue.Length ? string.Empty : nameValue.Substring(indexOfValue);
        }
    }
}
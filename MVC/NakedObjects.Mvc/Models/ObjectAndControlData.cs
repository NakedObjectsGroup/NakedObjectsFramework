// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Core.Util;
using NakedObjects.Web.Mvc.Html;

namespace NakedObjects.Web.Mvc.Models {
    public class ObjectAndControlData {

        private INakedObject nakedObject;
        private INakedObjectAction nakedObjectAction;

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
        } ;
        
        public SubActionType SubAction {
            get {
                var subActions = new List<string> { Finder, Selector, Redisplay, ActionAsFinder, InvokeAction, InvokeActionAsFinder, Details, Pager, Cancel };

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
                if (!string.IsNullOrEmpty(InvokeActionAsFinder))
                {
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

        public string PageSize { get; set; }
        public string Page { get; set; }
        public string Format { get; set; }

        public  IDictionary<string, HttpPostedFileBase> files = new Dictionary<string, HttpPostedFileBase>();

        public  IDictionary<string, HttpPostedFileBase> Files {
            get { return files; }
        }

        public FormCollection Form { get; set; }

        private IDictionary<string, string> dataDict;

        public IDictionary<string, string> DataDict {
            get {
                if (dataDict == null) {
                    string data = Finder ?? Selector ?? ActionAsFinder ??  Redisplay ?? InvokeActionAsFinder ?? InvokeActionAsSave ?? InvokeAction ?? Details ?? Pager ?? None ?? "";
                    dataDict = data.Split('&').ToDictionary(GetName, GetValue);
                }
                return dataDict;
            }
           
        }

        public INakedObject GetNakedObject( INakedObjectsFramework framework) {
            if (nakedObject == null) {
                nakedObject = framework.GetNakedObjectFromId(Id);
            }

            if (nakedObject == null) {
                throw new ObjectNotFoundException();
            }

            return nakedObject;
        }

        public INakedObjectAction GetAction( INakedObjectsFramework framework) {
            if (nakedObjectAction == null) {
                nakedObjectAction = framework.GetActions(GetNakedObject(framework)).SingleOrDefault(a => a.Id == ActionId);
            }
            return nakedObjectAction;
        }

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
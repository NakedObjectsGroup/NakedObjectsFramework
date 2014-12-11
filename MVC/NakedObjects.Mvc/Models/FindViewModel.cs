// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System.Collections;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Web.Mvc.Html;

namespace NakedObjects.Web.Mvc.Models {
    public class FindViewModel {
        public enum ViewTypes {
            Edit,
            Dialog
        } ;

        public ViewTypes ViewType {
            get { return ContextAction == null ? ViewTypes.Edit : ViewTypes.Dialog; }
        }

        public IEnumerable ActionResult { get; set; }
        public object TargetObject { get; set; }
        public IActionSpec TargetAction { get; set; }

        public object ContextObject { get; set; }
        public IActionSpec ContextAction { get; set; }

        public string PropertyName { get; set; }

        public string DialogClass( INakedObjectsFramework framework) {

            if (ViewType == ViewTypes.Dialog) {
                return ContextAction.ReturnSpec.IsFile(framework) ? IdHelper.DialogNameFileClass : IdHelper.DialogNameClass;
            }

            return IdHelper.EditName;
        }
    }
}
// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections;
using NakedObjects.Surface;
using NakedObjects.Surface.Utility;

namespace NakedObjects.Web.Mvc.Models {
    public class FindViewModel {
        #region ViewTypes enum

        public enum ViewTypes {
            Edit,
            Dialog
        };

        #endregion

        public ViewTypes ViewType {
            get { return ContextAction == null ? ViewTypes.Edit : ViewTypes.Dialog; }
        }

        public IEnumerable ActionResult { get; set; }
        public object TargetObject { get; set; }
        public INakedObjectActionSurface TargetAction { get; set; }
        public object ContextObject { get; set; }
        public INakedObjectActionSurface ContextAction { get; set; }
        public string PropertyName { get; set; }

        public string DialogClass(INakedObjectsSurface framework) {
            if (ViewType == ViewTypes.Dialog) {
                return ContextAction.ReturnType.IsFile ? IdConstants.DialogNameFileClass : IdConstants.DialogNameClass;
            }
            return IdConstants.EditName;
        }
    }
}
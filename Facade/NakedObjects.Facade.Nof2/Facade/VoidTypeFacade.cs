// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;

namespace NakedObjects.Facade.Nof2 {
    public class VoidTypeFacade : ITypeFacade {
        public ITypeFacade ElementType {
            get { return null; }
        }

        #region ITypeFacade Members

        public bool IsComplexType { get; private set; }

        public bool IsParseable {
            get { return false; }
        }

        public bool IsStream { get; private set; }

        public bool IsQueryable {
            get { return false; }
        }

        public bool IsService {
            get { return false; }
        }

        public bool IsVoid {
            get { return true; }
        }

        public bool IsDateTime {
            get { return false; }
        }

        public string FullName {
            get { return typeof (void).FullName; }
        }

        public string ShortName {
            get { return FullName.Split('.').Last(); }
        }

        public string UntitledName { get; private set; }

        public bool IsCollection {
            get { return false; }
        }

        public bool IsObject {
            get { return false; }
        }

        public string SingularName {
            get { return typeof (void).Name; }
        }

        public string PluralName {
            get { return typeof (void).Name; }
        }

        public string Description {
            get { return ""; }
        }

        public bool IsASet { get; private set; }
        public bool IsAggregated { get; private set; }
        public bool IsImage { get; private set; }
        public bool IsFileAttachment { get; private set; }
        public bool IsFile { get; private set; }
        public IDictionary<string, object> ExtensionData { get; private set; }
        public bool IsBoolean { get; private set; }
        public bool IsEnum { get; private set; }

        public IAssociationFacade[] Properties {
            get { return new IAssociationFacade[] {}; }
        }

        public IMenuFacade Menu { get; private set; }
        public string PresentationHint { get; private set; }
        public bool IsAlwaysImmutable { get; private set; }
        public bool IsImmutableOncePersisted { get; private set; }

        public ITypeFacade GetElementType(IObjectFacade nakedObject) {
            throw new NotImplementedException();
        }

        bool ITypeFacade.IsImmutable(IObjectFacade nakedObject) {
            return false;
        }

        public string GetIconName(IObjectFacade nakedObject) {
            return null;
        }

        public IActionFacade[] GetActionLeafNodes() {
            return new IActionFacade[] {};
        }

        public bool IsOfType(ITypeFacade otherSpec) {
            return false;
        }

        public Type GetUnderlyingType() {
            return typeof (void);
        }

        public IActionFacade[] GetCollectionContributedActions() {
            throw new NotImplementedException();
        }

        public IActionFacade[] GetFinderActions() {
            throw new NotImplementedException();
        }

        public bool Equals(ITypeFacade other) {
            throw new NotImplementedException();
        }

        public IFrameworkFacade FrameworkFacade { get; set; }

        #endregion

        public override bool Equals(object obj) {
            var nakedObjectSpecificationWrapper = obj as VoidTypeFacade;
            if (nakedObjectSpecificationWrapper != null) {
                return true;
            }
            return false;
        }

        public override int GetHashCode() {
            return (GetType().GetHashCode());
        }
    }
}
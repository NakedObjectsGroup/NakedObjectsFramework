// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using NakedObjects.Surface.Context;
using org.nakedobjects.@object;
using org.nakedobjects.@object.control;

namespace NakedObjects.Surface.Nof2.Wrapper {
    public class NakedObjectAssociationWrapper : INakedObjectAssociationSurface {
        private readonly NakedObjectField assoc;
        private readonly Naked target;

        public NakedObjectAssociationWrapper(NakedObjectField assoc, Naked target, INakedObjectsSurface surface) {
            this.assoc = assoc;
            this.target = target;
            Surface = surface;
        }

        public bool IsChoicesEnabled {
            get {
                return false;
                //return ((OneToOneFeature) assoc).IsChoicesEnabled;
            }
        }

        #region INakedObjectAssociationSurface Members

        public int MemberOrder {
            get { return 0; }
        }

        public bool IsInline {
            get { return false; }
        }

        public bool IsCollection {
            get { return assoc.isCollection(); }
        }

        public bool IsObject {
            get { return assoc.isObject(); }
        }

        public bool IsASet {
            get { return false; }
        }

        public string Mask {
            get { return ""; }
        }

        public int AutoCompleteMinLength {
            get { return 0; }
        }

        public bool IsConcurrency {
            get { return false; }
        }

        public IDictionary<string, object> ExtensionData {
            get { return null; }
        }

        public Tuple<bool, string[]> TableViewData {
            get { return null; }
        }

        public string Name {
            get { return assoc.getName(); }
        }

        public string Description {
            get { return assoc.getDescription(); }
        }

        public bool IsMandatory {
            get {
                //return assoc.isMandatory(); doesn't seem to be returning much useful - default to true for the moment
                return true;
            }
        }

        public int TypicalLength {
            get { return 0; }
        }

        public int? MaxLength {
            get { return null; }
        }

        public string PresentationHint {
            get { return ""; }
        }

        public string Pattern {
            get { return null; }
        }

        public INakedObjectSpecificationSurface Specification {
            get { return new NakedObjectSpecificationWrapper(assoc.getSpecification(), target, Surface); }
        }

        public INakedObjectSpecificationSurface ElementSpecification {
            get { return new NakedObjectSpecificationWrapper(org.nakedobjects.@object.NakedObjects.getSpecificationLoader().loadSpecification(typeof (object).FullName), null, Surface); }
        }

        Choices INakedObjectAssociationSurface.IsChoicesEnabled {
            get { return IsChoicesEnabled ? Choices.Single : Choices.NotEnabled; }
        }

        public bool IsAutoCompleteEnabled {
            get { return false; }
        }

        public bool IsFile { get; private set; }
        public bool IsEnum { get; private set; }
        public Tuple<Regex, string> RegEx { get; private set; }
        public Tuple<IConvertible, IConvertible, bool> Range { get; private set; }
        public bool IsFindMenuEnabled { get; private set; }
        public bool IsAjax { get; private set; }
        public bool IsNullable { get; private set; }
        public bool IsPassword { get; private set; }
        public bool DoNotCount { get; private set; }
        public bool RenderEagerly { get; private set; }
        public int NumberOfLines { get; private set; }
        public int Width { get; private set; }

        public string Id {
            get {
                //var id = assoc.getName();
                //return id.Substring(0, 1).ToUpper() + id.Substring(1);
                return assoc.getName().Replace(" ", "");
            }
        }

        public IConsentSurface IsUsable(INakedObjectSurface target) {
            Consent consent = assoc.isAvailable((NakedReference) ((NakedObjectWrapper) target).NakedObject);
            return new ConsentWrapper(consent);
        }

        public INakedObjectSurface GetNakedObject(INakedObjectSurface target) {
            Naked result = assoc.get((NakedObject) ((NakedObjectWrapper) target).NakedObject);
            return result == null ? null : new NakedObjectWrapper(result, Surface);
        }

        public bool IsVisible(INakedObjectSurface nakedObject) {
            return !assoc.isHidden() && assoc.isVisible((NakedReference) ((NakedObjectWrapper) nakedObject).NakedObject).isAllowed();
        }

        public bool IsEager(INakedObjectSurface nakedObject) {
            return false;
        }

        public INakedObjectSurface[] GetChoices(INakedObjectSurface target, IDictionary<string, object> parameterNameValues) {
            return new INakedObjectSurface[] {};
        }

        public Tuple<string, INakedObjectSpecificationSurface>[] GetChoicesParameters() {
            return new Tuple<string, INakedObjectSpecificationSurface>[] {};
        }

        public Tuple<INakedObjectSurface, string>[] GetChoicesAndTitles(INakedObjectSurface target, IDictionary<string, object> parameterNameValues) {
            return new Tuple<INakedObjectSurface, string>[] {};
        }

        public INakedObjectSurface[] GetCompletions(INakedObjectSurface target, string autoCompleteParm) {
            return new INakedObjectSurface[] {};
        }

        public int Count(INakedObjectSurface nakedObject) {
            if (IsCollection) {
                var result = (NakedCollection) assoc.get((NakedObject) ((NakedObjectWrapper) nakedObject).NakedObject);
                return result.size();
            }
            return 0;
        }

        public string GetMaskedValue(INakedObjectSurface valueNakedObject) {
            throw new NotImplementedException();
        }

        public bool DefaultTypeIsExplicit(INakedObjectSurface nakedObject) {
            throw new NotImplementedException();
        }

        public string GetTitle(INakedObjectSurface nakedObject) {
            return nakedObject.TitleString;
        }

        public INakedObjectsSurface Surface { get; set; }

        #endregion

        public INakedObjectSurface[] GetChoices(INakedObjectSurface target, IDictionary<string, INakedObjectSurface> parameterNameValues) {
            // return ((OneToOneFeature) assoc).GetChoices(((NakedObject2) target).NakedObject, null).Select(no => new NakedObject2(no)).ToArray();
            return null;
        }

        public override bool Equals(object obj) {
            var nakedObjectAssociationWrapper = obj as NakedObjectAssociationWrapper;
            if (nakedObjectAssociationWrapper != null) {
                return Equals(nakedObjectAssociationWrapper);
            }
            return false;
        }

        public bool Equals(NakedObjectAssociationWrapper other) {
            if (ReferenceEquals(null, other)) { return false; }
            if (ReferenceEquals(this, other)) { return true; }
            return Equals(other.assoc, assoc);
        }

        public override int GetHashCode() {
            return (assoc != null ? assoc.GetHashCode() : 0);
        }
    }
}
// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Core.Util;
using NakedObjects.Core.Util.Query;
using NakedObjects.Surface.Context;
using NakedObjects.Surface.Nof4.Utility;
using NakedObjects.Surface.Utility;

namespace NakedObjects.Surface.Nof4.Wrapper {
    public class NakedObjectAssociationWrapper : ScalarPropertyHolder, INakedObjectAssociationSurface {
        private readonly IAssociationSpec assoc;
        private readonly INakedObjectsFramework framework;


        public NakedObjectAssociationWrapper(IAssociationSpec assoc, INakedObjectsSurface surface, INakedObjectsFramework framework) {
            SurfaceUtils.AssertNotNull(assoc, "Assoc is null");
            SurfaceUtils.AssertNotNull(framework, "framework is null");
            SurfaceUtils.AssertNotNull(surface, "surface is null");

            this.assoc = assoc;
            this.framework = framework;
            Surface = surface;
        }


        public IAssociationSpec WrappedSpec { get { return assoc; } }

        protected IDictionary<string, object> ExtensionData {
            get {
                var extData = new Dictionary<string, object>();

                if (assoc.ContainsFacet<IPresentationHintFacet>()) {
                    extData[PresentationHint] = assoc.GetFacet<IPresentationHintFacet>().Value;
                }

                return extData.Any() ? extData : null;
            }
        }


        public string Name {
            get { return assoc.Name; }
        }

        public string Description {
            get { return assoc.Description; }
        }

        public bool IsMandatory {
            get { return assoc.IsMandatory; }
        }

        public bool IsCollection {
            get { return assoc is IOneToManyAssociationSpec; }
        }

        public bool IsObject {
            get { return assoc is IOneToOneAssociationSpec; }
        }

        public bool IsConcurrency {
            get { return assoc.ContainsFacet<IConcurrencyCheckFacet>(); }
        }

        public int? MaxLength {
            get {
                var facet = assoc.GetFacet<IMaxLengthFacet>();
                return facet != null ? (int?) facet.Value : null;
            }
        }

        public string Pattern {
            get {
                var facet = assoc.GetFacet<IRegExFacet>();
                return facet != null ? facet.Pattern.ToString() : null;
            }
        }

        protected int MemberOrder {
            get {
                var facet = assoc.GetFacet<IMemberOrderFacet>();

                int result;
                if (facet != null && Int32.TryParse(facet.Sequence, out result)) {
                    return result;
                }

                return 0;
            }
        }

        protected bool IsASet {
            get {
                var collection = assoc as IOneToManyAssociationSpec;
                return collection != null && collection.IsASet;
            }
        }

        protected bool IsInline {
            get { return assoc.IsInline; }
        }

        public string Mask {
            get {
                var facet = assoc.GetFacet<IMaskFacet>();
                return facet != null ? facet.Value : null;
            }
        }

        public int AutoCompleteMinLength {
            get {
                var facet = assoc.GetFacet<IAutoCompleteFacet>();
                return facet != null ? facet.MinLength : 0;
            }
        }

        #region INakedObjectAssociationSurface Members

        public INakedObjectSpecificationSurface Specification {
            get { return new NakedObjectSpecificationWrapper(assoc.ReturnSpec, Surface, framework); }
        }

        public INakedObjectSpecificationSurface ElementSpecification {
            get {
                var coll = assoc as IOneToManyAssociationSpec;
                var elementSpec = coll == null ? null : coll.ElementSpec;
                return elementSpec == null ? null : new NakedObjectSpecificationWrapper(elementSpec, Surface, framework);
            }
        }

        public string Id {
            get { return assoc.Id; }
        }

        public Choices IsChoicesEnabled {
            get { return ((IOneToOneFeatureSpec) assoc).IsChoicesEnabled ? Choices.Single : Choices.NotEnabled; }
        }

        public bool IsAutoCompleteEnabled {
            get {
                var single = assoc as IOneToOneFeatureSpec;
                return single != null && single.IsAutoCompleteEnabled;
            }
        }

        public IConsentSurface IsUsable(INakedObjectSurface target) {
            IConsent consent = assoc.IsUsable(((NakedObjectWrapper) target).WrappedNakedObject);
            return new ConsentWrapper(consent);
        }

        public INakedObjectSurface GetNakedObject(INakedObjectSurface target) {
            INakedObjectAdapter result = assoc.GetNakedObject(((NakedObjectWrapper) target).WrappedNakedObject);
            return NakedObjectWrapper.Wrap(result, Surface, framework);
        }

        public bool IsVisible(INakedObjectSurface nakedObject) {
            return assoc.IsVisible(((NakedObjectWrapper) nakedObject).WrappedNakedObject);
        }

        public bool IsEager(INakedObjectSurface nakedObject) {
            return ((NakedObjectSpecificationWrapper) nakedObject.Specification).WrappedValue.ContainsFacet<IEagerlyFacet>() ||
                   assoc.ContainsFacet<IEagerlyFacet>();
        }

        public INakedObjectSurface[] GetChoices(INakedObjectSurface target, IDictionary<string, object> parameterNameValues) {
            var oneToOneFeature = assoc as IOneToOneFeatureSpec;
            var pnv = parameterNameValues == null ? null : parameterNameValues.ToDictionary(kvp => kvp.Key, kvp => framework.GetNakedObject(kvp.Value));
            return oneToOneFeature != null ? oneToOneFeature.GetChoices(((NakedObjectWrapper) target).WrappedNakedObject, pnv).Select(no => NakedObjectWrapper.Wrap(no, Surface, framework)).Cast<INakedObjectSurface>().ToArray() : null;
        }

        public Tuple<string, INakedObjectSpecificationSurface>[] GetChoicesParameters() {
            var oneToOneFeature = assoc as IOneToOneFeatureSpec;
            return oneToOneFeature != null ? oneToOneFeature.GetChoicesParameters().Select(WrapChoiceParm).ToArray() : new Tuple<string, INakedObjectSpecificationSurface>[0];
        }

        public Tuple<INakedObjectSurface, string>[] GetChoicesAndTitles(INakedObjectSurface target, IDictionary<string, object> parameterNameValues) {
            var choices = GetChoices(target, parameterNameValues);
            return choices.Select(c => new Tuple<INakedObjectSurface, string>(c, c.TitleString())).ToArray();
        }

        public INakedObjectSurface[] GetCompletions(INakedObjectSurface target, string autoCompleteParm) {
            var oneToOneFeature = assoc as IOneToOneFeatureSpec;
            return oneToOneFeature != null ? oneToOneFeature.GetCompletions(((NakedObjectWrapper) target).WrappedNakedObject, autoCompleteParm).Select(no => NakedObjectWrapper.Wrap(no, Surface, framework)).Cast<INakedObjectSurface>().ToArray() : null;
        }

        public int Count(INakedObjectSurface target) {
            if (IsCollection) {
                INakedObjectAdapter result = assoc.GetNakedObject(((NakedObjectWrapper) target).WrappedNakedObject);
                return result.GetCollectionFacetFromSpec().AsQueryable(result).Count();
            }
            return 0;
        }

        public string GetTitle(INakedObjectSurface nakedObject) {

            var enumFacet = assoc.GetFacet<IEnumFacet>();
    
            if (enumFacet != null) {
                return enumFacet.GetTitle(((NakedObjectWrapper) nakedObject).WrappedNakedObject);
            }

            var mask = assoc.GetFacet<IMaskFacet>();
            if (mask == null) {
                return nakedObject.TitleString();
            }
            var titleFacet = ((NakedObjectSpecificationWrapper) nakedObject.Specification).WrappedValue.GetFacet<ITitleFacet>();
            return titleFacet.GetTitleWithMask(mask.Value, ((NakedObjectWrapper) nakedObject).WrappedNakedObject, framework.NakedObjectManager);
        }

        public INakedObjectsSurface Surface { get; set; }

        #endregion

        private Tuple<string, INakedObjectSpecificationSurface> WrapChoiceParm(Tuple<string, IObjectSpec> parm) {
            return new Tuple<string, INakedObjectSpecificationSurface>(parm.Item1, new NakedObjectSpecificationWrapper(parm.Item2, Surface, framework));
        }

        public override bool Equals(object obj) {
            var nakedObjectAssociationWrapper = obj as NakedObjectAssociationWrapper;
            if (nakedObjectAssociationWrapper != null) {
                return Equals(nakedObjectAssociationWrapper);
            }
            return false;
        }

        public bool Equals(NakedObjectAssociationWrapper other) {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.assoc, assoc);
        }

        public override int GetHashCode() {
            return (assoc != null ? assoc.GetHashCode() : 0);
        }

        public override object GetScalarProperty(ScalarProperty name) {
            switch (name) {
                case (ScalarProperty.Name):
                    return Name;
                case (ScalarProperty.Description):
                    return Description;
                case (ScalarProperty.IsCollection):
                    return IsCollection;
                case (ScalarProperty.IsObject):
                    return IsObject;
                case (ScalarProperty.IsMandatory):
                    return IsMandatory;
                case (ScalarProperty.MaxLength):
                    return MaxLength;
                case (ScalarProperty.Pattern):
                    return Pattern;
                case (ScalarProperty.MemberOrder):
                    return MemberOrder;
                case (ScalarProperty.IsASet):
                    return IsASet;
                case (ScalarProperty.IsInline):
                    return IsInline;
                case (ScalarProperty.Mask):
                    return Mask;
                case (ScalarProperty.AutoCompleteMinLength):
                    return AutoCompleteMinLength;
                case (ScalarProperty.IsConcurrency):
                    return IsConcurrency;
                case (ScalarProperty.NumberOfLines):
                    return NumberOfLines;
                case (ScalarProperty.Width):
                    return Width;
                case (ScalarProperty.TypicalLength):
                    return TypicalLength;
                case (ScalarProperty.ExtensionData):
                    return ExtensionData;
                case (ScalarProperty.TableViewData):
                    return TableViewData;
                case (ScalarProperty.RenderEagerly):
                    return RenderEagerly;
                case (ScalarProperty.DoNotCount):
                    return DoNotCount;
                case (ScalarProperty.IsNullable):
                    return IsNullable;
                case (ScalarProperty.IsAjax):
                    return IsAjax;
                case (ScalarProperty.IsPassword):
                    return IsPassword;
                case (ScalarProperty.Range):
                    return Range;
                case (ScalarProperty.RegEx):
                    return RegEx;
                case (ScalarProperty.IsFindMenuEnabled):
                    return IsFindMenuEnabled;
                case (ScalarProperty.PresentationHint):
                    return PresentationHintValue;
                case (ScalarProperty.IsEnum):
                    return IsEnum;
                case (ScalarProperty.IsFileAttachment):
                    return IsFile;
                default:
                    throw new NotImplementedException(string.Format("{0} doesn't support {1}", GetType(), name));
            }
        }

        public object IsFile {
            get { return assoc.IsFile(framework); }
        }

        public bool IsEnum
        {
            get { return assoc.ContainsFacet<IEnumFacet>(); }
        }

        public object PresentationHintValue {
            get {
                var hintFacet = assoc.GetFacet<IPresentationHintFacet>();
                return hintFacet == null ? "" : hintFacet.Value;
            }
        }

        public bool IsFindMenuEnabled {
            get { return assoc is IOneToOneAssociationSpec && ((IOneToOneAssociationSpec) assoc).IsFindMenuEnabled; }
        }

        public Tuple<Regex, string> RegEx {
            get {
                var regEx = assoc.GetFacet<IRegExFacet>();
                return regEx == null ? null : new Tuple<Regex, string>(regEx.Pattern, regEx.FailureMessage);
            }
        }

        public Tuple<IConvertible, IConvertible, bool> Range {
            get {
                var rangeFacet = assoc.GetFacet<IRangeFacet>();
                return rangeFacet == null ? null : new Tuple<IConvertible, IConvertible, bool>(rangeFacet.Min, rangeFacet.Max, rangeFacet.IsDateRange);
            }
        }

        public object IsAjax {
            get {
                return !assoc.ContainsFacet<IAjaxFacet>();
            }
        }

        public object DoNotCount {
            get {
                return assoc.ContainsFacet<INotCountedFacet>();
            }
        }

        public object Width {
            get {
                var multiline = assoc.GetFacet<IMultiLineFacet>();
                return multiline == null ? 0 : multiline.Width;
            }
        }

        // todo move common assoc/parameter code into helper or baseclass
        public string GetMaskedValue(INakedObjectSurface valueNakedObject) {
            var mask = assoc.GetFacet<IMaskFacet>();

            if (valueNakedObject == null) {
                return null;
            }
            var no = ((NakedObjectWrapper)valueNakedObject).WrappedNakedObject;
            return mask != null ? no.Spec.GetFacet<ITitleFacet>().GetTitleWithMask(mask.Value, no, framework.NakedObjectManager) : no.TitleString();
        }

        public bool DefaultTypeIsExplicit(INakedObjectSurface nakedObject) {
            var no = ((NakedObjectWrapper) nakedObject).WrappedNakedObject;
            return assoc.GetDefaultType(no) == TypeOfDefaultValue.Explicit;
        }

        public object TypicalLength {
            get {
                var typicalLength = assoc.GetFacet<ITypicalLengthFacet>();
                return typicalLength == null ? 0 : typicalLength.Value;
            }
        }

        public object NumberOfLines {
            get {
                var multiline = assoc.GetFacet<IMultiLineFacet>();
                return multiline == null ? 1 : multiline.NumberOfLines;
            }
        }

        public Tuple<bool, string[]> TableViewData {
            get {
                var facet = assoc.GetFacet<ITableViewFacet>();
                return facet == null ? null : new Tuple<bool, string[]>(facet.Title, facet.Columns);
            }
        }

        // todo move common assoc/action code into helper or baseclass
        public bool RenderEagerly {
            get {
                IEagerlyFacet eagerlyFacet = assoc.GetFacet<IEagerlyFacet>();
                return eagerlyFacet != null && eagerlyFacet.What == EagerlyAttribute.Do.Rendering;
            }
        }

        public object IsPassword {
            get {
                return assoc.ContainsFacet<IPasswordFacet>();
            }
        }

        public bool IsNullable {
            get { return assoc.ContainsFacet<INullableFacet>(); }
        }
    }
}
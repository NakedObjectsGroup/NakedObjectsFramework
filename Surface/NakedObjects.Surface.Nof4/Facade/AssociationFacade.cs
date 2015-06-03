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
using NakedObjects.Facade.Contexts;
using NakedObjects.Facade.Utility;
using NakedObjects.Surface.Nof4.Utility;

namespace NakedObjects.Facade.Nof4 {
    public class AssociationFacade : IAssociationFacade {
        private readonly IAssociationSpec assoc;
        private readonly INakedObjectsFramework framework;

        public AssociationFacade(IAssociationSpec assoc, IFrameworkFacade surface, INakedObjectsFramework framework) {
            SurfaceUtils.AssertNotNull(assoc, "Assoc is null");
            SurfaceUtils.AssertNotNull(framework, "framework is null");
            SurfaceUtils.AssertNotNull(surface, "surface is null");

            this.assoc = assoc;
            this.framework = framework;
            Surface = surface;
        }

        public IAssociationSpec WrappedSpec {
            get { return assoc; }
        }

        #region IAssociationFacade Members

        public IDictionary<string, object> ExtensionData {
            get {
                var extData = new Dictionary<string, object>();

                if (assoc.ContainsFacet<IPresentationHintFacet>()) {
                    extData[IdConstants.PresentationHint] = assoc.GetFacet<IPresentationHintFacet>().Value;
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

        public int MemberOrder {
            get {
                var facet = assoc.GetFacet<IMemberOrderFacet>();

                int result;
                if (facet != null && Int32.TryParse(facet.Sequence, out result)) {
                    return result;
                }

                return 0;
            }
        }

        public bool IsASet {
            get {
                var collection = assoc as IOneToManyAssociationSpec;
                return collection != null && collection.IsASet;
            }
        }

        public bool IsInline {
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

        public ITypeFacade Specification {
            get { return new TypeFacade(assoc.ReturnSpec, Surface, framework); }
        }

        public ITypeFacade ElementSpecification {
            get {
                var coll = assoc as IOneToManyAssociationSpec;
                var elementSpec = coll == null ? null : coll.ElementSpec;
                return elementSpec == null ? null : new TypeFacade(elementSpec, Surface, framework);
            }
        }

        public string Id {
            get { return assoc.Id; }
        }

        public Choices IsChoicesEnabled {
            get {
                var oneToOneFeature = assoc as IOneToOneFeatureSpec;
                return oneToOneFeature != null && oneToOneFeature.IsChoicesEnabled ? Choices.Single : Choices.NotEnabled;
            }
        }

        public bool IsAutoCompleteEnabled {
            get {
                var single = assoc as IOneToOneFeatureSpec;
                return single != null && single.IsAutoCompleteEnabled;
            }
        }

        public IConsentFacade IsUsable(IObjectFacade target) {
            IConsent consent = assoc.IsUsable(((ObjectFacade) target).WrappedNakedObject);
            return new ConsentFacade(consent);
        }

        public IObjectFacade GetNakedObject(IObjectFacade target) {
            INakedObjectAdapter result = assoc.GetNakedObject(((ObjectFacade) target).WrappedNakedObject);
            return ObjectFacade.Wrap(result, Surface, framework);
        }

        public bool IsVisible(IObjectFacade nakedObject) {
            return assoc.IsVisible(((ObjectFacade) nakedObject).WrappedNakedObject);
        }

        public bool IsEager(IObjectFacade nakedObject) {
            return ((TypeFacade) nakedObject.Specification).WrappedValue.ContainsFacet<IEagerlyFacet>() ||
                   assoc.ContainsFacet<IEagerlyFacet>();
        }

        public IObjectFacade[] GetChoices(IObjectFacade target, IDictionary<string, object> parameterNameValues) {
            var oneToOneFeature = assoc as IOneToOneFeatureSpec;
            var pnv = parameterNameValues == null ? null : parameterNameValues.ToDictionary(kvp => kvp.Key, kvp => framework.GetNakedObject(kvp.Value));
            return oneToOneFeature != null ? oneToOneFeature.GetChoices(((ObjectFacade) target).WrappedNakedObject, pnv).Select(no => ObjectFacade.Wrap(no, Surface, framework)).Cast<IObjectFacade>().ToArray() : null;
        }

        public Tuple<string, ITypeFacade>[] GetChoicesParameters() {
            var oneToOneFeature = assoc as IOneToOneFeatureSpec;
            return oneToOneFeature != null ? oneToOneFeature.GetChoicesParameters().Select(WrapChoiceParm).ToArray() : new Tuple<string, ITypeFacade>[0];
        }

        public Tuple<IObjectFacade, string>[] GetChoicesAndTitles(IObjectFacade target, IDictionary<string, object> parameterNameValues) {
            var choices = GetChoices(target, parameterNameValues);
            return choices.Select(c => new Tuple<IObjectFacade, string>(c, c.TitleString)).ToArray();
        }

        public IObjectFacade[] GetCompletions(IObjectFacade target, string autoCompleteParm) {
            var oneToOneFeature = assoc as IOneToOneFeatureSpec;
            return oneToOneFeature != null ? oneToOneFeature.GetCompletions(((ObjectFacade) target).WrappedNakedObject, autoCompleteParm).Select(no => ObjectFacade.Wrap(no, Surface, framework)).Cast<IObjectFacade>().ToArray() : null;
        }

        public int Count(IObjectFacade target) {
            if (IsCollection) {
                INakedObjectAdapter result = assoc.GetNakedObject(((ObjectFacade) target).WrappedNakedObject);
                return result.GetCollectionFacetFromSpec().AsQueryable(result).Count();
            }
            return 0;
        }

        public string GetTitle(IObjectFacade nakedObject) {
            var enumFacet = assoc.GetFacet<IEnumFacet>();

            if (enumFacet != null) {
                return enumFacet.GetTitle(((ObjectFacade) nakedObject).WrappedNakedObject);
            }

            var mask = assoc.GetFacet<IMaskFacet>();
            if (mask == null) {
                return nakedObject.TitleString;
            }
            var titleFacet = ((TypeFacade) nakedObject.Specification).WrappedValue.GetFacet<ITitleFacet>();
            return titleFacet.GetTitleWithMask(mask.Value, ((ObjectFacade) nakedObject).WrappedNakedObject, framework.NakedObjectManager);
        }

        public IFrameworkFacade Surface { get; set; }

        public bool IsFile {
            get { return assoc.IsFile(framework); }
        }

        public bool IsEnum {
            get { return assoc.ContainsFacet<IEnumFacet>(); }
        }

        public string PresentationHint {
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

        public bool IsAjax {
            get { return !assoc.ContainsFacet<IAjaxFacet>(); }
        }

        public bool DoNotCount {
            get { return assoc.ContainsFacet<INotCountedFacet>(); }
        }

        public int Width {
            get {
                var multiline = assoc.GetFacet<IMultiLineFacet>();
                return multiline == null ? 0 : multiline.Width;
            }
        }

        // todo move common assoc/parameter code into helper or baseclass
        public string GetMaskedValue(IObjectFacade valueNakedObject) {
            var mask = assoc.GetFacet<IMaskFacet>();

            if (valueNakedObject == null) {
                return null;
            }
            var no = ((ObjectFacade) valueNakedObject).WrappedNakedObject;
            return mask != null ? no.Spec.GetFacet<ITitleFacet>().GetTitleWithMask(mask.Value, no, framework.NakedObjectManager) : no.TitleString();
        }

        public bool DefaultTypeIsExplicit(IObjectFacade nakedObject) {
            var no = ((ObjectFacade) nakedObject).WrappedNakedObject;
            return assoc.GetDefaultType(no) == TypeOfDefaultValue.Explicit;
        }

        public int TypicalLength {
            get {
                var typicalLength = assoc.GetFacet<ITypicalLengthFacet>();
                return typicalLength == null ? 0 : typicalLength.Value;
            }
        }

        public int NumberOfLines {
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

        public bool IsPassword {
            get { return assoc.ContainsFacet<IPasswordFacet>(); }
        }

        public bool IsNullable {
            get { return assoc.ContainsFacet<INullableFacet>(); }
        }

        #endregion

        private Tuple<string, ITypeFacade> WrapChoiceParm(Tuple<string, IObjectSpec> parm) {
            return new Tuple<string, ITypeFacade>(parm.Item1, new TypeFacade(parm.Item2, Surface, framework));
        }

        public override bool Equals(object obj) {
            var nakedObjectAssociationWrapper = obj as AssociationFacade;
            if (nakedObjectAssociationWrapper != null) {
                return Equals(nakedObjectAssociationWrapper);
            }
            return false;
        }

        public bool Equals(AssociationFacade other) {
            if (ReferenceEquals(null, other)) { return false; }
            if (ReferenceEquals(this, other)) { return true; }
            return Equals(other.assoc, assoc);
        }

        public override int GetHashCode() {
            return (assoc != null ? assoc.GetHashCode() : 0);
        }
    }
}
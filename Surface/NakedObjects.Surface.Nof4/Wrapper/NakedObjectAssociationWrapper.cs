// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections.Generic;
using System.Linq;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets.AutoComplete;
using NakedObjects.Architecture.Facets.Objects.Ident.Title;
using NakedObjects.Architecture.Facets.Objects.ViewModel;
using NakedObjects.Architecture.Facets.Ordering.MemberOrder;
using NakedObjects.Architecture.Facets.Presentation;
using NakedObjects.Architecture.Facets.Properties.Eagerly;
using NakedObjects.Architecture.Facets.Propparam.Validate.Mask;
using NakedObjects.Architecture.Facets.Propparam.Validate.MaxLength;
using NakedObjects.Architecture.Facets.Propparam.Validate.RegEx;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Services;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.Util;
using NakedObjects.Core.Context;
using NakedObjects.Surface.Utility;

namespace NakedObjects.Surface.Nof4.Wrapper {
    public class NakedObjectAssociationWrapper : ScalarPropertyHolder, INakedObjectAssociationSurface {
        private readonly INakedObjectAssociation assoc;


        public NakedObjectAssociationWrapper(INakedObjectAssociation assoc, INakedObjectsSurface surface) {
            this.assoc = assoc;
            Surface = surface;
        }

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
            get { return assoc.IsCollection; }
        }

        public bool IsObject {
            get { return assoc.IsObject; }
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
            get { return assoc.IsASet; }
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
            get { return new NakedObjectSpecificationWrapper(assoc.Specification, Surface); }
        }

        public string Id {
            get { return assoc.Id; }
        }

        public bool IsChoicesEnabled {
            get { return ((IOneToOneFeature) assoc).IsChoicesEnabled; }
        }

        public bool IsAutoCompleteEnabled {
            get { return ((IOneToOneFeature)assoc).IsAutoCompleteEnabled; }
        }


        public IConsentSurface IsUsable(INakedObjectSurface target) {
            IConsent consent = assoc.IsUsable(NakedObjectsContext.Session, ((NakedObjectWrapper) target).WrappedNakedObject);
            return new ConsentWrapper(consent);
        }

        public INakedObjectSurface GetNakedObject(INakedObjectSurface target) {
            INakedObject result = assoc.GetNakedObject(((NakedObjectWrapper) target).WrappedNakedObject);
            return NakedObjectWrapper.Wrap(result, Surface);
        }

        public bool IsVisible(INakedObjectSurface nakedObject) {
            return assoc.IsVisible(NakedObjectsContext.Session, ((NakedObjectWrapper) nakedObject).WrappedNakedObject);
        }

        public bool IsEager(INakedObjectSurface nakedObject) {
            return ((NakedObjectSpecificationWrapper) nakedObject.Specification).WrappedValue.ContainsFacet<IEagerlyFacet>() ||
                   assoc.ContainsFacet<IEagerlyFacet>();
        }

        public INakedObjectSurface[] GetChoices(INakedObjectSurface target, IDictionary<string, INakedObjectSurface> parameterNameValues) {
            var oneToOneFeature = assoc as IOneToOneFeature;
            return oneToOneFeature != null ? oneToOneFeature.GetChoices(((NakedObjectWrapper)target).WrappedNakedObject, null).Select(no => NakedObjectWrapper.Wrap(no, Surface)).Cast<INakedObjectSurface>().ToArray() : null;
        }

        private Tuple<string, INakedObjectSpecificationSurface> WrapChoiceParm(Tuple<string, INakedObjectSpecification> parm) {
            return new Tuple<string, INakedObjectSpecificationSurface>(parm.Item1, new NakedObjectSpecificationWrapper(parm.Item2, Surface));
        }

        public Tuple<string, INakedObjectSpecificationSurface>[] GetChoicesParameters() {
            var oneToOneFeature = assoc as IOneToOneFeature;
            return oneToOneFeature != null ? oneToOneFeature.GetChoicesParameters().Select(WrapChoiceParm).ToArray() : new Tuple<string, INakedObjectSpecificationSurface>[0];
        }

        public Tuple<INakedObjectSurface, string>[] GetChoicesAndTitles(INakedObjectSurface target, IDictionary<string, INakedObjectSurface> parameterNameValues) {
            var choices = GetChoices(target, parameterNameValues);
            return choices.Select(c => new Tuple<INakedObjectSurface, string>(c, c.TitleString())).ToArray();
        }

        public INakedObjectSurface[] GetCompletions(INakedObjectSurface target, string autoCompleteParm) {
            var oneToOneFeature = assoc as IOneToOneFeature;
            return oneToOneFeature != null ? oneToOneFeature.GetCompletions(((NakedObjectWrapper)target).WrappedNakedObject, autoCompleteParm).Select(no => NakedObjectWrapper.Wrap(no, Surface)).Cast<INakedObjectSurface>().ToArray() : null;
        }

        public int Count(INakedObjectSurface target) {
            if (IsCollection) {
                INakedObject result = assoc.GetNakedObject(((NakedObjectWrapper) target).WrappedNakedObject);
                return result.GetCollectionFacetFromSpec().AsQueryable(result).Count();
            }
            return 0;
        }

        public string GetTitle(INakedObjectSurface nakedObject) {
            var mask = assoc.GetFacet<IMaskFacet>();
            if (mask == null) {
                return nakedObject.TitleString();
            }
            var titleFacet = ((NakedObjectSpecificationWrapper) nakedObject.Specification).WrappedValue.GetFacet<ITitleFacet>();
            return titleFacet.GetTitleWithMask(mask.Value, ((NakedObjectWrapper) nakedObject).WrappedNakedObject);
        }

        #endregion

        public INakedObjectsSurface Surface { get; set; }

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
                case (ScalarProperty.ExtensionData):
                    return ExtensionData;
                default:
                    throw new NotImplementedException(string.Format("{0} doesn't support {1}", GetType(), name));
            }
        }
    }
}
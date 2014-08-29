// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections.Generic;
using System.Linq;
using NakedObjects.Architecture.Facets.AutoComplete;
using NakedObjects.Architecture.Facets.Presentation;
using NakedObjects.Architecture.Facets.Propparam.Validate.Mask;
using NakedObjects.Architecture.Facets.Propparam.Validate.MaxLength;
using NakedObjects.Architecture.Facets.Propparam.Validate.RegEx;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Core.Context;
using NakedObjects.Surface.Utility;

namespace NakedObjects.Surface.Nof4.Wrapper {
    public class NakedObjectActionParameterWrapper : ScalarPropertyHolder, INakedObjectActionParameterSurface {
        private readonly INakedObjectActionParameter nakedObjectActionParameter;
        private readonly INakedObjectsFramework framework;
        private readonly string overloadedUniqueId;

        public NakedObjectActionParameterWrapper(INakedObjectActionParameter nakedObjectActionParameter, INakedObjectsSurface surface, INakedObjectsFramework framework, string overloadedUniqueId) {
            this.nakedObjectActionParameter = nakedObjectActionParameter;
            this.framework = framework;
            this.overloadedUniqueId = overloadedUniqueId;
            Surface = surface;
        }

        protected IDictionary<string, object> ExtensionData {
            get {
                var extData = new Dictionary<string, object>();

                if (nakedObjectActionParameter.ContainsFacet<IPresentationHintFacet>()) {
                    extData[PresentationHint] = nakedObjectActionParameter.GetFacet<IPresentationHintFacet>().Value;
                }

                return extData.Any() ? extData : null;
            }
        }

        public string Name {
            get { return nakedObjectActionParameter.Name; }
        }

        public string Description {
            get { return nakedObjectActionParameter.Description; }
        }

        public bool IsMandatory {
            get { return nakedObjectActionParameter.IsMandatory; }
        }

        public int? MaxLength {
            get {
                var facet = nakedObjectActionParameter.GetFacet<IMaxLengthFacet>();
                return facet != null ? (int?) facet.Value : null;
            }
        }

        public string Pattern {
            get {
                var facet = nakedObjectActionParameter.GetFacet<IRegExFacet>();
                return facet != null ? facet.Pattern.ToString() : null;
            }
        }

        public string Mask {
            get {
                var facet = nakedObjectActionParameter.GetFacet<IMaskFacet>();
                return facet != null ? facet.Value : null;
            }
        }

        public int AutoCompleteMinLength {
            get {
                var facet = nakedObjectActionParameter.GetFacet<IAutoCompleteFacet>();
                return facet != null ? facet.MinLength : 0;
            }
        }


        public int Number {
            get { return nakedObjectActionParameter.Number; }
        }

        #region INakedObjectActionParameterSurface Members

        public INakedObjectSpecificationSurface Specification {
            get { return new NakedObjectSpecificationWrapper(nakedObjectActionParameter.Specification, Surface, framework); }
        }

        public INakedObjectActionSurface Action {
            get { return new NakedObjectActionWrapper(nakedObjectActionParameter.Action, Surface, framework, overloadedUniqueId ?? ""); }
        }

        public string Id {
            get { return nakedObjectActionParameter.Id; }
        }

        public bool IsChoicesEnabled {
            get { return nakedObjectActionParameter.IsChoicesEnabled || nakedObjectActionParameter.IsMultipleChoicesEnabled; }
        }

        public bool IsAutoCompleteEnabled {
            get { return nakedObjectActionParameter.IsAutoCompleteEnabled; }
        }

        public INakedObjectSurface[] GetChoices(INakedObjectSurface nakedObject, IDictionary<string, INakedObjectSurface> parameterNameValues) {
            return nakedObjectActionParameter.GetChoices(((NakedObjectWrapper)nakedObject).WrappedNakedObject, null, framework.ObjectPersistor).Select(no => NakedObjectWrapper.Wrap(no, Surface, framework)).Cast<INakedObjectSurface>().ToArray();
        }

        private Tuple<string, INakedObjectSpecificationSurface> WrapChoiceParm(Tuple<string, INakedObjectSpecification> parm) {
            return new Tuple<string, INakedObjectSpecificationSurface>(parm.Item1, new NakedObjectSpecificationWrapper(parm.Item2, Surface, framework));
        }

        public Tuple<string, INakedObjectSpecificationSurface>[] GetChoicesParameters() {     
            return nakedObjectActionParameter.GetChoicesParameters().Select(WrapChoiceParm).ToArray();
        }

        public Tuple<INakedObjectSurface, string>[] GetChoicesAndTitles(INakedObjectSurface nakedObject, IDictionary<string, INakedObjectSurface> parameterNameValues) {
            var choices = GetChoices(nakedObject, parameterNameValues);
            return choices.Select(c => new Tuple<INakedObjectSurface, string>(c, c.TitleString())).ToArray();
        }

        public INakedObjectSurface[] GetCompletions(INakedObjectSurface nakedObject, string autoCompleteParm) {
            return nakedObjectActionParameter.GetCompletions(((NakedObjectWrapper)nakedObject).WrappedNakedObject, autoCompleteParm, framework.ObjectPersistor).Select(no => NakedObjectWrapper.Wrap(no, Surface, framework)).Cast<INakedObjectSurface>().ToArray();
        }

        public bool DefaultTypeIsExplicit(INakedObjectSurface nakedObject) {
            return nakedObjectActionParameter.GetDefaultType(((NakedObjectWrapper)nakedObject).WrappedNakedObject, framework.ObjectPersistor) == TypeOfDefaultValue.Explicit;
        }

        public INakedObjectSurface GetDefault(INakedObjectSurface nakedObject) {
            return NakedObjectWrapper.Wrap(nakedObjectActionParameter.GetDefault(((NakedObjectWrapper)nakedObject).WrappedNakedObject, framework.ObjectPersistor), Surface, framework);
        }

        #endregion

        public INakedObjectsSurface Surface { get; set; }

        public override bool Equals(object obj) {
            var nakedObjectActionParameterWrapper = obj as NakedObjectActionParameterWrapper;
            if (nakedObjectActionParameterWrapper != null) {
                return Equals(nakedObjectActionParameterWrapper);
            }
            return false;
        }

        public bool Equals(NakedObjectActionParameterWrapper other) {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.nakedObjectActionParameter, nakedObjectActionParameter);
        }

        public override int GetHashCode() {
            return (nakedObjectActionParameter != null ? nakedObjectActionParameter.GetHashCode() : 0);
        }

        public override object GetScalarProperty(ScalarProperty name) {
            switch (name) {
                case (ScalarProperty.Name):
                    return Name;
                case (ScalarProperty.Description):
                    return Description;
                case (ScalarProperty.IsMandatory):
                    return IsMandatory;
                case (ScalarProperty.MaxLength):
                    return MaxLength;
                case (ScalarProperty.Pattern):
                    return Pattern;
                case (ScalarProperty.Number):
                    return Number;
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
// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections.Generic;
using System.Linq;
using NakedObjects.Surface;
using NakedObjects.Surface.Context;
using NakedObjects.Surface.Nof2.Context;
using NakedObjects.Surface.Utility;
using org.nakedobjects.@object;

namespace NakedObjects.Surface.Nof2.Wrapper {
    public class NakedObjectActionParameterWrapper : ScalarPropertyHolder, INakedObjectActionParameterSurface {
        private readonly NakedObjectActionParameter nakedObjectActionParameter;
        private readonly Naked target;

        public NakedObjectActionParameterWrapper(NakedObjectActionParameter nakedObjectActionParameter, Naked target, INakedObjectsSurface surface) {
            this.nakedObjectActionParameter = nakedObjectActionParameter;
            this.target = target;
            Surface = surface; 
        }

        #region INakedObjectActionParameterSurface Members

        public INakedObjectSpecificationSurface Specification {
            get { return new NakedObjectSpecificationWrapper(nakedObjectActionParameter.getSpecification(), target, Surface); }
        }

        public INakedObjectSpecificationSurface ElementType { get; private set; }

        public INakedObjectActionSurface Action {
            get { return new NakedObjectActionWrapper(nakedObjectActionParameter.getAction(), target, Surface); }
        }

        public string Id {
            get { return nakedObjectActionParameter.getId(); }
        }

        Choices INakedObjectActionParameterSurface.IsChoicesEnabled {
            get { return IsChoicesEnabled ? Choices.Single : Choices.NotEnabled; }
        }
        public bool IsAutoCompleteEnabled {
            get { return false; }
        }

        public INakedObjectSurface[] GetChoices(INakedObjectSurface nakedObject, IDictionary<string, object> parameterNameValues) {
            //return GetChoices(nakedObject,  )
            return new INakedObjectSurface[]{};
        }

        public Tuple<INakedObjectSurface, string>[] GetChoicesAndTitles(INakedObjectSurface nakedObject, IDictionary<string, object> parameterNameValues) {
            throw new NotImplementedException();
        }

        public INakedObjectSurface[] GetCompletions(INakedObjectSurface nakedObject, string autoCompleteParm) {
            throw new NotImplementedException();
        }

        public bool IsChoicesEnabled {
            get { return nakedObjectActionParameter.isChoicesEnabled(); }
        }

        public string Name {
            get { return nakedObjectActionParameter.getName(); }
        }

        public string Description {
            get { return nakedObjectActionParameter.getDescription(); }
        }

        public bool IsMandatory {
            get { return nakedObjectActionParameter.isMandatory(); }
        }

        public int? MaxLength {
            get { return null; }
        }

        public string Pattern {
            get { return null; }
        }

        public int Number {
            get { return nakedObjectActionParameter.getNumber(); }
        }

        public INakedObjectSurface[] GetChoices(INakedObjectSurface nakedObject, IDictionary<string, INakedObjectSurface> parameterNameValues) {
            return nakedObjectActionParameter.getChoices(((NakedObjectWrapper) nakedObject).NakedObject).Select(no => new NakedObjectWrapper(no, Surface)).Cast<INakedObjectSurface>().ToArray();
        }

        public bool DefaultTypeIsExplicit(INakedObjectSurface nakedObject) {
            return nakedObjectActionParameter.getDefault(((NakedObjectWrapper) nakedObject).NakedObject) != null;
        }

        public INakedObjectSurface GetDefault(INakedObjectSurface nakedObject) {
            return new NakedObjectWrapper(nakedObjectActionParameter.getDefault(((NakedObjectWrapper) nakedObject).NakedObject), Surface);
        }

        public Tuple<string, INakedObjectSpecificationSurface>[] GetChoicesParameters() {
            return new Tuple<string, INakedObjectSpecificationSurface>[]{};
        }

        public string GetMaskedValue(INakedObjectSurface valueNakedObject) {
            return valueNakedObject.TitleString();
        }

        public IConsentSurface IsValid(INakedObjectSurface target, object value) {
            throw new NotImplementedException();
        }

        #endregion

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
                    return "";
                case (ScalarProperty.AutoCompleteMinLength):
                    return 0;
                case (ScalarProperty.TypicalLength):
                    return 0;
                case (ScalarProperty.IsNullable):
                    return false;
                case (ScalarProperty.IsAjax):
                    return false;
                case (ScalarProperty.IsPassword):
                    return false;
                case (ScalarProperty.NumberOfLines):
                    return 0;
                case (ScalarProperty.Width):
                    return 0;
                case (ScalarProperty.Range):
                    return 0;
                case (ScalarProperty.RegEx):
                    return null;
                case (ScalarProperty.PresentationHint):
                    return "";
                case (ScalarProperty.IsFindMenuEnabled):
                    return false;
                case (ScalarProperty.ExtensionData):
                    return null;
                default:
                    throw new NotImplementedException(string.Format("{0} doesn't support {1}", GetType(), name));
            }
        }

        public INakedObjectsSurface Surface { get; set; }
    }
}
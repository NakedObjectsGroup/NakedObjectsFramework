using System;
using System.Collections.Generic;
using System.Linq;

namespace NakedObjects.Surface {
    public abstract class ScalarPropertyHolder : IScalarPropertyHolder {
        // custom extensions 
        protected const string ServiceType = "x-ro-nof-serviceType";
        protected const string RenderInEditMode = "x-ro-nof-renderInEditMode";
        protected const string PresentationHint = "x-ro-nof-presentationHint";

        #region IScalarPropertyHolder Members

        public T GetScalarProperty<T>(ScalarProperty name) {
            return (T) GetScalarProperty(name);
        }

        public bool SupportsProperty(ScalarProperty name) {
            try {
                GetScalarProperty(name);
            }
            catch (NotImplementedException) {
                return false;
            }

            return true;
        }

        public IEnumerable<ScalarProperty> SupportedProperties() {
            return Enum.GetValues(typeof (ScalarProperty)).Cast<ScalarProperty>().Where(SupportsProperty);
        }

        #endregion

        public abstract object GetScalarProperty(ScalarProperty name);
    }
}
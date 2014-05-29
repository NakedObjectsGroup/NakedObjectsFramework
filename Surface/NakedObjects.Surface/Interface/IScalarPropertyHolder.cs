using System.Collections.Generic;

namespace NakedObjects.Surface {
    public interface IScalarPropertyHolder {
        T GetScalarProperty<T>(ScalarProperty name);
        bool SupportsProperty(ScalarProperty name);
        IEnumerable<ScalarProperty> SupportedProperties();
    }
}
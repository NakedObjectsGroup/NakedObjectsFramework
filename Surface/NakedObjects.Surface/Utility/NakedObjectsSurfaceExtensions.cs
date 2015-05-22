// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace NakedObjects.Surface.Utility {
    public static class NakedObjectsSurfaceExtensions {
       

        #region INakedObjectSpecificationSurface

        public static string PresentationHint(this INakedObjectSpecificationSurface nakedObjectSpecificationSurface) {
            return nakedObjectSpecificationSurface.GetScalarProperty<string>(ScalarProperty.PresentationHint);
        }

        public static bool IsAlwaysImmutable(this INakedObjectSpecificationSurface nakedObjectSpecificationSurface) {
            return nakedObjectSpecificationSurface.GetScalarProperty<bool>(ScalarProperty.IsAlwaysImmutable);
        }

        public static bool IsImmutableOncePersisted(this INakedObjectSpecificationSurface nakedObjectSpecificationSurface) {
            return nakedObjectSpecificationSurface.GetScalarProperty<bool>(ScalarProperty.IsImmutableOncePersisted);
        }

        public static bool IsComplexType(this INakedObjectSpecificationSurface nakedObjectSpecificationSurface) {
            return nakedObjectSpecificationSurface.GetScalarProperty<bool>(ScalarProperty.IsComplexType);
        }

        public static bool IsParseable(this INakedObjectSpecificationSurface nakedObjectSpecificationSurface) {
            return nakedObjectSpecificationSurface.GetScalarProperty<bool>(ScalarProperty.IsParseable);
        }

        public static bool IsStream(this INakedObjectSpecificationSurface nakedObjectSpecificationSurface) {
            return nakedObjectSpecificationSurface.GetScalarProperty<bool>(ScalarProperty.IsStream);
        }

        public static bool IsQueryable(this INakedObjectSpecificationSurface nakedObjectSpecificationSurface) {
            return nakedObjectSpecificationSurface.GetScalarProperty<bool>(ScalarProperty.IsQueryable);
        }

        public static bool IsService(this INakedObjectSpecificationSurface nakedObjectSpecificationSurface) {
            return nakedObjectSpecificationSurface.GetScalarProperty<bool>(ScalarProperty.IsService);
        }

        public static bool IsVoid(this INakedObjectSpecificationSurface nakedObjectSpecificationSurface) {
            return nakedObjectSpecificationSurface.GetScalarProperty<bool>(ScalarProperty.IsVoid);
        }

        public static bool IsDateTime(this INakedObjectSpecificationSurface nakedObjectSpecificationSurface) {
            return nakedObjectSpecificationSurface.GetScalarProperty<bool>(ScalarProperty.IsDateTime);
        }

        public static bool IsCollection(this INakedObjectSpecificationSurface nakedObjectSpecificationSurface) {
            return nakedObjectSpecificationSurface.GetScalarProperty<bool>(ScalarProperty.IsCollection);
        }

        public static bool IsObject(this INakedObjectSpecificationSurface nakedObjectSpecificationSurface) {
            return nakedObjectSpecificationSurface.GetScalarProperty<bool>(ScalarProperty.IsObject);
        }

        public static string FullName(this INakedObjectSpecificationSurface nakedObjectSpecificationSurface) {
            return nakedObjectSpecificationSurface.GetScalarProperty<string>(ScalarProperty.FullName);
        }

        // todo should remove this and move title stuff down
        public static string UntitledName(this INakedObjectSpecificationSurface nakedObjectSpecificationSurface) {
            return nakedObjectSpecificationSurface.GetScalarProperty<string>(ScalarProperty.UntitledName);
        }

        public static string SingularName(this INakedObjectSpecificationSurface nakedObjectSpecificationSurface) {
            return nakedObjectSpecificationSurface.GetScalarProperty<string>(ScalarProperty.SingularName);
        }

        public static string PluralName(this INakedObjectSpecificationSurface nakedObjectSpecificationSurface) {
            return nakedObjectSpecificationSurface.GetScalarProperty<string>(ScalarProperty.PluralName);
        }

        public static string Description(this INakedObjectSpecificationSurface nakedObjectSpecificationSurface) {
            return nakedObjectSpecificationSurface.GetScalarProperty<string>(ScalarProperty.Description);
        }

        public static bool IsASet(this INakedObjectSpecificationSurface nakedObjectSpecificationSurface) {
            return nakedObjectSpecificationSurface.GetScalarProperty<bool>(ScalarProperty.IsASet);
        }

        public static bool IsAggregated(this INakedObjectSpecificationSurface nakedObjectSpecificationSurface) {
            return nakedObjectSpecificationSurface.GetScalarProperty<bool>(ScalarProperty.IsAggregated);
        }

        public static bool IsImage(this INakedObjectSpecificationSurface nakedObjectSpecificationSurface) {
            return nakedObjectSpecificationSurface.GetScalarProperty<bool>(ScalarProperty.IsImage);
        }

        public static bool IsFileAttachment(this INakedObjectSpecificationSurface nakedObjectSpecificationSurface) {
            return nakedObjectSpecificationSurface.GetScalarProperty<bool>(ScalarProperty.IsFileAttachment);
        }

        public static bool IsFile(this INakedObjectSpecificationSurface nakedObjectSpecificationSurface) {
            return nakedObjectSpecificationSurface.GetScalarProperty<bool>(ScalarProperty.IsFile);
        }

        public static IDictionary<string, object> ExtensionData(this INakedObjectSpecificationSurface nakedObjectSpecificationSurface) {
            return nakedObjectSpecificationSurface.GetScalarProperty<IDictionary<string, object>>(ScalarProperty.ExtensionData);
        }

        public static bool IsBoolean(this INakedObjectSpecificationSurface nakedObjectSpecificationSurface) {
            return nakedObjectSpecificationSurface.GetScalarProperty<bool>(ScalarProperty.IsBoolean);
        }

        public static bool IsEnum(this INakedObjectSpecificationSurface nakedObjectSpecificationSurface) {
            return nakedObjectSpecificationSurface.GetScalarProperty<bool>(ScalarProperty.IsEnum);
        }

        #endregion
    }
}
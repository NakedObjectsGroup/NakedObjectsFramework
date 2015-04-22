// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections.Generic;

namespace NakedObjects.Surface.Utility {
    public static class NakedObjectsSurfaceExtensions {
        #region INakedObjectSurface



        public static bool IsCollectionMemento(this INakedObjectSurface nakedObjectSurface) {
            return nakedObjectSurface.GetScalarProperty<bool>(ScalarProperty.IsCollectionMemento);
        }

        public static bool IsTransient(this INakedObjectSurface nakedObjectSurface) {
            return nakedObjectSurface.GetScalarProperty<bool>(ScalarProperty.IsTransient);
        }

        public static bool IsUserPersistable(this INakedObjectSurface nakedObjectSurface) {
            return nakedObjectSurface.GetScalarProperty<bool>(ScalarProperty.IsUserPersistable);
        }

        public static bool IsNotPersistent(this INakedObjectSurface nakedObjectSurface) {
            return nakedObjectSurface.GetScalarProperty<bool>(ScalarProperty.IsNotPersistent);
        }

        public static string TitleString(this INakedObjectSurface nakedObjectSurface) {
            return nakedObjectSurface.GetScalarProperty<string>(ScalarProperty.TitleString);
        }

        public static IDictionary<string, object> ExtensionData(this INakedObjectSurface nakedObjectSurface) {
            return nakedObjectSurface.GetScalarProperty<IDictionary<string, object>>(ScalarProperty.ExtensionData);
        }

        #endregion

        #region INakedObjectActionParameterSurface

        public static string Name(this INakedObjectActionParameterSurface nakedObjectActionParameterSurface) {
            return nakedObjectActionParameterSurface.GetScalarProperty<string>(ScalarProperty.Name);
        }

        public static string Description(this INakedObjectActionParameterSurface nakedObjectActionParameterSurface) {
            return nakedObjectActionParameterSurface.GetScalarProperty<string>(ScalarProperty.Description);
        }

        public static bool IsMandatory(this INakedObjectActionParameterSurface nakedObjectActionParameterSurface) {
            return nakedObjectActionParameterSurface.GetScalarProperty<bool>(ScalarProperty.IsMandatory);
        }

        public static int? MaxLength(this INakedObjectActionParameterSurface nakedObjectActionParameterSurface) {
            return nakedObjectActionParameterSurface.GetScalarProperty<int?>(ScalarProperty.MaxLength);
        }

        public static string Pattern(this INakedObjectActionParameterSurface nakedObjectActionParameterSurface) {
            return nakedObjectActionParameterSurface.GetScalarProperty<string>(ScalarProperty.Pattern);
        }

        public static int Number(this INakedObjectActionParameterSurface nakedObjectActionParameterSurface) {
            return nakedObjectActionParameterSurface.GetScalarProperty<int>(ScalarProperty.Number);
        }


        public static string Mask(this INakedObjectActionParameterSurface nakedObjectActionParameterSurface) {
            return nakedObjectActionParameterSurface.GetScalarProperty<string>(ScalarProperty.Mask);
        }

        public static int AutoCompleteMinLength(this INakedObjectActionParameterSurface nakedObjectActionParameterSurface) {
            return nakedObjectActionParameterSurface.GetScalarProperty<int>(ScalarProperty.AutoCompleteMinLength);
        }

        public static IDictionary<string, object> ExtensionData(this INakedObjectActionParameterSurface nakedObjectActionParameterSurface) {
            return nakedObjectActionParameterSurface.GetScalarProperty<IDictionary<string, object>>(ScalarProperty.ExtensionData);
        }

        #endregion

        #region INakedObjectActionSurface

        public static string Name(this INakedObjectActionSurface nakedObjectActionSurface) {
            return nakedObjectActionSurface.GetScalarProperty<string>(ScalarProperty.Name);
        }

        public static string Description(this INakedObjectActionSurface nakedObjectActionSurface) {
            return nakedObjectActionSurface.GetScalarProperty<string>(ScalarProperty.Description);
        }

        public static bool IsQueryOnly(this INakedObjectActionSurface nakedObjectActionSurface) {
            return nakedObjectActionSurface.GetScalarProperty<bool>(ScalarProperty.IsQueryOnly);
        }

        public static bool IsIdempotent(this INakedObjectActionSurface nakedObjectActionSurface) {
            return nakedObjectActionSurface.GetScalarProperty<bool>(ScalarProperty.IsIdempotent);
        }

        public static bool IsContributed(this INakedObjectActionSurface nakedObjectActionSurface) {
            return nakedObjectActionSurface.GetScalarProperty<bool>(ScalarProperty.IsContributed);
        }

        public static int MemberOrder(this INakedObjectActionSurface nakedObjectActionSurface) {
            return nakedObjectActionSurface.GetScalarProperty<int>(ScalarProperty.MemberOrder);
        }

        public static IDictionary<string, object> ExtensionData(this INakedObjectActionSurface nakedObjectActionSurface) {
            return nakedObjectActionSurface.GetScalarProperty<IDictionary<string, object>>(ScalarProperty.ExtensionData);
        }

        #endregion

        #region INakedObjectAssociationSurface

        public static int? MaxLength(this INakedObjectAssociationSurface nakedObjectAssociationSurface) {
            return nakedObjectAssociationSurface.GetScalarProperty<int?>(ScalarProperty.MaxLength);
        }

        public static string Pattern(this INakedObjectAssociationSurface nakedObjectAssociationSurface) {
            return nakedObjectAssociationSurface.GetScalarProperty<string>(ScalarProperty.Pattern);
        }

        public static bool IsMandatory(this INakedObjectAssociationSurface nakedObjectAssociationSurface) {
            return nakedObjectAssociationSurface.GetScalarProperty<bool>(ScalarProperty.IsMandatory);
        }

        public static string Name(this INakedObjectAssociationSurface nakedObjectAssociationSurface) {
            return nakedObjectAssociationSurface.GetScalarProperty<string>(ScalarProperty.Name);
        }

        public static string Description(this INakedObjectAssociationSurface nakedObjectAssociationSurface) {
            return nakedObjectAssociationSurface.GetScalarProperty<string>(ScalarProperty.Description);
        }

        public static bool IsCollection(this INakedObjectAssociationSurface nakedObjectAssociationSurface) {
            return nakedObjectAssociationSurface.GetScalarProperty<bool>(ScalarProperty.IsCollection);
        }

        public static bool IsObject(this INakedObjectAssociationSurface nakedObjectAssociationSurface) {
            return nakedObjectAssociationSurface.GetScalarProperty<bool>(ScalarProperty.IsObject);
        }

        public static int MemberOrder(this INakedObjectAssociationSurface nakedObjectAssociationSurface) {
            return nakedObjectAssociationSurface.GetScalarProperty<int>(ScalarProperty.MemberOrder);
        }

        public static bool IsASet(this INakedObjectAssociationSurface nakedObjectAssociationSurface) {
            return nakedObjectAssociationSurface.GetScalarProperty<bool>(ScalarProperty.IsASet);
        }

        public static bool IsInline(this INakedObjectAssociationSurface nakedObjectAssociationSurface) {
            return nakedObjectAssociationSurface.GetScalarProperty<bool>(ScalarProperty.IsInline);
        }

        public static string Mask(this INakedObjectAssociationSurface nakedObjectAssociationSurface) {
            return nakedObjectAssociationSurface.GetScalarProperty<string>(ScalarProperty.Mask);
        }

        public static int AutoCompleteMinLength(this INakedObjectAssociationSurface nakedObjectAssociationSurface) {
            return nakedObjectAssociationSurface.GetScalarProperty<int>(ScalarProperty.AutoCompleteMinLength);
        }

        public static bool IsConcurrency(this INakedObjectAssociationSurface nakedObjectAssociationSurface) {
            return nakedObjectAssociationSurface.GetScalarProperty<bool>(ScalarProperty.IsConcurrency);
        }

        public static IDictionary<string, object> ExtensionData(this INakedObjectAssociationSurface nakedObjectAssociationSurface) {
            return nakedObjectAssociationSurface.GetScalarProperty<IDictionary<string, object>>(ScalarProperty.ExtensionData);
        }

        #endregion

        #region INakedObjectSpecificationSurface

        public static bool IsParseable(this INakedObjectSpecificationSurface nakedObjectSpecificationSurface) {
            return nakedObjectSpecificationSurface.GetScalarProperty<bool>(ScalarProperty.IsParseable);
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

        public static IDictionary<string, object> ExtensionData(this INakedObjectSpecificationSurface nakedObjectSpecificationSurface) {
            return nakedObjectSpecificationSurface.GetScalarProperty<IDictionary<string, object>>(ScalarProperty.ExtensionData);
        }

        #endregion
    }
}
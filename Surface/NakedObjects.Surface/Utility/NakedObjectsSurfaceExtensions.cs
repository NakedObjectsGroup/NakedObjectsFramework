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
        #region INakedObjectSurface

        public static T GetDomainObject<T>(this INakedObjectSurface nakedObjectSurface) {
            return nakedObjectSurface == null ? default(T) : (T)nakedObjectSurface.Object;
        }

        public static INakedObjectActionSurface MementoAction(this INakedObjectSurface nakedObjectSurface) {
            return nakedObjectSurface.GetScalarProperty<INakedObjectActionSurface>(ScalarProperty.MementoAction);
        }

        public static string EnumIntegralValue(this INakedObjectSurface nakedObjectSurface) {
            return nakedObjectSurface.GetScalarProperty<string>(ScalarProperty.EnumIntegralValue);
        }

        public static bool IsPaged(this INakedObjectSurface nakedObjectSurface) {
            return nakedObjectSurface.GetScalarProperty<bool>(ScalarProperty.IsPaged);
        }

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

        public static string InvariantString(this INakedObjectSurface nakedObjectSurface) {
            return nakedObjectSurface.GetScalarProperty<string>(ScalarProperty.InvariantString);
        }

        public static bool IsViewModelEditView(this INakedObjectSurface nakedObjectSurface) {
            return nakedObjectSurface.GetScalarProperty<bool>(ScalarProperty.IsViewModelEditView);
        }

        public static IDictionary<string, object> ExtensionData(this INakedObjectSurface nakedObjectSurface) {
            return nakedObjectSurface.GetScalarProperty<IDictionary<string, object>>(ScalarProperty.ExtensionData);
        }

        #endregion

        #region INakedObjectActionParameterSurface

        public static bool IsFindMenuEnabled(this INakedObjectActionParameterSurface nakedObjectActionParameterSurface) {
            return nakedObjectActionParameterSurface.GetScalarProperty<bool>(ScalarProperty.IsFindMenuEnabled);
        }

        public static string PresentationHint(this INakedObjectActionParameterSurface nakedObjectActionParameterSurface) {
            return nakedObjectActionParameterSurface.GetScalarProperty<string>(ScalarProperty.PresentationHint);
        }

        public static Tuple<Regex, string> RegEx(this INakedObjectActionParameterSurface nakedObjectActionParameterSurface) {
            return nakedObjectActionParameterSurface.GetScalarProperty<Tuple<Regex, string>>(ScalarProperty.RegEx);
        }

        public static Tuple<IConvertible, IConvertible, bool> Range(this INakedObjectActionParameterSurface nakedObjectActionParameterSurface) {
            return nakedObjectActionParameterSurface.GetScalarProperty<Tuple<IConvertible, IConvertible, bool>>(ScalarProperty.Range);
        }

        public static int NumberOfLines(this INakedObjectActionParameterSurface nakedObjectActionParameterSurface) {
            return nakedObjectActionParameterSurface.GetScalarProperty<int>(ScalarProperty.NumberOfLines);
        }

        public static int Width(this INakedObjectActionParameterSurface nakedObjectActionParameterSurface) {
            return nakedObjectActionParameterSurface.GetScalarProperty<int>(ScalarProperty.Width);
        }

        public static int TypicalLength(this INakedObjectActionParameterSurface nakedObjectActionParameterSurface) {
            return nakedObjectActionParameterSurface.GetScalarProperty<int>(ScalarProperty.TypicalLength);
        }

        public static bool IsAjax(this INakedObjectActionParameterSurface nakedObjectActionParameterSurface) {
            return nakedObjectActionParameterSurface.GetScalarProperty<bool>(ScalarProperty.IsAjax);
        }

        public static bool IsNullable(this INakedObjectActionParameterSurface nakedObjectActionParameterSurface) {
            return nakedObjectActionParameterSurface.GetScalarProperty<bool>(ScalarProperty.IsNullable);
        }

        public static bool IsPassword(this INakedObjectActionParameterSurface nakedObjectActionParameterSurface) {
            return nakedObjectActionParameterSurface.GetScalarProperty<bool>(ScalarProperty.IsPassword);
        }

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

        public static string PresentationHint(this INakedObjectActionSurface nakedObjectActionSurface) {
            return nakedObjectActionSurface.GetScalarProperty<string>(ScalarProperty.PresentationHint);
        }

        public static int PageSize(this INakedObjectActionSurface nakedObjectActionSurface) {
            return nakedObjectActionSurface.GetScalarProperty<int>(ScalarProperty.PageSize);
        }

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

        public static Tuple<bool, string[]> TableViewData(this INakedObjectActionSurface nakedObjectActionSurface) {
            return nakedObjectActionSurface.GetScalarProperty<Tuple<bool, string[]>>(ScalarProperty.TableViewData);
        }

        public static bool RenderEagerly(this INakedObjectActionSurface nakedObjectActionSurface) {
            return nakedObjectActionSurface.GetScalarProperty<bool>(ScalarProperty.RenderEagerly);
        }

        #endregion

        #region INakedObjectAssociationSurface

        public static bool IsFile(this INakedObjectAssociationSurface nakedObjectAssociationSurface) {
            return nakedObjectAssociationSurface.GetScalarProperty<bool>(ScalarProperty.IsFileAttachment);
        }

        public static bool IsEnum(this INakedObjectAssociationSurface nakedObjectAssociationSurface) {
            return nakedObjectAssociationSurface.GetScalarProperty<bool>(ScalarProperty.IsEnum);
        }

        public static Tuple<Regex, string> RegEx(this INakedObjectAssociationSurface nakedObjectAssociationSurface) {
            return nakedObjectAssociationSurface.GetScalarProperty<Tuple<Regex, string>>(ScalarProperty.RegEx);
        }

        public static Tuple<IConvertible, IConvertible, bool> Range(this INakedObjectAssociationSurface nakedObjectAssociationSurface) {
            return nakedObjectAssociationSurface.GetScalarProperty<Tuple<IConvertible, IConvertible, bool>>(ScalarProperty.Range);
        }

        public static bool IsFindMenuEnabled(this INakedObjectAssociationSurface nakedObjectAssociationSurface) {
            return nakedObjectAssociationSurface.GetScalarProperty<bool>(ScalarProperty.IsFindMenuEnabled);
        }

        public static bool IsAjax(this INakedObjectAssociationSurface nakedObjectAssociationSurface) {
            return nakedObjectAssociationSurface.GetScalarProperty<bool>(ScalarProperty.IsAjax);
        }

        public static bool IsNullable(this INakedObjectAssociationSurface nakedObjectAssociationSurface) {
            return nakedObjectAssociationSurface.GetScalarProperty<bool>(ScalarProperty.IsNullable);
        }

        public static bool IsPassword(this INakedObjectAssociationSurface nakedObjectAssociationSurface) {
            return nakedObjectAssociationSurface.GetScalarProperty<bool>(ScalarProperty.IsPassword);
        }

        public static bool DoNotCount(this INakedObjectAssociationSurface nakedObjectAssociationSurface) {
            return nakedObjectAssociationSurface.GetScalarProperty<bool>(ScalarProperty.DoNotCount);
        }

        public static bool RenderEagerly(this INakedObjectAssociationSurface nakedObjectAssociationSurface) {
            return nakedObjectAssociationSurface.GetScalarProperty<bool>(ScalarProperty.RenderEagerly);
        }

        public static int NumberOfLines(this INakedObjectAssociationSurface nakedObjectAssociationSurface) {
            return nakedObjectAssociationSurface.GetScalarProperty<int>(ScalarProperty.NumberOfLines);
        }

        public static int Width(this INakedObjectAssociationSurface nakedObjectAssociationSurface) {
            return nakedObjectAssociationSurface.GetScalarProperty<int>(ScalarProperty.Width);
        }

        public static int TypicalLength(this INakedObjectAssociationSurface nakedObjectAssociationSurface) {
            return nakedObjectAssociationSurface.GetScalarProperty<int>(ScalarProperty.TypicalLength);
        }

        public static int? MaxLength(this INakedObjectAssociationSurface nakedObjectAssociationSurface) {
            return nakedObjectAssociationSurface.GetScalarProperty<int?>(ScalarProperty.MaxLength);
        }

        public static string PresentationHint(this INakedObjectAssociationSurface nakedObjectAssociationSurface) {
            return nakedObjectAssociationSurface.GetScalarProperty<string>(ScalarProperty.PresentationHint);
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

        public static Tuple<bool, string[]> TableViewData(this INakedObjectAssociationSurface nakedObjectAssociationSurface) {
            return nakedObjectAssociationSurface.GetScalarProperty<Tuple<bool, string[]>>(ScalarProperty.TableViewData);
        }


        #endregion

        #region INakedObjectSpecificationSurface

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
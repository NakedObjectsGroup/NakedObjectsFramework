// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;

namespace NakedObjects.ParallelReflect.FacetFactory {
    /// <summary>
    ///     This class is not referenced and should not be used directly. It has been added back in
    ///     for backwards-compatibility only. Each constant delegates to the newer implementation
    ///     in NakedObjects.Types
    /// </summary>
    [Obsolete("Use NakedObjects.RecognisedMethodsAndPrefixes")]
    public static class PrefixesAndRecognisedMethods {
        public static readonly string AutoCompletePrefix;
        public static readonly string ChoicesPrefix;
        public static readonly string ClearPrefix;
        public static readonly string CreatedMethod;
        public static readonly string DefaultPrefix;
        public static readonly string DeletedMethod;
        public static readonly string DeletingMethod;
        public static readonly string DisablePrefix;
        public static readonly string GetEnumeratorMethod;
        public static readonly string HidePrefix;
        public static readonly string IconNameMethod;
        public static readonly string LoadedMethod;
        public static readonly string LoadingMethod;
        public static readonly string MenuMethod;
        public static readonly string ModifyPrefix;
        public static readonly string ParameterDefaultPrefix;
        public static readonly string ParameterChoicesPrefix;
        public static readonly string PersistedMethod;
        public static readonly string PersistingMethod;
        public static readonly string TitleMethod;
        public static readonly string ToStringMethod;
        public static readonly string UpdatedMethod;
        public static readonly string UpdatingMethod;
        public static readonly string ValidatePrefix;
        public static readonly string OnPersistingErrorMethod;
        public static readonly string OnUpdatingErrorMethod;

        static PrefixesAndRecognisedMethods() {
            ParameterDefaultPrefix = RecognisedMethodsAndPrefixes.ParameterDefaultPrefix;
            ParameterChoicesPrefix = RecognisedMethodsAndPrefixes.ParameterChoicesPrefix;
            ModifyPrefix = RecognisedMethodsAndPrefixes.ModifyPrefix;
            ValidatePrefix = RecognisedMethodsAndPrefixes.ValidatePrefix;
            ChoicesPrefix = RecognisedMethodsAndPrefixes.ChoicesPrefix;
            AutoCompletePrefix = RecognisedMethodsAndPrefixes.AutoCompletePrefix;
            DefaultPrefix = RecognisedMethodsAndPrefixes.DefaultPrefix;
            DisablePrefix = RecognisedMethodsAndPrefixes.DisablePrefix;
            HidePrefix = RecognisedMethodsAndPrefixes.HidePrefix;
            CreatedMethod = RecognisedMethodsAndPrefixes.CreatedMethod;
            DeletedMethod = RecognisedMethodsAndPrefixes.DeletedMethod;
            DeletingMethod = RecognisedMethodsAndPrefixes.DeletingMethod;
            LoadedMethod = RecognisedMethodsAndPrefixes.LoadedMethod;
            LoadingMethod = RecognisedMethodsAndPrefixes.LoadingMethod;
            PersistedMethod = RecognisedMethodsAndPrefixes.PersistedMethod;
            PersistingMethod = RecognisedMethodsAndPrefixes.PersistingMethod;
            UpdatedMethod = RecognisedMethodsAndPrefixes.UpdatedMethod;
            UpdatingMethod = RecognisedMethodsAndPrefixes.UpdatingMethod;
            IconNameMethod = RecognisedMethodsAndPrefixes.IconNameMethod;
            MenuMethod = RecognisedMethodsAndPrefixes.MenuMethod;
            TitleMethod = RecognisedMethodsAndPrefixes.TitleMethod;
            ToStringMethod = RecognisedMethodsAndPrefixes.ToStringMethod;
            GetEnumeratorMethod = RecognisedMethodsAndPrefixes.GetEnumeratorMethod;
            OnPersistingErrorMethod = RecognisedMethodsAndPrefixes.OnPersistingErrorMethod;
            OnUpdatingErrorMethod = RecognisedMethodsAndPrefixes.OnUpdatingErrorMethod;
        }
    }
}
// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

namespace NakedObjects {
    /// <summary>
    /// Defines all the domain model methods recognised by NakedObjects, and the prefixes for 'complementary' methods
    /// (associated with an action or property).
    /// </summary>
    public static class RecognisedMethodsAndPrefixes {
        #region Standard methods ignored as actions

        public static readonly string GetEnumeratorMethod;

        #endregion

        //Defines methods in domain models that are recognised by the NOF and result in
        //specific behaviour. (See also OtherIgnoredMethods).
        public static readonly string TitleMethod;
        public static readonly string ToStringMethod;
        public static readonly string MenuMethod;
        public static readonly string IconNameMethod;
        public static readonly string CreatedMethod;
        public static readonly string DeletedMethod;
        public static readonly string DeletingMethod;
        public static readonly string LoadedMethod;
        public static readonly string LoadingMethod;
        public static readonly string PersistedMethod;
        public static readonly string PersistingMethod;
        public static readonly string UpdatedMethod;
        public static readonly string UpdatingMethod;
        public static readonly string OnPersistingErrorMethod;
        public static readonly string OnUpdatingErrorMethod;

        public static string[] RecognisedMethods = new string[] {
            CreatedMethod,
            DeletedMethod,
            DeletingMethod,
            IconNameMethod,
            LoadedMethod,
            LoadingMethod,
            MenuMethod,
            OnPersistingErrorMethod,
            OnUpdatingErrorMethod,
            PersistedMethod,
            PersistingMethod,
            TitleMethod,
            ToStringMethod,
            UpdatedMethod,
            UpdatingMethod
        };

        //Defines the prefixes recognised by NOF for 'complementary' methods associated
        //with an action or property.
        public static readonly string AutoCompletePrefix;
        public static readonly string ChoicesPrefix;
        public static readonly string ClearPrefix;
        public static readonly string DefaultPrefix;
        public static readonly string DisablePrefix;
        public static readonly string HidePrefix;
        public static readonly string ModifyPrefix;
        public static readonly string ParameterDefaultPrefix;
        public static readonly string ParameterChoicesPrefix;
        public static readonly string ValidatePrefix;

        public static string[] RecognisedPrefixes = new string[] {
            AutoCompletePrefix,
            ChoicesPrefix,
            ClearPrefix,
            DefaultPrefix,
            DisablePrefix,
            HidePrefix,
            ModifyPrefix,
            ParameterChoicesPrefix,
            ParameterDefaultPrefix,
            ValidatePrefix
        };

        //Defines any other methods (not included in AllRecognisedMethods) that are
        //recognised by the NOF but solely for the purpose of not rendering them
        //as user-visible actions.
        public static string[] OtherIgnoredMethods = new string[] {
            GetEnumeratorMethod
        };

        #region Value definitions

        static RecognisedMethodsAndPrefixes() {
            ParameterDefaultPrefix = "Default";
            ParameterChoicesPrefix = "Choices";
            ClearPrefix = "Clear";
            ModifyPrefix = "Modify";
            ValidatePrefix = "Validate";
            ChoicesPrefix = "Choices";
            AutoCompletePrefix = "AutoComplete";
            DefaultPrefix = "Default";
            DisablePrefix = "Disable";
            HidePrefix = "Hide";
            CreatedMethod = "Created";
            DeletedMethod = "Deleted";
            DeletingMethod = "Deleting";
            LoadedMethod = "Loaded";
            LoadingMethod = "Loading";
            PersistedMethod = "Persisted";
            PersistingMethod = "Persisting";
            UpdatedMethod = "Updated";
            UpdatingMethod = "Updating";
            IconNameMethod = "IconName";
            MenuMethod = "Menu";
            TitleMethod = "Title";
            ToStringMethod = "ToString";
            GetEnumeratorMethod = "GetEnumerator";
            OnPersistingErrorMethod = "OnPersistingError";
            OnUpdatingErrorMethod = "OnUpdatingError";
        }

        #endregion

        #region Type-level methods

        #endregion

        #region LifeCycle methods

        #endregion

        #region Complementary methods for actions or properties

        #endregion
    }
}
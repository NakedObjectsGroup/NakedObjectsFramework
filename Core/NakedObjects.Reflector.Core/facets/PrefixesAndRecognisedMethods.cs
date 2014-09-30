// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

namespace NakedObjects.Reflector.DotNet {
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
        public static readonly string ModifyPrefix;
        public static readonly string ParameterDefaultPrefix;
        public static readonly string ParameterChoicesPrefix;
        public static readonly string PersistedMethod;
        public static readonly string PersistingMethod;
        public static readonly string SavedMethod;
        public static readonly string SavingMethod;
        public static readonly string TitleMethod;
        public static readonly string ToStringMethod;
        public static readonly string UpdatedMethod;
        public static readonly string UpdatingMethod;
        public static readonly string ValidatePrefix;
        public static readonly string OnPersistingErrorMethod;
        public static readonly string OnUpdatingErrorMethod;

        static PrefixesAndRecognisedMethods() {
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
            SavedMethod = "Saved";
            SavingMethod = "Saving";
            UpdatedMethod = "Updated";
            UpdatingMethod = "Updating";
            IconNameMethod = "IconName";
            TitleMethod = "Title";
            ToStringMethod = "ToString";
            GetEnumeratorMethod = "GetEnumerator";
            OnPersistingErrorMethod = "OnPersistingError";
            OnUpdatingErrorMethod = "OnUpdatingError";
        }
    }
}
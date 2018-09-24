namespace NakedObjects
{
    /// <summary>
    /// TODO: This is due to be moved into the NakedObjects.Types and hence be part
    /// of the programming model package.
    /// Defines all the domain model methods recognised by NakedObjects, and the prefixes for 'complementary' methods
    /// (associated with an action or property).
    /// </summary>
    public static class RecognisedMethodsAndPrefixes
    {

        #region Type-level methods
        public static readonly string TitleMethod;
        public static readonly string ToStringMethod;
        public static readonly string MenuMethod;
        public static readonly string IconNameMethod;
        #endregion

        #region LifeCycle methods
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
        #endregion

        #region Complementary methods for actions or properties
        public static readonly string AutoCompletePrefix;
        public static readonly string ChoicesPrefix;
        public static readonly string DefaultPrefix;
        public static readonly string DisablePrefix;
        public static readonly string HidePrefix;
        public static readonly string ModifyPrefix;
        public static readonly string ParameterDefaultPrefix;
        public static readonly string ParameterChoicesPrefix;
        public static readonly string ValidatePrefix;
        #endregion

        #region Standard methods ignored as actions
        public static readonly string GetEnumeratorMethod;
        #endregion

        //Defines methods in domain models that are recognised by the NOF and result in
        //specific behaviour. (See also OtherIgnoredMethods).
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
        public static string[] RecognisedPrefixes = new string[] {
                AutoCompletePrefix,
                ChoicesPrefix,
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
        static RecognisedMethodsAndPrefixes()
        {
            ParameterDefaultPrefix = "Default";
            ParameterChoicesPrefix = "Choices";
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
    }
}

namespace NakedLegacy.Types {
    //[JavaInterface]
    public interface FieldAbout : IAbout {

        bool IsPersistent { get; set; }  //TODO: Current API 'void nonPersistent()' implies this should be settable, but why?
        object[] Options { get; set; }

        bool IsValid { get; set; }
        string InvalidReason { get; set; }

    }
}
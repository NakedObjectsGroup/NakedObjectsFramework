namespace NakedLegacy.Types {
    public interface IAbout {
        bool Usable { get; set; }
        bool Visible { get; set; }

        AboutTypeCodes TypeCode { get; }
    }
}
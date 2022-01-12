namespace NakedLegacy;

public interface IAbout {
    AboutTypeCodes TypeCode { get; }

    string Name { get; set; }
    string Description { get; set; }

    bool Visible { get; set; }

    bool Usable { get; set; }
    string UnusableReason { get; set; }
}
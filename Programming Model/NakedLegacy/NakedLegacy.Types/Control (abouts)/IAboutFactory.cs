namespace NakedLegacy;

public interface IAboutFactory {
    public ActionAbout NewActionAbout(AboutTypeCodes code);

    public FieldAbout NewFieldAbout(AboutTypeCodes code);
}
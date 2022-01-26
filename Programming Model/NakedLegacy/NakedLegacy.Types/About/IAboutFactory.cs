namespace NakedLegacy.About;

public interface IAboutFactory {
    public ActionAbout NewActionAbout(AboutTypeCodes code);

    public FieldAbout NewFieldAbout(AboutTypeCodes code);
}
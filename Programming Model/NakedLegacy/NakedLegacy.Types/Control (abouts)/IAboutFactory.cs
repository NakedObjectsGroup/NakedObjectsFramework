namespace NakedLegacy;

public interface IAboutFactory {
    public IActionAbout NewActionAbout(AboutTypeCodes code);

    public IFieldAbout NewFieldAbout(AboutTypeCodes code);
}
namespace NakedLegacy.Reflector.Component; 

public class AboutFactory : IAboutFactory {
    public IActionAbout NewActionAbout(AboutTypeCodes code) => new ActionAboutImpl(code);

    public IFieldAbout NewFieldAbout(AboutTypeCodes code) => new FieldAboutImpl(code);
}
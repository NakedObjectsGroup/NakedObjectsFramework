using NakedLegacy.About;

namespace NakedLegacy.Reflector.Component;

public class AboutFactory : IAboutFactory {
    public ActionAbout NewActionAbout(AboutTypeCodes code) => new ActionAboutImpl(code);

    public FieldAbout NewFieldAbout(AboutTypeCodes code) => new FieldAboutImpl(code);
}
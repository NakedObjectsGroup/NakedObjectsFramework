using NOF2.About;

namespace NOF2.Reflector.Component;

public class AboutFactory : IAboutFactory {
    public ActionAbout NewActionAbout(AboutTypeCodes code) => new ActionAboutImpl(code);

    public FieldAbout NewFieldAbout(AboutTypeCodes code) => new FieldAboutImpl(code);
}
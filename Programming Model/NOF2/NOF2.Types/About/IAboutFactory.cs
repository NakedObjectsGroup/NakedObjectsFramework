namespace NOF2.About {
    public interface IAboutFactory {
        ActionAbout NewActionAbout(AboutTypeCodes code);

        FieldAbout NewFieldAbout(AboutTypeCodes code);
    }
}
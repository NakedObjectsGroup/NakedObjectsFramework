namespace Legacy.Types {
    public class FieldAboutImpl : FieldAbout {
        public FieldAboutImpl(AboutTypeCodes typeCode) => TypeCode = typeCode;

        public AboutTypeCodes TypeCode { get; }

        public bool Usable { get; set; } = true;
        public bool Visible { get; set; } = true;
    }
}
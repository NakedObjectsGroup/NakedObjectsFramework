namespace Template.Test.Helpers {
    public record Key(string Type, string Id);

    public record Key<T>(string Id) : Key(TestHelpers.FullName<T>(), Id);
}
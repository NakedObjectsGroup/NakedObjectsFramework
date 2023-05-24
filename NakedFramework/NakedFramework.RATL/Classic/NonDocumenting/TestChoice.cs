namespace NakedFramework.RATL.Classic.NonDocumenting;

internal class TestChoice : TestValue {
    public TestChoice(object value, string title) : base(value) => Title = title;

    public override string Title { get; }
}
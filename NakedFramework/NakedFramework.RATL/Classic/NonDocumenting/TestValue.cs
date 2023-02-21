using NakedFramework.RATL.Classic.Interface;

namespace NakedFramework.RATL.Classic.NonDocumenting;

internal class TestValue : ITestValue {
    public TestValue(object value) => Value = value;

    public object Value { get; }
    public string Title => Value.ToString();
}
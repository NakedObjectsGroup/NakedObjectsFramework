namespace NakedLegacy;

public interface IValueHolder<T> : ITitledObject {
    public T Value { get; set; }

    public string Mask { get; }

    public string ToString();

    public object Parse(string fromString);

    public object Display(string mask = null);
}


using System.Globalization;

namespace AdventureWorksLegacy.AppLib;

public abstract class ValueHolder<T> : IValueHolder<T>, ITitledObject {
    private T value;

    protected ValueHolder() { }

    protected ValueHolder(T value) => this.value = value;

    protected ValueHolder(T value, Action<T> callback) : this(value) => UpdateBackingField = callback;

    public T Value {
        get => value;
        set {
            this.value = value;
            UpdateBackingField(value);
        }
    }

    private Action<T> UpdateBackingField { get; } = _ => { };

    public virtual string Mask => null;

    public abstract Title Title();

    public override string ToString() => Value.ToString();

    public abstract object Parse(string fromString);

    public abstract object Display(string mask = null);

    protected CultureInfo Culture = new CultureInfo("en-GB");
}
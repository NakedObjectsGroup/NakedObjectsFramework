

using System.Globalization;

namespace AdventureWorksLegacy.AppLib;

public abstract class ValueHolder<T> : IValueHolder<T>, ITitledObject {
    private T value;

    protected bool IsNull = true;

    protected ValueHolder() { }

    protected ValueHolder(T value) => this.value = value;

    protected ValueHolder(T value, Action<T> callback) : this(value) => UpdateBackingField = callback;

    public T Value {
        get => value;
        set {
            this.value = value;
            UpdateBackingField(value);
            IsNull = value == null;
        }
    }

    private Action<T> UpdateBackingField { get; } = _ => { };

    public virtual string Mask => null;

    public override string ToString() => Value.ToString();

    public abstract object Parse(string fromString);

    public virtual object Display(string mask = null) => IsNull ? "" : Value;

    protected CultureInfo Culture = new CultureInfo("en-GB");

    public bool IsEmpty() => IsNull;

    public virtual Title Title() => new Title(ToString());
}
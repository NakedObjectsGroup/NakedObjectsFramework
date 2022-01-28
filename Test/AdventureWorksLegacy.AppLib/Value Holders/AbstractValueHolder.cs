

using System.Globalization;

namespace AdventureWorks.NOF2.AppLib;

public abstract class AbstractValueHolder<T> : IValueHolder<T>, ITitledObject {
    private T value;

    protected bool IsNull;

    protected AbstractValueHolder() {
        IsNull = true;
    }

    protected AbstractValueHolder(T value)
    {
        this.value = value;
        IsNull = value == null;
    }

    protected AbstractValueHolder(T value, Action<T> callback) : this(value) => UpdateBackingField = callback;

    //Used only for initialzing property on a transient object.
    protected AbstractValueHolder(Action<T> callback) : this() => UpdateBackingField = callback;

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
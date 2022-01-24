using System;

namespace NakedLegacy.Rest.Test.Data.AppLib;

public abstract class ValueHolder<T> : IValueHolder<T>, ITitledObject {
    private T value;

    protected ValueHolder() { }

    protected ValueHolder(T value) => this.value = value;

    protected ValueHolder(T value, Action<T> callback) : this(value) => UpdateBackingField = callback;

    private Action<T> UpdateBackingField { get; } = _ => { };

    public abstract ITitle Title();

    public T Value {
        get => value;
        set {
            this.value = value;
            UpdateBackingField(value);
        }
    }

    public virtual string Mask => null;

    public override string ToString() => Value.ToString();

    public abstract object Parse(string fromString);

    public abstract object Display(string mask = null);
}
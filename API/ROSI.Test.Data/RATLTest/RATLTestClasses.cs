using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NakedFramework.Menu;
using NakedObjects;

namespace ROSI.Test.Data;

public enum TestEnum1 {
    Value1,
    Value2
}

public class Service1 {
    public IDomainObjectContainer Container { private get; set; }
    public IQueryable<Object1> GetClasses() => Container.Instances<Object1>();

    [QueryOnly]
    public Object1 GetTransient() => Container.NewTransientInstance<Object1>();
}

public class Object1 {
    [Key]
    public int Id { get; set; }

    [DefaultValue(8)]
    public int Prop1 { get; set; }

    [DisplayName("Foo")]
    public string Prop2 { get; set; }

    [Mask("d")]
    public DateTime Prop3 { get; set; } = new(2013, 8, 16);

    [Hidden]
    public string Foo { get; set; }

    public string Title() => "FooBar";

    public Object1 DoSomething([DefaultValue(8)] int param0, [DefaultValue("Foo")] string param1) =>
        param0 switch {
            0 => null,
            _ => this
        };

    public Object1 DoSomethingElse([DefaultValue(TestEnum1.Value2)] TestEnum1 param0, TestEnum1 param1) => null;

    public string DoReturnString() => "a string";

    public Object1 DoSomethingOnMenu([DefaultValue(8)] int param0, [DefaultValue("Foo")] string param1) => null;

    public Object1 DoSomethingOnSubMenu() => null;

    public static void Menu(IMenu menu) {
        var sub = menu.CreateSubMenu("Sub1");
        sub.AddAction(nameof(DoSomethingOnMenu));
        var subSub = sub.CreateSubMenu("SubSub");
        subSub.AddAction(nameof(DoSomethingOnSubMenu));
        menu.AddRemainingNativeActions();
    }
}

[DescribedAs("an object")]
public class Object2 {
    public IDomainObjectContainer Container { protected get; set; }

    [Key]
    public int Id { get; set; }

    [Disabled]
    public void AlwaysDisabled() { }

    public Object1 WithRefParam(Object1 refParam) => refParam;

    public Object1[] Choices0WithRefParam() => Container.Instances<Object1>().ToArray();

    public IQueryable<Object2> ReturnCollection([DefaultValue(8)] int param0, [DefaultValue("Foo")] string param1) => Container.Instances<Object2>().Take(param0);

    public int[] Choices0ReturnCollection() => new[] { 0, 1, 2 };

    [DescribedAs("Does nothing")]
    public void ReturnVoid() { }
}
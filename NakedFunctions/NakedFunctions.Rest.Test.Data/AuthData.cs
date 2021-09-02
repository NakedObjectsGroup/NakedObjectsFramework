namespace NakedFunctions.Rest.Test.Data {
    public record Foo {
        public virtual int Id { get; set; }

        public virtual string Prop1 { get; set; }

        public override string ToString() => "foo1";
    }

    public record Bar {
        public virtual int Id { get; set; }

        public virtual string Prop1 { get; set; }
    }

    public record Qux {
        public virtual int Id { get; set; }

        public virtual string Prop1 { get; set; }

        public override string ToString() => "qux1";
    }

    public record FooSub : Foo {
        public virtual string Prop2 { get; set; }
    }

    public record SubTypeOfFoo : Foo {
        public virtual string Prop2 { get; set; }
    }

    public static class BarFunctions {
        public static Bar Act1(this Bar bar, IContext context) => bar;
    }
}
﻿using System.Linq;
using NakedFunctions;

// non NakedFunctions namespace for I18N tests
namespace Rest.Test.Data {
    [DescribedAs("Foo")]
    public record Foo {
        public virtual int Id { get; set; }

        [DescribedAs("Prop1")]
        public virtual string Prop1 { get; set; }

        public override string ToString() => "foo1";
    }

    public record AuditRecord {
        public virtual int Id { get; set; }
        public virtual string Message { get; set; }
    }

    public record Bar {
        public virtual int Id { get; set; }

        public virtual string Prop1 { get; set; }
    }

    public record FooSub : Foo {
        public virtual string Prop2 { get; set; }
    }

    public static class BarFunctions {
        public static Bar Act1(this Bar bar, IContext context) => bar;
    }

    [DescribedAs("FooFunctions")]
    public static class FooFunctions {
        [DescribedAs("Act1")]
        public static Foo Act1(this Foo foo, IContext context) => foo;
        public static IContext QueryableAct(this IQueryable<Foo> foos, IContext context) => context;
    }

    public static class FooSubFunctions {
        public static FooSub Act2(this FooSub foosub, IContext context) => foosub;
    }

    [DescribedAs("FooMenuFunctions")]
    public static class FooMenuFunctions {
        [DescribedAs("Act1")]
        public static Foo Act1(IContext context) => context.Instances<Foo>().FirstOrDefault();

        public static Foo Act2(IContext context) => context.Instances<Foo>().FirstOrDefault();
    }
}

namespace Rest.Test.Data.Sub {
    public record Qux {
        public virtual int Id { get; set; }

        public virtual string Prop1 { get; set; }

        public override string ToString() => "qux1";
    }

    public static class QuxFunctions {
        public static Qux Act1(this Qux qux, IContext context) => qux;
    }

    public static class QuxMenuFunctions {
        public static Foo Act1(IContext context) => context.Instances<Foo>().FirstOrDefault();
        public static Foo Act2(IContext context) => context.Instances<Foo>().FirstOrDefault();
        public static IQueryable<Foo> Act3(IContext context) => context.Instances<Foo>();
    }
}
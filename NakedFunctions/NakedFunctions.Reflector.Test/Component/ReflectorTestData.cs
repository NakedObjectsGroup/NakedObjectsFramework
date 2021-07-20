// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using NakedFramework.Menu;

namespace NakedFunctions.Reflector.Test.Component {
    public class NullMenuFactory : IMenuFactory {
        public IMenu NewMenu(string name) => null;

        #region IMenuFactory Members

        public IMenu NewMenu<T>(bool addAllActions, string name = null) => null;

        public IMenu NewMenu(Type type, bool addAllActions = false, string name = null) => null;

        public IMenu NewMenu(string name, string id) => null;

        public IMenu NewMenu(string name, string id, Type defaultType, bool addAllActions = false) => null;

        #endregion
    }

    public static class PageSizeFunctions {
        [PageSize(66)]
        public static SimpleClass PageSizeFunction(this SimpleClass dac, IContext context) => dac;
    }

    public static class PasswordFunctions {
        public static SimpleClass PasswordFunction(this SimpleClass dac, [Password] string parm, IContext context) => dac;
    }

    [Plural("Class Plural")]
    public record PluralClass;

    [DescribedAs("Class Description")]
    public record DescribedAsClass {
        [DescribedAs("Property Description")]
        public string DescribedProperty { get; init; }
    }

    public static class DescribedAsFunctions {
        [DescribedAs("Function Description")]
        public static DescribedAsClass DescribedAsFunction(this DescribedAsClass dac, IContext context) => dac;
    }

    [RenderEagerly]
    public record RenderEagerlyClass {
        [RenderEagerly]
        public string RenderEagerlyProperty { get; init; }
    }

    public static class RenderEagerlyFunctions {
        [RenderEagerly]
        public static RenderEagerlyClass RenderEagerlyFunction(this RenderEagerlyClass dac, IContext context) => dac;
    }

    public record TableViewClass {
        [TableView(true)]
        public IList<TableViewClass> TableViewProperty { get; init; }
    }

    public static class TableViewFunctions {
        [TableView(true)]
        public static IQueryable<TableViewClass> TableViewFunction(this TableViewClass dac, IContext context) => new[] {dac}.AsQueryable();

        [TableView(true)]
        public static (IQueryable<TableViewClass>, IContext) TableViewFunction1(this TableViewClass dac, IContext context) => (new[] {dac}.AsQueryable(), context);
    }

    //[Mask("Class Mask")] (currently) allowed only on property
    public record MaskClass {
        [Mask("Property Mask")]
        public string MaskProperty { get; init; }
    }

    public static class MaskFunctions {
        //[Mask("Function Mask")] (currently) allowed only on property
        public static MaskClass MaskFunction(this MaskClass dac, /*[Mask("Parameter Mask")]*/ string parm, IContext context) => dac;
    }

    public static class ContributedCollectionFunctions {
        public static IContext ContributedCollectionFunction(this IQueryable<SimpleClass> c, IContext context) => context;
    }

    public record OptionallyClass {
        //[Optionally]  cannot (currently) be applied to a property
        public string OptionallyProperty { get; init; }

        public string NotOptionallyProperty { get; init; }
    }

    public static class OptionallyFunctions {
        public static OptionallyClass OptionallyFunction(this OptionallyClass dac, [Optionally] string parm1, string parm2, IContext context) => dac;
    }

    [Named("Class Name")]
    public record NamedClass {
        [Named("Property Name")]
        public string NamedProperty { get; init; }
    }

    public static class NamedFunctions {
        [Named("Function Name")]
        public static NamedClass NamedFunction(this NamedClass dac, [Named("Parameter Name")] string parm, IContext context) => dac;
    }

    public record RegexClass {
        public string RegexProperty { get; init; }
    }

    public static class RegexFunctions {
        public static string RegexFunction(this RegexClass dac, [RegEx("Parameter Regex")] string parm, IContext context) => "";
    }

    [PresentationHint("Class Hint")]
    public record HintClass {
        [PresentationHint("Property Hint")]
        public string HintProperty { get; init; }
    }

    public static class HintFunctions {
        [PresentationHint("Function Hint")]
        public static HintClass HintFunction(this HintClass dac, [PresentationHint("Parameter Hint")] string parm, IContext context) => dac;
    }

    public record MultilineClass {
        [MultiLine(1, 2)]
        public string MultilineProperty { get; init; }
    }

    public static class MultiLineFunctions {
        [MultiLine(3, 4)]
        public static MultilineClass MaskFunction(this MultilineClass dac, [MultiLine(5, 6)] string parm, IContext context) => dac;
    }

    public record OrderClass {
        [MemberOrder("Property Order", 0)]
        public string OrderProperty { get; init; }

        [MemberOrder("Collection Order", 1)]
        public IList<OrderClass> OrderList { get; init; }
    }

    public static class OrderFunctions {
        [MemberOrder("Function Order", 2)]
        public static OrderClass MaskFunction(this OrderClass dac, IContext context) => dac;
    }

    public record HiddenClass {
        [Hidden]
        public string HiddenProperty { get; init; }

        public string HiddenPropertyViaFunction { get; init; }
    }

    public record VersionedClass {
        [Versioned]
        public string VersionedProperty { get; init; }
    }

    [Bounded]
    public record BoundedClass;

    public record IgnoredClass {
        internal virtual string IgnoredProperty { get; init; }
    }

    public static class ParameterDefaultClass {
        public static SimpleClass ParameterWithBoolDefaultFunction(this SimpleClass target, [DefaultValue(true)] bool parameter) => target;
        public static SimpleClass ParameterWithByteDefaultFunction(this SimpleClass target, [DefaultValue((byte) 66)] byte parameter) => target;
        public static SimpleClass ParameterWithCharDefaultFunction(this SimpleClass target, [DefaultValue('g')] char parameter) => target;
        public static SimpleClass ParameterWithDoubleDefaultFunction(this SimpleClass target, [DefaultValue(56.23)] double parameter) => target;
        public static SimpleClass ParameterWithFloatDefaultFunction(this SimpleClass target, [DefaultValue((float) 22.82)] float parameter) => target;
        public static SimpleClass ParameterWithIntDefaultFunction(this SimpleClass target, [DefaultValue(72)] int parameter) => target;
        public static SimpleClass ParameterWithLongDefaultFunction(this SimpleClass target, [DefaultValue((long) 91)] long parameter) => target;
        public static SimpleClass ParameterWithShortDefaultFunction(this SimpleClass target, [DefaultValue((short) 30)] short parameter) => target;
        public static SimpleClass ParameterWithStringDefaultFunction(this SimpleClass target, [DefaultValue("a default")] string parameter) => target;
        public static SimpleClass ParameterWithDateTimeDefaultFunction(this SimpleClass target, [DefaultValue(35)] DateTime parameter) => target;
    }

    public static class RangeClass {
        public static SimpleClass ParameterWithRangeFunction(this SimpleClass target, [ValueRange(1, 56)] int parameter) => target;
    }

    public record SimpleClass {
        public virtual SimpleClass SimpleProperty { get; init; }
    }

    [ViewModel(typeof(ViewModelFunctions))]
    public record ViewModel {
        public virtual SimpleClass SimpleProperty { get; init; }
    }

    public class NavigableClass {
        public SimpleClass SimpleProperty { get; init; }
    }

    public static class SimpleFunctions {
        public static SimpleClass SimpleFunction(this SimpleClass target) => target;

        public static IList<SimpleClass> SimpleFunction1(this SimpleClass target) {
            return new[] {target};
        }
    }

    public static class HideFunctions {
        public static bool HideHiddenPropertyViaFunction(this HiddenClass target) => false;
    }

    public static class PotentFunctions {
        public static SimpleClass QueryFunction(this SimpleClass target, IContext context) => target;

        public static (SimpleClass, IContext) PostFunction(this SimpleClass target, IContext context) => (target, context);

        [Edit]
        public static (SimpleClass, IContext) PutFunction(this SimpleClass target, IContext context) => (target, context);
    }

    public static class SimpleInjectedFunctions {
        public static SimpleClass SimpleInjectedFunction(IQueryable<SimpleClass> injected) => injected.First();
    }

    public static class TupleFunctions {
        public static (SimpleClass, SimpleClass) TupleFunction(IQueryable<SimpleClass> injected) => (injected.First(), injected.First());

        public static (IList<SimpleClass>, IList<SimpleClass>) TupleFunction1(IQueryable<SimpleClass> injected) => (injected.ToList(), injected.ToList());
    }

    public static class UnsupportedTupleFunctions {
        public static ValueTuple TupleFunction(IQueryable<SimpleClass> injected) => new();
    }

    public static class LifeCycleFunctions {
        public static SimpleClass Persisting(this SimpleClass target) => target;
        public static SimpleClass Persisted(this SimpleClass target) => target;
        public static SimpleClass Updating(this SimpleClass target) => target;
        public static SimpleClass Updated(this SimpleClass target) => target;
    }

    public static class ViewModelFunctions {
        public static string[] DeriveKeys(this ViewModel target) => null;
        public static ViewModel CreateFromKeys(string[] keys) => new();
    }

    public static class CreateNewFunctions {
        [CreateNew]
        public static (SimpleClass, IContext) SimpleFunction(this SimpleClass target, SimpleClass simpleProperty,  IContext context) => (target, context);
    }

    public static class DisplayAsPropertyFunctions {
        [DisplayAsProperty]
        public static SimpleClass SimpleFunction(this SimpleClass target) => target;

        [DisplayAsProperty]
        public static IQueryable<SimpleClass> SimpleFunctionCollection(this SimpleClass target, IContext context) => context.Instances<SimpleClass>();
    }

    public record EditClass {
        public virtual SimpleClass SimpleProperty { get; init; }

        public virtual int IntProperty { get; init; }

        public virtual string StringProperty { get; init; }

        public virtual string NotMatchedProperty { get; init; }
    }

    public static class EditClassFunctions {
        [Edit]
        public static IContext EditFunction(this EditClass target, SimpleClass simpleProperty, int intProperty, string stringProperty, IContext context) => context;
    }

    public static class DuplicateFunctions {
        public static SimpleClass Function(this SimpleClass sr) => sr;
        public static SimpleClass Function(this SimpleClass sr, IContext c) => sr;
    }

    public static class DuplicateFunctions1 {
        public static SimpleClass Function(this SimpleClass sr) => sr;
    }

    public static class DuplicateFunctions2 {
        public static SimpleClass Function(this SimpleClass sr) => sr;
    }

    public static class ChoicesClass {
        public static IContext ActionWithChoices(this SimpleClass target, int parm1, string parm2, IContext context) => context;

        public static IList<int> Choices1ActionWithChoices(this SimpleClass target, string parm2) => new List<int> {0};
        public static IList<string> Choices2ActionWithChoices(this SimpleClass target, int parm1) => new List<string> {"0"};


        public static IContext ActionWithMismatchedChoices(this SimpleClass target, int parm1, string parm2, IContext context) => context;

        public static IList<int> Choices1ActionWithMismatchedChoices(this SimpleClass target, string parm3) => new List<int> { 0 };
        public static IList<string> Choices2ActionWithMismatchedChoices(this SimpleClass target, string parm1) => new List<string> { "0" };
    }

    public static class MismatchedTargetClass
    {
        public static IContext ActionWithChoices(this SimpleClass target, int parm1, string parm2, IContext context) => context;

        public static IList<int> Choices1ActionWithChoices(this NavigableClass target, string parm2) => new List<int> { 0 };
    }
}
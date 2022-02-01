namespace Template.AppLib
{
    public static class AppLibConfig
    {
        public static Type[] ValueHolderTypes { get; } = {
            typeof(TextString),
            typeof(Money),typeof(MoneyNullable),
            typeof(Logical),
            typeof(MultiLineTextString),
            typeof(WholeNumber),typeof(WholeNumberNullable),
            typeof(NODate),typeof(NODateNullable),
            typeof(TimeStamp),
            typeof(FloatingPointNumber),typeof(FloatingPointNumberNullable),
            typeof(Percentage)
            };
    }
}

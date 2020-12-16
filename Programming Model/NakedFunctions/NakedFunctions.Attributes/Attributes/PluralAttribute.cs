using System;

namespace NakedFunctions.Attributes
{
    /// <summary>
    ///     Specify the plural form of the object's name
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Where the framework displays a collection of objects it may use the plural form of the object type
    ///         in view. By default the plural name will be created by adding an 's' to the end of the singular name
    ///         (whether that is the class name or another name specified using the Named attribute). Where the single
    ///         name ends in 'y' then the default plural name will end in 'ies' - for example a collection of Country
    ///         objects will be titled Countries. Where these conventions do not work, the programmer may specify
    ///         the plural form of the name using the Plural atttibute
    ///     </para>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class PluralAttribute : Attribute
    {
        public PluralAttribute(string s)
        {
            Value = s;
        }

        public string Value { get; private set; }
    }
}

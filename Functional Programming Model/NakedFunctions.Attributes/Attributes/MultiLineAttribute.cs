using System;

namespace NakedFunctions
{
    /// <summary>
    ///     Specifies that a <see cref="string" /> property or action parameter may contain carriage returns, and an optional default number of lines and width,
    ///     which may be used by the display.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Parameter, Inherited = true, AllowMultiple = false)]
    public class MultiLineAttribute : Attribute
    {
        public MultiLineAttribute() : this(6, 0)
        {
        }

        public MultiLineAttribute(int numberOfLines) : this(numberOfLines, 0)
        {
        }

        public MultiLineAttribute(int numberOfLines, int width)
        {
            NumberOfLines = numberOfLines;
            Width = width;
        }

        public int NumberOfLines { get; set; }

        public int Width { get; set; }
    }
}

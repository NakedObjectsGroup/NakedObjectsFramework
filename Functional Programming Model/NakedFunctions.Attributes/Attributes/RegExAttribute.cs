using System;

namespace NakedFunctions
{
    /// <summary>
    ///     Validate and potentially to normalise the format of the input using supplied regular expression.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         The RegEx attribute may be applied to any property, or to any parameter within an action method,
    ///         that allows the user to type in text as input. It serves both to validate and potentially to
    ///         normalise the format of the input. RegEx is therefore similar in use to Mask but provides more
    ///         flexibility.
    ///     </para>
    ///     <para>
    ///         For example this annotation should validate UK postcodes and format them with a space
    ///         <code>
    /// [RegEx(Validation = @"([A-Z]{1,2}\d[A-Z\d]?).*(\d[ABD-HJLNP-UW-Z]{2})", Format = @"$1 $2")]
    /// </code>
    ///         <para>
    ///             (Regex from Ben Forta, Regular Expressions, SAMS)
    ///         </para>
    ///     </para>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Parameter, Inherited = true, AllowMultiple = false)]
    public class RegExAttribute : Attribute
    {
        public RegExAttribute() : this("") { }
        public RegExAttribute(string format) : this(format, false) { }

        public RegExAttribute(string format, bool caseSensitive)
        {
            CaseSensitive = caseSensitive;
            Format = format;
        }

        /// <summary>
        ///     Validation regular expression string a match is considered success.
        /// </summary>
        public string Validation { get; set; }

        /// <summary>
        ///     Message to display if the validation fails
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        ///     Format regular expression substitution string
        /// </summary>
        /// <para>
        ///     http://msdn.microsoft.com/en-us/library/ewy2t5e0.aspx"/>
        /// </para>
        public string Format { get; set; }

        /// <summary>
        ///     Case sensitivity - defaults to false (non-sensitive)
        /// </summary>
        public bool CaseSensitive { get; set; }
    }
}

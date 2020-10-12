using System;

namespace NakedFunctions
{
    /// <summary>
    ///     Serves to validate, and potentially to normalise, the format of the input. The characters that can
    ///     be used are based on Microsoft's MaskedEdit control
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Parameter, Inherited = true, AllowMultiple = false)]
    public class MaskAttribute : Attribute
    {
        public MaskAttribute(string s)
        {
            Value = s;
        }

        public string Value { get; private set; }
    }
}

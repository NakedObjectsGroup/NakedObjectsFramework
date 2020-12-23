using System;

namespace NakedFunctions
{
    /// <summary>
    /// Provides a statically-defined default value for an action parameter.
    /// A DefaultValue of type Int may be applied to a DateTime parameter, where
    /// the value 0 means 'today', -1 means 'yesterday', 30 means '30 days from today'.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, Inherited = true, AllowMultiple = false)]
    public class DefaultValueAttribute : Attribute
    {
        public object Value { get; private set; }

        public DefaultValueAttribute(bool value)
        {
            Value = value;
        }

        public DefaultValueAttribute(byte value)
        {
            Value = value;
        }

        public DefaultValueAttribute(char value)
        {
            Value = value;
        }


        public DefaultValueAttribute(double value)
        {
            Value = value;
        }

        public DefaultValueAttribute(float value)
        {
            Value = value;
        }

        public DefaultValueAttribute(int value)
        {
            Value = value;
        }

        public DefaultValueAttribute(long value)
        {
            Value = value;
        }

        public DefaultValueAttribute(short value)
        {
            Value = value;
        }

        public DefaultValueAttribute(string value)
        {
            Value = value;
        }
    }
}

﻿using System;

namespace NakedFunctions
{
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
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

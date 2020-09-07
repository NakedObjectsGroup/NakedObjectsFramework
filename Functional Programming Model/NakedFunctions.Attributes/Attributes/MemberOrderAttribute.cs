using System;

namespace NakedFunctions
{
    /// <summary>
    ///     For specifying the order in which properties and/or actions are presented to
    ///     the user.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class MemberOrderAttribute : Attribute
    {
        public MemberOrderAttribute(int sequence, string name)
        {
            Sequence = sequence;
            Name = name;
        }

        public MemberOrderAttribute(int sequence)
        {
            Sequence = sequence;
        }

        public int Sequence { get; private set; }

        public string Name { get; private set; }
    }
}

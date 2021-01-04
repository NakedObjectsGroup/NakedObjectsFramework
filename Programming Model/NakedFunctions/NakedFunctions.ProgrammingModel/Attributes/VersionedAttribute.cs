using System;

namespace NakedFunctions
{
    /// <summary>
    ///     Tells framework that the property (typically a DateTime or an integer) represents
    ///     the version of the instance. This is so that the framework can test that the user is
    ///     making changes to the current version - given that the user view may be minutes, even
    ///     hours, old. This attribute may be applied to the same property that is also used
    ///     for the EntityFramework 'concurrency check' (which may be specified by the 
    ///     System.ComponentModel.DataAnnotations.ConcurrencyCheck attribute, or using the
    ///     fluent data mapping API) - but these are two separate checks, and both are typically .
    ///     necessary in a multi-user system. 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
        public class VersionedAttribute : Attribute { }
}

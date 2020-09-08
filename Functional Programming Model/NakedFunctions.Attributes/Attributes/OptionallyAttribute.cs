using System;

namespace NakedFunctions
{
    /// <summary>
    ///     Used to indicate that an action parameter, is optional. May be applied to properties, but relevant to editable view models only.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         By default, the system assumes that all parameters in an action are required and will not let the
    ///         user execute that action unless values have been specified for each parameter.
    ///     </para>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class OptionallyAttribute : Attribute { }
}

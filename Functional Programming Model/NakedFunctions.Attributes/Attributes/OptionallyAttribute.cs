using System;

namespace NakedFunctions
{
    /// <summary>
    ///     Used to indicate that an action parameter, is optional.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         By default, the system assumes that all parameters in an action are required and will not let the
    ///         user execute that action unless values have been specified for each parameter.
    ///     </para>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Parameter, Inherited = true, AllowMultiple = false)]
    public class OptionallyAttribute : Attribute { }
}

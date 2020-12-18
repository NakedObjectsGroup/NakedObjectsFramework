using System;

namespace NakedFunctions
{
    //Defines a well-known service that may be accessed via the GetService method on IContainer.
    //NakedFunctions provides a default implementation, which may be replaced with a custom one, registered in Services Configuration.
    public interface IGuidGenerator
    {
        public Guid NewGuid();
    }
}

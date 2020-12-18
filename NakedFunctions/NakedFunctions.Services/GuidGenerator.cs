

using System;

namespace NakedFunctions.Services
{
    public class GuidGenerator : IGuidGenerator
    {
        public Guid NewGuid() => Guid.NewGuid();
    }
}

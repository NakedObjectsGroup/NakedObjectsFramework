using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NakedObjects.Surface.Utility {
    public interface IMessageBrokerSurface  {
        string[] PeekMessages { get; }
        string[] PeekWarnings { get; }
        string[] Messages { get; }
        string[] Warnings { get; }
        void AddWarning(string message);
        void AddMessage(string message);
        void ClearWarnings();
        void ClearMessages();
        void EnsureEmpty();
    }
}

// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Collections.Generic;
using System.Linq;
using NakedObjects.Architecture.Component;

namespace NakedObjects.Core.Context {
    public class SimpleMessageBroker : IMessageBroker {
        private readonly List<string> messages = new List<string>();
        private readonly List<string> warnings = new List<string>();

        #region IMessageBroker Members

        public virtual string[] PeekMessages {
            get { return messages.ToArray(); }
        }

        public virtual string[] PeekWarnings {
            get { return warnings.ToArray(); }
        }


        public virtual string[] Messages {
            get {
                string[] messageArray = messages.ToArray();
                ClearMessages();
                return messageArray;
            }
        }

        public virtual string[] Warnings {
            get {
                string[] warningArray = warnings.ToArray();
                ClearWarnings();
                return warningArray;
            }
        }

        public virtual void EnsureEmpty() {
            if (warnings.Count > 0) {
                throw new InvalidStateException("Message broker still has warnings: " + warnings.Aggregate((s, t) => s + t + "; "));
            }
            if (messages.Count > 0) {
                throw new InvalidStateException("Message broker still has messages: " + messages.Aggregate((s, t) => s + t + "; "));
            }
        }

        public virtual void AddWarning(string message) {
            warnings.Add(message);
        }

        public virtual void AddMessage(string message) {
            messages.Add(message);
        }

        #endregion

        public virtual void ClearMessages() {
            messages.Clear();
        }

        public virtual void ClearWarnings() {
            warnings.Clear();
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}
// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections.Generic;
using System.Linq;
using NakedObjects.Architecture.Component;

namespace NakedObjects.Core.Component {
    public class MessageBroker : IMessageBroker {
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

        public virtual void ClearMessages() {
            messages.Clear();
        }

        public virtual void ClearWarnings() {
            warnings.Clear();
        }

        #endregion
    }

    // Copyright (c) Naked Objects Group Ltd.
}
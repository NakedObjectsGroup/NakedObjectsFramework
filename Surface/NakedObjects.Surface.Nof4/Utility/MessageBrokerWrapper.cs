// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using NakedObjects.Architecture.Component;

namespace NakedObjects.Facade.Impl.Utility {
    public class MessageBrokerWrapper : IMessageBrokerSurface {
        private readonly IMessageBroker messageBroker;

        public MessageBrokerWrapper(IMessageBroker messageBroker) {
            this.messageBroker = messageBroker;
        }

        #region IMessageBrokerSurface Members

        public string[] PeekMessages {
            get { return messageBroker.PeekMessages; }
        }

        public string[] PeekWarnings {
            get { return messageBroker.PeekWarnings; }
        }

        public string[] Messages {
            get { return messageBroker.Messages; }
        }

        public string[] Warnings {
            get { return messageBroker.Warnings; }
        }

        public void AddWarning(string message) {
            messageBroker.AddWarning(message);
        }

        public void AddMessage(string message) {
            messageBroker.AddMessage(message);
        }

        public void ClearWarnings() {
            messageBroker.ClearWarnings();
        }

        public void ClearMessages() {
            messageBroker.ClearMessages();
        }

        public void EnsureEmpty() {
            messageBroker.EnsureEmpty();
        }

        #endregion
    }
}
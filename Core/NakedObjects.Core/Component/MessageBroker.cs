// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections.Generic;
using System.Linq;
using Common.Logging;
using Microsoft.Extensions.Logging;
using NakedObjects.Architecture.Component;
using NakedObjects.Core.Util;

namespace NakedObjects.Core.Component {
    public sealed class MessageBroker : IMessageBroker {
        private readonly ILogger<MessageBroker> logger;

        private readonly List<string> messages = new List<string>();
        private readonly List<string> warnings = new List<string>();

        public MessageBroker(ILogger<MessageBroker> logger) => this.logger = logger;

        #region IMessageBroker Members

        public string[] PeekMessages => messages.ToArray();

        public string[] PeekWarnings => warnings.ToArray();

        public string[] Messages => messages.ToArray();

        public string[] Warnings => warnings.ToArray();

        public void EnsureEmpty() {
            if (warnings.Count > 0) {
                throw new InvalidStateException(logger.LogAndReturn($"Message broker still has warnings: {warnings.Aggregate((s, t) => s + t + "; ")}"));
            }

            if (messages.Count > 0) {
                throw new InvalidStateException(logger.LogAndReturn($"Message broker still has messages: {messages.Aggregate((s, t) => s + t + "; ")}"));
            }
        }

        public void AddWarning(string message) => warnings.Add(message);

        public void AddMessage(string message) => messages.Add(message);

        public void ClearMessages() => messages.Clear();

        public void ClearWarnings() => warnings.Clear();

        #endregion
    }

    // Copyright (c) Naked Objects Group Ltd.
}
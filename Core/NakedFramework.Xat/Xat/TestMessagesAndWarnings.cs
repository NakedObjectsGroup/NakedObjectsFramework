// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Architecture.Component;

namespace NakedObjects.Xat {
    public static class TestMessagesAndWarnings {
        /// <summary>
        ///     Messages written by <see cref="IDomainObjectContainer.InformUser" /> - This clears once read !
        /// </summary>
        public static string[] Messages(IMessageBroker messageBroker) => messageBroker.Messages;

        /// <summary>
        ///     Warnings written by <see cref="IDomainObjectContainer.WarnUser" /> - This clears once read !
        /// </summary>
        public static string[] Warnings(IMessageBroker messageBroker) => messageBroker.Warnings;

        /// <summary>
        ///     Messages written by <see cref="IDomainObjectContainer.InformUser" /> - This clears all messages once asserted !
        /// </summary>
        public static void AssertLastMessageIs(string expected, IMessageBroker messageBroker) {
            Assert.AreEqual(expected, Messages(messageBroker).Last());
        }

        /// <summary>
        ///     Warnings written by <see cref="IDomainObjectContainer.WarnUser" /> - This clears all warnings once asserted !
        /// </summary>
        public static void AssertLastWarningIs(string expected, IMessageBroker messageBroker) {
            Assert.AreEqual(expected, Warnings(messageBroker).Last());
        }

        /// <summary>
        ///     Messages written by <see cref="IDomainObjectContainer.InformUser" /> - This clears all messages once asserted !
        /// </summary>
        public static void AssertLastMessageContains(string expected, IMessageBroker messageBroker) {
            var lastMessage = Messages(messageBroker).Last();
            Assert.IsTrue(lastMessage.Contains(expected), @"Last message expected to contain: '{0}' actual: '{1}'", expected, lastMessage);
        }

        /// <summary>
        ///     Warnings written by <see cref="IDomainObjectContainer.WarnUser" /> - This clears all warnings once asserted  !
        /// </summary>
        public static void AssertLastWarningContains(string expected, IMessageBroker messageBroker) {
            var lastWarning = Warnings(messageBroker).Last();
            Assert.IsTrue(lastWarning.Contains(expected), @"Last warning expected to contain: '{0}' actual: '{1}'", expected, lastWarning);
        }
    }
}
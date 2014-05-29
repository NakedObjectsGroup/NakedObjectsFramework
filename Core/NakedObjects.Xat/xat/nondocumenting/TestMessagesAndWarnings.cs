// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Core.Context;

namespace NakedObjects.Xat {
    public static class TestMessagesAndWarnings {

        /// <summary>
        /// Messages written by <see cref="IDomainObjectContainer.InformUser"/> - This clears once read !
        /// </summary>
        public static string[] Messages {
            get { return NakedObjectsContext.MessageBroker.Messages; }
        }

        /// <summary>
        /// Warnings written by <see cref="IDomainObjectContainer.WarnUser"/> - This clears once read !
        /// </summary>
        public static string[] Warnings {
            get { return NakedObjectsContext.MessageBroker.Warnings; }
        }

        /// <summary>
        /// Messages written by <see cref="IDomainObjectContainer.InformUser"/> - This clears all messages once asserted !
        /// </summary>
        public static void AssertLastMessageIs(string expected) {
            Assert.AreEqual(expected, Messages.Last());
        }

        /// <summary>
        /// Warnings written by <see cref="IDomainObjectContainer.WarnUser"/> - This clears all warnings once asserted !
        /// </summary>
        public static void AssertLastWarningIs(string expected) {
            Assert.AreEqual(expected, Warnings.Last());
        }

        /// <summary>
        /// Messages written by <see cref="IDomainObjectContainer.InformUser"/> - This clears all messages once asserted !
        /// </summary>
        public static void AssertLastMessageContains(string expected) {
            string lastMessage = Messages.Last();
            Assert.IsTrue(lastMessage.Contains(expected), @"Last message expected to contain: '{0}' actual: '{1}'", expected, lastMessage);
        }

        /// <summary>
        /// Warnings written by <see cref="IDomainObjectContainer.WarnUser"/> - This clears all warnings once asserted  !
        /// </summary>
        public static void AssertLastWarningContains(string expected) {
            string lastWarning = Warnings.Last();
            Assert.IsTrue(lastWarning.Contains(expected), @"Last warning expected to contain: '{0}' actual: '{1}'", expected, lastWarning);
        }
    }
}
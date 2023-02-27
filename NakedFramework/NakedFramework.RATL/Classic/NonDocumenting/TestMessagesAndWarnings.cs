namespace NakedFramework.RATL.Classic.NonDocumenting; 

public static class TestMessagesAndWarnings {
    /// <summary>
    ///     Messages written by <see cref="IDomainObjectContainer.InformUser" /> - This clears once read !
    /// </summary>
    public static string[] Messages => throw new NotImplementedException();//NakedObjectsContext.MessageBroker.Messages;

    /// <summary>
    ///     Warnings written by <see cref="IDomainObjectContainer.WarnUser" /> - This clears once read !
    /// </summary>
    public static string[] Warnings =>throw new NotImplementedException();// NakedObjectsContext.MessageBroker.Warnings;

    /// <summary>
    ///     Messages written by <see cref="IDomainObjectContainer.InformUser" /> - This clears all messages once asserted !
    /// </summary>
    public static void AssertLastMessageIs(string expected) {
        Assert.AreEqual(expected, Messages.Last());
    }

    /// <summary>
    ///     Warnings written by <see cref="IDomainObjectContainer.WarnUser" /> - This clears all warnings once asserted !
    /// </summary>
    public static void AssertLastWarningIs(string expected) {
        Assert.AreEqual(expected, Warnings.Last());
    }

    /// <summary>
    ///     Messages written by <see cref="IDomainObjectContainer.InformUser" /> - This clears all messages once asserted !
    /// </summary>
    public static void AssertLastMessageContains(string expected) {
        var lastMessage = Messages.Last();
        Assert.IsTrue(lastMessage.Contains(expected), @"Last message expected to contain: '{0}' actual: '{1}'", expected, lastMessage);
    }

    /// <summary>
    ///     Warnings written by <see cref="IDomainObjectContainer.WarnUser" /> - This clears all warnings once asserted  !
    /// </summary>
    public static void AssertLastWarningContains(string expected) {
        var lastWarning = Warnings.Last();
        Assert.IsTrue(lastWarning.Contains(expected), @"Last warning expected to contain: '{0}' actual: '{1}'", expected, lastWarning);
    }
}


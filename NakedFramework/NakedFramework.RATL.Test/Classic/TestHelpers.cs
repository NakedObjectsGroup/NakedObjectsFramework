namespace NakedFramework.RATL.Test.Classic;

public static class TestHelpers {
    public static void AssertExpectException(Action f, string msg) {
        try {
            f();
            Assert.Fail("Expect exception");
        }
        catch (Exception ex) {
            Assert.IsInstanceOfType(ex, typeof(AssertFailedException));
            Assert.AreEqual(msg, ex.Message);
        }
    }
}
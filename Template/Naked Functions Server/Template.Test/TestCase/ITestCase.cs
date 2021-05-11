using System;

namespace Template.Test.TestCase {
    public interface ITestCase {
        IServiceProvider GetConfiguredContainer();
        void StartServerTransaction();
        void EndServerTransaction();
    }
}
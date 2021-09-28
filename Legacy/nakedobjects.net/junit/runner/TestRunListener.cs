// Decompiled with JetBrains decompiler
// Type: junit.runner.TestRunListener
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;

namespace junit.runner
{
  [JavaInterface]
  public interface TestRunListener
  {
    const int STATUS_ERROR = 1;
    const int STATUS_FAILURE = 2;

    void testRunStarted(string testSuiteName, int testCount);

    void testRunEnded(long elapsedTime);

    void testRunStopped(long elapsedTime);

    void testStarted(string testName);

    void testEnded(string testName);

    void testFailed(int status, string testName, string trace);
  }
}

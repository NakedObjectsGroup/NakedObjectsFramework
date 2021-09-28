// Decompiled with JetBrains decompiler
// Type: junit.runner.BaseTestRunner
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.io;
using java.lang;
using java.text;
using java.util;
using junit.framework;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace junit.runner
{
  [JavaInterfaces("1;junit/framework/TestListener;")]
  public abstract class BaseTestRunner : TestListener
  {
    public const string SUITE_METHODNAME = "suite";
    private static Properties fPreferences;
    [JavaFlags(8)]
    public static int fgMaxMessageLength;
    [JavaFlags(8)]
    public static bool fgFilterStack;
    [JavaFlags(0)]
    public bool fLoading;

    [MethodImpl(MethodImplOptions.Synchronized)]
    public virtual void startTest(Test test) => this.testStarted(test.ToString());

    [JavaFlags(12)]
    public static void setPreferences(Properties preferences) => BaseTestRunner.fPreferences = preferences;

    [JavaFlags(12)]
    public static Properties getPreferences()
    {
      if (BaseTestRunner.fPreferences == null)
      {
        BaseTestRunner.fPreferences = new Properties();
        ((Hashtable) BaseTestRunner.fPreferences).put((object) "loading", (object) "true");
        ((Hashtable) BaseTestRunner.fPreferences).put((object) "filterstack", (object) "true");
        BaseTestRunner.readPreferences();
      }
      return BaseTestRunner.fPreferences;
    }

    [JavaThrownExceptions("1;java/io/IOException;")]
    public static void savePreferences()
    {
    }

    public virtual void setPreference(string key, string value)
    {
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public virtual void endTest(Test test) => this.testEnded(test.ToString());

    [MethodImpl(MethodImplOptions.Synchronized)]
    public virtual void addError(Test test, Throwable t) => this.testFailed(1, test, t);

    [MethodImpl(MethodImplOptions.Synchronized)]
    public virtual void addFailure(Test test, AssertionFailedError t) => this.testFailed(2, test, (Throwable) t);

    public abstract void testStarted(string testName);

    public abstract void testEnded(string testName);

    public abstract void testFailed(int status, Test test, Throwable t);

    public virtual Test getTest(string suiteClassName)
    {
      // ISSUE: unable to decompile the method.
    }

    public virtual string elapsedTimeAsString(long runTime) => NumberFormat.getInstance().format((double) runTime / 1000.0);

    [JavaFlags(4)]
    public virtual string processArguments(string[] args)
    {
      string str = (string) null;
      for (int index = 0; index < args.Length; ++index)
      {
        if (StringImpl.equals(args[index], (object) "-noloading"))
          this.setLoading(false);
        else if (StringImpl.equals(args[index], (object) "-nofilterstack"))
          BaseTestRunner.fgFilterStack = false;
        else if (StringImpl.equals(args[index], (object) "-c"))
        {
          if (args.Length > index + 1)
            str = this.extractClassName(args[index + 1]);
          else
            ((PrintStream) java.lang.System.@out).println("Missing Test class name");
          ++index;
        }
        else
          str = args[index];
      }
      return str;
    }

    public virtual void setLoading(bool enable) => this.fLoading = enable;

    public virtual string extractClassName(string className) => StringImpl.startsWith(className, "Default package for") ? StringImpl.substring(className, StringImpl.lastIndexOf(className, ".") + 1) : className;

    public static string truncate(string s)
    {
      if (BaseTestRunner.fgMaxMessageLength != -1 && StringImpl.length(s) > BaseTestRunner.fgMaxMessageLength)
        s = new StringBuffer().append(StringImpl.substring(s, 0, BaseTestRunner.fgMaxMessageLength)).append("...").ToString();
      return s;
    }

    [JavaFlags(1028)]
    public abstract void runFailed(string message);

    [JavaFlags(4)]
    [JavaThrownExceptions("1;java/lang/ClassNotFoundException;")]
    public virtual Class loadSuiteClass(string suiteClassName) => this.getLoader().load(suiteClassName);

    [JavaFlags(4)]
    public virtual void clearStatus()
    {
    }

    public virtual TestSuiteLoader getLoader() => this.useReloadingTestSuiteLoader() ? (TestSuiteLoader) new ReloadingTestSuiteLoader() : (TestSuiteLoader) new StandardTestSuiteLoader();

    [JavaFlags(4)]
    public virtual bool useReloadingTestSuiteLoader() => StringImpl.equals(BaseTestRunner.getPreference("loading"), (object) "true") && !BaseTestRunner.inVAJava() && this.fLoading;

    private static File getPreferencesFile() => new File(java.lang.System.getProperty("user.home"), "junit.properties");

    private static void readPreferences()
    {
      InputStream inputStream = (InputStream) null;
      try
      {
        inputStream = (InputStream) new FileInputStream(BaseTestRunner.getPreferencesFile());
        BaseTestRunner.setPreferences(new Properties(BaseTestRunner.getPreferences()));
        BaseTestRunner.getPreferences().load(inputStream);
      }
      catch (IOException ex1)
      {
        try
        {
          inputStream?.close();
        }
        catch (IOException ex2)
        {
        }
      }
    }

    public static string getPreference(string key) => BaseTestRunner.getPreferences().getProperty(key);

    public static int getPreference(string key, int dflt)
    {
      // ISSUE: unable to decompile the method.
    }

    public static bool inVAJava() => true;

    public static string getFilteredTrace(Throwable t)
    {
      StringWriter stringWriter = new StringWriter();
      PrintWriter printWriter = new PrintWriter((Writer) stringWriter);
      t.printStackTrace(printWriter);
      return BaseTestRunner.getFilteredTrace(stringWriter.getBuffer().ToString());
    }

    public static string getFilteredTrace(string stack)
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(12)]
    public static bool showStackRaw() => !StringImpl.equals(BaseTestRunner.getPreference("filterstack"), (object) "true") || !BaseTestRunner.fgFilterStack;

    [JavaFlags(8)]
    public static bool filterLine(string line)
    {
      int length = 8;
      string[] strArray = length >= 0 ? new string[length] : throw new NegativeArraySizeException();
      strArray[0] = "junit.framework.TestCase";
      strArray[1] = "junit.framework.TestResult";
      strArray[2] = "junit.framework.TestSuite";
      strArray[3] = "junit.framework.Assert.";
      strArray[4] = "junit.swingui.TestRunner";
      strArray[5] = "junit.awtui.TestRunner";
      strArray[6] = "junit.textui.TestRunner";
      strArray[7] = "java.lang.reflect.Method.invoke(";
      foreach (string str in strArray)
      {
        if (StringImpl.indexOf(line, str) > 0)
          return true;
      }
      return false;
    }

    public BaseTestRunner() => this.fLoading = true;

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static BaseTestRunner()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      BaseTestRunner baseTestRunner = this;
      ObjectImpl.clone((object) baseTestRunner);
      return ((object) baseTestRunner).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}

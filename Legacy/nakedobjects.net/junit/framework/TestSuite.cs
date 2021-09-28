// Decompiled with JetBrains decompiler
// Type: junit.framework.TestSuite
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.io;
using java.lang;
using java.lang.reflect;
using java.util;
using System.Runtime.InteropServices;

namespace junit.framework
{
  [JavaInterfaces("1;junit/framework/Test;")]
  public class TestSuite : Test
  {
    private Vector fTests;
    private string fName;

    public TestSuite() => this.fTests = new Vector(10);

    public TestSuite(Class theClass, string name)
      : this(theClass)
    {
      this.setName(name);
    }

    public TestSuite(Class theClass)
    {
      this.fTests = new Vector(10);
      this.fName = theClass.getName();
      try
      {
        TestSuite.getTestConstructor(theClass);
      }
      catch (NoSuchMethodException ex)
      {
        this.addTest(TestSuite.warning(new StringBuffer().append("Class ").append(theClass.getName()).append(" has no public constructor TestCase(String name) or TestCase()").ToString()));
        return;
      }
      if (!Modifier.isPublic(theClass.getModifiers()))
      {
        this.addTest(TestSuite.warning(new StringBuffer().append("Class ").append(theClass.getName()).append(" is not public").ToString()));
      }
      else
      {
        Class @class = theClass;
        Vector names = new Vector();
        for (; Class.FromType(typeof (Test)).isAssignableFrom(@class); @class = @class.getSuperclass())
        {
          foreach (Method declaredMethod in @class.getDeclaredMethods())
            this.addTestMethod(declaredMethod, names, theClass);
        }
        if (this.fTests.size() != 0)
          return;
        this.addTest(TestSuite.warning(new StringBuffer().append("No tests found in ").append(theClass.getName()).ToString()));
      }
    }

    public TestSuite(string name)
    {
      this.fTests = new Vector(10);
      this.setName(name);
    }

    public virtual void addTest(Test test) => this.fTests.addElement((object) test);

    public virtual void addTestSuite(Class testClass) => this.addTest((Test) new TestSuite(testClass));

    private void addTestMethod(Method m, Vector names, Class theClass)
    {
      string name = m.getName();
      if (names.contains((object) name))
        return;
      if (!this.isPublicTestMethod(m))
      {
        if (!this.isTestMethod(m))
          return;
        this.addTest(TestSuite.warning(new StringBuffer().append("Test method isn't public: ").append(m.getName()).ToString()));
      }
      else
      {
        names.addElement((object) name);
        this.addTest(TestSuite.createTest(theClass, name));
      }
    }

    public static Test createTest(Class theClass, string name)
    {
      Constructor testConstructor;
      try
      {
        testConstructor = TestSuite.getTestConstructor(theClass);
      }
      catch (NoSuchMethodException ex)
      {
        return TestSuite.warning(new StringBuffer().append("Class ").append(theClass.getName()).append(" has no public constructor TestCase(String name) or TestCase()").ToString());
      }
      object obj;
      try
      {
        if (testConstructor.getParameterTypes().Length == 0)
        {
          Constructor constructor = testConstructor;
          int length = 0;
          object[] objArray = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
          obj = constructor.newInstance(objArray);
          if (obj is TestCase)
            ((TestCase) obj).setName(name);
        }
        else
        {
          Constructor constructor = testConstructor;
          int length = 1;
          object[] objArray = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
          objArray[0] = (object) name;
          obj = constructor.newInstance(objArray);
        }
      }
      catch (InstantiationException ex)
      {
        return TestSuite.warning(new StringBuffer().append("Cannot instantiate test case: ").append(name).append(" (").append(TestSuite.exceptionToString((Throwable) ex)).append(")").ToString());
      }
      catch (InvocationTargetException ex)
      {
        return TestSuite.warning(new StringBuffer().append("Exception in constructor: ").append(name).append(" (").append(TestSuite.exceptionToString(ex.getTargetException())).append(")").ToString());
      }
      catch (IllegalAccessException ex)
      {
        return TestSuite.warning(new StringBuffer().append("Cannot access test case: ").append(name).append(" (").append(TestSuite.exceptionToString((Throwable) ex)).append(")").ToString());
      }
      return (Test) obj;
    }

    private static string exceptionToString(Throwable t)
    {
      StringWriter stringWriter = new StringWriter();
      PrintWriter printWriter = new PrintWriter((Writer) stringWriter);
      t.printStackTrace(printWriter);
      return stringWriter.ToString();
    }

    public virtual int countTestCases()
    {
      int num = 0;
      Enumeration enumeration = this.tests();
      while (enumeration.hasMoreElements())
      {
        Test test = (Test) enumeration.nextElement();
        num += test.countTestCases();
      }
      return num;
    }

    [JavaThrownExceptions("1;java/lang/NoSuchMethodException;")]
    public static Constructor getTestConstructor(Class theClass)
    {
      int length1 = 1;
      Class[] classArray1 = length1 >= 0 ? new Class[length1] : throw new NegativeArraySizeException();
      classArray1[0] = Class.FromType(typeof (string));
      Class[] classArray2 = classArray1;
      try
      {
        return theClass.getConstructor(classArray2);
      }
      catch (NoSuchMethodException ex)
      {
      }
      Class @class = theClass;
      int length2 = 0;
      Class[] classArray3 = length2 >= 0 ? new Class[length2] : throw new NegativeArraySizeException();
      return @class.getConstructor(classArray3);
    }

    private bool isPublicTestMethod(Method m) => this.isTestMethod(m) && Modifier.isPublic(m.getModifiers());

    private bool isTestMethod(Method m)
    {
      string name = m.getName();
      Class[] parameterTypes = m.getParameterTypes();
      Class returnType = m.getReturnType();
      return parameterTypes.Length == 0 && StringImpl.startsWith(name, "test") && ((object) returnType).Equals((object) Void.TYPE);
    }

    public virtual void run(TestResult result)
    {
      Enumeration enumeration = this.tests();
      while (enumeration.hasMoreElements() && !result.shouldStop())
        this.runTest((Test) enumeration.nextElement(), result);
    }

    public virtual void runTest(Test test, TestResult result) => test.run(result);

    public virtual Test testAt(int index) => (Test) this.fTests.elementAt(index);

    public virtual int testCount() => this.fTests.size();

    public virtual Enumeration tests() => this.fTests.elements();

    public override string ToString() => this.getName() != null ? this.getName() : ObjectImpl.jloToString((object) this);

    public virtual void setName(string name) => this.fName = name;

    public virtual string getName() => this.fName;

    private static Test warning(string message) => (Test) new TestSuite.\u0031(nameof (warning), message);

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      TestSuite testSuite = this;
      ObjectImpl.clone((object) testSuite);
      return ((object) testSuite).MemberwiseClone();
    }

    [JavaFlags(32)]
    [Inner]
    public class \u0031 : TestCase
    {
      [JavaFlags(16)]
      public readonly string message_\u003E;

      [JavaFlags(4)]
      public override void runTest() => Assert.fail(this.message_\u003E);

      public \u0031(string dummy0, [In] string obj1)
        : base(dummy0)
      {
        this.message_\u003E = obj1;
      }
    }
  }
}

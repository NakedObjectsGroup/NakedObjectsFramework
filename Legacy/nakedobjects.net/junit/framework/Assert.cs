// Decompiled with JetBrains decompiler
// Type: junit.framework.Assert
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;

namespace junit.framework
{
  public class Assert
  {
    [JavaFlags(4)]
    public Assert()
    {
    }

    public static void assertTrue(string message, bool condition)
    {
      if (condition)
        return;
      Assert.fail(message);
    }

    public static void assertTrue(bool condition) => Assert.assertTrue((string) null, condition);

    public static void assertFalse(string message, bool condition) => Assert.assertTrue(message, ((condition ? 1 : 0) ^ 1) != 0);

    public static void assertFalse(bool condition) => Assert.assertFalse((string) null, condition);

    public static void fail(string message) => throw new AssertionFailedError(message);

    public static void fail() => Assert.fail((string) null);

    public static void assertEquals(string message, object expected, object actual)
    {
      if (expected == null && actual == null || expected != null && expected.Equals(actual))
        return;
      Assert.failNotEquals(message, expected, actual);
    }

    public static void assertEquals(object expected, object actual) => Assert.assertEquals((string) null, expected, actual);

    public static void assertEquals(string message, string expected, string actual)
    {
      if ((expected != null || actual != null) && (expected == null || !StringImpl.equals(expected, (object) actual)))
        throw new ComparisonFailure(message, expected, actual);
    }

    public static void assertEquals(string expected, string actual) => Assert.assertEquals((string) null, expected, actual);

    public static void assertEquals(string message, double expected, double actual, double delta)
    {
      if (Double.isInfinite(expected))
      {
        if (expected == actual)
          return;
        Assert.failNotEquals(message, (object) new Double(expected), (object) new Double(actual));
      }
      else
      {
        if (Math.abs(expected - actual) <= delta)
          return;
        Assert.failNotEquals(message, (object) new Double(expected), (object) new Double(actual));
      }
    }

    public static void assertEquals(double expected, double actual, double delta) => Assert.assertEquals((string) null, expected, actual, delta);

    public static void assertEquals(string message, float expected, float actual, float delta)
    {
      if (Float.isInfinite(expected))
      {
        if ((double) expected == (double) actual)
          return;
        Assert.failNotEquals(message, (object) new Float(expected), (object) new Float(actual));
      }
      else
      {
        if ((double) Math.abs(expected - actual) <= (double) delta)
          return;
        Assert.failNotEquals(message, (object) new Float(expected), (object) new Float(actual));
      }
    }

    public static void assertEquals(float expected, float actual, float delta) => Assert.assertEquals((string) null, expected, actual, delta);

    public static void assertEquals(string message, long expected, long actual) => Assert.assertEquals(message, (object) new Long(expected), (object) new Long(actual));

    public static void assertEquals(long expected, long actual) => Assert.assertEquals((string) null, expected, actual);

    public static void assertEquals(string message, bool expected, bool actual) => Assert.assertEquals(message, (object) new Boolean(expected), (object) new Boolean(actual));

    public static void assertEquals(bool expected, bool actual) => Assert.assertEquals((string) null, expected, actual);

    public static void assertEquals(string message, sbyte expected, sbyte actual) => Assert.assertEquals(message, (object) new Byte(expected), (object) new Byte(actual));

    public static void assertEquals(sbyte expected, sbyte actual) => Assert.assertEquals((string) null, expected, actual);

    public static void assertEquals(string message, char expected, char actual) => Assert.assertEquals(message, (object) new Character(expected), (object) new Character(actual));

    public static void assertEquals(char expected, char actual) => Assert.assertEquals((string) null, expected, actual);

    public static void assertEquals(string message, short expected, short actual) => Assert.assertEquals(message, (object) new Short(expected), (object) new Short(actual));

    public static void assertEquals(short expected, short actual) => Assert.assertEquals((string) null, expected, actual);

    public static void assertEquals(string message, int expected, int actual) => Assert.assertEquals(message, (object) new Integer(expected), (object) new Integer(actual));

    public static void assertEquals(int expected, int actual) => Assert.assertEquals((string) null, expected, actual);

    public static void assertNotNull(object @object) => Assert.assertNotNull((string) null, @object);

    public static void assertNotNull(string message, object @object) => Assert.assertTrue(message, @object != null);

    public static void assertNull(object @object) => Assert.assertNull((string) null, @object);

    public static void assertNull(string message, object @object) => Assert.assertTrue(message, @object == null);

    public static void assertSame(string message, object expected, object actual)
    {
      if (expected == actual)
        return;
      Assert.failNotSame(message, expected, actual);
    }

    public static void assertSame(object expected, object actual) => Assert.assertSame((string) null, expected, actual);

    public static void assertNotSame(string message, object expected, object actual)
    {
      if (expected != actual)
        return;
      Assert.failSame(message);
    }

    public static void assertNotSame(object expected, object actual) => Assert.assertNotSame((string) null, expected, actual);

    private static void failSame(string message)
    {
      string str = "";
      if (message != null)
        str = new StringBuffer().append(message).append(" ").ToString();
      Assert.fail(new StringBuffer().append(str).append("expected not same").ToString());
    }

    private static void failNotSame(string message, object expected, object actual)
    {
      string str = "";
      if (message != null)
        str = new StringBuffer().append(message).append(" ").ToString();
      Assert.fail(new StringBuffer().append(str).append("expected same:<").append(expected).append("> was not:<").append(actual).append(">").ToString());
    }

    private static void failNotEquals(string message, object expected, object actual) => Assert.fail(Assert.format(message, expected, actual));

    [JavaFlags(8)]
    public static string format(string message, object expected, object actual)
    {
      string str = "";
      if (message != null)
        str = new StringBuffer().append(message).append(" ").ToString();
      return new StringBuffer().append(str).append("expected:<").append(expected).append("> but was:<").append(actual).append(">").ToString();
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      Assert assert = this;
      ObjectImpl.clone((object) assert);
      return ((object) assert).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}

// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.utility.Assert
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;

namespace org.nakedobjects.utility
{
  public class Assert
  {
    public static void assertEquals(object expected, object actual) => Assert.assertEquals("", expected, actual);

    public static void assertEquals(string message, object expected, object actual)
    {
      if ((expected != null || actual != null) && (expected == null || !expected.Equals(actual)))
        throw new NakedObjectAssertException(new StringBuffer().append(message).append(": expected ").append(expected).append(" but was ").append(actual).ToString());
    }

    public static void assertFalse(bool flag) => Assert.assertFalse("expected false", flag);

    public static void assertFalse(string message, bool flag) => Assert.assertTrue(message, ((flag ? 1 : 0) ^ 1) != 0);

    public static void assertFalse(string message, object target, bool flag) => Assert.assertTrue(message, target, ((flag ? 1 : 0) ^ 1) != 0);

    public static void assertNotNull(object identified) => Assert.assertNotNull("", identified);

    public static void assertNotNull(string message, object @object)
    {
      if (@object == null)
        throw new NakedObjectAssertException(new StringBuffer().append("unexpected null: ").append(message).ToString());
    }

    public static void assertNotNull(string message, object target, object @object) => Assert.assertTrue(message, target, @object != null);

    public static void assertNull(object @object) => Assert.assertTrue("unexpected reference; should be null", @object == null);

    public static void assertNull(string message, object @object) => Assert.assertTrue(message, @object == null);

    public static void assertTrue(bool flag) => Assert.assertTrue("expected true", flag);

    public static void assertTrue(string message, bool flag) => Assert.assertTrue(message, (object) null, flag);

    public static void assertTrue(string message, object target, bool flag)
    {
      if (!flag)
        throw new NakedObjectAssertException(new StringBuffer().append(message).append(target != null ? new StringBuffer().append(": ").append(target).ToString() : "").ToString());
    }

    public static void assertEquals(string message, int expected, int value)
    {
      if (expected != value)
        throw new NakedObjectAssertException(new StringBuffer().append(message).append(" expected ").append(expected).append("; but was ").append(value).ToString());
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

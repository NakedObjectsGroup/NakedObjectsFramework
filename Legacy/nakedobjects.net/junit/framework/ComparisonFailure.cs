// Decompiled with JetBrains decompiler
// Type: junit.framework.ComparisonFailure
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.lang;
using java.lang;

namespace junit.framework
{
  public class ComparisonFailure : AssertionFailedError
  {
    private string fExpected;
    private string fActual;

    public ComparisonFailure(string message, string expected, string actual)
      : base(message)
    {
      this.fExpected = expected;
      this.fActual = actual;
    }

    public virtual string getMessage()
    {
      if (this.fExpected == null || this.fActual == null)
        return Assert.format(((Throwable) this).getMessage(), (object) this.fExpected, (object) this.fActual);
      int num1 = Math.min(StringImpl.length(this.fExpected), StringImpl.length(this.fActual));
      int num2 = 0;
      while (num2 < num1 && (int) StringImpl.charAt(this.fExpected, num2) == (int) StringImpl.charAt(this.fActual, num2))
        ++num2;
      int num3 = StringImpl.length(this.fExpected) - 1;
      int num4;
      for (num4 = StringImpl.length(this.fActual) - 1; num4 >= num2 && num3 >= num2 && (int) StringImpl.charAt(this.fExpected, num3) == (int) StringImpl.charAt(this.fActual, num4); num3 += -1)
        num4 += -1;
      string str1;
      string str2;
      if (num3 < num2 && num4 < num2)
      {
        str1 = this.fExpected;
        str2 = this.fActual;
      }
      else
      {
        str1 = StringImpl.substring(this.fExpected, num2, num3 + 1);
        str2 = StringImpl.substring(this.fActual, num2, num4 + 1);
        if (num2 <= num1 && num2 > 0)
        {
          str1 = new StringBuffer().append("...").append(str1).ToString();
          str2 = new StringBuffer().append("...").append(str2).ToString();
        }
        if (num3 < StringImpl.length(this.fExpected) - 1)
          str1 = new StringBuffer().append(str1).append("...").ToString();
        if (num4 < StringImpl.length(this.fActual) - 1)
          str2 = new StringBuffer().append(str2).append("...").ToString();
      }
      return Assert.format(((Throwable) this).getMessage(), (object) str1, (object) str2);
    }
  }
}

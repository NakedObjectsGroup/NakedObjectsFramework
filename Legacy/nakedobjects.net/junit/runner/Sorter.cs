// Decompiled with JetBrains decompiler
// Type: junit.runner.Sorter
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.util;

namespace junit.runner
{
  public class Sorter
  {
    public static void sortStrings(Vector values, int left, int right, Sorter.Swapper swapper)
    {
      int left1 = left;
      int right1 = right;
      string str = \u003CVerifierFix\u003E.genCastToString(values.elementAt((left + right) / 2));
      do
      {
        while (StringImpl.compareTo(\u003CVerifierFix\u003E.genCastToString(values.elementAt(left)), str) < 0)
          ++left;
        while (StringImpl.compareTo(str, \u003CVerifierFix\u003E.genCastToString(values.elementAt(right))) < 0)
          right += -1;
        if (left <= right)
        {
          swapper.swap(values, left, right);
          ++left;
          right += -1;
        }
      }
      while (left <= right);
      if (left1 < right)
        Sorter.sortStrings(values, left1, right, swapper);
      if (left >= right1)
        return;
      Sorter.sortStrings(values, left, right1, swapper);
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      Sorter sorter = this;
      ObjectImpl.clone((object) sorter);
      return ((object) sorter).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);

    [JavaFlags(1545)]
    [JavaInterface]
    public interface Swapper
    {
      void swap(Vector values, int left, int right);
    }
  }
}

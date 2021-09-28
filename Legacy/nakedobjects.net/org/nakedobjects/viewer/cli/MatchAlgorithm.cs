// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.cli.MatchAlgorithm
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;

namespace org.nakedobjects.viewer.cli
{
  public sealed class MatchAlgorithm
  {
    private MatchAlgorithm()
    {
    }

    public static object match(string entry, Matcher matcher)
    {
      string entry1 = MatchAlgorithm.removeSpaces(StringImpl.toLowerCase(entry));
      int num1 = 0;
      object obj = (object) null;
      while (matcher.hasMoreElements())
      {
        string lowerCase = StringImpl.toLowerCase(matcher.nextElement());
        int num2 = MatchAlgorithm.compare(entry1, lowerCase);
        if (num2 > num1)
        {
          num1 = num2;
          obj = matcher.getElement();
        }
      }
      return obj;
    }

    private static int compare(string entry, string title)
    {
      title = MatchAlgorithm.removeSpaces(title);
      int num1 = Math.max(StringImpl.indexOf(entry, 42), StringImpl.indexOf(entry, 45));
      if (num1 >= 0)
      {
        string str1 = StringImpl.substring(entry, 0, num1);
        int num2 = StringImpl.indexOf(title, str1);
        if (num2 == -1)
          return 0;
        string str2 = StringImpl.substring(entry, num1 + 1);
        int num3 = StringImpl.indexOf(title, str2, num2 + StringImpl.length(str1) + 1);
        return num3 > 0 ? 200 - num2 - num3 : 0;
      }
      return StringImpl.indexOf(title, entry) >= 0 ? 100 - (StringImpl.length(title) - StringImpl.length(entry)) : 0;
    }

    public static string removeSpaces(string title)
    {
      int num = StringImpl.indexOf(title, 32);
      return num > 0 ? MatchAlgorithm.removeSpaces(new StringBuffer().append(StringImpl.substring(title, 0, num)).append(StringImpl.substring(title, num + 1)).ToString()) : title;
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      MatchAlgorithm matchAlgorithm = this;
      ObjectImpl.clone((object) matchAlgorithm);
      return ((object) matchAlgorithm).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}

// Decompiled with JetBrains decompiler
// Type: org.apache.log4j.helpers.Transform
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using System.ComponentModel;

namespace org.apache.log4j.helpers
{
  public class Transform
  {
    private const string CDATA_START = "<![CDATA[";
    private const string CDATA_END = "]]>";
    private const string CDATA_PSEUDO_END = "]]&gt;";
    private static readonly string CDATA_EMBEDED_END;
    private static readonly int CDATA_END_LEN;

    public static string escapeTags(string input)
    {
      if (input == null || StringImpl.length(input) == 0)
        return input;
      StringBuffer stringBuffer = new StringBuffer(StringImpl.length(input) + 6);
      int num = StringImpl.length(input);
      for (int index = 0; index < num; ++index)
      {
        char ch = StringImpl.charAt(input, index);
        switch (ch)
        {
          case '<':
            stringBuffer.append("&lt;");
            break;
          case '>':
            stringBuffer.append("&gt;");
            break;
          default:
            stringBuffer.append(ch);
            break;
        }
      }
      return stringBuffer.ToString();
    }

    public static void appendEscapingCDATA(StringBuffer buf, string str)
    {
      int num1 = StringImpl.indexOf(str, "]]>");
      if (num1 < 0)
      {
        buf.append(str);
      }
      else
      {
        int num2 = 0;
        for (; num1 > -1; num1 = StringImpl.indexOf(str, "]]>", num2))
        {
          buf.append(StringImpl.substring(str, num2, num1));
          buf.append(Transform.CDATA_EMBEDED_END);
          num2 = num1 + Transform.CDATA_END_LEN;
          if (num2 >= StringImpl.length(str))
            return;
        }
        buf.append(StringImpl.substring(str, num2));
      }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [JavaFlags(32778)]
    static Transform()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      Transform transform = this;
      ObjectImpl.clone((object) transform);
      return ((object) transform).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}

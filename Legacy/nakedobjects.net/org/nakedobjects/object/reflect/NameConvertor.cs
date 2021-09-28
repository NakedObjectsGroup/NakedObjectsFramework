// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.reflect.NameConvertor
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.nakedobjects.@object.reflect;

namespace org.nakedobjects.@object.reflect
{
  public class NameConvertor
  {
    private const char SPACE = ' ';

    public static string simpleName(string name)
    {
      int num = StringImpl.length(name);
      StringBuffer stringBuffer = new StringBuffer(num);
      for (int index = 0; index < num; ++index)
      {
        char ch = StringImpl.charAt(name, index);
        if (ch != ' ')
          stringBuffer.append(Character.toLowerCase(ch));
      }
      return stringBuffer.ToString();
    }

    public static string naturalName(string name)
    {
      int num = StringImpl.length(name);
      if (num <= 1)
        return name;
      StringBuffer stringBuffer = new StringBuffer(num);
      char ch1 = StringImpl.charAt(name, 0);
      stringBuffer.append(ch1);
      char ch2 = StringImpl.charAt(name, 1);
      for (int index = 2; index < num; ++index)
      {
        char ch3 = ch1;
        ch1 = ch2;
        ch2 = StringImpl.charAt(name, index);
        if (ch3 != ' ')
        {
          if (Character.isUpperCase(ch1) && !Character.isUpperCase(ch3))
            stringBuffer.append(' ');
          if (Character.isUpperCase(ch1) && Character.isLowerCase(ch2) && Character.isUpperCase(ch3))
            stringBuffer.append(' ');
          if (Character.isDigit(ch1) && !Character.isDigit(ch3))
            stringBuffer.append(' ');
        }
        stringBuffer.append(ch1);
      }
      stringBuffer.append(ch2);
      return stringBuffer.ToString();
    }

    public static string pluralName(string name) => !StringImpl.endsWith(name, "y") ? (StringImpl.endsWith(name, "s") || StringImpl.endsWith(name, "x") ? new StringBuffer().append(name).append("es").ToString() : new StringBuffer().append(name).append('s').ToString()) : new StringBuffer().append(StringImpl.substring(name, 0, StringImpl.length(name) - 1)).append("ies").ToString();

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      NameConvertor nameConvertor = this;
      ObjectImpl.clone((object) nameConvertor);
      return ((object) nameConvertor).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}

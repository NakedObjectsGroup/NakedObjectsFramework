// Decompiled with JetBrains decompiler
// Type: org.apache.crimson.util.XmlNames
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;

namespace org.apache.crimson.util
{
  public class XmlNames
  {
    public const string SPEC_XML_URI = "http://www.w3.org/XML/1998/namespace";
    public const string SPEC_XMLNS_URI = "http://www.w3.org/2000/xmlns/";

    private XmlNames()
    {
    }

    public static bool isName(string value)
    {
      if (value == null || StringImpl.equals("", (object) value))
        return false;
      char c = StringImpl.charAt(value, 0);
      if (!XmlChars.isLetter(c) && c != '_' && c != ':')
        return false;
      for (int index = 1; index < StringImpl.length(value); ++index)
      {
        if (!XmlChars.isNameChar(StringImpl.charAt(value, index)))
          return false;
      }
      return true;
    }

    public static bool isUnqualifiedName(string value)
    {
      if (value == null || StringImpl.length(value) == 0)
        return false;
      char c = StringImpl.charAt(value, 0);
      if (!XmlChars.isLetter(c) && c != '_')
        return false;
      for (int index = 1; index < StringImpl.length(value); ++index)
      {
        if (!XmlChars.isNCNameChar(StringImpl.charAt(value, index)))
          return false;
      }
      return true;
    }

    public static bool isQualifiedName(string value)
    {
      if (value == null)
        return false;
      int num = StringImpl.indexOf(value, 58);
      if (num <= 0)
        return XmlNames.isUnqualifiedName(value);
      return StringImpl.lastIndexOf(value, 58) == num && XmlNames.isUnqualifiedName(StringImpl.substring(value, 0, num)) && XmlNames.isUnqualifiedName(StringImpl.substring(value, num + 1));
    }

    public static bool isNmtoken(string token)
    {
      int num = StringImpl.length(token);
      for (int index = 0; index < num; ++index)
      {
        if (!XmlChars.isNameChar(StringImpl.charAt(token, index)))
          return false;
      }
      return true;
    }

    public static bool isNCNmtoken(string token) => XmlNames.isNmtoken(token) && StringImpl.indexOf(token, 58) < 0;

    public static string getPrefix(string qualifiedName)
    {
      int num = StringImpl.indexOf(qualifiedName, 58);
      return num <= 0 ? (string) null : StringImpl.substring(qualifiedName, 0, num);
    }

    public static string getLocalPart(string qualifiedName)
    {
      int num = StringImpl.indexOf(qualifiedName, 58);
      if (num < 0)
        return qualifiedName;
      return num == StringImpl.length(qualifiedName) - 1 ? (string) null : StringImpl.substring(qualifiedName, num + 1);
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      XmlNames xmlNames = this;
      ObjectImpl.clone((object) xmlNames);
      return ((object) xmlNames).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}

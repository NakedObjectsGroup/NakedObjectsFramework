// Decompiled with JetBrains decompiler
// Type: org.apache.crimson.tree.DomEx
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using java.util;
using org.w3c.dom;

namespace org.apache.crimson.tree
{
  [JavaFlags(32)]
  public class DomEx : DOMException
  {
    [JavaFlags(8)]
    public static string messageString(Locale locale, int code)
    {
      switch (code)
      {
        case 1:
          return XmlDocument.catalog.getMessage(locale, "D-000");
        case 2:
          return XmlDocument.catalog.getMessage(locale, "D-001");
        case 3:
          return XmlDocument.catalog.getMessage(locale, "D-002");
        case 4:
          return XmlDocument.catalog.getMessage(locale, "D-003");
        case 5:
          return XmlDocument.catalog.getMessage(locale, "D-004");
        case 6:
          return XmlDocument.catalog.getMessage(locale, "D-005");
        case 7:
          return XmlDocument.catalog.getMessage(locale, "D-006");
        case 8:
          return XmlDocument.catalog.getMessage(locale, "D-007");
        case 9:
          return XmlDocument.catalog.getMessage(locale, "D-008");
        case 10:
          return XmlDocument.catalog.getMessage(locale, "D-009");
        case 11:
          return XmlDocument.catalog.getMessage(locale, "D-010");
        case 12:
          return XmlDocument.catalog.getMessage(locale, "D-011");
        case 13:
          return XmlDocument.catalog.getMessage(locale, "D-012");
        case 14:
          return XmlDocument.catalog.getMessage(locale, "D-013");
        case 15:
          return XmlDocument.catalog.getMessage(locale, "D-014");
        default:
          return XmlDocument.catalog.getMessage(locale, "D-900");
      }
    }

    public DomEx(short code)
      : base(code, DomEx.messageString(Locale.getDefault(), (int) code))
    {
    }

    public DomEx(Locale locale, short code)
      : base(code, DomEx.messageString(locale, (int) code))
    {
    }
  }
}

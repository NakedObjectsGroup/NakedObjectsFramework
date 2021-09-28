// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.utility.Debug
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;

namespace org.nakedobjects.utility
{
  public sealed class Debug
  {
    public static string indentString(int indentSpaces) => StringImpl.substring("                                                                                                            ", 0, indentSpaces);

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      Debug debug = this;
      ObjectImpl.clone((object) debug);
      return ((object) debug).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}

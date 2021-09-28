// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.application.valueholder.Case
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using System.ComponentModel;

namespace org.nakedobjects.application.valueholder
{
  public class Case
  {
    public static readonly Case INSENSITIVE;
    public static readonly Case SENSITIVE;
    private string name;

    private Case(string name) => this.name = name;

    public override string ToString() => this.name;

    [EditorBrowsable(EditorBrowsableState.Never)]
    [JavaFlags(32778)]
    static Case()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      Case @case = this;
      ObjectImpl.clone((object) @case);
      return ((object) @case).MemberwiseClone();
    }
  }
}

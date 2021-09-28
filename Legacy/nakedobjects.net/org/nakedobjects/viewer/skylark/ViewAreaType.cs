// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.ViewAreaType
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using System.ComponentModel;

namespace org.nakedobjects.viewer.skylark
{
  public class ViewAreaType
  {
    public static readonly ViewAreaType VIEW;
    public static readonly ViewAreaType CONTENT;
    public static readonly ViewAreaType INTERNAL;
    private string name;

    public ViewAreaType(string name) => this.name = name;

    public override string ToString() => this.name;

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static ViewAreaType()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      ViewAreaType viewAreaType = this;
      ObjectImpl.clone((object) viewAreaType);
      return ((object) viewAreaType).MemberwiseClone();
    }
  }
}

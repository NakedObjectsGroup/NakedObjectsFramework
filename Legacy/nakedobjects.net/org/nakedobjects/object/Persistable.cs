// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.Persistable
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using org.nakedobjects.@object;
using System.ComponentModel;

namespace org.nakedobjects.@object
{
  public class Persistable
  {
    public static readonly Persistable IMMUTABLE;
    public static readonly Persistable PROGRAM_PERSISTABLE;
    public static readonly Persistable TRANSIENT;
    public static readonly Persistable USER_PERSISTABLE;
    private string name;

    private Persistable(string name) => this.name = name;

    public override string ToString() => this.name;

    [EditorBrowsable(EditorBrowsableState.Never)]
    [JavaFlags(32778)]
    static Persistable()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      Persistable persistable = this;
      ObjectImpl.clone((object) persistable);
      return ((object) persistable).MemberwiseClone();
    }
  }
}

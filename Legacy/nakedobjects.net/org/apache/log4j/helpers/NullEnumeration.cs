// Decompiled with JetBrains decompiler
// Type: org.apache.log4j.helpers.NullEnumeration
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.util;
using System.ComponentModel;

namespace org.apache.log4j.helpers
{
  [JavaInterfaces("1;java/util/Enumeration;")]
  public class NullEnumeration : Enumeration
  {
    private static readonly NullEnumeration instance;

    private NullEnumeration()
    {
    }

    public static NullEnumeration getInstance() => NullEnumeration.instance;

    public virtual bool hasMoreElements() => false;

    public virtual object nextElement() => throw new NoSuchElementException();

    [EditorBrowsable(EditorBrowsableState.Never)]
    [JavaFlags(32778)]
    static NullEnumeration()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      NullEnumeration nullEnumeration = this;
      ObjectImpl.clone((object) nullEnumeration);
      return ((object) nullEnumeration).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}

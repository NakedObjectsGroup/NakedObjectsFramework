// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.ConcurrencyException
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using java.lang;
using org.nakedobjects.@object;
using System;

namespace org.nakedobjects.@object
{
  public class ConcurrencyException : ObjectPerstsistenceException
  {
    private Oid source;

    [Obsolete(null, false)]
    public ConcurrencyException(string message)
      : base(message)
    {
    }

    public ConcurrencyException(string message, Oid source)
      : base(message)
    {
      this.source = source;
    }

    public ConcurrencyException(string message, Throwable cause)
      : base(message, cause)
    {
    }

    public virtual Oid getSource() => this.source;
  }
}

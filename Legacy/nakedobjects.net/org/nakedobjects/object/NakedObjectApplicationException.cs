// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.NakedObjectApplicationException
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using java.lang;
using org.nakedobjects.utility;
using System;

namespace org.nakedobjects.@object
{
  public class NakedObjectApplicationException : NakedObjectRuntimeException
  {
    private const long serialVersionUID = 5526281393674150252;

    public NakedObjectApplicationException()
    {
    }

    public NakedObjectApplicationException(string msg)
      : base(msg)
    {
    }

    public NakedObjectApplicationException(Throwable cause)
      : base(cause)
    {
    }

    public NakedObjectApplicationException(string msg, Throwable cause)
      : base(msg, cause)
    {
    }

    public NakedObjectApplicationException(string msg, Exception innerException)
      : base(msg, innerException)
    {
    }
  }
}

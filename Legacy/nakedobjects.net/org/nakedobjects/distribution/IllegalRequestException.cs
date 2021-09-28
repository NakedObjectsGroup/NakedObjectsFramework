// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.distribution.IllegalRequestException
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using java.lang;
using org.nakedobjects.utility;

namespace org.nakedobjects.distribution
{
  public class IllegalRequestException : NakedObjectRuntimeException
  {
    public IllegalRequestException()
    {
    }

    public IllegalRequestException(string msg, Throwable cause)
      : base(msg, cause)
    {
    }

    public IllegalRequestException(string msg)
      : base(msg)
    {
    }

    public IllegalRequestException(Throwable cause)
      : base(cause)
    {
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.cli.IllegalDispatchException
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using java.lang;
using org.nakedobjects.utility;

namespace org.nakedobjects.viewer.cli
{
  public class IllegalDispatchException : NakedObjectRuntimeException
  {
    public IllegalDispatchException()
    {
    }

    public IllegalDispatchException(string msg, Throwable cause)
      : base(msg, cause)
    {
    }

    public IllegalDispatchException(string msg)
      : base(msg)
    {
    }

    public IllegalDispatchException(Throwable cause)
      : base(cause)
    {
    }
  }
}

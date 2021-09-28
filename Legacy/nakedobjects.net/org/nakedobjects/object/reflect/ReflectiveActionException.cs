// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.reflect.ReflectiveActionException
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using java.lang;
using org.nakedobjects.utility;

namespace org.nakedobjects.@object.reflect
{
  public class ReflectiveActionException : NakedObjectRuntimeException
  {
    public ReflectiveActionException()
    {
    }

    public ReflectiveActionException(string msg)
      : base(msg)
    {
    }

    public ReflectiveActionException(Throwable cause)
      : base(cause)
    {
    }

    public ReflectiveActionException(string msg, Throwable cause)
      : base(msg, cause)
    {
    }
  }
}

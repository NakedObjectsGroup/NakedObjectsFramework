// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.utility.UnknownTypeException
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.lang;
using java.lang;

namespace org.nakedobjects.utility
{
  public class UnknownTypeException : NakedObjectRuntimeException
  {
    public UnknownTypeException()
    {
    }

    public UnknownTypeException(string message)
      : base(message)
    {
    }

    public UnknownTypeException(Throwable cause)
      : base(cause)
    {
    }

    public UnknownTypeException(string message, Throwable cause)
      : base(message, cause)
    {
    }

    public UnknownTypeException(object @object)
      : this(@object != null ? ObjectImpl.getClass(@object).getName() : "null")
    {
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.UnsupportedFindException
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using java.lang;
using org.nakedobjects.utility;

namespace org.nakedobjects.@object
{
  public class UnsupportedFindException : NakedObjectRuntimeException
  {
    public UnsupportedFindException()
    {
    }

    public UnsupportedFindException(string message)
      : base(message)
    {
    }

    public UnsupportedFindException(Throwable cause)
      : base(cause)
    {
    }

    public UnsupportedFindException(string message, Throwable cause)
      : base(message, cause)
    {
    }
  }
}

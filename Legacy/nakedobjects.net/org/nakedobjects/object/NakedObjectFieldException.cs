// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.NakedObjectFieldException
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using java.lang;
using org.nakedobjects.utility;

namespace org.nakedobjects.@object
{
  public class NakedObjectFieldException : NakedObjectRuntimeException
  {
    private const long serialVersionUID = 730819879598414522;

    public NakedObjectFieldException()
    {
    }

    public NakedObjectFieldException(string msg)
      : base(msg)
    {
    }

    public NakedObjectFieldException(string msg, Throwable cause)
      : base(msg, cause)
    {
    }

    public NakedObjectFieldException(Throwable cause)
      : base(cause)
    {
    }
  }
}

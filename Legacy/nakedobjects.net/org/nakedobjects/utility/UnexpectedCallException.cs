// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.utility.UnexpectedCallException
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

namespace org.nakedobjects.utility
{
  public class UnexpectedCallException : NakedObjectRuntimeException
  {
    private const long serialVersionUID = 1;

    public UnexpectedCallException()
      : base("This method call was not expected")
    {
    }

    public UnexpectedCallException(string arg0)
      : base(arg0)
    {
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.ObjectNotFoundException
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using java.lang;
using org.nakedobjects.@object;

namespace org.nakedobjects.@object
{
  public class ObjectNotFoundException : ObjectPerstsistenceException
  {
    public ObjectNotFoundException()
    {
    }

    public ObjectNotFoundException(object oid)
      : base(new StringBuffer().append("Object not found in store with oid ").append(oid).ToString())
    {
    }

    public ObjectNotFoundException(string s)
      : base(s)
    {
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.distribution.command.Response
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;

namespace org.nakedobjects.distribution.command
{
  public class Response
  {
    private int id;
    private object @object;

    public Response(Request request)
    {
      this.id = request.getId();
      this.@object = request.getResponse();
    }

    public virtual object getObject() => this.@object;

    public virtual int getId() => this.id;

    public override string ToString()
    {
      org.nakedobjects.utility.ToString toString = new org.nakedobjects.utility.ToString((object) this);
      toString.append("id", this.id);
      toString.append("object", this.@object);
      return toString.ToString();
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      Response response = this;
      ObjectImpl.clone((object) response);
      return ((object) response).MemberwiseClone();
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.defaults.SimpleUser
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using org.nakedobjects.@object;
using org.nakedobjects.@object.defaults;

namespace org.nakedobjects.@object.defaults
{
  [JavaInterfaces("1;org/nakedobjects/object/User;")]
  public class SimpleUser : User
  {
    private Naked @object;
    private string name;
    private readonly string id;

    public SimpleUser(string name, string id)
    {
      this.name = name;
      this.id = id;
    }

    public virtual string getName() => this.name;

    public virtual string getId() => this.id;

    public virtual void setName(string name) => this.name = name;

    public virtual void setRootObject(Naked @object) => this.@object = @object;

    public virtual Naked getObject() => this.@object;

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      SimpleUser simpleUser = this;
      ObjectImpl.clone((object) simpleUser);
      return ((object) simpleUser).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}

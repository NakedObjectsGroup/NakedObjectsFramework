// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.system.DefaultApplicationContext
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using org.nakedobjects.@object;
using org.nakedobjects.@object.defaults;

namespace org.nakedobjects.system
{
  public class DefaultApplicationContext : ApplicationContext
  {
    private string name;

    public virtual void setUpUsers(NakedCollection users)
    {
    }

    public override string getName() => this.name;

    public override NakedClass addClass(string className) => base.addClass(className);
  }
}

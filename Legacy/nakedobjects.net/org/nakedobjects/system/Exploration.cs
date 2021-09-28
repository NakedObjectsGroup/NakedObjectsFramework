// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.system.Exploration
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.nakedobjects.@object.fixture;
using org.nakedobjects.reflector.java.fixture;

namespace org.nakedobjects.system
{
  public class Exploration
  {
    private JavaFixtureBuilder builder;
    private NakedObjectsSystem system;
    private string title;

    public Exploration()
    {
      this.system = new NakedObjectsSystem();
      this.system.init();
      this.builder = new JavaFixtureBuilder();
    }

    public virtual void display()
    {
      this.builder.installFixtures();
      this.system.displayUserInterface(this.builder.getClasses(), this.title);
      this.system.clearSplash();
    }

    public virtual void addFixture(Fixture fixture) => this.builder.addFixture(fixture);

    public virtual void registerClass(Class cls) => this.builder.registerClass(cls.getName());

    public virtual object createInstance(Class cls) => this.builder.createInstance(cls.getName());

    public virtual void setTitle(string title) => this.title = title;

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      Exploration exploration = this;
      ObjectImpl.clone((object) exploration);
      return ((object) exploration).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}

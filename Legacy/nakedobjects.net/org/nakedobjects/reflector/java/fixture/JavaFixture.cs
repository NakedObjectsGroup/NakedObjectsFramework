// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.reflector.java.fixture.JavaFixture
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.apache.log4j;
using org.nakedobjects.@object.fixture;
using System;

namespace org.nakedobjects.reflector.java.fixture
{
  [JavaInterfaces("1;org/nakedobjects/object/fixture/Fixture;")]
  public abstract class JavaFixture : Fixture
  {
    private JavaFixtureBuilder builder;

    [JavaFlags(17)]
    public object createInstance(Class cls) => this.createInstance(cls.getName());

    [JavaFlags(17)]
    public object createInstance(string className) => this.builder.createInstance(className);

    [JavaFlags(17)]
    public void registerClass(string className) => this.builder.registerClass(className);

    [JavaFlags(17)]
    public void registerClass(Class cls) => this.builder.registerClass(cls.getName());

    public virtual FixtureBuilder getBuilder() => (FixtureBuilder) this.builder;

    public virtual void resetClock() => this.builder.resetClock();

    public virtual void setBuilder(FixtureBuilder builder) => this.builder = (JavaFixtureBuilder) builder;

    public virtual void setDate(int year, int month, int day) => this.builder.setDate(year, month, day);

    public virtual void setTime(int hour, int minute) => this.builder.setTime(hour, minute);

    [JavaThrownExceptions("1;java/lang/Throwable;")]
    [JavaFlags(4)]
    public override void Finalize()
    {
      try
      {
        // ISSUE: explicit finalizer call
        base.Finalize();
        Logger.getLogger(Class.FromType(typeof (JavaFixture))).info((object) "finalizing fixture");
      }
      catch (Exception ex)
      {
      }
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      JavaFixture javaFixture = this;
      ObjectImpl.clone((object) javaFixture);
      return ((object) javaFixture).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);

    public abstract void install();
  }
}

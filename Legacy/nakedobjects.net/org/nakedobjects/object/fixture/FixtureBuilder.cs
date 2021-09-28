// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.fixture.FixtureBuilder
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using java.util;
using org.apache.log4j;
using org.nakedobjects.@object;
using org.nakedobjects.@object.fixture;
using System.ComponentModel;

namespace org.nakedobjects.@object.fixture
{
  public abstract class FixtureBuilder
  {
    private static readonly Logger LOG;
    [JavaFlags(4)]
    public Vector classes;
    [JavaFlags(4)]
    public Vector fixtures;

    [JavaFlags(17)]
    public void addFixture(Fixture fixture) => this.fixtures.addElement((object) fixture);

    [JavaFlags(17)]
    public string[] getClasses()
    {
      int length = this.classes.size();
      string[] strArray = length >= 0 ? new string[length] : throw new NegativeArraySizeException();
      this.classes.copyInto((object[]) strArray);
      return strArray;
    }

    [JavaFlags(17)]
    public void installFixtures()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(4)]
    public virtual void postInstallFixtures(NakedObjectPersistor objectManager)
    {
    }

    [JavaFlags(4)]
    public virtual void preInstallFixtures(NakedObjectPersistor objectManager)
    {
    }

    [JavaFlags(1028)]
    public abstract void installFixture(NakedObjectPersistor objectManager, Fixture fixture);

    [JavaFlags(17)]
    public void registerClass(string className) => this.classes.addElement((object) className);

    public FixtureBuilder()
    {
      this.classes = new Vector();
      this.fixtures = new Vector();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [JavaFlags(32778)]
    static FixtureBuilder()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      FixtureBuilder fixtureBuilder = this;
      ObjectImpl.clone((object) fixtureBuilder);
      return ((object) fixtureBuilder).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}

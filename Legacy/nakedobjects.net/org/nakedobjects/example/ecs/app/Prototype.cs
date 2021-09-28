// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.example.ecs.app.Prototype
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using com.ms.vjsharp.util;
using org.nakedobjects.@object.fixture;
using org.nakedobjects.example.ecs.fixtures;
using org.nakedobjects.system;
using System.Runtime.CompilerServices;

namespace org.nakedobjects.example.ecs.app
{
  public class Prototype
  {
    public static void main(string[] args)
    {
      // ISSUE: type reference
      RuntimeHelpers.RunClassConstructor(__typeref (ObjectImpl));
      Exploration exploration = new Exploration();
      CitiesFixture cities;
      exploration.addFixture((Fixture) (cities = new CitiesFixture()));
      exploration.addFixture((Fixture) new BookingsFixture(cities));
      exploration.addFixture((Fixture) new ClassesFixture());
      exploration.display();
      Utilities.cleanupAfterMainReturns();
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      Prototype prototype = this;
      ObjectImpl.clone((object) prototype);
      return ((object) prototype).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}

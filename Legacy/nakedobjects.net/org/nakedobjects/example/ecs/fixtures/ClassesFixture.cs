// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.example.ecs.fixtures.ClassesFixture
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using java.lang;
using org.nakedobjects.reflector.java.fixture;

namespace org.nakedobjects.example.ecs.fixtures
{
  public class ClassesFixture : JavaFixture
  {
    public override void install()
    {
      this.registerClass(Class.FromType(typeof (Booking)));
      this.registerClass(Class.FromType(typeof (City)));
      this.registerClass(Class.FromType(typeof (Location)));
      this.registerClass(Class.FromType(typeof (CreditCard)));
      this.registerClass(Class.FromType(typeof (Customer)));
      this.registerClass(Class.FromType(typeof (Telephone)));
    }
  }
}

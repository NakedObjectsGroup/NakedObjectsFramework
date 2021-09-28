// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.example.ecs.fixtures.CitiesFixture
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using java.lang;
using org.nakedobjects.reflector.java.fixture;

namespace org.nakedobjects.example.ecs.fixtures
{
  public class CitiesFixture : JavaFixture
  {
    [JavaFlags(0)]
    public City boston;
    [JavaFlags(0)]
    public City newYork;
    [JavaFlags(0)]
    public City washington;

    public override void install()
    {
      int length1 = 7;
      string[] strArray1 = length1 >= 0 ? new string[length1] : throw new NegativeArraySizeException();
      strArray1[0] = "New York";
      strArray1[1] = "Boston";
      strArray1[2] = "Washington";
      strArray1[3] = "Chicago";
      strArray1[4] = "Tampa";
      strArray1[5] = "Seattle";
      strArray1[6] = "Atlanta";
      string[] strArray2 = strArray1;
      int length2 = strArray2.Length;
      City[] cityArray = length2 >= 0 ? new City[length2] : throw new NegativeArraySizeException();
      for (int index = 0; index < strArray2.Length; ++index)
      {
        cityArray[index] = (City) this.createInstance(Class.FromType(typeof (City)));
        cityArray[index].getName().setValue(strArray2[index]);
      }
      this.boston = cityArray[1];
      this.newYork = cityArray[0];
      this.washington = cityArray[2];
    }
  }
}

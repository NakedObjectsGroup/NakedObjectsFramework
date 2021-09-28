// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.example.ecs.fixtures.EcsFixture
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using java.lang;
using org.nakedobjects.reflector.java.fixture;

namespace org.nakedobjects.example.ecs.fixtures
{
  public class EcsFixture : JavaFixture
  {
    public override void install()
    {
      this.setupClasses();
      this.setupClock();
      this.setupObjects(this.setupCities());
      this.resetClock();
    }

    private void setupObjects(City[] cities)
    {
      Customer instance1 = (Customer) this.createInstance(Class.FromType(typeof (Customer)));
      instance1.getFirstName().setValue("Richard");
      instance1.getLastName().setValue("Pawson");
      Location instance2 = (Location) this.createInstance(Class.FromType(typeof (Location)));
      instance2.setCity(cities[1]);
      instance2.getKnownAs().setValue("Home");
      instance2.getStreetAddress().setValue("433 Pine St.");
      instance1.addToLocations(instance2);
      Location instance3 = (Location) this.createInstance(Class.FromType(typeof (Location)));
      instance3.setCity(cities[1]);
      instance3.getKnownAs().setValue("Office");
      instance3.getStreetAddress().setValue("944 Main St, Cambridge");
      instance1.addToLocations(instance3);
      Location instance4 = (Location) this.createInstance(Class.FromType(typeof (Location)));
      instance4.setCity(cities[0]);
      instance4.getKnownAs().setValue("QIC Headquaters");
      instance4.getStreetAddress().setValue("285 Park Avenue");
      instance1.addToLocations(instance4);
      Location instance5 = (Location) this.createInstance(Class.FromType(typeof (Location)));
      instance5.setCity(cities[0]);
      instance5.getStreetAddress().setValue("234 E 42nd Street");
      instance1.addToLocations(instance5);
      Location instance6 = (Location) this.createInstance(Class.FromType(typeof (Location)));
      instance6.setCity(cities[0]);
      instance6.getKnownAs().setValue("JFK Airport, BA Terminal");
      instance1.addToLocations(instance6);
      Telephone instance7 = (Telephone) this.createInstance(Class.FromType(typeof (Telephone)));
      instance7.getKnownAs().setValue("Home");
      instance7.getNumber().setValue("617/211 2899");
      instance1.getPhoneNumbers().addElement((object) instance7);
      Telephone instance8 = (Telephone) this.createInstance(Class.FromType(typeof (Telephone)));
      instance8.getKnownAs().setValue("Office");
      instance8.getNumber().setValue("617/353 9828");
      instance1.getPhoneNumbers().addElement((object) instance8);
      Telephone instance9 = (Telephone) this.createInstance(Class.FromType(typeof (Telephone)));
      instance9.getKnownAs().setValue("Mobile");
      instance9.getNumber().setValue("8777662671");
      instance1.getPhoneNumbers().addElement((object) instance9);
      CreditCard instance10 = (CreditCard) this.createInstance(Class.FromType(typeof (CreditCard)));
      instance10.getNumber().setValue("4525365234232233");
      instance10.getExpires().setValue("12/06");
      instance10.getNameOnCard().setValue("MR R Pawson");
      instance1.setPreferredPaymentMethod((PaymentMethod) instance10);
      Customer instance11 = (Customer) this.createInstance(Class.FromType(typeof (Customer)));
      instance11.getFirstName().setValue("Robert");
      instance11.getLastName().setValue("Matthews");
      Booking instance12 = (Booking) this.createInstance(Class.FromType(typeof (Booking)));
      instance12.associateCustomer(instance11);
      Location instance13 = (Location) this.createInstance(Class.FromType(typeof (Location)));
      instance13.setCity(cities[5]);
      instance13.getKnownAs().setValue("Home");
      instance13.getStreetAddress().setValue("1112 Condor St, Carlton Park");
      instance11.addToLocations(instance13);
      instance12.setPickUp(instance13);
      Location instance14 = (Location) this.createInstance(Class.FromType(typeof (Location)));
      instance14.setCity(cities[5]);
      instance14.getKnownAs().setValue("Office");
      instance14.getStreetAddress().setValue("299 Union St");
      instance11.addToLocations(instance14);
      Location instance15 = (Location) this.createInstance(Class.FromType(typeof (Location)));
      instance15.setCity(cities[0]);
      instance15.getKnownAs().setValue("Headquaters");
      instance15.getStreetAddress().setValue("285 Park Avenue");
      instance11.addToLocations(instance15);
      instance12.setDropOff(instance15);
      Telephone instance16 = (Telephone) this.createInstance(Class.FromType(typeof (Telephone)));
      instance16.getKnownAs().setValue("Home");
      instance16.getNumber().setValue("206/545 8444");
      instance11.getPhoneNumbers().addElement((object) instance16);
      instance12.setContactTelephone(instance16);
      Telephone instance17 = (Telephone) this.createInstance(Class.FromType(typeof (Telephone)));
      instance17.getKnownAs().setValue("Office");
      instance17.getNumber().setValue("206/234 443");
      instance11.getPhoneNumbers().addElement((object) instance17);
      CreditCard instance18 = (CreditCard) this.createInstance(Class.FromType(typeof (CreditCard)));
      instance18.getNumber().setValue("773829889938221");
      instance18.getExpires().setValue("10/04");
      instance18.getNameOnCard().setValue("MR R MATTHEWS");
      instance12.setPaymentMethod((PaymentMethod) instance18);
    }

    private City[] setupCities()
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
      return cityArray;
    }

    private void setupClock()
    {
      this.setDate(2003, 10, 23);
      this.setTime(20, 15);
    }

    private void setupClasses()
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

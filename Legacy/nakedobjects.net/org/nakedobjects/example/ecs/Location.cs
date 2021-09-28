// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.example.ecs.Location
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.io;
using java.lang;
using java.util;
using org.nakedobjects.application;
using org.nakedobjects.application.control;
using org.nakedobjects.application.valueholder;
using System;

namespace org.nakedobjects.example.ecs
{
  public class Location
  {
    private readonly TextString streetAddress;
    private readonly TextString knownAs;
    private City city;
    private Customer customer;
    private bool isDirty;
    [JavaFlags(130)]
    [NonSerialized]
    private BusinessObjectContainer container;
    private Option type;

    public virtual Option getType() => this.type;

    public Location()
    {
      int length = 3;
      string[] options = length >= 0 ? new string[length] : throw new NegativeArraySizeException();
      options[0] = "One";
      options[1] = "Two";
      options[2] = "Threee";
      this.type = new Option(options);
      this.streetAddress = new TextString();
      this.knownAs = new TextString();
    }

    public virtual void aboutActionNewBooking(ActionAbout about, Location location)
    {
      about.setDescription("Giving one location to another location creates a new booking going from the given location to the recieving location.");
      about.unusableOnCondition(this.Equals((object) location), "Two different locations are required");
      bool flag = this.getCity() != null && location != null && this.getCity().Equals((object) location.getCity());
      about.unusableOnCondition(((flag ? 1 : 0) ^ 1) != 0, "Locations must be in the same city");
    }

    public virtual void explorationActionExplorationMethod()
    {
      Vector vector1 = this.container.allInstances(Class.FromType(typeof (City)));
      if (vector1.size() > 0)
        this.city = (City) vector1.elementAt(0);
      Vector vector2 = this.container.allInstances(Class.FromType(typeof (Customer)));
      if (vector2.size() <= 0)
        return;
      this.customer = (Customer) vector2.elementAt(0);
    }

    public virtual void debugActionDebugMethod()
    {
      ((PrintStream) java.lang.System.@out).println((object) this);
      ((PrintStream) java.lang.System.@out).println(new StringBuffer().append("  ").append(this.knownAs.titleString()).ToString());
      ((PrintStream) java.lang.System.@out).println(new StringBuffer().append("  ").append(this.streetAddress.titleString()).ToString());
      ((PrintStream) java.lang.System.@out).println(new StringBuffer().append("  ").append((object) this.city.title()).ToString());
      ((PrintStream) java.lang.System.@out).println(new StringBuffer().append("  ").append((object) this.customer.title()).ToString());
    }

    public virtual Booking actionNewBooking(Location location)
    {
      Booking transientInstance = (Booking) this.container.createTransientInstance(Class.FromType(typeof (Booking)));
      Customer customer = location.getCustomer();
      transientInstance.setPickUp(location);
      transientInstance.setDropOff(this);
      transientInstance.setCity(location.getCity());
      this.container.makePersistent((object) transientInstance);
      if (customer != null)
      {
        transientInstance.associateCustomer(customer);
        transientInstance.setPaymentMethod(customer.getPreferredPaymentMethod());
      }
      return transientInstance;
    }

    public virtual Booking actionSlowAction()
    {
      try
      {
        Thread.sleep(2000L);
      }
      catch (InterruptedException ex)
      {
        ((Throwable) ex).printStackTrace();
      }
      Booking transientInstance = (Booking) this.container.createTransientInstance(Class.FromType(typeof (Booking)));
      transientInstance.setPickUp(this);
      transientInstance.setCity(this.getCity());
      this.container.makePersistent((object) transientInstance);
      Customer customer = this.getCustomer();
      if (customer != null)
      {
        transientInstance.associateCustomer(customer);
        transientInstance.setPaymentMethod(customer.getPreferredPaymentMethod());
      }
      return transientInstance;
    }

    public virtual void setContainer(BusinessObjectContainer container) => this.container = container;

    public virtual City getCity()
    {
      this.container.resolve((object) this, (object) this.city);
      return this.city;
    }

    public virtual Customer getCustomer()
    {
      this.container.resolve((object) this, (object) this.customer);
      return this.customer;
    }

    [JavaFlags(17)]
    public TextString getKnownAs() => this.knownAs;

    [JavaFlags(17)]
    public TextString getStreetAddress() => this.streetAddress;

    public virtual void setCity(City newCity)
    {
      this.city = newCity;
      this.isDirty = true;
    }

    public virtual bool isDirty() => this.isDirty;

    public virtual void clearDirty() => this.isDirty = false;

    public virtual void markDirty() => this.isDirty = true;

    public virtual void associateCustomer(Customer newCustomer) => newCustomer.addToLocations(this);

    public virtual void dissociateCustomer(Customer newCustomer) => newCustomer.removeFromLocations(this);

    public virtual void setCustomer(Customer newCustomer)
    {
      this.customer = newCustomer;
      this.isDirty = true;
    }

    public virtual Title title() => this.knownAs.isEmpty() ? this.streetAddress.title().append(",", (TitledObject) this.getCity()) : this.knownAs.title().append(",", (TitledObject) this.getCity());

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      Location location = this;
      ObjectImpl.clone((object) location);
      return ((object) location).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}

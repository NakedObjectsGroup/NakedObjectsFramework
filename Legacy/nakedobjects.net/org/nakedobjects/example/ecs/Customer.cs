// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.example.ecs.Customer
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using java.util;
using org.nakedobjects.application;
using org.nakedobjects.application.control;
using org.nakedobjects.application.valueholder;
using System;

namespace org.nakedobjects.example.ecs
{
  public class Customer
  {
    private readonly Vector bookings;
    [JavaFlags(130)]
    [NonSerialized]
    private BusinessObjectContainer container;
    private readonly TextString firstName;
    private Location home;
    private bool isChanged;
    private readonly TextString lastName;
    private readonly Vector locations;
    private Percentage membership;
    private readonly Vector phoneNumbers;
    private PaymentMethod preferredPaymentMethod;

    public static string fieldOrder() => "firstname, LAST name, phone numbers, locations, bOOkings";

    public Customer()
    {
      this.firstName = new TextString();
      this.lastName = new TextString();
      this.locations = new Vector();
      this.phoneNumbers = new Vector();
      this.bookings = new Vector();
      this.membership = new Percentage();
    }

    public virtual void aboutActionCreateBooking(
      ActionAbout about,
      Location from,
      Location to,
      TextString text,
      Date date)
    {
      about.setParameter(0, "Pick up");
      about.setParameter(1, "Drop off");
      about.setParameter(3, "Date");
      if (!this.getLocations().isEmpty())
        about.setParameter(0, this.getLocations().firstElement());
      text.ToString();
      date.ToString();
      about.setParameter(2, "Name", (object) new TextString("#23"), true);
    }

    public virtual Booking actionCreateBooking(
      Location from,
      Location to,
      TextString text,
      Date date)
    {
      Booking instance = (Booking) this.container.createInstance(Class.FromType(typeof (Booking)));
      instance.associateCustomer(this);
      instance.setPaymentMethod(this.getPreferredPaymentMethod());
      instance.setPickUp(from);
      instance.setDropOff(to);
      instance.getReference().setValue(text);
      instance.getDate().setValue(date);
      return instance;
    }

    public virtual Vector actionLocations()
    {
      Vector vector = new Vector();
      for (int index = 0; index < this.locations.size(); ++index)
        vector.addElement(this.locations.elementAt(index));
      return vector;
    }

    public virtual Booking actionUsePaymentMethod(PaymentMethod method)
    {
      Booking instance = (Booking) this.container.createInstance(Class.FromType(typeof (Booking)));
      instance.associateCustomer(this);
      instance.setPaymentMethod(method);
      return instance;
    }

    public virtual Booking actionNewBooking()
    {
      Booking instance = (Booking) this.container.createInstance(Class.FromType(typeof (Booking)));
      instance.associateCustomer(this);
      instance.setPaymentMethod(this.getPreferredPaymentMethod());
      return instance;
    }

    public virtual void addToBookings(Booking booking)
    {
      this.getBookings().addElement((object) booking);
      this.markDirty();
      booking.setCustomer(this);
    }

    public virtual void addToLocations(Location location)
    {
      if (this.locations.contains((object) location))
        return;
      this.locations.addElement((object) location);
      this.markDirty();
      location.setCustomer(this);
    }

    public virtual void addToPhoneNumbers(Telephone telephone)
    {
      this.phoneNumbers.addElement((object) telephone);
      this.markDirty();
    }

    public virtual void clearDirty() => this.isChanged = false;

    public virtual Booking createBooking()
    {
      Booking instance = (Booking) this.container.createInstance(Class.FromType(typeof (Booking)));
      instance.associateCustomer(this);
      instance.setPaymentMethod(this.getPreferredPaymentMethod());
      return instance;
    }

    [JavaFlags(17)]
    public Vector getBookings() => this.bookings;

    [JavaFlags(17)]
    public TextString getFirstName() => this.firstName;

    public virtual Location getHome()
    {
      this.container.resolve((object) this, (object) this.home);
      return this.home;
    }

    [JavaFlags(17)]
    public TextString getLastName() => this.lastName;

    [JavaFlags(17)]
    public Vector getLocations() => this.locations;

    public virtual Percentage getMembership() => this.membership;

    [JavaFlags(17)]
    public Vector getPhoneNumbers() => this.phoneNumbers;

    public virtual PaymentMethod getPreferredPaymentMethod()
    {
      this.container.resolve((object) this, (object) this.preferredPaymentMethod);
      return this.preferredPaymentMethod;
    }

    public virtual bool isDirty() => this.isChanged;

    public virtual void markDirty() => this.isChanged = true;

    public virtual void removeFromBookings(Booking booking)
    {
      this.getBookings().removeElement((object) booking);
      this.markDirty();
      booking.setCustomer((Customer) null);
    }

    public virtual void removeFromLocations(Location location)
    {
      this.locations.removeElement((object) location);
      this.markDirty();
      location.setCustomer((Customer) null);
    }

    public virtual void removeFromPhoneNumbers(Telephone telephone)
    {
      this.phoneNumbers.removeElement((object) telephone);
      this.markDirty();
    }

    public virtual void setContainer(BusinessObjectContainer container) => this.container = container;

    public virtual void setHome(Location home)
    {
      this.home = home;
      this.markDirty();
    }

    public virtual void setPreferredPaymentMethod(PaymentMethod method)
    {
      this.preferredPaymentMethod = method;
      this.isChanged = true;
    }

    public virtual Title title() => this.firstName.title().append(new StringBuffer().append((object) this.lastName).append("").ToString());

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      Customer customer = this;
      ObjectImpl.clone((object) customer);
      return ((object) customer).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}

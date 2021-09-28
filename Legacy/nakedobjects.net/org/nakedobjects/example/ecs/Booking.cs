// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.example.ecs.Booking
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.nakedobjects.application;
using org.nakedobjects.application.control;
using org.nakedobjects.application.valueholder;
using System;

namespace org.nakedobjects.example.ecs
{
  public class Booking
  {
    private readonly TextString reference;
    private readonly TextString status;
    private readonly Date date;
    private readonly Time time;
    private City city;
    private Customer customer;
    private Telephone contactTelephone;
    private Location pickUp;
    private Location dropOff;
    private PaymentMethod paymentMethod;
    private bool isChanged;
    [JavaFlags(130)]
    [NonSerialized]
    private BusinessObjectContainer container;

    public Booking()
    {
      this.reference = new TextString();
      this.status = new TextString();
      this.date = new Date();
      this.time = new Time();
    }

    public virtual void aboutActionSave(ActionAbout about)
    {
      about.unusableOnCondition(this.date.isEmpty(), "Must have a date specified");
      about.unusableOnCondition(this.pickUp == null, "Must have a pick up");
      about.unusableOnCondition(this.dropOff == null, "Must have a drop off");
    }

    public virtual void actionSave()
    {
      this.status.setValue("made persistent");
      this.container.makePersistent((object) this);
    }

    public virtual void aboutActionReturnBooking(ActionAbout about)
    {
      about.setDescription("Creates a new Booking based on the current booking.  The new booking has the pick up amd drop off locations reversed.");
      about.unusableOnCondition(((this.getStatus().isSameAs("Confirmed") ? 1 : 0) ^ 1) != 0, "Can only create a return based on a confirmed booking");
      about.unusableOnCondition(this.getPickUp() == null, "Pick Up location required");
      about.unusableOnCondition(this.getDropOff() == null, "Drop Off location required");
    }

    public virtual void aboutPickUp(FieldAbout about, Location newPickup)
    {
      about.setDescription("The location to pick up the customer from.");
      if (newPickup == null || this.getCity() == null)
        return;
      if (newPickup.Equals((object) this.getDropOff()))
      {
        about.unmodifiable("Pick up must differ from the drop off location");
      }
      else
      {
        bool conditionMet = ((this.getCity().Equals((object) newPickup.getCity()) ? 1 : 0) ^ 1) != 0;
        about.unmodifiableOnCondition(conditionMet, new StringBuffer().append("Location must be in ").append((object) this.getCity()).ToString());
      }
    }

    public virtual void aboutDropOff(FieldAbout about, Location newDropOff)
    {
      about.setDescription("The location to drop the customer off at.");
      if (newDropOff == null || this.getCity() == null)
        return;
      if (newDropOff.Equals((object) this.getPickUp()))
      {
        about.unmodifiable("Drop off must differ from the Pick up location");
      }
      else
      {
        bool conditionMet = ((this.getCity().Equals((object) newDropOff.getCity()) ? 1 : 0) ^ 1) != 0;
        about.unmodifiableOnCondition(conditionMet, new StringBuffer().append("Location must be in ").append((object) this.getCity()).ToString());
      }
    }

    public virtual void actionCheckAvailability()
    {
      this.getStatus().setValue("Available");
      this.isChanged = true;
    }

    public virtual void actionConfirm()
    {
      this.getStatus().setValue("Confirmed");
      this.getCustomer().addToLocations(this.getPickUp());
      this.getCustomer().addToLocations(this.getDropOff());
      if (this.getCustomer().getPreferredPaymentMethod() == null)
        this.getCustomer().setPreferredPaymentMethod(this.getPaymentMethod());
      this.isChanged = true;
    }

    public virtual void aboutActionSimilarBooking(
      ActionAbout about,
      Location pickup,
      Location dropoff)
    {
      about.setParameter(0, "From");
      about.setParameter(1, "To");
    }

    public virtual Booking actionSimilarBooking(
      Location pickup,
      Location dropoff,
      Logical flag)
    {
      Booking booking = new Booking();
      booking.setContainer(this.container);
      booking.created();
      booking.associateCustomer(this.getCustomer());
      booking.associatePickUp(pickup);
      booking.setDropOff(dropoff);
      booking.setPaymentMethod(this.getPaymentMethod());
      booking.setContactTelephone(this.getContactTelephone());
      if (!flag.isSet())
      {
        booking.getDate().clear();
        booking.getTime().clear();
      }
      this.container.makePersistent((object) booking);
      return booking;
    }

    public virtual Booking actionCopyBooking()
    {
      Booking transientInstance = (Booking) this.container.createTransientInstance(Class.FromType(typeof (Booking)));
      if (this.getCustomer() != null)
        transientInstance.associateCustomer(this.getCustomer());
      if (this.getPickUp() != null)
        transientInstance.setPickUp(this.getPickUp());
      if (this.getDropOff() != null)
        transientInstance.setDropOff(this.getDropOff());
      if (this.getPaymentMethod() != null)
        transientInstance.setPaymentMethod(this.getPaymentMethod());
      if (this.getContactTelephone() != null)
        transientInstance.setContactTelephone(this.getContactTelephone());
      this.container.makePersistent((object) transientInstance);
      return transientInstance;
    }

    public static Booking actionNewBooking(Customer customer) => customer.createBooking();

    public static Booking actionCreateBooking() => (Booking) null;

    public static string actionOrder() => "Check Availability, Confirm, Copy Booking, Return Booking";

    public virtual Booking actionReturnBooking()
    {
      Booking transientInstance = (Booking) this.container.createTransientInstance(Class.FromType(typeof (Booking)));
      transientInstance.associateCustomer(this.getCustomer());
      transientInstance.setPickUp(this.getDropOff());
      transientInstance.setDropOff(this.getPickUp());
      transientInstance.setPaymentMethod(this.getPaymentMethod());
      transientInstance.setContactTelephone(this.getContactTelephone());
      this.container.makePersistent((object) transientInstance);
      return transientInstance;
    }

    public virtual void associateCustomer(Customer customer) => customer.addToBookings(this);

    public virtual void associateDropOff(Location newDropOff)
    {
      this.setDropOff(newDropOff);
      this.setCity(newDropOff.getCity());
    }

    public virtual void associatePickUp(Location newPickUp)
    {
      if (newPickUp == null)
        return;
      this.setPickUp(newPickUp);
      this.setCity(newPickUp.getCity());
    }

    private long createBookingRef() => this.container.serialNumber("booking ref");

    public virtual void created()
    {
      this.status.setValue("New Booking");
      this.reference.setValue(new StringBuffer().append("#").append(this.createBookingRef()).ToString());
    }

    public virtual void dissociateCustomer(Customer customer) => customer.removeFromBookings(this);

    public static string fieldOrder() => "reference, status, customer, date, time, pick up, drop off, payment method";

    public virtual City getCity()
    {
      this.container.resolve((object) this, (object) this.city);
      return this.city;
    }

    public virtual Telephone getContactTelephone()
    {
      this.container.resolve((object) this, (object) this.contactTelephone);
      return this.contactTelephone;
    }

    public virtual Customer getCustomer()
    {
      this.container.resolve((object) this, (object) this.customer);
      return this.customer;
    }

    [JavaFlags(17)]
    public Date getDate() => this.date;

    public virtual void validDate(Validity validity)
    {
      validity.cannotBeEmpty();
      Date date = new Date();
      date.today();
      validity.invalidOnCondition(this.date.isLessThan((Magnitude) date), "Booking must be in the future");
    }

    public virtual Location getDropOff()
    {
      this.container.resolve((object) this, (object) this.dropOff);
      return this.dropOff;
    }

    public virtual PaymentMethod getPaymentMethod()
    {
      this.container.resolve((object) this, (object) this.paymentMethod);
      return this.paymentMethod;
    }

    public virtual void setContainer(BusinessObjectContainer container) => this.container = container;

    public virtual Location getPickUp()
    {
      this.container.resolve((object) this, (object) this.pickUp);
      return this.pickUp;
    }

    [JavaFlags(17)]
    public TextString getReference() => this.reference;

    [JavaFlags(17)]
    public TextString getStatus() => this.status;

    [JavaFlags(17)]
    public Time getTime() => this.time;

    public virtual void setCity(City newCity)
    {
      this.city = newCity;
      this.isChanged = true;
    }

    public virtual void setContactTelephone(Telephone newContactTelephone)
    {
      this.contactTelephone = newContactTelephone;
      this.isChanged = true;
    }

    public virtual void setCustomer(Customer newCustomer)
    {
      this.customer = newCustomer;
      this.isChanged = true;
    }

    public virtual void setDropOff(Location newDropOff)
    {
      this.dropOff = newDropOff;
      this.isChanged = true;
    }

    public virtual void setPaymentMethod(PaymentMethod newPaymentMethod)
    {
      this.paymentMethod = newPaymentMethod;
      this.isChanged = true;
    }

    public virtual void setPickUp(Location newPickUp)
    {
      this.pickUp = newPickUp;
      this.isChanged = true;
    }

    public virtual bool isDirty() => this.isChanged;

    public virtual void clearDirty() => this.isChanged = false;

    public virtual Title title() => this.reference.title().append(new StringBuffer().append((object) this.status).append("").ToString());

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      Booking booking = this;
      ObjectImpl.clone((object) booking);
      return ((object) booking).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}

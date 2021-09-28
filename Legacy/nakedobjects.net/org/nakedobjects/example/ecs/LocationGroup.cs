// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.example.ecs.LocationGroup
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using java.util;
using org.nakedobjects.application.collection;
using org.nakedobjects.application.valueholder;

namespace org.nakedobjects.example.ecs
{
  public class LocationGroup
  {
    private City city;
    private readonly Vector locations;
    private readonly TextString name;
    private readonly InternalCollection collection;

    public virtual InternalCollection getCollection() => this.collection;

    public virtual City getCity() => this.city;

    public virtual Vector getLocations() => this.locations;

    public virtual TextString getName() => this.name;

    public virtual void setCity(City city) => this.city = city;

    public virtual void addToLocations(Location location) => this.locations.addElement((object) location);

    public virtual void removeFromLocations(Location location) => this.locations.removeElement((object) location);

    public override string ToString() => this.name.stringValue();

    public LocationGroup()
    {
      this.locations = new Vector();
      this.name = new TextString();
      this.collection = new InternalCollection(Class.FromType(typeof (Location)).getName());
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      LocationGroup locationGroup = this;
      ObjectImpl.clone((object) locationGroup);
      return ((object) locationGroup).MemberwiseClone();
    }
  }
}

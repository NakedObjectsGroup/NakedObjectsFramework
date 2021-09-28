// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.example.ecs.City
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
  [JavaInterfaces("2;org/nakedobjects/application/Lookup;org/nakedobjects/application/TitledObject;")]
  public class City : Lookup, TitledObject
  {
    private readonly TextString name;
    [JavaFlags(130)]
    [NonSerialized]
    private BusinessObjectContainer container;

    public virtual void setContainer(BusinessObjectContainer container) => this.container = container;

    public virtual Location actionNewLocation()
    {
      Location instance = (Location) this.container.createInstance(Class.FromType(typeof (Location)));
      instance.setCity(this);
      return instance;
    }

    public virtual void aboutName(FieldAbout about)
    {
    }

    [JavaFlags(17)]
    public TextString getName() => this.name;

    public static string pluralName() => "Cities";

    public override string ToString() => this.name.titleString();

    public virtual string lookupDescription() => (string) null;

    public virtual bool isSelected(string text) => false;

    public virtual Title title() => new Title((TitledObject) this.name);

    public City() => this.name = new TextString();

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      City city = this;
      ObjectImpl.clone((object) city);
      return ((object) city).MemberwiseClone();
    }
  }
}

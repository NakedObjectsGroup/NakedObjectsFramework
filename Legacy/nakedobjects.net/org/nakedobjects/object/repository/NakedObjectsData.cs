// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.repository.NakedObjectsData
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.nakedobjects.@object;
using org.nakedobjects.@object.repository;
using org.nakedobjects.utility;

namespace org.nakedobjects.@object.repository
{
  public class NakedObjectsData
  {
    [JavaFlags(4)]
    public NakedObjectLoader objectLoader;
    [JavaFlags(4)]
    public NakedObjectConfiguration configuration;
    [JavaFlags(4)]
    public NakedObjectPersistor objectPersistor;
    [JavaFlags(4)]
    public NakedObjectSpecificationLoader specificationLoader;
    [JavaFlags(4)]
    public MessageBroker messageBroker;
    [JavaFlags(4)]
    public Session session;

    public override string ToString()
    {
      org.nakedobjects.utility.ToString toString = new org.nakedobjects.utility.ToString((object) this);
      toString.append("thread", (object) Thread.currentThread());
      toString.append("objectPersistor", (object) this.objectPersistor);
      toString.append("session", (object) this.session);
      toString.append("messageBroker", (object) this.messageBroker);
      toString.append("objectLoader", (object) this.objectLoader);
      return toString.ToString();
    }

    public NakedObjectsData() => this.messageBroker = (MessageBroker) new SimpleMessageBroker();

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      NakedObjectsData nakedObjectsData = this;
      ObjectImpl.clone((object) nakedObjectsData);
      return ((object) nakedObjectsData).MemberwiseClone();
    }
  }
}

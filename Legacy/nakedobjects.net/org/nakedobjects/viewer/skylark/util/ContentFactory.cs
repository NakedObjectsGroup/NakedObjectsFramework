// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.util.ContentFactory
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.nakedobjects.@object;
using org.nakedobjects.utility;

namespace org.nakedobjects.viewer.skylark.util
{
  public class ContentFactory
  {
    public virtual Content createRootContent(Naked @object)
    {
      Assert.assertNotNull((object) @object);
      switch (@object)
      {
        case NakedCollection _:
          return (Content) new RootCollection((NakedCollection) @object);
        case NakedObject _:
          return (Content) new RootObject((NakedObject) @object);
        default:
          throw new IllegalArgumentException(new StringBuffer().append("Must be an object or collection: ").append((object) @object).ToString());
      }
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      ContentFactory contentFactory = this;
      ObjectImpl.clone((object) contentFactory);
      return ((object) contentFactory).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}

// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.xat.TestCollectionImpl
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.nakedobjects.@object;

namespace org.nakedobjects.xat
{
  [JavaInterfaces("1;org/nakedobjects/xat/TestCollection;")]
  public class TestCollectionImpl : TestCollection
  {
    private NakedCollection collection;

    public TestCollectionImpl()
    {
    }

    public TestCollectionImpl(NakedCollection collection) => this.collection = collection;

    public virtual Naked getForNaked() => (Naked) this.collection;

    public virtual void setForNaked(Naked @object) => this.collection = @object is NakedCollection ? (NakedCollection) @object : throw new IllegalArgumentException("Object must be a NakedCollection");

    public virtual string getTitle() => "collection";

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      TestCollectionImpl testCollectionImpl = this;
      ObjectImpl.clone((object) testCollectionImpl);
      return ((object) testCollectionImpl).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}

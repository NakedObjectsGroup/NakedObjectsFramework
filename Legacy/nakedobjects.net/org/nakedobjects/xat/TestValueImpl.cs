// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.xat.TestValueImpl
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using org.nakedobjects.@object;
using org.nakedobjects.utility;

namespace org.nakedobjects.xat
{
  [JavaInterfaces("1;org/nakedobjects/xat/TestValue;")]
  public class TestValueImpl : TestValue
  {
    private NakedValue @object;

    public TestValueImpl(NakedValue @object) => this.@object = @object;

    public virtual void fieldEntry(string value) => throw new NotImplementedException();

    [JavaFlags(17)]
    public Naked getForNaked() => (Naked) this.@object;

    [JavaFlags(17)]
    public object getForObject() => this.@object.getObject();

    public virtual void setForNaked(Naked @object) => this.@object = (NakedValue) @object;

    public virtual string getTitle() => this.@object.titleString();

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      TestValueImpl testValueImpl = this;
      ObjectImpl.clone((object) testValueImpl);
      return ((object) testValueImpl).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}

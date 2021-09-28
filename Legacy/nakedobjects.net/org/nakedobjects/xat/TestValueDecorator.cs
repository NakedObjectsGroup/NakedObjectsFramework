// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.xat.TestValueDecorator
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using org.nakedobjects.@object;

namespace org.nakedobjects.xat
{
  [JavaInterfaces("1;org/nakedobjects/xat/TestValue;")]
  public class TestValueDecorator : TestValue
  {
    private readonly TestValue wrappedObject;

    public TestValueDecorator(TestValue wrappedObject) => this.wrappedObject = wrappedObject;

    public virtual void fieldEntry(string value) => this.wrappedObject.fieldEntry(value);

    public virtual Naked getForNaked() => this.wrappedObject.getForNaked();

    public virtual string getTitle() => this.wrappedObject.getTitle();

    public virtual void setForNaked(Naked @object) => this.wrappedObject.setForNaked(@object);

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      TestValueDecorator testValueDecorator = this;
      ObjectImpl.clone((object) testValueDecorator);
      return ((object) testValueDecorator).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}

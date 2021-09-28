// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.xat.TestNakedNullParameter
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.nakedobjects.@object;
using org.nakedobjects.utility;

namespace org.nakedobjects.xat
{
  [JavaFlags(32)]
  [JavaInterfaces("1;org/nakedobjects/xat/TestNaked;")]
  public class TestNakedNullParameter : TestNaked
  {
    private NakedObjectSpecification specification;

    public TestNakedNullParameter(Class cls) => this.specification = NakedObjects.getSpecificationLoader().loadSpecification(cls);

    public TestNakedNullParameter(string cls) => this.specification = NakedObjects.getSpecificationLoader().loadSpecification(cls);

    public virtual Naked getForNaked() => (Naked) null;

    public virtual void setForNaked(Naked @object) => throw new NakedObjectRuntimeException();

    public virtual NakedObjectSpecification getSpecification() => this.specification;

    public virtual string getTitle() => "empty";

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      TestNakedNullParameter nakedNullParameter = this;
      ObjectImpl.clone((object) nakedNullParameter);
      return ((object) nakedNullParameter).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}

// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.xat.ParameterValueImpl
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
  public class ParameterValueImpl : TestValue
  {
    private object @object;

    public ParameterValueImpl(object @object) => this.@object = @object;

    public virtual void fieldEntry(string value) => throw new NakedObjectRuntimeException();

    public virtual string getTitle() => StringImpl.toString(this.getForNaked().titleString());

    public virtual Naked getForNaked() => (Naked) NakedObjects.getObjectLoader().createAdapterForValue(this.@object);

    public virtual void setForNaked(Naked value) => throw new NakedObjectRuntimeException();

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      ParameterValueImpl parameterValueImpl = this;
      ObjectImpl.clone((object) parameterValueImpl);
      return ((object) parameterValueImpl).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}

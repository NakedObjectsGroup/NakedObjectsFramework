// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.reflector.java.Sequence
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using org.nakedobjects.application.valueholder;

namespace org.nakedobjects.reflector.java
{
  public class Sequence
  {
    private TextString name;
    private SerialNumber serial;

    public virtual void actionIncrement() => this.serial.next();

    public virtual SerialNumber getSerialNumber() => this.serial;

    public virtual TextString getName() => this.name;

    public Sequence()
    {
      this.name = new TextString();
      this.serial = new SerialNumber();
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      Sequence sequence = this;
      ObjectImpl.clone((object) sequence);
      return ((object) sequence).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}

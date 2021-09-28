// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.xat.NullDocumentor
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;

namespace org.nakedobjects.xat
{
  [JavaFlags(32)]
  [JavaInterfaces("1;org/nakedobjects/xat/Documentor;")]
  public class NullDocumentor : Documentor
  {
    public virtual void close()
    {
    }

    public virtual void doc(string text)
    {
    }

    public virtual void docln(string text)
    {
    }

    public virtual void flush()
    {
    }

    public virtual void start()
    {
    }

    public virtual void stop()
    {
    }

    public virtual void step(string @string)
    {
    }

    public virtual void subtitle(string text)
    {
    }

    public virtual void title(string text)
    {
    }

    [JavaFlags(0)]
    public NullDocumentor()
    {
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      NullDocumentor nullDocumentor = this;
      ObjectImpl.clone((object) nullDocumentor);
      return ((object) nullDocumentor).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}

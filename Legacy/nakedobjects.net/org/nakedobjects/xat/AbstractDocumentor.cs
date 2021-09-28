// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.xat.AbstractDocumentor
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;

namespace org.nakedobjects.xat
{
  [JavaInterfaces("1;org/nakedobjects/xat/Documentor;")]
  public abstract class AbstractDocumentor : Documentor
  {
    private bool isGenerating;

    public virtual bool isGenerating() => this.isGenerating;

    public virtual void start() => this.isGenerating = true;

    public virtual void stop() => this.isGenerating = false;

    [JavaFlags(4)]
    public virtual string makeTitle(string name)
    {
      int num = 0;
      while (num < StringImpl.length(name) && Character.isLowerCase(StringImpl.charAt(name, num)))
        ++num;
      if (num == StringImpl.length(name))
        return "invalid name";
      StringBuffer stringBuffer = new StringBuffer(StringImpl.length(name) - num);
      for (int index = num; index < StringImpl.length(name); ++index)
      {
        if (index > num && Character.isUpperCase(StringImpl.charAt(name, index)))
          stringBuffer.append(' ');
        stringBuffer.append(StringImpl.charAt(name, index));
      }
      return stringBuffer.ToString();
    }

    public AbstractDocumentor() => this.isGenerating = true;

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      AbstractDocumentor abstractDocumentor = this;
      ObjectImpl.clone((object) abstractDocumentor);
      return ((object) abstractDocumentor).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);

    public abstract void close();

    public abstract void doc(string text);

    public abstract void docln(string text);

    public abstract void flush();

    public abstract void step(string @string);

    public abstract void subtitle(string text);

    public abstract void title(string text);
  }
}

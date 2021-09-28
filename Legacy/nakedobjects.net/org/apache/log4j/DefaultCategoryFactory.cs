// Decompiled with JetBrains decompiler
// Type: org.apache.log4j.DefaultCategoryFactory
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using org.apache.log4j.spi;

namespace org.apache.log4j
{
  [JavaInterfaces("1;org/apache/log4j/spi/LoggerFactory;")]
  [JavaFlags(32)]
  public class DefaultCategoryFactory : LoggerFactory
  {
    [JavaFlags(0)]
    public DefaultCategoryFactory()
    {
    }

    public virtual Logger makeNewLoggerInstance(string name) => new Logger(name);

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      DefaultCategoryFactory defaultCategoryFactory = this;
      ObjectImpl.clone((object) defaultCategoryFactory);
      return ((object) defaultCategoryFactory).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}

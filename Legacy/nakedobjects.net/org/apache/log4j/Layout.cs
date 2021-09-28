// Decompiled with JetBrains decompiler
// Type: org.apache.log4j.Layout
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using org.apache.log4j.spi;
using System.ComponentModel;

namespace org.apache.log4j
{
  [JavaInterfaces("1;org/apache/log4j/spi/OptionHandler;")]
  public abstract class Layout : OptionHandler
  {
    public static readonly string LINE_SEP;
    public static readonly int LINE_SEP_LEN;

    public abstract string format(LoggingEvent @event);

    public virtual string getContentType() => "text/plain";

    public virtual string getHeader() => (string) null;

    public virtual string getFooter() => (string) null;

    public abstract bool ignoresThrowable();

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static Layout()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      Layout layout = this;
      ObjectImpl.clone((object) layout);
      return ((object) layout).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);

    public abstract void activateOptions();
  }
}

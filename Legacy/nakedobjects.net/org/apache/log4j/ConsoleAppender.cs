// Decompiled with JetBrains decompiler
// Type: org.apache.log4j.ConsoleAppender
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.io;
using java.lang;
using org.apache.log4j.helpers;

namespace org.apache.log4j
{
  public class ConsoleAppender : WriterAppender
  {
    public const string SYSTEM_OUT = "System.out";
    public const string SYSTEM_ERR = "System.err";
    [JavaFlags(4)]
    public string target;

    public ConsoleAppender() => this.target = "System.out";

    public ConsoleAppender(Layout layout)
      : this(layout, "System.out")
    {
    }

    public ConsoleAppender(Layout layout, string target)
    {
      this.target = "System.out";
      this.layout = layout;
      if (StringImpl.equals("System.out", (object) target))
        this.setWriter((Writer) new OutputStreamWriter((OutputStream) System.@out));
      else if (StringImpl.equalsIgnoreCase("System.err", target))
        this.setWriter((Writer) new OutputStreamWriter((OutputStream) System.err));
      else
        this.targetWarn(target);
    }

    public virtual void setTarget(string value)
    {
      string str = StringImpl.trim(value);
      if (StringImpl.equalsIgnoreCase("System.out", str))
        this.target = "System.out";
      else if (StringImpl.equalsIgnoreCase("System.err", str))
        this.target = "System.err";
      else
        this.targetWarn(value);
    }

    public virtual string getTarget() => this.target;

    [JavaFlags(0)]
    public virtual void targetWarn(string val)
    {
      LogLog.warn(new StringBuffer().append("[").append(val).append("] should be System.out or System.err.").ToString());
      LogLog.warn("Using previously set target, System.out by default.");
    }

    public override void activateOptions()
    {
      if (StringImpl.equals(this.target, (object) "System.out"))
        this.setWriter((Writer) new OutputStreamWriter((OutputStream) System.@out));
      else
        this.setWriter((Writer) new OutputStreamWriter((OutputStream) System.err));
    }

    [JavaFlags(20)]
    public override sealed void closeWriter()
    {
    }
  }
}

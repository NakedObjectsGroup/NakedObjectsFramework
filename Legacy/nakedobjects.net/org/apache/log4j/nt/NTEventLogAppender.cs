// Decompiled with JetBrains decompiler
// Type: org.apache.log4j.nt.NTEventLogAppender
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.apache.log4j.spi;
using System;
using System.ComponentModel;

namespace org.apache.log4j.nt
{
  public class NTEventLogAppender : AppenderSkeleton
  {
    private int _handle;
    private string source;
    private string server;
    private static readonly int FATAL;
    private static readonly int ERROR;
    private static readonly int WARN;
    private static readonly int INFO;
    private static readonly int DEBUG;

    public NTEventLogAppender()
      : this((string) null, (string) null, (Layout) null)
    {
    }

    public NTEventLogAppender(string source)
      : this((string) null, source, (Layout) null)
    {
    }

    public NTEventLogAppender(string server, string source)
      : this(server, source, (Layout) null)
    {
    }

    public NTEventLogAppender(Layout layout)
      : this((string) null, (string) null, layout)
    {
    }

    public NTEventLogAppender(string source, Layout layout)
      : this((string) null, source, layout)
    {
    }

    public NTEventLogAppender(string server, string source, Layout layout)
    {
      // ISSUE: unable to decompile the method.
    }

    public override void close()
    {
    }

    public override void activateOptions()
    {
      // ISSUE: unable to decompile the method.
    }

    public override void append(LoggingEvent @event)
    {
      StringBuffer stringBuffer = new StringBuffer();
      stringBuffer.append(this.layout.format(@event));
      if (this.layout.ignoresThrowable())
      {
        string[] throwableStrRep = @event.getThrowableStrRep();
        if (throwableStrRep != null)
        {
          int length = throwableStrRep.Length;
          for (int index = 0; index < length; ++index)
            stringBuffer.append(throwableStrRep[index]);
        }
      }
      int level = @event.getLevel().toInt();
      this.reportEvent(this._handle, stringBuffer.ToString(), level);
    }

    public override void Finalize()
    {
      try
      {
        this.deregisterEventSource(this._handle);
        this._handle = 0;
      }
      catch (Exception ex)
      {
      }
    }

    public virtual void setSource(string source) => this.source = StringImpl.trim(source);

    public virtual string getSource() => this.source;

    public override bool requiresLayout() => true;

    [JavaFlags(258)]
    private int registerEventSource(string server, string source) => throw new UnsatisfiedLinkError(" native method should be Static");

    [JavaFlags(258)]
    private void reportEvent(int handle, string message, int level) => throw new UnsatisfiedLinkError(" native method should be Static");

    [JavaFlags(258)]
    private void deregisterEventSource(int handle) => throw new UnsatisfiedLinkError(" native method should be Static");

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static NTEventLogAppender()
    {
      // ISSUE: unable to decompile the method.
    }
  }
}

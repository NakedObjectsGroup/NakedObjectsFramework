// Decompiled with JetBrains decompiler
// Type: org.apache.log4j.WriterAppender
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using java.io;
using java.lang;
using org.apache.log4j.helpers;
using org.apache.log4j.spi;
using System.Runtime.CompilerServices;

namespace org.apache.log4j
{
  public class WriterAppender : AppenderSkeleton
  {
    [JavaFlags(4)]
    public bool immediateFlush;
    [JavaFlags(4)]
    public string encoding;
    [JavaFlags(4)]
    public QuietWriter qw;

    public WriterAppender() => this.immediateFlush = true;

    public WriterAppender(Layout layout, OutputStream os)
      : this(layout, (Writer) new OutputStreamWriter(os))
    {
    }

    public WriterAppender(Layout layout, Writer writer)
    {
      this.immediateFlush = true;
      this.layout = layout;
      this.setWriter(writer);
    }

    public virtual void setImmediateFlush(bool value) => this.immediateFlush = value;

    public virtual bool getImmediateFlush() => this.immediateFlush;

    public override void activateOptions()
    {
    }

    public override void append(LoggingEvent @event)
    {
      if (!this.checkEntryConditions())
        return;
      this.subAppend(@event);
    }

    [JavaFlags(4)]
    public virtual bool checkEntryConditions()
    {
      if (this.closed)
      {
        LogLog.warn("Not allowed to write to a closed appender.");
        return false;
      }
      if (this.qw == null)
      {
        this.errorHandler.error(new StringBuffer().append("No output stream or file set for the appender named [").append(this.name).append("].").ToString());
        return false;
      }
      if (this.layout != null)
        return true;
      this.errorHandler.error(new StringBuffer().append("No layout set for the appender named [").append(this.name).append("].").ToString());
      return false;
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public override void close()
    {
      if (this.closed)
        return;
      this.closed = true;
      this.writeFooter();
      this.reset();
    }

    [JavaFlags(4)]
    public virtual void closeWriter()
    {
      if (this.qw == null)
        return;
      try
      {
        this.qw.close();
      }
      catch (IOException ex)
      {
        LogLog.error(new StringBuffer().append("Could not close ").append((object) this.qw).ToString(), (Throwable) ex);
      }
    }

    [JavaFlags(4)]
    public virtual OutputStreamWriter createWriter(OutputStream os)
    {
      OutputStreamWriter outputStreamWriter = (OutputStreamWriter) null;
      string encoding = this.getEncoding();
      if (encoding != null)
      {
        try
        {
          outputStreamWriter = new OutputStreamWriter(os, encoding);
        }
        catch (IOException ex)
        {
          LogLog.warn("Error initializing output writer.");
          LogLog.warn("Unsupported encoding?");
        }
      }
      if (outputStreamWriter == null)
        outputStreamWriter = new OutputStreamWriter(os);
      return outputStreamWriter;
    }

    public virtual string getEncoding() => this.encoding;

    public virtual void setEncoding(string value) => this.encoding = value;

    [MethodImpl(MethodImplOptions.Synchronized)]
    public override void setErrorHandler(ErrorHandler eh)
    {
      if (eh == null)
      {
        LogLog.warn("You have tried to set a null error-handler.");
      }
      else
      {
        this.errorHandler = eh;
        if (this.qw == null)
          return;
        this.qw.setErrorHandler(eh);
      }
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public virtual void setWriter(Writer writer)
    {
      this.reset();
      this.qw = new QuietWriter(writer, this.errorHandler);
      this.writeHeader();
    }

    [JavaFlags(4)]
    public virtual void subAppend(LoggingEvent @event)
    {
      this.qw.write(this.layout.format(@event));
      if (this.layout.ignoresThrowable())
      {
        string[] throwableStrRep = @event.getThrowableStrRep();
        if (throwableStrRep != null)
        {
          int length = throwableStrRep.Length;
          for (int index = 0; index < length; ++index)
          {
            this.qw.write(throwableStrRep[index]);
            this.qw.write(Layout.LINE_SEP);
          }
        }
      }
      if (!this.immediateFlush)
        return;
      this.qw.flush();
    }

    public override bool requiresLayout() => true;

    [JavaFlags(4)]
    public virtual void reset()
    {
      this.closeWriter();
      this.qw = (QuietWriter) null;
    }

    [JavaFlags(4)]
    public virtual void writeFooter()
    {
      if (this.layout == null)
        return;
      string footer = this.layout.getFooter();
      if (footer == null || this.qw == null)
        return;
      this.qw.write(footer);
      this.qw.flush();
    }

    [JavaFlags(4)]
    public virtual void writeHeader()
    {
      if (this.layout == null)
        return;
      string header = this.layout.getHeader();
      if (header == null || this.qw == null)
        return;
      this.qw.write(header);
    }
  }
}

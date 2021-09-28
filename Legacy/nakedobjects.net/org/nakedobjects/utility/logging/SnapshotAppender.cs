// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.utility.logging.SnapshotAppender
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using java.util;
using org.apache.log4j;
using org.apache.log4j.helpers;
using org.apache.log4j.spi;
using System.Runtime.CompilerServices;

namespace org.nakedobjects.utility.logging
{
  public abstract class SnapshotAppender : AppenderSkeleton
  {
    private int bufferSize;
    [JavaFlags(4)]
    public CyclicBuffer buffer;
    private bool locationInfo;
    [JavaFlags(4)]
    public TriggeringEventEvaluator triggerEvaluator;
    private bool addInfo;

    public SnapshotAppender()
      : this((TriggeringEventEvaluator) new DefaultEvaluator())
    {
    }

    public SnapshotAppender(TriggeringEventEvaluator evaluator)
    {
      this.bufferSize = 512;
      this.buffer = new CyclicBuffer(this.bufferSize);
      this.locationInfo = false;
      this.triggerEvaluator = evaluator;
    }

    public override void append(LoggingEvent @event)
    {
      if (!this.shouldAppend())
        return;
      @event.getThreadName();
      @event.getNDC();
      if (this.locationInfo)
        @event.getLocationInformation();
      this.buffer.add(@event);
      if (!this.triggerEvaluator.isTriggeringEvent(@event))
        return;
      this.writeSnapshot(this.buffer);
    }

    public virtual void forceSnapshot() => this.writeSnapshot(this.buffer);

    private void writeSnapshot(CyclicBuffer buffer)
    {
      StringBuffer stringBuffer = new StringBuffer();
      string header = this.layout.getHeader();
      if (header != null)
        stringBuffer.append(header);
      if (this.addInfo)
      {
        string property = java.lang.System.getProperty("user.name");
        string str1 = new StringBuffer().append(java.lang.System.getProperty("os.name")).append(" (").append(java.lang.System.getProperty("os.arch")).append(") ").append(java.lang.System.getProperty("os.version")).ToString();
        string str2 = new StringBuffer().append(java.lang.System.getProperty("java.vm.name")).append(" ").append(java.lang.System.getProperty("java.vm.version")).ToString();
        string str3 = new StringBuffer().append(AboutNakedObjects.getFrameworkVersion()).append(" ").append(AboutNakedObjects.getFrameworkBuild()).ToString();
        LoggingEvent @event = new LoggingEvent("", (Category) org.apache.log4j.Logger.getRootLogger(), (Priority) Level.INFO, (object) new StringBuffer().append("Snapshot:- ").append((object) new Date()).append("\n\t").append(property).append("\n\t").append(str1).append("\n\t").append(str2).append("\n\t").append(str3).ToString(), (Throwable) null);
        stringBuffer.append(this.layout.format(@event));
      }
      int num = buffer.length();
      string message = "";
      for (int index1 = 0; index1 < num; ++index1)
      {
        LoggingEvent @event = buffer.get();
        message = new StringBuffer().append(@event.getLoggerName()).append(": ").append(@event.getMessage()).ToString();
        stringBuffer.append(this.layout.format(@event));
        if (this.layout.ignoresThrowable())
        {
          string[] throwableStrRep = @event.getThrowableStrRep();
          if (throwableStrRep != null)
          {
            for (int index2 = 0; index2 < throwableStrRep.Length; ++index2)
            {
              stringBuffer.append(throwableStrRep[index2]);
              stringBuffer.append('\n');
            }
          }
        }
      }
      string footer = this.layout.getFooter();
      if (footer != null)
        stringBuffer.append(footer);
      this.writeSnapshot(message, stringBuffer.ToString());
    }

    [JavaFlags(1028)]
    public abstract void writeSnapshot(string message, string details);

    [MethodImpl(MethodImplOptions.Synchronized)]
    public override void close() => this.closed = true;

    public virtual int getBufferSize() => this.bufferSize;

    public virtual string getEvaluatorClass() => this.triggerEvaluator == null ? (string) null : ObjectImpl.getClass((object) this.triggerEvaluator).getName();

    public virtual bool getLocationInfo() => this.locationInfo;

    public override bool requiresLayout() => true;

    public virtual void setBufferSize(int bufferSize)
    {
      this.bufferSize = bufferSize;
      this.buffer.resize(bufferSize);
    }

    public virtual void setEvaluatorClass(string value) => this.triggerEvaluator = (TriggeringEventEvaluator) OptionConverter.instantiateByClassName(value, Class.FromType(typeof (TriggeringEventEvaluator)), (object) this.triggerEvaluator);

    public virtual void setAddInfo(bool addInfo) => this.addInfo = addInfo;

    public virtual void setLocationInfo(bool locationInfo) => this.locationInfo = locationInfo;

    [JavaFlags(4)]
    public virtual bool shouldAppend()
    {
      if (this.triggerEvaluator == null)
      {
        this.errorHandler.error(new StringBuffer().append("No TriggeringEventEvaluator is set for appender [").append(this.name).append("].").ToString());
        return false;
      }
      if (this.layout != null)
        return true;
      this.errorHandler.error(new StringBuffer().append("No layout set for appender named [").append(this.name).append("].").ToString());
      return false;
    }
  }
}

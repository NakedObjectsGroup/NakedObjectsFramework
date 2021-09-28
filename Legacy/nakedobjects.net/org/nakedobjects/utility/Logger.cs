// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.utility.Logger
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.io;
using java.lang;
using java.text;
using java.util;

namespace org.nakedobjects.utility
{
  public abstract class Logger
  {
    private string fileName;
    private DateFormat format;
    private bool logAlso;
    private org.apache.log4j.Logger logger;
    private bool showTime;
    private readonly long start;
    private PrintStream stream;

    public Logger()
    {
      this.start = this.time();
      this.logAlso = true;
      this.showTime = true;
      this.format = (DateFormat) new SimpleDateFormat("yyyyMMdd-hhmm-ss.SSS");
    }

    public Logger(string fileName, bool logAlso)
      : this()
    {
      this.fileName = fileName;
      this.logAlso = logAlso;
    }

    public virtual void close()
    {
      if (this.stream == null)
        return;
      this.stream.close();
    }

    [JavaFlags(1028)]
    public abstract Class getDecoratedClass();

    public virtual bool isLogToFile() => this.fileName != null;

    public virtual bool isLogToLog4j() => this.logAlso;

    public virtual void log(string message)
    {
      if (this.logAlso)
        this.logger().info((object) message);
      if (this.fileName == null)
        return;
      if (this.stream == null)
      {
        try
        {
          if (this.fileName == null)
            return;
          this.stream = new PrintStream((OutputStream) new FileOutputStream(this.fileName));
        }
        catch (IOException ex)
        {
          ((Throwable) ex).printStackTrace();
        }
      }
      if (this.showTime)
        this.stream.print(this.format.format(new Date(this.time())));
      else
        this.stream.print(this.time() - this.start);
      this.stream.print("  ");
      this.stream.println(message);
      this.stream.flush();
    }

    public virtual void log(string request, object result) => this.log(new StringBuffer().append(request).append("  -> ").append(result).ToString());

    private org.apache.log4j.Logger logger()
    {
      if (this.logger == null)
        this.logger = org.apache.log4j.Logger.getLogger(this.getDecoratedClass());
      return this.logger;
    }

    public virtual void setFileName(string fileName) => this.fileName = fileName;

    public virtual void setLogAlso(bool logAlso) => this.logAlso = logAlso;

    public virtual void setShowTime(bool showTime) => this.showTime = showTime;

    public virtual void setTimeFormat(string format) => this.format = (DateFormat) new SimpleDateFormat(format);

    private long time() => System.currentTimeMillis();

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      Logger logger = this;
      ObjectImpl.clone((object) logger);
      return ((object) logger).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}

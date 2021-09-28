// Decompiled with JetBrains decompiler
// Type: org.apache.log4j.helpers.FileWatchdog
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.io;
using java.lang;

namespace org.apache.log4j.helpers
{
  public abstract class FileWatchdog : Thread
  {
    public const long DEFAULT_DELAY = 60000;
    [JavaFlags(4)]
    public string filename;
    [JavaFlags(4)]
    public long delay;
    [JavaFlags(0)]
    public File file;
    [JavaFlags(0)]
    public long lastModif;
    [JavaFlags(0)]
    public bool warnedAlready;
    [JavaFlags(0)]
    public bool interrupted;

    [JavaFlags(4)]
    public FileWatchdog(string filename)
    {
      this.delay = 60000L;
      this.lastModif = 0L;
      this.warnedAlready = false;
      this.interrupted = false;
      this.filename = filename;
      this.file = new File(filename);
      this.setDaemon(true);
      this.checkAndConfigure();
    }

    public virtual void setDelay(long delay) => this.delay = delay;

    [JavaFlags(1028)]
    public abstract void doOnChange();

    [JavaFlags(4)]
    public virtual void checkAndConfigure()
    {
      // ISSUE: unable to decompile the method.
    }

    public virtual void run()
    {
      while (!this.interrupted)
      {
        try
        {
          Thread.currentThread();
          Thread.sleep(this.delay);
        }
        catch (InterruptedException ex)
        {
        }
        this.checkAndConfigure();
      }
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public virtual object MemberwiseClone()
    {
      FileWatchdog fileWatchdog = this;
      ObjectImpl.clone((object) fileWatchdog);
      return ((object) fileWatchdog).MemberwiseClone();
    }
  }
}

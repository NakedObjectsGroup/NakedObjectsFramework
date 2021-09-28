// Decompiled with JetBrains decompiler
// Type: org.apache.log4j.RollingFileAppender
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
  public class RollingFileAppender : FileAppender
  {
    [JavaFlags(4)]
    public long maxFileSize;
    [JavaFlags(4)]
    public int maxBackupIndex;

    public RollingFileAppender()
    {
      this.maxFileSize = 10485760L;
      this.maxBackupIndex = 1;
    }

    [JavaThrownExceptions("1;java/io/IOException;")]
    public RollingFileAppender(Layout layout, string filename, bool append)
      : base(layout, filename, append)
    {
      this.maxFileSize = 10485760L;
      this.maxBackupIndex = 1;
    }

    [JavaThrownExceptions("1;java/io/IOException;")]
    public RollingFileAppender(Layout layout, string filename)
      : base(layout, filename)
    {
      this.maxFileSize = 10485760L;
      this.maxBackupIndex = 1;
    }

    public virtual int getMaxBackupIndex() => this.maxBackupIndex;

    public virtual long getMaximumFileSize() => this.maxFileSize;

    public virtual void rollOver()
    {
      LogLog.debug(new StringBuffer().append("rolling over count=").append(((CountingQuietWriter) this.qw).getCount()).ToString());
      LogLog.debug(new StringBuffer().append("maxBackupIndex=").append(this.maxBackupIndex).ToString());
      if (this.maxBackupIndex > 0)
      {
        File file1 = new File(new StringBuffer().append(this.fileName).append('.').append(this.maxBackupIndex).ToString());
        if (file1.exists())
          file1.delete();
        for (int index = this.maxBackupIndex - 1; index >= 1; index += -1)
        {
          File file2 = new File(new StringBuffer().append(this.fileName).append(".").append(index).ToString());
          if (file2.exists())
          {
            File file3 = new File(new StringBuffer().append(this.fileName).append('.').append(index + 1).ToString());
            LogLog.debug(new StringBuffer().append("Renaming file ").append((object) file2).append(" to ").append((object) file3).ToString());
            file2.renameTo(file3);
          }
        }
        File file4 = new File(new StringBuffer().append(this.fileName).append(".").append(1).ToString());
        this.closeFile();
        File file5 = new File(this.fileName);
        LogLog.debug(new StringBuffer().append("Renaming file ").append((object) file5).append(" to ").append((object) file4).ToString());
        file5.renameTo(file4);
      }
      try
      {
        this.setFile(this.fileName, false, this.bufferedIO, this.bufferSize);
      }
      catch (IOException ex)
      {
        LogLog.error(new StringBuffer().append("setFile(").append(this.fileName).append(", false) call failed.").ToString(), (Throwable) ex);
      }
    }

    [JavaThrownExceptions("1;java/io/IOException;")]
    [MethodImpl(MethodImplOptions.Synchronized)]
    public override void setFile(string fileName, bool append, bool bufferedIO, int bufferSize)
    {
      base.setFile(fileName, append, this.bufferedIO, this.bufferSize);
      if (!append)
        return;
      ((CountingQuietWriter) this.qw).setCount(new File(fileName).length());
    }

    public virtual void setMaxBackupIndex(int maxBackups) => this.maxBackupIndex = maxBackups;

    public virtual void setMaximumFileSize(long maxFileSize) => this.maxFileSize = maxFileSize;

    public virtual void setMaxFileSize(string value) => this.maxFileSize = OptionConverter.toFileSize(value, this.maxFileSize + 1L);

    [JavaFlags(4)]
    public override void setQWForFiles(Writer writer) => this.qw = (QuietWriter) new CountingQuietWriter(writer, this.errorHandler);

    [JavaFlags(4)]
    public override void subAppend(LoggingEvent @event)
    {
      base.subAppend(@event);
      if (this.fileName == null || ((CountingQuietWriter) this.qw).getCount() < this.maxFileSize)
        return;
      this.rollOver();
    }
  }
}

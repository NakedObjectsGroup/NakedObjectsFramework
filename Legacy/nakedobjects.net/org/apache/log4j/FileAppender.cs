// Decompiled with JetBrains decompiler
// Type: org.apache.log4j.FileAppender
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.io;
using java.lang;
using org.apache.log4j.helpers;
using System.Runtime.CompilerServices;

namespace org.apache.log4j
{
  public class FileAppender : WriterAppender
  {
    [JavaFlags(4)]
    public bool fileAppend;
    [JavaFlags(4)]
    public string fileName;
    [JavaFlags(4)]
    public bool bufferedIO;
    [JavaFlags(4)]
    public int bufferSize;

    public FileAppender()
    {
      this.fileAppend = true;
      this.fileName = (string) null;
      this.bufferedIO = false;
      this.bufferSize = 8192;
    }

    [JavaThrownExceptions("1;java/io/IOException;")]
    public FileAppender(
      Layout layout,
      string filename,
      bool append,
      bool bufferedIO,
      int bufferSize)
    {
      this.fileAppend = true;
      this.fileName = (string) null;
      this.bufferedIO = false;
      this.bufferSize = 8192;
      this.layout = layout;
      this.setFile(filename, append, bufferedIO, bufferSize);
    }

    [JavaThrownExceptions("1;java/io/IOException;")]
    public FileAppender(Layout layout, string filename, bool append)
    {
      this.fileAppend = true;
      this.fileName = (string) null;
      this.bufferedIO = false;
      this.bufferSize = 8192;
      this.layout = layout;
      this.setFile(filename, append, false, this.bufferSize);
    }

    [JavaThrownExceptions("1;java/io/IOException;")]
    public FileAppender(Layout layout, string filename)
      : this(layout, filename, true)
    {
    }

    public virtual void setFile(string file) => this.fileName = StringImpl.trim(file);

    public virtual bool getAppend() => this.fileAppend;

    public virtual string getFile() => this.fileName;

    public override void activateOptions()
    {
      if (this.fileName != null)
      {
        try
        {
          this.setFile(this.fileName, this.fileAppend, this.bufferedIO, this.bufferSize);
        }
        catch (IOException ex)
        {
          this.errorHandler.error(new StringBuffer().append("setFile(").append(this.fileName).append(",").append(this.fileAppend).append(") call failed.").ToString(), (Exception) ex, 4);
        }
      }
      else
      {
        LogLog.warn(new StringBuffer().append("File option not set for appender [").append(this.name).append("].").ToString());
        LogLog.warn("Are you using FileAppender instead of ConsoleAppender?");
      }
    }

    [JavaFlags(4)]
    public virtual void closeFile()
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

    public virtual bool getBufferedIO() => this.bufferedIO;

    public virtual int getBufferSize() => this.bufferSize;

    public virtual void setAppend(bool flag) => this.fileAppend = flag;

    public virtual void setBufferedIO(bool bufferedIO)
    {
      this.bufferedIO = bufferedIO;
      if (!bufferedIO)
        return;
      this.immediateFlush = false;
    }

    public virtual void setBufferSize(int bufferSize) => this.bufferSize = bufferSize;

    [JavaThrownExceptions("1;java/io/IOException;")]
    [MethodImpl(MethodImplOptions.Synchronized)]
    public virtual void setFile(string fileName, bool append, bool bufferedIO, int bufferSize)
    {
      LogLog.debug(new StringBuffer().append("setFile called: ").append(fileName).append(", ").append(append).ToString());
      if (bufferedIO)
        this.setImmediateFlush(false);
      this.reset();
      Writer writer = (Writer) this.createWriter((OutputStream) new FileOutputStream(fileName, append));
      if (bufferedIO)
        writer = (Writer) new BufferedWriter(writer, bufferSize);
      this.setQWForFiles(writer);
      this.fileName = fileName;
      this.fileAppend = append;
      this.bufferedIO = bufferedIO;
      this.bufferSize = bufferSize;
      this.writeHeader();
      LogLog.debug("setFile ended");
    }

    [JavaFlags(4)]
    public virtual void setQWForFiles(Writer writer) => this.qw = new QuietWriter(writer, this.errorHandler);

    [JavaFlags(4)]
    public override void reset()
    {
      this.closeFile();
      this.fileName = (string) null;
      base.reset();
    }
  }
}

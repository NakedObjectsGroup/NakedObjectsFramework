// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.utility.logging.FileSnapshotAppender
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using java.io;
using java.lang;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace org.nakedobjects.utility.logging
{
  public class FileSnapshotAppender : SnapshotAppender
  {
    private static readonly org.apache.log4j.Logger LOG;
    private string directoryPath;
    private string extension;
    private string fileName;

    public virtual string getDirectory() => this.directoryPath;

    public virtual string getExtension() => this.extension;

    public virtual string getFileName() => this.fileName;

    public virtual void setDirectory(string directoryPath) => this.directoryPath = directoryPath;

    public virtual void setExtension(string extension) => this.extension = extension;

    public virtual void setFileName(string fileName) => this.fileName = fileName;

    [JavaFlags(36)]
    [MethodImpl(MethodImplOptions.Synchronized)]
    public override void writeSnapshot(string message, string details)
    {
      try
      {
        SnapshotWriter snapshotWriter = new SnapshotWriter(this.directoryPath, this.fileName, this.extension, message);
        snapshotWriter.appendLog(details);
        snapshotWriter.close();
      }
      catch (FileNotFoundException ex)
      {
        if (!FileSnapshotAppender.LOG.isInfoEnabled())
          return;
        FileSnapshotAppender.LOG.info((object) "failed to open log file", (Throwable) ex);
      }
      catch (IOException ex)
      {
        if (!FileSnapshotAppender.LOG.isInfoEnabled())
          return;
        FileSnapshotAppender.LOG.info((object) "failed to write log file", (Throwable) ex);
      }
    }

    public FileSnapshotAppender() => this.fileName = "log-snapshot-";

    [EditorBrowsable(EditorBrowsableState.Never)]
    [JavaFlags(32778)]
    static FileSnapshotAppender()
    {
      // ISSUE: unable to decompile the method.
    }
  }
}

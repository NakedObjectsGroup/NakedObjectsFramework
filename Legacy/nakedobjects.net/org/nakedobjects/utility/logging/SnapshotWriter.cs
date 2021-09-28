// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.utility.logging.SnapshotWriter
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.io;
using java.lang;
using java.text;
using java.util;
using System.ComponentModel;

namespace org.nakedobjects.utility.logging
{
  public class SnapshotWriter
  {
    private static readonly Format FORMAT;
    private static readonly org.apache.log4j.Logger LOG;
    private readonly PrintStream os;

    [JavaThrownExceptions("1;java/io/IOException;")]
    public SnapshotWriter(
      string directoryPath,
      string baseFileName,
      string extension,
      string message)
    {
      File file1 = new File(directoryPath == null || StringImpl.length(directoryPath) == 0 ? "." : directoryPath);
      if (!file1.exists())
        file1.mkdirs();
      File file2 = new File(file1, "index.txt");
      Date date = new Date();
      extension = extension == null || StringImpl.length(extension) == 0 ? "log" : extension;
      File file3 = new File(file1, new StringBuffer().append(baseFileName).append(SnapshotWriter.FORMAT.format((object) date)).append(".").append(extension).ToString());
      RandomAccessFile randomAccessFile = new RandomAccessFile(file2, "rw");
      randomAccessFile.seek(randomAccessFile.length());
      randomAccessFile.writeBytes(new StringBuffer().append(file3.getName()).append(": ").append(message).append("\n").ToString());
      randomAccessFile.close();
      this.os = new PrintStream((OutputStream) new FileOutputStream(file3));
    }

    public virtual void appendLog(string details) => this.os.println(details);

    public virtual void close()
    {
      if (this.os == null)
        return;
      this.os.close();
    }

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static SnapshotWriter()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      SnapshotWriter snapshotWriter = this;
      ObjectImpl.clone((object) snapshotWriter);
      return ((object) snapshotWriter).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}

// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.xat.performance.TimingDocumentor
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using java.io;
using java.lang;
using java.util;
using org.nakedobjects.utility;

namespace org.nakedobjects.xat.performance
{
  public class TimingDocumentor : AbstractDocumentor
  {
    private readonly string directory;
    private readonly Vector timings;

    public TimingDocumentor(string directory)
    {
      this.timings = new Vector(250, 250);
      this.directory = directory;
    }

    public override void close() => this.save();

    public override void doc(string text)
    {
    }

    public override void docln(string text)
    {
    }

    public override void flush()
    {
    }

    public virtual void record(Profiler timer) => this.timings.addElement((object) timer);

    private void save()
    {
      File file = new File(this.directory);
      ((PrintStream) System.@out).println(file.getAbsolutePath());
      if (!file.exists())
        file.mkdirs();
      PrintWriter printWriter = (PrintWriter) null;
      try
      {
        printWriter = new PrintWriter((Writer) new OutputStreamWriter((OutputStream) new FileOutputStream(new File(this.directory, "timing-xat.data"))));
        int num = 0;
        for (int index = this.timings.size(); num < index; ++num)
        {
          Profiler profiler = (Profiler) this.timings.elementAt(num);
          printWriter.println(profiler.log());
        }
      }
      catch (IOException ex)
      {
        ((Throwable) ex).printStackTrace();
      }
      finally
      {
        printWriter?.close();
      }
    }

    public override void step(string @string)
    {
    }

    public override void subtitle(string text)
    {
    }

    public override void title(string text)
    {
    }
  }
}

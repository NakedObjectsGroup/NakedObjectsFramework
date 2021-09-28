// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.ExternalHelpViewerProgram
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.io;
using java.lang;
using org.nakedobjects.utility;
using System.ComponentModel;

namespace org.nakedobjects.viewer.skylark
{
  [JavaInterfaces("1;org/nakedobjects/viewer/skylark/HelpViewer;")]
  public class ExternalHelpViewerProgram : HelpViewer
  {
    private static readonly org.apache.log4j.Logger LOG;
    private readonly string program;

    public ExternalHelpViewerProgram(string program) => this.program = program;

    public virtual void open(Location location, string name, string description, string help)
    {
      string str = new StringBuffer().append(this.program).append(" ").append(help).ToString();
      try
      {
        Runtime.getRuntime().exec(str);
        if (!ExternalHelpViewerProgram.LOG.isDebugEnabled())
          return;
        ExternalHelpViewerProgram.LOG.debug((object) new StringBuffer().append("executing '").append(str).append("'").ToString());
      }
      catch (IOException ex)
      {
        throw new NakedObjectRuntimeException(new StringBuffer().append("faile to execute '").append(str).append("'").ToString(), (Throwable) ex);
      }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [JavaFlags(32778)]
    static ExternalHelpViewerProgram()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      ExternalHelpViewerProgram helpViewerProgram = this;
      ObjectImpl.clone((object) helpViewerProgram);
      return ((object) helpViewerProgram).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}

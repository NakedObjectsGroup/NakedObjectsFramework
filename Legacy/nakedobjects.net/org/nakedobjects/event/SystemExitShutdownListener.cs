// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.event.SystemExitShutdownListener
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.io;
using java.lang;
using org.nakedobjects.@event;

namespace org.nakedobjects.@event
{
  [JavaInterfaces("1;org/nakedobjects/event/ObjectViewingMechanismListener;")]
  public class SystemExitShutdownListener : ObjectViewingMechanismListener
  {
    public virtual void viewerClosing()
    {
      ((PrintStream) System.@out).println("EXITED");
      System.exit(0);
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      SystemExitShutdownListener shutdownListener = this;
      ObjectImpl.clone((object) shutdownListener);
      return ((object) shutdownListener).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}

// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.NakedObjectsViewer
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using org.nakedobjects.@event;
using org.nakedobjects.@object;
using org.nakedobjects.viewer.skylark;

namespace org.nakedobjects.viewer
{
  [JavaInterfaces("1;org/nakedobjects/object/NakedObjectsComponent;")]
  public abstract class NakedObjectsViewer : NakedObjectsComponent
  {
    public abstract void init();

    public abstract void shutdown();

    public virtual void setApplication(UserContext applicationContext)
    {
    }

    public virtual void setExploration(bool inExplorationMode)
    {
    }

    public virtual void setTitle(string title)
    {
    }

    public virtual void setShutdownListener(ObjectViewingMechanismListener shutdownListener)
    {
    }

    public virtual UserContext Application
    {
      set => this.setApplication(value);
    }

    public virtual bool Exploration
    {
      set => this.setExploration(value);
    }

    public virtual ObjectViewingMechanismListener ShutdownListener
    {
      set => this.setShutdownListener(value);
    }

    public virtual string Title
    {
      set => this.setTitle(value);
    }

    public virtual void setUpdateNotifier(ViewUpdateNotifier updateNotifier)
    {
    }

    public virtual void setHelpViewer(HelpViewer helpViewer)
    {
    }

    public virtual HelpViewer HelpViewer
    {
      set => this.setHelpViewer(value);
    }

    public virtual ViewUpdateNotifier UpdateNotifier
    {
      set => this.setUpdateNotifier(value);
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      NakedObjectsViewer nakedObjectsViewer = this;
      ObjectImpl.clone((object) nakedObjectsViewer);
      return ((object) nakedObjectsViewer).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}

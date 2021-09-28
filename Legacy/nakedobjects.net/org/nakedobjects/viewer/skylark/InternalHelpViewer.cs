// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.InternalHelpViewer
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;

namespace org.nakedobjects.viewer.skylark
{
  [JavaInterfaces("1;org/nakedobjects/viewer/skylark/HelpViewer;")]
  public class InternalHelpViewer : HelpViewer
  {
    private readonly Viewer viewer;

    public InternalHelpViewer(Viewer viewer) => this.viewer = viewer;

    public virtual void open(Location location, string name, string description, string help)
    {
      this.viewer.clearStatus();
      this.viewer.clearOverlayView();
      View view = (View) new HelpView(name, description, help);
      location.add(20, 20);
      view.setLocation(location);
      this.viewer.setOverlayView(view);
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      InternalHelpViewer internalHelpViewer = this;
      ObjectImpl.clone((object) internalHelpViewer);
      return ((object) internalHelpViewer).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}

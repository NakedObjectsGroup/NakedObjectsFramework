// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.SimpleInternalDrag
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;

namespace org.nakedobjects.viewer.skylark
{
  public class SimpleInternalDrag : InternalDrag
  {
    private readonly Location location;
    private readonly Location offset;
    private readonly View view;

    public SimpleInternalDrag(View view, Location location)
    {
      this.view = view;
      this.location = new Location(location);
      this.offset = view.getAbsoluteLocation();
      Padding padding1 = view.getPadding();
      Padding padding2 = view.getView().getPadding();
      this.offset.add(padding2.getLeft() - padding1.getLeft(), padding2.getTop() - padding1.getTop());
      this.location.subtract(this.offset);
    }

    public SimpleInternalDrag(View view, Offset off)
    {
      this.view = view;
      this.location = new Location();
      this.offset = new Location(off.getDeltaX(), off.getDeltaY());
      Padding padding1 = view.getPadding();
      Padding padding2 = view.getView().getPadding();
      this.offset.add(padding2.getLeft() - padding1.getLeft(), padding2.getTop() - padding1.getTop());
      this.location.subtract(this.offset);
    }

    [JavaFlags(4)]
    public override void cancel(Viewer viewer) => this.view.dragCancel((InternalDrag) this);

    [JavaFlags(4)]
    public override void drag(Viewer viewer, Location location, int mods)
    {
      this.location.x = location.x;
      this.location.y = location.y;
      this.location.subtract(this.offset);
      this.view.drag((InternalDrag) this);
    }

    [JavaFlags(4)]
    public override void end(Viewer viewer) => this.view.dragTo((InternalDrag) this);

    public override Location getLocation() => new Location(this.location);

    public override View getOverlay() => (View) null;

    [JavaFlags(4)]
    public override void start(Viewer viewer)
    {
    }

    public override string ToString()
    {
      org.nakedobjects.utility.ToString toString = new org.nakedobjects.utility.ToString((object) this, base.ToString());
      toString.append("location", (object) this.location);
      toString.append("relative", (object) this.getLocation());
      return toString.ToString();
    }
  }
}

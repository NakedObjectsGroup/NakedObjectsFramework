// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.ViewDrag
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using java.lang;

namespace org.nakedobjects.viewer.skylark
{
  public class ViewDrag : Drag
  {
    private readonly View dragView;
    private Location location;
    private readonly Offset overlayOffset;
    private readonly View view;
    private readonly View viewsDecoratedWorkspace;
    private readonly Workspace viewsWorkspace;

    public ViewDrag(View view, Offset offset, View dragView)
    {
      this.view = view;
      this.dragView = dragView;
      this.overlayOffset = offset;
      this.viewsWorkspace = view.getWorkspace();
      this.viewsDecoratedWorkspace = this.viewsWorkspace.getView();
    }

    [JavaFlags(4)]
    public override void cancel(Viewer viewer) => this.getSourceView().getViewManager().showDefaultCursor();

    [JavaFlags(4)]
    public virtual void drag(Viewer viewer)
    {
      if (this.dragView == null)
        return;
      this.dragView.markDamaged();
      this.updateDraggingLocation();
      this.dragView.markDamaged();
    }

    [JavaFlags(4)]
    public override void drag(Viewer viewer, Location location, int mods)
    {
      this.location = location;
      if (this.dragView == null)
        return;
      this.dragView.markDamaged();
      this.updateDraggingLocation();
      this.dragView.markDamaged();
    }

    [JavaFlags(4)]
    public override void end(Viewer viewer)
    {
      viewer.disposeOverlayView();
      this.viewsDecoratedWorkspace.drop(this);
    }

    public override View getOverlay() => this.dragView;

    public virtual Location getLocation() => this.location;

    public virtual View getSourceView() => this.view;

    public virtual Location getViewDropLocation()
    {
      Location location = new Location(this.location);
      location.subtract(this.overlayOffset);
      location.subtract(this.viewsDecoratedWorkspace.getAbsoluteLocation());
      location.move(-this.viewsDecoratedWorkspace.getPadding().left, -this.viewsDecoratedWorkspace.getPadding().top);
      return location;
    }

    [JavaFlags(4)]
    public override void start(Viewer viewer)
    {
    }

    public virtual void subtract(Location location) => location.subtract(location);

    public override string ToString() => new StringBuffer().append("ViewDrag [").append(base.ToString()).append("]").ToString();

    private void updateDraggingLocation()
    {
      Location point = new Location(this.location);
      point.subtract(this.overlayOffset);
      this.dragView.setLocation(point);
      this.dragView.limitBoundsWithin(this.viewsWorkspace.getSize());
    }

    public virtual void subtract(int x, int y) => this.location.subtract(x, y);
  }
}

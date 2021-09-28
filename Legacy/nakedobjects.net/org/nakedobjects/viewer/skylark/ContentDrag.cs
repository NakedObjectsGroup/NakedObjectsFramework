// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.ContentDrag
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using java.lang;

namespace org.nakedobjects.viewer.skylark
{
  public class ContentDrag : Drag
  {
    private readonly View dragView;
    private Location location;
    private View previousTarget;
    private readonly Content sourceContent;
    private View target;
    private readonly Workspace workspace;
    private Location offset;
    private readonly View source;

    public ContentDrag(View source, Location offset, View dragView)
    {
      if (dragView == null)
        throw new NullPointerException();
      this.workspace = source.getWorkspace();
      this.sourceContent = source.getContent();
      this.dragView = dragView;
      this.offset = offset;
      this.source = source.getView();
    }

    [JavaFlags(4)]
    public override void cancel(Viewer viewer)
    {
      if (this.target != null)
        this.target.dragOut(this);
      viewer.clearStatus();
      viewer.clearOverlayView();
      viewer.showDefaultCursor();
    }

    [JavaFlags(4)]
    public override void drag(Viewer viewer, Location location, int mods)
    {
      this.location = location;
      this.target = viewer.identifyView(new Location(location), false);
      this.mods = mods;
      if (this.dragView != null)
      {
        this.dragView.markDamaged();
        Location point = new Location(this.location);
        point.subtract(this.offset);
        this.dragView.setLocation(point);
        this.dragView.limitBoundsWithin(this.workspace.getSize());
        this.dragView.markDamaged();
      }
      if (this.target == this.previousTarget)
        return;
      if (this.previousTarget != null)
      {
        viewer.getSpy().addAction(new StringBuffer().append("drag out ").append((object) this.previousTarget).ToString());
        this.previousTarget.dragOut(this);
        this.previousTarget = (View) null;
      }
      viewer.getSpy().addAction(new StringBuffer().append("drag in ").append((object) this.target).ToString());
      this.target.dragIn(this);
      this.previousTarget = this.target;
    }

    [JavaFlags(4)]
    public override void end(Viewer viewer)
    {
      viewer.getSpy().addAction(new StringBuffer().append("drop on ").append((object) this.target).ToString());
      this.target.drop(this);
      viewer.clearStatus();
      viewer.clearOverlayView();
      viewer.showDefaultCursor();
    }

    public override View getOverlay() => this.dragView;

    public virtual View getSource() => this.source;

    public virtual Content getSourceContent() => this.sourceContent;

    public virtual Location getTargetLocation()
    {
      Location location = new Location(this.location);
      location.subtract(this.target.getAbsoluteLocation());
      location.add(-this.getOffset().x, -this.getOffset().y);
      location.add(-this.getOffset().x, -this.getOffset().y);
      return location;
    }

    public virtual Location getOffset() => this.offset;

    public virtual View getTargetView() => this.target;

    public override string ToString() => new StringBuffer().append("ContentDrag [").append(base.ToString()).append("]").ToString();

    [JavaFlags(4)]
    public override void start(Viewer viewer)
    {
    }

    public virtual void subtract(int left, int top)
    {
    }
  }
}

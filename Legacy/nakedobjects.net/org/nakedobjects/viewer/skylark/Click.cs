// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.Click
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using java.lang;

namespace org.nakedobjects.viewer.skylark
{
  public class Click : PointerEvent
  {
    private readonly Location location;
    private Location locationWithinViewer;

    public Click(View source, Location mouseLocation, int modifiers)
      : base(modifiers)
    {
      this.location = new Location(mouseLocation);
      this.locationWithinViewer = new Location(mouseLocation);
    }

    public virtual Location getLocation() => this.location;

    public virtual Location getLocationWithinViewer() => this.locationWithinViewer;

    public virtual void subtract(int x, int y) => this.location.subtract(x, y);

    public override string ToString() => new StringBuffer().append("Click [location=").append((object) this.location).append(",").append(base.ToString()).append("]").ToString();

    public virtual void add(Offset offset) => this.location.add(offset.getDeltaX(), offset.getDeltaY());

    public virtual void subtract(Offset offset) => this.subtract(offset.getDeltaX(), offset.getDeltaY());

    public virtual void subtract(Location location) => this.subtract(location.getX(), location.getY());
  }
}

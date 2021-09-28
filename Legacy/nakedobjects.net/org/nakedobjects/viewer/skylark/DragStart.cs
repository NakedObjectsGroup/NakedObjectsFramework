// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.DragStart
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

namespace org.nakedobjects.viewer.skylark
{
  public class DragStart : PointerEvent
  {
    private readonly Location location;

    public DragStart(Location location, int mods)
      : base(mods)
    {
      this.location = location;
    }

    public virtual Location getLocation() => this.location;

    public virtual void subtract(Location location) => this.location.subtract(location);

    public virtual void subtract(int x, int y) => this.location.subtract(x, y);

    public virtual void add(Offset offset) => this.location.add(offset);

    public override string ToString()
    {
      org.nakedobjects.utility.ToString toString = new org.nakedobjects.utility.ToString((object) this);
      toString.append("location", (object) this.location);
      toString.append("buttons", base.ToString());
      return toString.ToString();
    }
  }
}

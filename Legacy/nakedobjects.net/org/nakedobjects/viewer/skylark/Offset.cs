// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.Offset
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;

namespace org.nakedobjects.viewer.skylark
{
  public class Offset
  {
    private int dx;
    private int dy;

    public Offset(Location locationInViewer, Location locationInView)
    {
      this.dx = locationInViewer.getX() - locationInView.getX();
      this.dy = locationInViewer.getY() - locationInView.getY();
    }

    public Offset(int dx, int dy)
    {
      this.dx = dx;
      this.dy = dy;
    }

    public Offset(Location location)
    {
      this.dx = location.getX();
      this.dy = location.getY();
    }

    public virtual int getDeltaX() => this.dx;

    public virtual int getDeltaY() => this.dy;

    public virtual Location offset(Location locationInViewer)
    {
      Location location = new Location(locationInViewer);
      location.move(this.dx, this.dy);
      return location;
    }

    public override bool Equals(object obj)
    {
      if (obj == this)
        return true;
      if (!(obj is Offset))
        return false;
      Offset offset = (Offset) obj;
      return offset.dx == this.dx && offset.dy == this.dy;
    }

    public override string ToString() => new StringBuffer().append("Offset ").append(this.dx).append(", ").append(this.dy).ToString();

    public virtual void add(int dx, int dy)
    {
      this.dx += dx;
      this.dy += dy;
    }

    public virtual void subtract(int dx, int dy) => this.add(-dx, -dy);

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      Offset offset = this;
      ObjectImpl.clone((object) offset);
      return ((object) offset).MemberwiseClone();
    }
  }
}

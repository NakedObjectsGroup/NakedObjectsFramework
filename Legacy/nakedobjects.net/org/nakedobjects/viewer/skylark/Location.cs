// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.Location
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.awt;
using java.lang;

namespace org.nakedobjects.viewer.skylark
{
  public class Location
  {
    [JavaFlags(0)]
    public int x;
    [JavaFlags(0)]
    public int y;

    public Location(int x, int y)
    {
      this.x = x;
      this.y = y;
    }

    public Location(Location location)
    {
      this.x = location.x;
      this.y = location.y;
    }

    public Location()
    {
      this.x = 0;
      this.y = 0;
    }

    [JavaFlags(0)]
    public Location(Point point)
    {
      this.x = (int) point.x;
      this.y = (int) point.y;
    }

    public virtual void setX(int x) => this.x = x;

    public virtual int getX() => this.x;

    public virtual void setY(int y) => this.y = y;

    public virtual int getY() => this.y;

    public override bool Equals(object obj)
    {
      if (obj == this)
        return true;
      if (!(obj is Location))
        return false;
      Location location = (Location) obj;
      return location.x == this.x && location.y == this.y;
    }

    public override string ToString() => new StringBuffer().append(this.x).append(",").append(this.y).ToString();

    public virtual void move(int dx, int dy)
    {
      this.x += dx;
      this.y += dy;
    }

    public virtual void translate(Location offset) => this.move(offset.x, offset.y);

    public virtual void translate(Offset offset) => this.move(offset.getDeltaX(), offset.getDeltaY());

    public virtual Offset offsetFrom(Location location) => new Offset(this.x - location.x, this.y - location.y);

    public virtual void subtract(Offset offset) => this.move(-offset.getDeltaX(), -offset.getDeltaY());

    public virtual void subtract(Location location) => this.move(-location.x, -location.y);

    public virtual void add(Offset offset) => this.move(offset.getDeltaX(), offset.getDeltaY());

    public virtual void subtract(int x, int y) => this.move(-x, -y);

    public virtual void add(int x, int y) => this.move(x, y);

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      Location location = this;
      ObjectImpl.clone((object) location);
      return ((object) location).MemberwiseClone();
    }
  }
}

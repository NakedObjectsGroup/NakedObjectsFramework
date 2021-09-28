// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.Bounds
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.apache.log4j;
using System.ComponentModel;

namespace org.nakedobjects.viewer.skylark
{
  public class Bounds
  {
    [JavaFlags(0)]
    public int height;
    private static readonly Logger LOG;
    [JavaFlags(0)]
    public int width;
    [JavaFlags(0)]
    public int x;
    [JavaFlags(0)]
    public int y;

    public Bounds()
    {
      this.x = 0;
      this.y = 0;
      this.width = 0;
      this.height = 0;
    }

    public Bounds(Bounds bounds)
      : this(bounds.x, bounds.y, bounds.width, bounds.height)
    {
    }

    public Bounds(int x, int y, int width, int height)
    {
      this.x = x;
      this.y = y;
      this.width = width;
      this.height = height;
    }

    public Bounds(Location location, Size size)
      : this(location.x, location.y, size.width, size.height)
    {
    }

    public Bounds(Size size)
      : this(0, 0, size.width, size.height)
    {
    }

    public virtual bool contains(Location location)
    {
      int x = location.getX();
      int y = location.getY();
      int num1 = this.x + this.width - 1;
      int num2 = this.y + this.height - 1;
      return x >= this.x && x <= num1 && y >= this.y && y <= num2;
    }

    public virtual void contract(int width, int height)
    {
      this.width -= width;
      this.height -= height;
    }

    public virtual void contract(Padding padding)
    {
      this.height -= padding.top + padding.bottom;
      this.width -= padding.left + padding.right;
      this.x += padding.left;
      this.y += padding.top;
    }

    public virtual void contract(Size size)
    {
      this.width -= size.width;
      this.height -= size.height;
    }

    public virtual void contractHeight(int height) => this.height -= height;

    public virtual void contractWidth(int width) => this.width -= width;

    public virtual void ensureHeight(int height) => this.height = Math.max(this.height, height);

    public virtual void ensureWidth(int width) => this.width = Math.max(this.width, width);

    public override bool Equals(object obj)
    {
      if (obj == this)
        return true;
      if (!(obj is Bounds))
        return false;
      Bounds bounds = (Bounds) obj;
      return bounds.x == this.x && bounds.y == this.y && bounds.width == this.width && bounds.height == this.height;
    }

    public virtual void extend(int width, int height)
    {
      this.width += width;
      this.height += height;
    }

    public virtual void extend(Padding padding)
    {
      this.width += padding.getLeftRight();
      this.height += padding.getTopBottom();
    }

    public virtual void extend(Size size)
    {
      this.width += size.width;
      this.height += size.height;
    }

    public virtual void extendHeight(int height) => this.height += height;

    public virtual void extendWidth(int width) => this.width += width;

    public virtual int getHeight() => this.height;

    public virtual Location getLocation() => new Location(this.x, this.y);

    public virtual Size getSize() => new Size(this.width, this.height);

    public virtual int getWidth() => this.width;

    public virtual int getX() => this.x;

    public virtual int getX2() => this.x + this.width - 1;

    public virtual int getY() => this.y;

    public virtual int getY2() => this.y + this.height - 1;

    public virtual bool intersects(Bounds bounds)
    {
      int x1 = this.x;
      int num1 = this.x + this.width - 1;
      int x2 = bounds.x;
      int num2 = bounds.x + bounds.width - 1;
      bool flag1 = x1 <= x2 && x2 <= num1 || x1 <= num2 && x2 <= num1 || x2 <= x1 && x1 <= num2 || x2 <= num1 && x1 <= num2;
      int y1 = this.y;
      int num3 = this.y + this.height - 1;
      int y2 = bounds.y;
      int num4 = bounds.y + bounds.height - 1;
      bool flag2 = y1 <= y2 && y2 <= num3 || y1 <= num4 && y2 <= num3 || y2 <= y1 && y1 <= num4 || y2 <= num3 && y1 <= num4;
      return flag1 && flag2;
    }

    public virtual void limitLocation(Size bounds)
    {
      if (this.x + this.width > bounds.width)
        this.x = bounds.width - this.width;
      if (this.y + this.height <= bounds.height)
        return;
      this.y = bounds.height - this.height;
    }

    public virtual bool limitBounds(Bounds toLimit)
    {
      bool flag1 = Bounds.LOG.isInfoEnabled();
      bool flag2 = false;
      Location location = toLimit.getLocation();
      Size size1 = toLimit.getSize();
      int x1 = location.getX();
      int y1 = location.getY();
      int num1 = x1 + size1.getWidth();
      int num2 = y1 + size1.getHeight();
      Size size2 = this.getSize();
      int x2 = this.x;
      int y2 = this.y;
      int num3 = this.x + this.width;
      int num4 = this.y + this.height;
      if (num1 > num3)
      {
        x1 = num3 - size1.getWidth();
        flag2 = true;
        if (flag1)
          Bounds.LOG.info((object) new StringBuffer().append("right side oustide limits, moving left to ").append(x1).ToString());
      }
      if (x1 < x2)
      {
        x1 = x2;
        flag2 = true;
        if (flag1)
          Bounds.LOG.info((object) new StringBuffer().append("left side outside limit, moving left to ").append(x1).ToString());
      }
      if (num2 > num4)
      {
        y1 = num4 - size1.getHeight();
        flag2 = true;
        if (flag1)
          Bounds.LOG.info((object) new StringBuffer().append("bottom outside limit, moving top to ").append(y1).ToString());
      }
      if (y1 < y2)
      {
        y1 = y2;
        flag2 = true;
        if (flag1)
          Bounds.LOG.info((object) new StringBuffer().append("top outside limit, moving top to ").append(y1).ToString());
      }
      toLimit.setX(x1);
      toLimit.setY(y1);
      int num5 = y1 + size1.getHeight();
      if (x1 + size1.getWidth() > num3)
      {
        toLimit.width = size2.width;
        flag2 = true;
        if (flag1)
          Bounds.LOG.info((object) new StringBuffer().append("width outside limit, reducing width to ").append(y1).ToString());
      }
      if (num5 > num4)
      {
        toLimit.height = size2.height;
        flag2 = true;
        if (flag1)
          Bounds.LOG.info((object) new StringBuffer().append("height outside limit, reducing height to ").append(y1).ToString());
      }
      if (flag2 && flag1)
        Bounds.LOG.info((object) new StringBuffer().append("limited ").append((object) toLimit).ToString());
      return flag2;
    }

    public virtual void setBounds(Bounds bounds)
    {
      this.x = bounds.x;
      this.y = bounds.y;
      this.width = bounds.width;
      this.height = bounds.height;
    }

    public virtual void setHeight(int height) => this.height = height;

    public virtual void setWidth(int width) => this.width = width;

    public virtual void setX(int x) => this.x = x;

    public virtual void setY(int y) => this.y = y;

    public override string ToString() => new StringBuffer().append(this.x).append(",").append(this.y).append(" ").append(this.width).append("x").append(this.height).ToString();

    public virtual void translate(int x, int y)
    {
      this.x += x;
      this.y += y;
    }

    public virtual void union(Bounds bounds)
    {
      int num1 = Math.min(this.x, bounds.x);
      int num2 = Math.min(this.y, bounds.y);
      this.width = Math.max(this.x + this.width, bounds.x + bounds.width) - num1;
      this.height = Math.max(this.y + this.height, bounds.y + bounds.height) - num2;
      this.x = num1;
      this.y = num2;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [JavaFlags(32778)]
    static Bounds()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      Bounds bounds = this;
      ObjectImpl.clone((object) bounds);
      return ((object) bounds).MemberwiseClone();
    }
  }
}

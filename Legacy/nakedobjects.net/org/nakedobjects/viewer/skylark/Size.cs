// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.Size
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.awt;
using java.lang;
using System.ComponentModel;

namespace org.nakedobjects.viewer.skylark
{
  public class Size
  {
    public static readonly Size ALL;
    [JavaFlags(0)]
    public int height;
    [JavaFlags(0)]
    public int width;

    public Size()
    {
      this.width = 0;
      this.height = 0;
    }

    [JavaFlags(0)]
    public Size(Dimension dimension)
      : this((int) dimension.width, (int) dimension.height)
    {
    }

    public Size(int width, int height)
    {
      this.width = width;
      this.height = height;
    }

    public Size(Size size)
    {
      this.width = size.width;
      this.height = size.height;
    }

    public virtual void contract(int width, int height)
    {
      this.width -= width;
      this.height -= height;
    }

    public virtual void contract(Size size)
    {
      this.width -= size.width;
      this.height -= size.height;
    }

    public virtual void contractHeight(int height) => this.height -= height;

    public virtual void contract(Padding padding)
    {
      this.height -= padding.top + padding.bottom;
      this.width -= padding.left + padding.right;
    }

    public virtual void contractWidth(int width) => this.width -= width;

    public virtual void ensureHeight(int height) => this.height = Math.max(this.height, height);

    public virtual void ensureWidth(int width) => this.width = Math.max(this.width, width);

    public override bool Equals(object obj)
    {
      if (obj == this)
        return true;
      if (!(obj is Size))
        return false;
      Size size = (Size) obj;
      return size.width == this.width && size.height == this.height;
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

    public virtual int getWidth() => this.width;

    public virtual void limitHeight(int maximum) => this.height = Math.min(this.height, maximum);

    public virtual void limitWidth(int maximum) => this.width = Math.min(this.width, maximum);

    public virtual void limitSize(Size maximum)
    {
      this.limitWidth(maximum.width);
      this.limitHeight(maximum.height);
    }

    public virtual void setHeight(int height) => this.height = height;

    public virtual void setWidth(int width) => this.width = width;

    public override string ToString() => new StringBuffer().append(this.width).append("x").append(this.height).ToString();

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static Size()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      Size size = this;
      ObjectImpl.clone((object) size);
      return ((object) size).MemberwiseClone();
    }
  }
}

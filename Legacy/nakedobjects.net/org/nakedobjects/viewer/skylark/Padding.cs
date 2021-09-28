// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.Padding
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.awt;
using java.lang;

namespace org.nakedobjects.viewer.skylark
{
  public class Padding
  {
    [JavaFlags(0)]
    public int bottom;
    [JavaFlags(0)]
    public int left;
    [JavaFlags(0)]
    public int right;
    [JavaFlags(0)]
    public int top;

    public Padding(int top, int left, int bottom, int right)
    {
      this.top = top;
      this.bottom = bottom;
      this.left = left;
      this.right = right;
    }

    public Padding()
    {
      this.top = 0;
      this.bottom = 0;
      this.left = 0;
      this.right = 0;
    }

    public Padding(Padding padding)
    {
      this.top = padding.top;
      this.bottom = padding.bottom;
      this.left = padding.left;
      this.right = padding.right;
    }

    [JavaFlags(0)]
    public Padding(Insets insets)
      : this((int) insets.top, (int) insets.left, (int) insets.bottom, (int) insets.right)
    {
    }

    public virtual void setBottom(int bottom) => this.bottom = bottom;

    public virtual int getBottom() => this.bottom;

    public virtual void setLeft(int left) => this.left = left;

    public virtual int getLeft() => this.left;

    public virtual int getLeftRight() => this.left + this.right;

    public virtual void setRight(int right) => this.right = right;

    public virtual int getRight() => this.right;

    public virtual void setTop(int top) => this.top = top;

    public virtual int getTop() => this.top;

    public virtual int getTopBottom() => this.top + this.bottom;

    public virtual void extendBottom(int pad) => this.bottom += pad;

    public virtual void extendLeft(int pad) => this.left += pad;

    public virtual void extendRight(int pad) => this.right += pad;

    public virtual void extendTop(int pad) => this.top += pad;

    public override bool Equals(object obj)
    {
      if (obj == this)
        return true;
      if (!(obj is Padding))
        return false;
      Padding padding = (Padding) obj;
      return padding.top == this.top && padding.bottom == this.bottom && padding.left == this.left && padding.right == this.right;
    }

    public override string ToString() => new StringBuffer().append("Padding [top=").append(this.top).append(",bottom=").append(this.bottom).append(",left=").append(this.left).append(",right=").append(this.right).append("]").ToString();

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      Padding padding = this;
      ObjectImpl.clone((object) padding);
      return ((object) padding).MemberwiseClone();
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.special.ScrollBar
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;

namespace org.nakedobjects.viewer.skylark.special
{
  public class ScrollBar
  {
    private int maximum;
    private int minimum;
    private int scrollPosition;
    private int visibleAmount;

    public ScrollBar() => this.scrollPosition = 0;

    public virtual void setPostion(int position)
    {
      this.scrollPosition = Math.min(position, this.maximum);
      this.scrollPosition = Math.max(this.scrollPosition, this.minimum);
    }

    public virtual void firstClick(int x, bool alt)
    {
      if (alt)
        this.setPostion(x - this.visibleAmount / 2);
      else if (x < this.scrollPosition)
      {
        this.setPostion(this.scrollPosition - this.visibleAmount);
      }
      else
      {
        if (x <= this.scrollPosition + this.visibleAmount)
          return;
        this.setPostion(this.scrollPosition + this.visibleAmount);
      }
    }

    public virtual void scrollClicks(int clicks) => this.scroll(clicks * -10);

    public virtual void scroll(int offset) => this.setPostion(this.scrollPosition + offset);

    public virtual int getMaximum() => this.maximum;

    public virtual int getMinimum() => this.minimum;

    public virtual int getPosition() => this.scrollPosition;

    public virtual int getVisibleAmount() => this.visibleAmount;

    public virtual void limit()
    {
      if (this.scrollPosition <= this.maximum)
        return;
      this.scrollPosition = this.maximum;
    }

    public virtual void reset() => this.scrollPosition = 0;

    public virtual bool isOnThumb(int pos) => pos > this.scrollPosition && pos < this.scrollPosition + this.visibleAmount;

    public virtual void setSize(int viewportSize, int contentSize)
    {
      this.visibleAmount = contentSize != 0 ? viewportSize * viewportSize / contentSize : 0;
      this.maximum = viewportSize - this.visibleAmount;
    }

    public virtual void secondClick(int y)
    {
      int num = (this.maximum + this.visibleAmount) / 2;
      this.setPostion(y >= num ? this.maximum : this.minimum);
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      ScrollBar scrollBar = this;
      ObjectImpl.clone((object) scrollBar);
      return ((object) scrollBar).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}

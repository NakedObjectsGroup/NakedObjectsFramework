// Decompiled with JetBrains decompiler
// Type: junit.awtui.Logo
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.awt;
using java.awt.image;

namespace junit.awtui
{
  public class Logo : Canvas
  {
    private Image fImage;
    private int fWidth;
    private int fHeight;

    public Logo()
    {
      // ISSUE: unable to decompile the method.
    }

    public virtual Image loadImage(string name)
    {
      // ISSUE: unable to decompile the method.
    }

    public virtual void paint(Graphics g)
    {
      this.paintBackground(g);
      if (this.fImage == null)
        return;
      g.drawImage(this.fImage, 0, 0, this.fWidth, this.fHeight, (ImageObserver) this);
    }

    public virtual void paintBackground(Graphics g)
    {
      g.setColor((Color) SystemColor.control);
      g.fillRect(0, 0, (int) ((Component) this).getBounds().width, (int) ((Component) this).getBounds().height);
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public virtual object MemberwiseClone()
    {
      Logo logo = this;
      ObjectImpl.clone((object) logo);
      return ((object) logo).MemberwiseClone();
    }
  }
}
